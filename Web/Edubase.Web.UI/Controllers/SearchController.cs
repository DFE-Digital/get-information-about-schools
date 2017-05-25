using Edubase.Common;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    using Common.Spatial;
    using Services.IntegrationEndPoints.Google;
    using eStatus = Services.Enums.eLookupEstablishmentStatus;

    public class SearchController : EduBaseController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGroupReadService _groupReadService;
        private readonly IGooglePlacesService _googlePlacesService;

        public SearchController(IEstablishmentReadService establishmentReadService,
            ICachedLookupService cachedLookupService,
            IGroupReadService groupReadService,
            IGooglePlacesService googlePlacesService)
        {
            _googlePlacesService = googlePlacesService;
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }
        
        [HttpGet, Route]
        public async Task<ActionResult> Index(SearchViewModel viewModel)
        {
            if (!viewModel.NoResultsForLocation && !viewModel.NoResultsForName && !viewModel.NoResultsForLA)
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
                            return await ProcessLocationDisambiguation(viewModel);
                        }
                    }
                }
            }

            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
            viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));

            return View(viewModel);
        }

        [HttpGet, Route("Search/Results", Name = "SearchResults")]
        public async Task<ActionResult> IndexResults(SearchViewModel viewModel)
        {
            if (!viewModel.NoResultsForLocation && !viewModel.NoResultsForName && !viewModel.NoResultsForLA)
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
                            return await ProcessLocationDisambiguation(viewModel);
                        }

                        if (viewModel.SearchType.OneOfThese(eSearchType.ByLocalAuthority, eSearchType.Location, eSearchType.Text))
                        {
                            var url = Url.Action("Index", "EstablishmentsSearch", new { area = "Establishments" });
                            url = viewModel.OpenOnly
                                ? $"{url}?{Request.QueryString.AddIfNonExistent(SearchViewModel.BIND_ALIAS_STATUSIDS, (int)eStatus.Open, (int)eStatus.OpenButProposedToClose)}"
                                : $"{url}?{Request.QueryString.AddIfNonExistent("OpenOnly", "false")}";

                            return Redirect(url);
                        }

                        if (viewModel.SearchType == eSearchType.Group)
                        {
                            return Redirect(Url.Action("Index", "GroupSearch", new { area = "Groups" }) + "?" + Request.QueryString);
                        }

                        if (viewModel.SearchType == eSearchType.Governor)
                        {
                            return Redirect(
                                $"{Url.Action("Index", "GovernorSearch", new {area = "Governors"})}?{Request.QueryString}&{string.Join("&", viewModel.GovernorSearchModel.RoleId.Select(r => $"&{Areas.Governors.Models.GovernorSearchViewModel.BIND_ALIAS_ROLE_ID}={r}"))}");
                        }

                        throw new NotSupportedException($"The search type '{viewModel.SearchType}' is not recognised.");
                    }
                }
            }

            return RedirectToAction("Index", viewModel);
        }

        [Route("Search/Suggest"), HttpGet]
        public async Task<ActionResult> Suggest(string text) => Json(await _establishmentReadService.SuggestAsync(StringUtil.DistillEstablishmentName(text), User));

        [Route("Search/SuggestGroup"), HttpGet]
        public async Task<ActionResult> SuggestGroup(string text) => Json(await _groupReadService.SuggestAsync(text.Distill(), User));

        

        private async Task<ActionResult> ProcessLocalAuthorityDisambiguation(SearchViewModel model)
        {
            var localAuthorities = await _cachedLookupService.LocalAuthorityGetAllAsync();
            var localAuthority = localAuthorities.FirstOrDefault(x => x.Name.Equals(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase));
            if (localAuthority != null) return Redirect("/?" + QueryStringHelper.ToQueryString(SearchViewModel.BIND_ALIAS_LAIDS,
                model.AddLocalAuthorityId(localAuthority.Id).SelectedLocalAuthorityIds.ToArray()) + "#la");
            else
            {
                var localAuthorityDisambiguationViewModel = new LocalAuthorityDisambiguationViewModel(model.SelectedLocalAuthorityIds, model.LocalAuthorityToAdd ?? "",
                    localAuthorities.Where(x => x.Name.IndexOf(model.LocalAuthorityToAdd ?? "", StringComparison.OrdinalIgnoreCase) > -1).Take(10).ToList());
                return View("LocalAuthorityDisambiguation", localAuthorityDisambiguationViewModel);
            }
        }

        [Route("Search/ResolveLocation"), HttpGet]
        public async Task<ActionResult> ResolveLocation(string placeId)
        {
            var location = await _googlePlacesService.GetCoordinateAsync(placeId);
            var url = Url.Action("Index", "Search", new { area = "" });
            const string key = "LocationSearchModel.AutoSuggestValue";
            url += "?" + Request.QueryString.RemoveKeys("placeId", key).AddIfNonExistent(key, $"{location.Latitude},{location.Longitude}");
            return Redirect(url);
        }

        private async Task<ActionResult> ProcessLocationDisambiguation(SearchViewModel model)
        {
            var items = await _googlePlacesService.SearchAsync(model.LocationSearchModel.Text);
            return View("LocationDisambiguation", new LocationDisambiguationViewModel() { SearchText = model.LocationSearchModel.Text, MatchingLocations = items.ToList() });

        }
    }
}