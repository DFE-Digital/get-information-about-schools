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
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;

namespace Edubase.Web.UI.Controllers
{
    public class SitemapController : EduBaseController
    {
        private IEstablishmentReadService _establishmentReadService;
        private IGroupReadService _groupReadService;
        private ICacheAccessor _cacheAccessor;
        public const string cacheTag = "sitemap";
        public const int take = 5000;

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
            Server.ScriptTimeout = 300;
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
                var cacheDays = ConfigurationManager.AppSettings["SitemapCacheDays"].ToInteger() ?? 1;
                var freshMap = await GenerateSitemapDocument();
                await _cacheAccessor.SetAsync(cacheTag, freshMap, TimeSpan.FromDays(cacheDays));
                return freshMap;
            }
        }

        public async Task<string> GenerateSitemapDocument()
        {
            var siteNodes = GetSitemapNodes().ToList();
            siteNodes.AddRange(await GetEstablishmentNodes(0.8));
            siteNodes.AddRange(await GetGroupNodes(0.8));
            return BuildSitemapXml(siteNodes);
        }

        public string BuildSitemapXml(IReadOnlyCollection<SitemapNode> siteNodes)
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

            foreach (var tab in (SearchViewModel.Tab[]) Enum.GetValues(typeof(SearchViewModel.Tab)))
            {
                nodes.Add(
                    new SitemapNode()
                    {
                        Url = urlHelper.AbsoluteActionUrl("Index", "Search", new { SelectedTab = tab }),
                        Priority = 1,
                        Frequency = SitemapFrequency.Yearly
                    });
            }

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
            var nodes = new List<SitemapNode>();
            if (ExcludeApiLookup())
            {
                return nodes;
            }

            var resultCount = await _establishmentReadService.SearchAsync(new EstablishmentSearchPayload { Filters = new EstablishmentSearchFilters { StatusIds = new[] { 1, 4 } }, Take = 1 }, User);
            var resultItems = new List<EstablishmentSearchResultModel>();
            if (resultCount.Count > 0)
            {
                // the backend will timeout if we query too large a dataset, so we loop it.
                var skip = 0;
                do
                {
                    var lookup = await _establishmentReadService.SearchAsync(new EstablishmentSearchPayload { Filters = new EstablishmentSearchFilters { StatusIds = new[] { 1, 4 } }, Skip = skip, Take = take }, User);
                    resultItems.AddRange(lookup.Items);
                    if (skip + take >= resultCount.Count)
                    {
                        break;
                    }
                    skip += take;
                } while (true);
            }

            foreach (var item in resultItems)
            {
                nodes.Add(new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl("Details", "Establishment", new { area = "Establishments", id = item.Urn }),
                    Priority = priority,
                    Frequency = SitemapFrequency.Weekly,
                    LastModified = item.LastUpdatedUtc
                });
            }

            return nodes;
        }

        public async Task<IReadOnlyCollection<SitemapNode>> GetGroupNodes(double? priority)
        {
            var urlHelper = this.Url;
            var nodes = new List<SitemapNode>();
            if (ExcludeApiLookup())
            {
                return nodes;
            }

            var resultCount = await _groupReadService.SearchAsync(new GroupSearchPayload { GroupStatusIds = new[] { 1, 4 }, Take = 1 }, User);
            var resultItems = new List<SearchGroupDocument>();
            if (resultCount.Count > 0)
            {
                // the backend will timeout if we query too large a dataset, so we loop it.
                var skip = 0;
                do
                {
                    var lookup = await _groupReadService.SearchAsync(new GroupSearchPayload { GroupStatusIds = new[] { 1, 4 }, Skip = skip, Take = take }, User);
                    resultItems.AddRange(lookup.Items);
                    if (skip + take >= resultCount.Count)
                    {
                        break;
                    }
                    skip += take;
                } while (true);
            }

            foreach (var item in resultItems)
            {
                nodes.Add(new SitemapNode()
                {
                    Url = urlHelper.AbsoluteActionUrl($"Details", "Group", new { area = "Groups", id = item.GroupUId }),
                    Frequency = SitemapFrequency.Weekly,
                    Priority = priority
                });
            }

            return nodes;
        }

        private bool ExcludeApiLookup()
        {
            return (ConfigurationManager.AppSettings["SitemapExcludeApi"].ToInteger() ?? 0) != 0;
        }
    }
}
