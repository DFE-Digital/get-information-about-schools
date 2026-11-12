using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Geo;
using Edubase.Services.Governors.Factories;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Edubase.Web.UI.Controllers
{
    using eStatus = Services.Enums.eLookupEstablishmentStatus;

    [Route("Search")]
    public class SearchController : EduBaseController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGroupReadService _groupReadService;
        private readonly IPlacesLookupService _placesService;

        private const string ConstIndex = "Index";

        public SearchController(
            IEstablishmentReadService establishmentReadService,
            ICachedLookupService cachedLookupService,
            IGroupReadService groupReadService,
            IPlacesLookupService placesService)
        {
            _placesService = placesService;
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        [HttpGet("", Name = "Search")]
        [HttpGet("Index")]
        public async Task<ActionResult> Index(SearchViewModel viewModel)
        {
            if (!viewModel.NoResults)
            {
                if (viewModel.LocalAuthorityToRemove.HasValue)
                {
                    return Redirect("/?" + QueryStringHelper.ToQueryString(
                        SearchViewModel.BIND_ALIAS_LAIDS,
                        viewModel.RemoveLocalAuthorityId(viewModel.LocalAuthorityToRemove.Value).SelectedLocalAuthorityIds.ToArray()) + "#la");
                }

                if (viewModel.SearchType.HasValue)
                {
                    if (viewModel.SearchType == eSearchType.LocalAuthorityDisambiguation)
                    {
                        return await ProcessLocalAuthorityDisambiguation(viewModel);
                    }

                    if (ModelState.IsValid)
                    {
                        if (viewModel.SearchType == eSearchType.Location
                            && LatLon.Parse(viewModel.LocationSearchModel.AutoSuggestValue) == null
                            && !viewModel.LocationSearchModel.Text.IsNullOrEmpty())
                        {
                            var disambiguationResult = await ProcessLocationDisambiguation(viewModel.LocationSearchModel.Text);
                            if (disambiguationResult != null)
                            {
                                return disambiguationResult;
                            }
                        }
                    }
                    else if (viewModel.SearchType == eSearchType.GovernorReference)
                    {
                        var fieldError = viewModel.GovernorSearchModel.Gid.HasValue
                            ? "We could not find any governors matching your search criteria"
                            : "Please enter a governor ID to start a search";

                        viewModel.ErrorPanel = "GovernorId";

                        ModelState.AddModelError("GovernorSearchModel.Gid", fieldError);
                    }
                }
            }
            else
            {
                var fieldId = "TextSearchModel.Text";
                var fieldError = "We could not find any establishments matching your search criteria";

                switch (viewModel.SearchType)
                {
                    case eSearchType.Text:
                        if (viewModel.TextSearchModel.Text.IsNullOrEmpty())
                        {
                            fieldError = "Please enter an establishment name, URN, LAESTAB or UKPRN to start a search";
                        }

                        viewModel.ErrorPanel = "Name";
                        break;

                    case eSearchType.Location:
                        if (viewModel.LocationSearchModel.Text.IsNullOrEmpty())
                        {
                            fieldId = "LocationSearchModel.Text";
                            fieldError = "Please enter a postcode, town or city to start a search";
                            viewModel.ErrorPanel = "Location";
                        }
                        else
                        {
                            fieldId = "LocationSearchModel.Text";
                            fieldError = "We couldn't find any establishments at that location";
                            viewModel.ErrorPanel = "Location";
                        }

                        break;

                    case eSearchType.ByLocalAuthority:
                        if (!string.IsNullOrEmpty(viewModel.LocalAuthorityToAdd))
                        {
                            return await ProcessLocalAuthorityDisambiguation(viewModel);
                        }

                        fieldId = "LocalAuthorityToAdd";
                        fieldError = viewModel.SelectedLocalAuthorityIds.Any()
                            ? "This local authority has no open schools"
                            : "Please enter a local authority to start a search";
                        viewModel.ErrorPanel = "LocalAuthority";

                        break;

                    case eSearchType.Group:
                        fieldId = "GroupSearchModel.Text";

                        fieldError = viewModel.GroupSearchModel.Text.IsNullOrEmpty()
                            ? "Please enter an establishment group to start a search"
                            : "We could not find any establishment groups matching your search criteria";

                        viewModel.ErrorPanel = "Group";
                        break;

                    case eSearchType.Governor:
                        fieldId = "GovernorSearchModel.Forename";

                        fieldError = viewModel.GovernorSearchModel.Forename.IsNullOrEmpty()
                                     && viewModel.GovernorSearchModel.Surname.IsNullOrEmpty()
                                     && viewModel.GovernorSearchModel.RoleId.Length == 0
                            ? "Please enter a governor to start a search"
                            : "We could not find any governors matching your search criteria";

                        viewModel.ErrorPanel = "Governor";

                        break;

                    case eSearchType.GovernorReference:
                        fieldId = "GovernorSearchModel.Gid";
                        fieldError = viewModel.GovernorSearchModel.Gid.HasValue
                            ? "We could not find any governors matching your search criteria"
                            : "Please enter a governor ID to start a search";

                        viewModel.ErrorPanel = "GovernorId";

                        break;
                }

                ModelState.AddModelError(fieldId, fieldError);
            }

            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync())
                .OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));

            viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync())
                .OrderBy(x => x.Name)
                .Select(x =>
                {
                    var staffRole = new LookupItemViewModel(x);

                    // If recognised as an enum and we have a C#-overridden name for it, use that instead
                    if (Enum.IsDefined(typeof(eLookupGovernorRole), (eLookupGovernorRole)x.Id))
                    {
                        var factoryName = GovernorRoleNameFactory.Create((eLookupGovernorRole)x.Id);
                        if (!factoryName.ToLowerInvariant().Equals(staffRole.Name.ToLowerInvariant()))
                        {
                            Console.Error.WriteLine($"Governor role name mismatch - not just a case difference: {staffRole.Name} -> {factoryName}");
                        }

                        staffRole.Name = factoryName;
                    }

                    return staffRole;
                });

            return View(ConstIndex, viewModel);
        }

        [HttpGet("Results", Name = "SearchResults")]
        public async Task<ActionResult> IndexResults(SearchViewModel viewModel)
        {
            if (!viewModel.NoResults)
            {
                if (viewModel.LocalAuthorityToRemove.HasValue)
                {
                    return Redirect("?" + QueryStringHelper.ToQueryString(
                        SearchViewModel.BIND_ALIAS_LAIDS,
                        viewModel.RemoveLocalAuthorityId(viewModel.LocalAuthorityToRemove.Value).SelectedLocalAuthorityIds.ToArray()) + "#la");
                }

                if (viewModel.SearchType.HasValue)
                {
                    if (viewModel.SearchType == eSearchType.LocalAuthorityDisambiguation)
                    {
                        return await ProcessLocalAuthorityDisambiguation(viewModel);
                    }

                    if (viewModel.SearchType == eSearchType.ByLocalAuthority &&
                        !string.IsNullOrEmpty(viewModel.LocalAuthorityToAdd))
                    {
                        return await ProcessLocalAuthorityDisambiguation(viewModel);
                    }

                    if (ModelState.IsValid)
                    {
                        if (viewModel.SearchType == eSearchType.Location
                            && LatLon.Parse(viewModel.LocationSearchModel.AutoSuggestValue) == null
                            && !viewModel.LocationSearchModel.Text.IsNullOrEmpty())
                        {
                            var disambiguationResult = await ProcessLocationDisambiguation(viewModel.LocationSearchModel.Text);
                            if (disambiguationResult != null)
                            {
                                return disambiguationResult;
                            }
                        }

                        if (viewModel.SearchType.OneOfThese(eSearchType.ByLocalAuthority, eSearchType.Location, eSearchType.Text, eSearchType.EstablishmentAll))
                        {
                            var url = Url.Action(ConstIndex, "EstablishmentsSearch", new { area = "Establishments" });
                            var queryString = viewModel.OpenOnly
                                ? AddIfNonExistentToCurrentQuery(SearchViewModel.BIND_ALIAS_STATUSIDS, (int)eStatus.Open, (int)eStatus.OpenButProposedToClose)
                                : AddIfNonExistentToCurrentQuery("OpenOnly", "false");

                            return Redirect(AppendQueryString(url, queryString));
                        }

                        if (viewModel.SearchType.OneOfThese(eSearchType.Group, eSearchType.GroupAll))
                        {
                            var groupUrl = Url.Action(ConstIndex, "GroupSearch", new { area = "Groups" });
                            return Redirect(AppendQueryString(groupUrl, CurrentQueryStringWithoutQuestionMark()));
                        }

                        if (viewModel.SearchType.OneOfThese(eSearchType.Governor, eSearchType.GovernorReference, eSearchType.GovernorAll))
                        {
                            string[] allowedKeys =
                            {
                                "SelectedTab", "SearchType", "searchtype", "gsearch", "GovernorSearchModel.Forename", "GovernorSearchModel.Surname", "GovernorSearchModel.RoleId", "GovernorSearchModel.IncludeHistoric", "GovernorSearchModel.Gid"
                            };

                            var qs = CleanQueryString(allowedKeys);
                            var roleQuery = string.Join("&", viewModel.GovernorSearchModel.RoleId.Select(r => $"t={r}"));
                            var redirectQuery = string.IsNullOrWhiteSpace(roleQuery) ? qs : string.Join("&", new[] { qs, roleQuery }.Where(x => !string.IsNullOrWhiteSpace(x)));
                            var redirectUrl = Url.Action(ConstIndex, "GovernorSearch", new { area = "Governors" });

                            return Redirect(AppendQueryString(redirectUrl, redirectQuery));
                        }

                        throw new NotSupportedException($"The search type '{viewModel.SearchType}' is not recognised.");
                    }
                }
            }

            return await Index(viewModel);
        }

        /// <summary>
        /// Extract an allowed list of query string parameters with the keys contained in allowedKeys.
        /// </summary>
        /// <param name="allowedKeys"></param>
        /// <returns></returns>
        public string CleanQueryString(string[] allowedKeys)
        {
            var queryBuilder = new QueryBuilder();

            foreach (var key in allowedKeys)
            {
                if (!Request.Query.TryGetValue(key, out var values))
                {
                    continue;
                }

                foreach (var value in values)
                {
                    queryBuilder.Add(key, value);
                }
            }

            return queryBuilder.ToQueryString().Value?.TrimStart('?') ?? string.Empty;
        }

        [HttpGet("Suggest")]
        public async Task<ActionResult> Suggest(string text)
        {
            var suggestions = (await _establishmentReadService.SuggestAsync(StringUtil.DistillEstablishmentName(text), User)).ToArray();

            // we only want to show the city/postcode on the duplicates, so we can remove that content from the unique items
            var duplicates = suggestions.GroupBy(x => x.Name).SelectMany(grp => grp.Skip(1)).ToArray();
            foreach (var establishmentSuggestionItem in suggestions)
            {
                if (duplicates.Any(x => x.Name == establishmentSuggestionItem.Name))
                {
                    continue;
                }

                establishmentSuggestionItem.Address_CityOrTown = string.Empty;
                establishmentSuggestionItem.Address_PostCode = string.Empty;
            }

            return Json(suggestions);
        }

        [HttpGet("SuggestGroup")]
        public async Task<ActionResult> SuggestGroup(string text) => Json(await _groupReadService.SuggestAsync(text.Distill(), User));

        [HttpGet("SuggestPlace")]
        public async Task<ActionResult> SuggestPlace(string text)
        {
            return QueryValidator.ValidatePlaceSuggestionQuery(text)
                ? Json(await _placesService.SearchAsync(text, true))
                : BadRequest();
        }

        private async Task<ActionResult> ProcessLocalAuthorityDisambiguation(SearchViewModel model)
        {
            var localAuthorities = await _cachedLookupService.LocalAuthorityGetAllAsync();
            var localAuthority = localAuthorities.FirstOrDefault(x => x.Name.Equals(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase));
            if (localAuthority != null)
            {
                return Redirect("?SearchType=ByLocalAuthority&" + QueryStringHelper.ToQueryString(
                    SearchViewModel.BIND_ALIAS_LAIDS,
                    model.AddLocalAuthorityId(localAuthority.Id).SelectedLocalAuthorityIds.ToArray()) + "#la");
            }

            var localAuthorityDisambiguationViewModel = new LocalAuthorityDisambiguationViewModel(
                model.SelectedLocalAuthorityIds,
                model.LocalAuthorityToAdd ?? string.Empty,
                localAuthorities.Where(x => x.Name.IndexOf(model.LocalAuthorityToAdd ?? string.Empty, StringComparison.OrdinalIgnoreCase) > -1).Take(10).ToList(),
                model.OpenOnly);
            return View("LocalAuthorityDisambiguation", localAuthorityDisambiguationViewModel);
        }

        private async Task<ActionResult> ProcessLocationDisambiguation(string query)
        {
            var items = await _placesService.SearchAsync(query, false);
            return items.Any()
                ? View("LocationDisambiguation", new LocationDisambiguationViewModel
                {
                    SearchText = query,
                    MatchingLocations = items.ToList()
                })
                : null;
        }

        private string AddIfNonExistentToCurrentQuery(string key, params object[] values)
        {
            var queryValues = QueryHelpers.ParseQuery(Request.QueryString.Value ?? string.Empty)
                .ToDictionary(k => k.Key, v => v.Value.ToList(), StringComparer.OrdinalIgnoreCase);

            if (!queryValues.TryGetValue(key, out var existingValues))
            {
                existingValues = new List<string>();
                queryValues[key] = existingValues;
            }

            foreach (var value in values.Select(x => x?.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
            {
                if (!existingValues.Contains(value, StringComparer.Ordinal))
                {
                    existingValues.Add(value);
                }
            }

            var queryBuilder = new QueryBuilder();
            foreach (var entry in queryValues)
            {
                foreach (var value in entry.Value)
                {
                    queryBuilder.Add(entry.Key, value);
                }
            }

            return queryBuilder.ToQueryString().Value?.TrimStart('?') ?? string.Empty;
        }

        private string CurrentQueryStringWithoutQuestionMark() =>
            Request.QueryString.HasValue
                ? Request.QueryString.Value.TrimStart('?')
                : string.Empty;

        private static string AppendQueryString(string url, string queryString)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(queryString))
            {
                return url;
            }

            return url.Contains("?", StringComparison.Ordinal)
                ? string.Concat(url, "&", queryString)
                : string.Concat(url, "?", queryString);
        }
    }
}
