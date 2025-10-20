using System;
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
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("search")]
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
        public async Task<IActionResult> Index(SearchViewModel viewModel)
        {
            if (!viewModel.NoResults)
            {
                if (viewModel.LocalAuthorityToRemove.HasValue)
                {
                    var query = QueryStringHelper.ToQueryString(
                        SearchViewModel.BIND_ALIAS_LAIDS,
                        viewModel.RemoveLocalAuthorityId(viewModel.LocalAuthorityToRemove.Value)
                            .SelectedLocalAuthorityIds.ToArray());
                    return Redirect("/?" + query + "#la");
                }

                if (viewModel.SearchType.HasValue)
                {
                    if (viewModel.SearchType == eSearchType.LocalAuthorityDisambiguation)
                        return await ProcessLocalAuthorityDisambiguation(viewModel);

                    if (ModelState.IsValid)
                    {
                        if (viewModel.SearchType == eSearchType.Location &&
                            LatLon.Parse(viewModel.LocationSearchModel.AutoSuggestValue) == null &&
                            !viewModel.LocationSearchModel.Text.IsNullOrEmpty())
                        {
                            var disambiguationResult = await ProcessLocationDisambiguation(viewModel.LocationSearchModel.Text);
                            if (disambiguationResult != null)
                                return disambiguationResult;
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
                string fieldId = "TextSearchModel.Text";
                string fieldError = "We could not find any establishments matching your search criteria";

                switch (viewModel.SearchType)
                {
                    case eSearchType.Text:
                        if (viewModel.TextSearchModel.Text.IsNullOrEmpty())
                            fieldError = "Please enter an establishment name, URN, LAESTAB or UKPRN to start a search";
                        viewModel.ErrorPanel = "Name";
                        break;

                    case eSearchType.Location:
                        fieldId = "LocationSearchModel.Text";
                        fieldError = viewModel.LocationSearchModel.Text.IsNullOrEmpty()
                            ? "Please enter a postcode, town or city to start a search"
                            : "We couldn't find any establishments at that location";
                        viewModel.ErrorPanel = "Location";
                        break;

                    case eSearchType.ByLocalAuthority:
                        if (!string.IsNullOrEmpty(viewModel.LocalAuthorityToAdd))
                            return await ProcessLocalAuthorityDisambiguation(viewModel);
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
                        fieldError = viewModel.GovernorSearchModel.Forename.IsNullOrEmpty() &&
                                     viewModel.GovernorSearchModel.Surname.IsNullOrEmpty() &&
                                     viewModel.GovernorSearchModel.RoleId.Length == 0
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
                    if (Enum.IsDefined(typeof(eLookupGovernorRole), (eLookupGovernorRole) x.Id))
                    {
                        var factoryName = GovernorRoleNameFactory.Create((eLookupGovernorRole) x.Id);
                        if (!factoryName.Equals(staffRole.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.Error.WriteLine($"Governor role name mismatch: {staffRole.Name} -> {factoryName}");
                        }
                        staffRole.Name = factoryName;
                    }
                    return staffRole;
                });

            return View(ConstIndex, viewModel);
        }

        [HttpGet("suggest")]
        public async Task<IActionResult> Suggest(string text)
        {
            var suggestions = (await _establishmentReadService.SuggestAsync(StringUtil.DistillEstablishmentName(text), User)).ToArray();
            var duplicates = suggestions.GroupBy(x => x.Name).SelectMany(grp => grp.Skip(1)).ToArray();

            foreach (var item in suggestions)
            {
                if (!duplicates.Any(x => x.Name == item.Name))
                {
                    item.Address_CityOrTown = string.Empty;
                    item.Address_PostCode = string.Empty;
                }
            }

            return Json(suggestions);
        }

        [HttpGet("suggest-group")]
        public async Task<IActionResult> SuggestGroup(string text) =>
            Json(await _groupReadService.SuggestAsync(text.Distill(), User));

        [HttpGet("suggest-place")]
        public async Task<IActionResult> SuggestPlace(string text)
        {
            return QueryValidator.ValidatePlaceSuggestionQuery(text)
                ? Json(await _placesService.SearchAsync(text, true))
                : new StatusCodeResult((int) HttpStatusCode.BadRequest);
        }

        private async Task<IActionResult> ProcessLocalAuthorityDisambiguation(SearchViewModel model)
        {
            var localAuthorities = await _cachedLookupService.LocalAuthorityGetAllAsync();
            var localAuthority = localAuthorities.FirstOrDefault(x =>
                x.Name.Equals(model.LocalAuthorityToAdd, StringComparison.OrdinalIgnoreCase));

            if (localAuthority != null)
            {
                var query = QueryStringHelper.ToQueryString(
                    SearchViewModel.BIND_ALIAS_LAIDS,
                    model.AddLocalAuthorityId(localAuthority.Id).SelectedLocalAuthorityIds.ToArray());

                return Redirect($"?SearchType=ByLocalAuthority&{query}#la");
            }

            var matches = localAuthorities
                .Where(x => x.Name.IndexOf(model.LocalAuthorityToAdd ?? "", StringComparison.OrdinalIgnoreCase) > -1)
                .Take(10)
                .ToList();

            var viewModel = new LocalAuthorityDisambiguationViewModel(
                model.SelectedLocalAuthorityIds,
                model.LocalAuthorityToAdd ?? "",
                matches,
                model.OpenOnly);

            return View("LocalAuthorityDisambiguation", viewModel);
        }

        private async Task<IActionResult> ProcessLocationDisambiguation(string query)
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
    }
}
