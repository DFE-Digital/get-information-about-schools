using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Search;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers
{
    public class SitemapController : EduBaseController
    {
        private IEstablishmentReadService _establishmentReadService;
        private IGroupReadService _groupReadService;
        private ICacheAccessor _cacheAccessor;
        public const string cacheTag = "sitemap";

        public SitemapController(IEstablishmentReadService establishmentReadService,
        IGroupReadService groupReadService,
        ICacheAccessor cacheAccessor)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _cacheAccessor = cacheAccessor;
        }

        [Route("~/sitemap.xml")]
        public async Task<ActionResult> SitemapXml()
        {
            var xml = await GetSitemapDocument();
            return this.Content(xml, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        
        public async Task<string> GetSitemapDocument()
        {
            var cache = await _cacheAccessor.GetAsync<string>(cacheTag);
            if (cache != null)
            {
                return (string) cache;
            }
            else
            {
                var cacheTime = ConfigurationManager.AppSettings["SitemapCacheHours"].ToInteger() ?? 24;
                var freshMap = await GenerateSitemapDocument();
                await _cacheAccessor.SetAsync(cacheTag, freshMap, TimeSpan.FromHours(cacheTime));
                return freshMap;
            }
        }

        public async Task<string> GenerateSitemapDocument()
        {
            List<SitemapNode> siteNodes = GetSitemapNodes().ToList();
            siteNodes.AddRange(await GetEstablishmentNodes(0.8));
            siteNodes.AddRange(await GetGroupNodes(0.8));

            return BuildSitemapXml(siteNodes);
        }

        public string BuildSitemapXml(List<SitemapNode> siteNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in siteNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }


        public IReadOnlyCollection<SitemapNode> GetSitemapNodes()
        {
            var urlHelper = this.Url;
            string scheme = urlHelper.RequestContext.HttpContext.Request.Url.Scheme;
            List<SitemapNode> nodes = new List<SitemapNode>();

            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl("Index", "Search"),
                    Priority = 1,
                    Frequency = SitemapFrequency.Yearly
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl("News", "Home"),
                    Priority = 0.9,
                    Frequency = SitemapFrequency.Weekly
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl("Help", "Home"),
                    Priority = 0.5,
                    Frequency = SitemapFrequency.Yearly
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl("Index", "Faq"),
                    Priority = 0.6,
                    Frequency = SitemapFrequency.Monthly
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl("Cookies", "Home"),
                    Priority = 0.5,
                    Frequency = SitemapFrequency.Monthly
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl("Index", "Glossary"),
                    Priority = 0.5,
                    Frequency = SitemapFrequency.Monthly
                });
            return nodes;
        }

        public async Task<IReadOnlyCollection<SitemapNode>> GetEstablishmentNodes(double? priority)
        {
            var urlHelper = this.Url;
            List<SitemapNode> nodes = new List<SitemapNode>();

            // first we need to see how many results there will be:
            var resultCount = await _establishmentReadService.SearchAsync(new EstablishmentSearchPayload { Filters = new EstablishmentSearchFilters { StatusIds = new[] { 1, 4 } }, Take = 1 }, User);
            if (resultCount.Count > 0)
            {
                // then call the endpoint for all of the possible establishments. //resultCount.Count
                var resultTotal = await _establishmentReadService.SearchAsync(new EstablishmentSearchPayload { Filters = new EstablishmentSearchFilters { StatusIds = new[] { 1, 4 } }, Take = 100 }, User);
                foreach (var establishmentSearchResultModel in resultTotal.Items)
                {
                    nodes.Add(new SitemapNode()
                    {
                        Url = urlHelper.AbsoluteActionUrl("Details","Establishment", new {area="Establishments", id = establishmentSearchResultModel.Urn }),
                        Priority = priority,
                        Frequency = SitemapFrequency.Weekly,
                        LastModified = establishmentSearchResultModel.LastUpdatedUtc
                    });
                }
            }

            return nodes;
        }

        public async Task<IReadOnlyCollection<SitemapNode>> GetGroupNodes(double? priority)
        {
            var urlHelper = this.Url;
            List<SitemapNode> nodes = new List<SitemapNode>();

            var results = await _groupReadService.SearchAsync(new GroupSearchPayload { GroupStatusIds = new []{1,4}, Take = 1}, User);

            // first we need to see how many results there will be:
            var resultCount = await _groupReadService.SearchAsync(new GroupSearchPayload { GroupStatusIds = new[] { 1, 4 }, Take = 1 }, User);
            if (resultCount.Count > 0)
            {
                // then call the endpoint for all of the possible establishments. //resultCount.Count
                var resultTotal = await _groupReadService.SearchAsync(new GroupSearchPayload { GroupStatusIds = new[] { 1, 4 }, Take = 100 }, User);
                foreach (var establishmentSearchResultModel in resultTotal.Items)
                {
                    nodes.Add(new SitemapNode()
                    {
                        Url = urlHelper.AbsoluteActionUrl($"Details","Group", new {area="Groups", id=establishmentSearchResultModel.GroupUId}),
                        Frequency = SitemapFrequency.Weekly,
                        Priority = priority
                    });
                }
            }

            return nodes;
        }
    }
}
