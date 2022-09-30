using Edubase.Common;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;
using Antlr.Runtime;
using Edubase.Services.Establishments.Search;

namespace Edubase.Web.UI.Controllers
{
    using Common.Spatial;
    using Edubase.Services.Geo;
    using eStatus = Services.Enums.eLookupEstablishmentStatus;

    [RoutePrefix("Search"), Route("{action=index}")]
    public class SearchController : EduBaseController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGroupReadService _groupReadService;
        private readonly IPlacesLookupService _placesService;

        public SearchController(IEstablishmentReadService establishmentReadService,
            ICachedLookupService cachedLookupService,
            IGroupReadService groupReadService,
            IPlacesLookupService placesService)
        {
            _placesService = placesService;
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        [HttpGet, Route(Name = "Search")]
        public async Task<ActionResult> Index(SearchViewModel viewModel)
        {
            if (!viewModel.NoResults)
            {

                if (viewModel.LocalAuthorityToRemove.HasValue)
                {
                    return Redirect("/?" + QueryStringHelper.ToQueryString(SearchViewModel.BIND_ALIAS_LAIDS,
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
                        if (viewModel.SearchType == eSearchType.Location && LatLon.Parse(viewModel.LocationSearchModel.AutoSuggestValue) == null && !viewModel.LocationSearchModel.Text.IsNullOrEmpty())
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
                var la = Request.QueryString["LocalAuthorityToAdd"];
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

                        fieldError = viewModel.SelectedLocalAuthorityIds.Any() ?
                            "This local authority has no open schools" :
                            "Please enter a local authority to start a search";
                        viewModel.ErrorPanel = "LocalAuthority";

                        break;

                    case eSearchType.Group:
                        fieldId = "GroupSearchModel.Text";

                        fieldError = viewModel.GroupSearchModel.Text.IsNullOrEmpty() ?
                            "Please enter an establishment group to start a search" :
                            "We could not find any establishment groups matching your search criteria";

                        viewModel.ErrorPanel = "Group";
                        break;

                    case eSearchType.Governor:
                        fieldId = "GovernorSearchModel.Forename";

                        fieldError = viewModel.GovernorSearchModel.Forename.IsNullOrEmpty() &&
                                     viewModel.GovernorSearchModel.Surname.IsNullOrEmpty() &&
                                     viewModel.GovernorSearchModel.RoleId.Length == 0 ?
                            "Please enter a governor to start a search" :
                            "We could not find any governors matching your search criteria";

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

            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
            viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));

            return View("Index", viewModel);
        }

        [HttpGet, Route("Results", Name = "SearchResults")]
        public async Task<ActionResult> IndexResults(SearchViewModel viewModel)
        {
            if (!viewModel.NoResults)
            {

                if (viewModel.LocalAuthorityToRemove.HasValue)
                {
                    return Redirect("?" + QueryStringHelper.ToQueryString(SearchViewModel.BIND_ALIAS_LAIDS,
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
                            var url = Url.Action("Index", "EstablishmentsSearch", new { area = "Establishments" });
                            url = viewModel.OpenOnly
                                ? $"{url}?{Request.QueryString.AddIfNonExistent(SearchViewModel.BIND_ALIAS_STATUSIDS, (int)eStatus.Open, (int)eStatus.OpenButProposedToClose)}"
                                : $"{url}?{Request.QueryString.AddIfNonExistent("OpenOnly", "false")}";

                            return Redirect(url);
                        }

                        if (viewModel.SearchType.OneOfThese(eSearchType.Group, eSearchType.GroupAll))
                        {
                            return Redirect(Url.Action("Index", "GroupSearch", new { area = "Groups" }) + "?" + Request.QueryString);
                        }

                        if (viewModel.SearchType.OneOfThese(eSearchType.Governor, eSearchType.GovernorReference, eSearchType.GovernorAll))
                        {
                            return Redirect(
                                $"{Url.Action("Index", "GovernorSearch", new { area = "Governors" })}?{Request.QueryString}&{string.Join("&", viewModel.GovernorSearchModel.RoleId.Select(r => $"&{Areas.Governors.Models.GovernorSearchViewModel.BIND_ALIAS_ROLE_ID}={r}"))}");
                        }

                        throw new NotSupportedException($"The search type '{viewModel.SearchType}' is not recognised.");
                    }
                }
            }

            return await Index(viewModel);
        }

        [Route("Suggest"), HttpGet]
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

        [Route("SuggestGroup"), HttpGet]
        public async Task<ActionResult> SuggestGroup(string text) => Json(await _groupReadService.SuggestAsync(text.Distill(), User));

        [Route("SuggestPlace"), HttpGet]
        public async Task<ActionResult> SuggestPlace(string text)
        {
            if (!QueryValidator.ValidatePlaceSuggestionQuery(text))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return Json(await _placesService.SearchAsync(text, true));
        }

        private async Task<ActionResult> ProcessLocalAuthorityDisambiguation(SearchViewModel model)
        {
            var localAuthorities = await _cachedLookupService.LocalAuthorityGetAllAsync();
            var localAuthority = localAuthorities.FirstOrDefault(x => x.Name.Equals(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase));
            if (localAuthority != null) return Redirect("?SearchType=ByLocalAuthority&" + QueryStringHelper.ToQueryString(SearchViewModel.BIND_ALIAS_LAIDS,
                model.AddLocalAuthorityId(localAuthority.Id).SelectedLocalAuthorityIds.ToArray()) + "#la");
            else
            {
                var localAuthorityDisambiguationViewModel = new LocalAuthorityDisambiguationViewModel(model.SelectedLocalAuthorityIds, model.LocalAuthorityToAdd ?? "",
                    localAuthorities.Where(x => x.Name.IndexOf(model.LocalAuthorityToAdd ?? "", StringComparison.OrdinalIgnoreCase) > -1).Take(10).ToList(), model.OpenOnly);
                return View("LocalAuthorityDisambiguation", localAuthorityDisambiguationViewModel);
            }
        }

        private async Task<ActionResult> ProcessLocationDisambiguation(string query)
        {
            var items = await _placesService.SearchAsync(query, false);
            return items.Any()
                ? View(
                    "LocationDisambiguation",
                    new LocationDisambiguationViewModel
                    {
                        SearchText = query,
                        MatchingLocations = items.ToList()
                    })
                : null;
        }
    }
}
