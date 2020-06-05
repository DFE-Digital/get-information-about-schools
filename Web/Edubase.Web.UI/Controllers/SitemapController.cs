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
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Web.UI.Areas.Establishments.Models;
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
        public async Task<ActionResult> SitemapXml(bool? refresh)
        {
            Server.ScriptTimeout = 300;
            var xml = await GetSitemapDocument(refresh.GetValueOrDefault());
            return this.Content(xml, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        
        public async Task<string> GetSitemapDocument(bool refresh)
        {
            var sitemap = refresh ? string.Empty : await _cacheAccessor.GetAsync<string>(cacheTag);

            if (sitemap.IsNullOrEmpty())
            {
                var cacheDays = ConfigurationManager.AppSettings["SitemapCacheDays"].ToInteger() ?? 1;
                sitemap = await GenerateSitemapDocument();
                await _cacheAccessor.SetAsync(cacheTag, sitemap, TimeSpan.FromDays(cacheDays));
            }

            return sitemap;
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

            nodes.Add(BuildNode("Index", "Search", null, null, 1, SitemapFrequency.Yearly));
            foreach (var tab in (SearchViewModel.Tab[]) Enum.GetValues(typeof(SearchViewModel.Tab)))
            {
                nodes.Add(BuildNode("Index", "Search", new { SelectedTab = tab }, null, 1, SitemapFrequency.Yearly));
            }

            nodes.Add(BuildNode("News", "Home", null, null, 0.9, SitemapFrequency.Weekly));
            nodes.Add(BuildNode("Help", "Home", null, null, 0.5, SitemapFrequency.Yearly));
            nodes.Add(BuildNode("Index", "Faq", null, null, 0.6));
            nodes.Add(BuildNode("Cookies", "Home"));
            nodes.Add(BuildNode("Index", "Glossary"));
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
                nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, null, priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
                nodes.AddRange(GetEstablishmentTabs(item,priority-0.1));
            }

            return nodes;
        }

        public IReadOnlyCollection<SitemapNode> GetEstablishmentTabs(EstablishmentModel item, double? priority)
        {
            var urlHelper = this.Url;
            var nodes = new List<SitemapNode>();

            nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-dashboard", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
            nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-links", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
            nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-location", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));

            // not all establishments have the governance page displayed for the general public
            var displayPolicy = new EstablishmentDisplayEditPolicy { IEBTDetail = new IEBTDetailDisplayEditPolicy() };
            var tabPolicy = new TabDisplayPolicy(item, displayPolicy, User);
            if (tabPolicy.Governance)
            {
                nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-governance", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
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
                nodes.Add(BuildNode("Details", "Group", new { area = "Groups", id = item.GroupUId }, null, priority, SitemapFrequency.Weekly));
                nodes.AddRange(GetGroupTabs(item, priority - 0.1));
            }

            return nodes;
        }


        public IReadOnlyCollection<SitemapNode> GetGroupTabs(SearchGroupDocument item, double? priority)
        {
            var urlHelper = this.Url;
            var nodes = new List<SitemapNode>();

            nodes.Add(BuildNode("Details", "Group", new { area = "Groups", id = item.GroupUId }, "details", priority, SitemapFrequency.Weekly));
            nodes.Add(BuildNode("Details", "Group", new { area = "Groups", id = item.GroupUId }, "list", priority, SitemapFrequency.Weekly));

            // not all groups have access to governance tab
            if  (item.GroupTypeId == (int) eLookupGroupType.MultiacademyTrust)
            {
                nodes.Add(BuildNode("Details","Group", new { area = "Groups", id = item.GroupUId }, "governance", priority, SitemapFrequency.Weekly));
            }

            return nodes;
        }


        private SitemapNode BuildNode(string action, string controllerName, object routeValues = null, string tag = null, double ? priority = 0.5, SitemapFrequency frequency = SitemapFrequency.Monthly, DateTime? lastUpdated = null)
        {
            var urlHelper = this.Url;
            var node = new SitemapNode()
            {
                Url = urlHelper.AbsoluteActionUrl(action, controllerName, routeValues),
                Priority = priority,
                Frequency = frequency
            };

            if (lastUpdated.HasValue)
            {
                node.LastModified = lastUpdated;
            }

            if (string.IsNullOrEmpty(tag))
            {
                node.Url = $"{node.Url}#{tag}";
            }

            return node;
        }

        private bool ExcludeApiLookup()
        {
            return (ConfigurationManager.AppSettings["SitemapExcludeApi"].ToInteger() ?? 0) != 0;
        }
    }
}
