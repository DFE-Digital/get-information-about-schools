using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
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
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Edubase.Web.UI.Controllers
{
    public class SitemapController : EduBaseController
    {
        private static readonly string[] SearchTabs =
        {
            "Establishments",
            "Groups",
            "Governors"
        };

        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly ICacheAccessor _cacheAccessor;
        private readonly ICachedLookupService _lookupService;
        private readonly IConfiguration _configuration;

        public const int Take = 5000;
        public const string CacheTag = "sitemap";

        private readonly int _cacheDays;

        public SitemapController(
            IEstablishmentReadService establishmentReadService,
            IGroupReadService groupReadService,
            ICacheAccessor cacheAccessor,
            ICachedLookupService lookupService,
            IConfiguration configuration)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _cacheAccessor = cacheAccessor;
            _lookupService = lookupService;
            _configuration = configuration;
            _cacheDays = _configuration["SitemapCacheDays"].ToInteger() ?? 1;
        }

        [Route("~/sitemap.xml")]
        public async Task<ActionResult> SitemapIndex(bool? refresh)
        {
            if (refresh.GetValueOrDefault())
            {
                await CleanSitemapCache();
            }

            var cacheName = $"{CacheTag}_index";
            var responseCopy = await _cacheAccessor.GetAsync<string>(cacheName);

            if (responseCopy.IsNullOrEmpty())
            {
                responseCopy = await GetSitemapIndex();
                await _cacheAccessor.SetAsync(cacheName, responseCopy, TimeSpan.FromDays(_cacheDays));
            }

            return Content(responseCopy, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        [Route("~/sitemap_site.xml")]
        public async Task<ActionResult> SitemapSite()
        {
            var cacheName = $"{CacheTag}_site";
            var responseCopy = await _cacheAccessor.GetAsync<string>(cacheName);

            if (responseCopy.IsNullOrEmpty())
            {
                responseCopy = BuildSitemapXml(GetSitemapNodes());
                await _cacheAccessor.SetAsync(cacheName, responseCopy, TimeSpan.FromDays(_cacheDays));
            }

            return Content(responseCopy, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        [Route("~/sitemap_est{estType}.xml")]
        public async Task<ActionResult> SitemapEstablishmentByType(int estType)
        {
            var cacheName = $"{CacheTag}_est_{estType}";
            var responseCopy = await _cacheAccessor.GetAsync<string>(cacheName);

            if (responseCopy.IsNullOrEmpty())
            {
                responseCopy = await GenerateEstablishmentDocument(estType);
                await _cacheAccessor.SetAsync(cacheName, responseCopy, TimeSpan.FromDays(_cacheDays));
            }

            return Content(responseCopy, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        [Route("~/sitemap_group{groupType}.xml")]
        public async Task<ActionResult> SitemapGroupByType(int groupType)
        {
            var cacheName = $"{CacheTag}_group_{groupType}";
            var responseCopy = await _cacheAccessor.GetAsync<string>(cacheName);

            if (responseCopy.IsNullOrEmpty())
            {
                responseCopy = await GenerateGroupDocument(groupType);
                await _cacheAccessor.SetAsync(cacheName, responseCopy, TimeSpan.FromDays(_cacheDays));
            }

            return Content(responseCopy, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        private async Task<string> GetSitemapIndex()
        {
            var urls = new List<string> { AbsoluteActionUrl("SitemapSite", "Sitemap") };

            foreach (var est in await GetEstablishmentTypes())
            {
                urls.Add(AbsoluteActionUrl("SitemapEstablishmentByType", "Sitemap", new { estType = est }));
            }

            foreach (var grp in await GetGroupTypes())
            {
                urls.Add(AbsoluteActionUrl("SitemapGroupByType", "Sitemap", new { groupType = grp }));
            }

            return BuildSitemapIndexXml(urls);
        }

        private async Task<string> GenerateEstablishmentDocument(int estType)
        {
            var siteNodes = await GetEstablishmentNodes(estType, 0.8);
            return BuildSitemapXml(siteNodes);
        }

        private async Task<string> GenerateGroupDocument(int groupType)
        {
            var siteNodes = await GetGroupNodes(groupType, 0.8);
            return BuildSitemapXml(siteNodes);
        }

        private async Task<bool> CleanSitemapCache()
        {
            await _cacheAccessor.DeleteAsync($"{CacheTag}_index");
            await _cacheAccessor.DeleteAsync($"{CacheTag}_site");

            foreach (var est in await GetEstablishmentTypes())
            {
                await _cacheAccessor.DeleteAsync($"{CacheTag}_est_{est}");
            }

            foreach (var grp in await GetGroupTypes())
            {
                await _cacheAccessor.DeleteAsync($"{CacheTag}_group_{grp}");
            }

            return true;
        }

        private IReadOnlyCollection<SitemapNode> GetSitemapNodes()
        {
            var nodes = new List<SitemapNode>();

            nodes.Add(BuildNode("Index", "Home", null, null, 1, SitemapFrequency.Yearly));
            nodes.Add(BuildNode("Index", "Search", null, null, 1, SitemapFrequency.Yearly));
            foreach (var tab in SearchTabs)
            {
                nodes.Add(BuildNode("Index", "Search", new { SelectedTab = tab }, null, 1, SitemapFrequency.Yearly));
            }

            nodes.Add(BuildNode("Index", "News", null, null, 0.9, SitemapFrequency.Weekly));
            nodes.Add(BuildNode("Help", "Home", null, null, 0.5, SitemapFrequency.Yearly));
            nodes.Add(BuildNode("Index", "Faq", null, null, 0.6));
            nodes.Add(BuildNode("Cookies", "Home"));
            nodes.Add(BuildNode("Index", "Glossary"));
            nodes.Add(BuildNode("Index", "Guidance"));
            nodes.Add(BuildNode("General", "Guidance", null, null, 0.3));
            nodes.Add(BuildNode("EstablishmentBulkUpdate", "Guidance", null, null, 0.3));
            nodes.Add(BuildNode("ChildrensCentre", "Guidance", null, null, 0.3));
            nodes.Add(BuildNode("Federation", "Guidance", null, null, 0.3));
            nodes.Add(BuildNode("Governance", "Guidance", null, null, 0.3));
            nodes.Add(BuildNode("Responsibilities", "Home"));
            nodes.Add(BuildNode("Privacy", "Home"));
            nodes.Add(BuildNode("Index", "TermsofUse"));
            nodes.Add(BuildNode("Contact", "Home"));
            nodes.Add(BuildNode("About", "Home", null, null, 0.7));
            nodes.Add(BuildNode("Accessibility", "Home"));
            nodes.Add(BuildNode("AccessibilityReport", "Home"));
            return nodes;
        }

        private async Task<IReadOnlyCollection<SitemapNode>> GetEstablishmentNodes(int estType, double? priority)
        {
            var nodes = new List<SitemapNode>();
            if (ExcludeApiLookup())
            {
                return nodes;
            }

            var resultCount = await _establishmentReadService.SearchAsync(new EstablishmentSearchPayload { Filters = new EstablishmentSearchFilters { TypeIds = new[] { estType }, StatusIds = new[] { 1, 4 } }, Take = 1 }, User);
            var resultItems = new List<EstablishmentSearchResultModel>();

            if (resultCount.Count > 0)
            {
                var skip = 0;
                do
                {
                    var lookup = await _establishmentReadService.SearchAsync(new EstablishmentSearchPayload { Filters = new EstablishmentSearchFilters { TypeIds = new[] { estType }, StatusIds = new[] { 1, 4 } }, Skip = skip, Take = Take }, User);
                    resultItems.AddRange(lookup.Items);
                    if (skip + Take >= resultCount.Count)
                    {
                        break;
                    }
                    skip += Take;
                } while (true);
            }

            foreach (var item in resultItems)
            {
                nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, null, priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
                nodes.AddRange(GetEstablishmentTabs(item, priority - 0.1));
            }

            return nodes;
        }

        private IReadOnlyCollection<SitemapNode> GetEstablishmentTabs(EstablishmentModel item, double? priority)
        {
            var nodes = new List<SitemapNode>();

            nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-dashboard", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
            nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-links", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
            nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-location", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));

            var displayPolicy = new EstablishmentDisplayEditPolicy { IEBTDetail = new IEBTDetailDisplayEditPolicy() };
            var tabPolicy = new TabDisplayPolicy(item, displayPolicy, User);
            if (tabPolicy.Governance)
            {
                nodes.Add(BuildNode("Details", "Establishment", new { area = "Establishments", id = item.Urn }, "school-governance", priority, SitemapFrequency.Weekly, item.LastUpdatedUtc));
            }

            return nodes;
        }

        private async Task<IReadOnlyCollection<SitemapNode>> GetGroupNodes(int groupType, double? priority)
        {
            var nodes = new List<SitemapNode>();
            if (ExcludeApiLookup())
            {
                return nodes;
            }

            var resultCount = await _groupReadService.SearchAsync(new GroupSearchPayload { GroupTypeIds = new[] { groupType }, GroupStatusIds = new[] { 1, 4 }, Take = 1 }, User);
            var resultItems = new List<SearchGroupDocument>();
            if (resultCount.Count > 0)
            {
                var skip = 0;
                do
                {
                    var lookup = await _groupReadService.SearchAsync(new GroupSearchPayload { GroupTypeIds = new[] { groupType }, GroupStatusIds = new[] { 1, 4 }, Skip = skip, Take = Take }, User);
                    resultItems.AddRange(lookup.Items);
                    if (skip + Take >= resultCount.Count)
                    {
                        break;
                    }
                    skip += Take;
                } while (true);
            }

            foreach (var item in resultItems)
            {
                nodes.Add(BuildNode("Details", "Group", new { area = "Groups", id = item.GroupUId }, null, priority, SitemapFrequency.Weekly));
                nodes.AddRange(GetGroupTabs(item, priority - 0.1));
            }

            return nodes;
        }

        private IReadOnlyCollection<SitemapNode> GetGroupTabs(SearchGroupDocument item, double? priority)
        {
            var nodes = new List<SitemapNode>();

            nodes.Add(BuildNode("Details", "Group", new { area = "Groups", id = item.GroupUId }, "details", priority, SitemapFrequency.Weekly));
            nodes.Add(BuildNode("Details", "Group", new { area = "Groups", id = item.GroupUId }, "list", priority, SitemapFrequency.Weekly));

            if (item.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust)
            {
                nodes.Add(BuildNode("Details", "Group", new { area = "Groups", id = item.GroupUId }, "governance", priority, SitemapFrequency.Weekly));
            }

            return nodes;
        }

        private SitemapNode BuildNode(string action, string controllerName, object routeValues = null, string tag = null, double? priority = 0.5, SitemapFrequency frequency = SitemapFrequency.Monthly, DateTime? lastUpdated = null)
        {
            var node = new SitemapNode
            {
                Url = AbsoluteActionUrl(action, controllerName, routeValues),
                Priority = priority,
                Frequency = frequency
            };

            if (lastUpdated.HasValue)
            {
                node.LastModified = lastUpdated;
            }

            if (!string.IsNullOrEmpty(tag))
            {
                node.Url = $"{node.Url}#{tag}";
            }

            return node;
        }

        private string BuildSitemapIndexXml(IReadOnlyCollection<string> siteIndex)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "sitemapindex");

            foreach (var url in siteIndex)
            {
                var urlElement = new XElement(
                    xmlns + "sitemap",
                    new XElement(xmlns + "loc", NormalizeSitemapUrl(url)));
                root.Add(urlElement);
            }

            var document = new XDocument(root);
            return document.ToString();
        }

        public string BuildSitemapXml(IReadOnlyCollection<SitemapNode> siteNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (var sitemapNode in siteNodes)
            {
                var urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", NormalizeSitemapUrl(sitemapNode.Url)),
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

            var document = new XDocument(root);
            return document.ToString();
        }

        private async Task<IEnumerable<int>> GetEstablishmentTypes()
        {
            var types = await _lookupService.EstablishmentTypesGetAllAsync();
            return types.OrderBy(x => x.Id).Select(x => x.Id);
        }

        private async Task<IEnumerable<int>> GetGroupTypes()
        {
            var types = await _lookupService.GroupTypesGetAllAsync();
            return types.OrderBy(x => x.Id).Select(x => x.Id);
        }

        private bool ExcludeApiLookup()
        {
            return (_configuration["SitemapExcludeApi"].ToInteger() ?? 0) != 0;
        }

        private string AbsoluteActionUrl(string actionName, string controllerName, object routeValues = null)
        {
            var request = HttpContext.Request;
            var requestHeaderHost = request.Headers["X-Forwarded-Host"].ToString();
            var host = request.Host.HasValue ? request.Host.Host : string.Empty;
            var scheme = request.Scheme;

            if (!string.IsNullOrWhiteSpace(requestHeaderHost))
            {
                var allowedForwardedHostNames = (_configuration["AllowedForwardedHostNames"] ?? string.Empty)
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x));

                if (allowedForwardedHostNames.Contains(requestHeaderHost, StringComparer.OrdinalIgnoreCase))
                {
                    host = requestHeaderHost;
                }
                else
                {
                    throw new InvalidOperationException($"The `X-Forwarded-Host` header contains an invalid value: {requestHeaderHost}");
                }
            }

            var absoluteActionUrl = Url.Action(actionName, controllerName, routeValues, scheme, host);
            if (absoluteActionUrl is null)
            {
                return null;
            }

            var uriBuilder = new UriBuilder(absoluteActionUrl)
            {
                Scheme = scheme,
                Host = host
            };

            return uriBuilder.Uri.ToString();
        }

        private static string NormalizeSitemapUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }

            return Uri.TryCreate(url, UriKind.Absolute, out var normalized)
                ? normalized.AbsoluteUri
                : url;
        }
    }
}
