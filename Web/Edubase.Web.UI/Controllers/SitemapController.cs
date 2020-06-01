using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Search;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers
{
    public class SitemapController : EduBaseController
    {
        private IEstablishmentReadService _establishmentReadService;
        private IGroupReadService _groupReadService;
        public SitemapController(IEstablishmentReadService establishmentReadService,
        IGroupReadService groupReadService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;

        }

        [Route("~/sitemap.xml")]
        public async Task<ActionResult> SitemapXml()
        {
            var xml = await GetSitemapDocument();
            return this.Content(xml, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }



        public async Task<string> GetSitemapDocument()
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            List<SitemapNode> siteNodes = GetSitemapNodes().ToList();
            siteNodes.AddRange(await GetEstablishmentNodes());
            siteNodes.AddRange(await GetGroupNodes());

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

        public async Task<IReadOnlyCollection<SitemapNode>> GetEstablishmentNodes()
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
                        Url = urlHelper.Content($"/Establishments/Establishment/Details/{establishmentSearchResultModel.Urn}"),
                        Priority = 0.8,
                        LastModified = establishmentSearchResultModel.LastUpdatedUtc
                    });
                }
            }

            return nodes;
        }

        public async Task<IReadOnlyCollection<SitemapNode>> GetGroupNodes()
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
                        Url = urlHelper.Content($"/Groups/Group/Details/{establishmentSearchResultModel.GroupUId}"),
                        Priority = 0.8
                    });
                }
            }

            return nodes;
        }


        public IReadOnlyCollection<SitemapNode> GetSitemapNodes()
        {
            var urlHelper = this.Url;
            List<SitemapNode> nodes = new List<SitemapNode>();

            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.Action("Index", "Faq"),
                    Priority = 1
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.Action("Index", "Search"),
                    Priority = 0.9
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.Action("News", "Home"),
                    Priority = 0.9
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.Action("Cookies", "Home"),
                    Priority = 0.9
                });
            nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.Action("Index", "Glossary"),
                    Priority = 0.9
                });
            return nodes;
        }
    }
}
