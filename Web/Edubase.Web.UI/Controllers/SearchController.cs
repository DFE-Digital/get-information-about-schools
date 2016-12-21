using Edubase.Common;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Edubase.Web.UI.Controllers
{
    using Common.Spatial;
    using Services;
    using Services.Domain;
    using Services.Establishments;
    using Services.Establishments.Search;
    using Services.Groups;
    using Services.Groups.Models;
    using Services.Groups.Search;
    using StackExchange.Profiling;
    using System;
    using System.Threading.Tasks;
    using ViewModel = AdvancedSearchViewModel;

    public class SearchController : EduBaseController
    {
        private IEstablishmentReadService _establishmentReadService;
        private IGroupReadService _groupReadService;

        public SearchController(IEstablishmentReadService establishmentReadService, IGroupReadService groupReadService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        public ActionResult Index() => View();
        

        [HttpGet]
        public async Task<ActionResult> Results(ViewModel model)
        {
            if (model.SearchType == ViewModel.eSearchType.Text) return await SearchByTextSearch(model);
            else if (model.SearchType == ViewModel.eSearchType.Location) return await SearchByLocation(model);
            else if (model.SearchType == ViewModel.eSearchType.LocalAuthority) return await SearchByLocalAuthority(model);
            else if (model.SearchType == ViewModel.eSearchType.Group) return await SearchGroups(model);
            else if (model.SearchType == ViewModel.eSearchType.Governor) return SearchGovernors(model.GovernorSearchModel);
            throw new NotImplementedException();
        }

        private ActionResult SearchGovernors(Areas.Governors.Models.SearchModel governorSearchModel)
        {
            return Redirect("/Governors/Search?Forename=" + governorSearchModel.Forename + "&Surname=" + governorSearchModel.Surname + "&RoleId=" + governorSearchModel.RoleId + "&IncludeHistoric=" + governorSearchModel.IncludeHistoric);
        }


        private async Task<ActionResult> SearchGroups(ViewModel model)
        {
            if (model.GroupSearchModel.AutoSuggestValueAsInt.HasValue)
            {
                return new RedirectToRouteResult(null, new RouteValueDictionary
                {
                    { "action", "Details" },
                    { "controller", "Group" },
                    { "id", model.GroupSearchModel.AutoSuggestValueAsInt }
                });
            }
            else
            {
                var text = model.GroupSearchModel.Text.Clean();
                var viewModel = new GroupSearchResultsModel(text) { StartIndex = model.StartIndex, Count = model.Count };
                using (MiniProfiler.Current.Step("Searching groups..."))
                {
                    var results = await _groupReadService.SearchByIdsAsync(text, text.ToInteger(), text, User);

                    if (results.Count > 0)
                    {
                        viewModel.Results.Add(results.Items[0]);
                        viewModel.Count = 1;
                    }
                    else
                    {
                        var payload = new GroupSearchPayload(nameof(GroupModel.Name), model.StartIndex, model.PageSize) { Text = text };
                        using (MiniProfiler.Current.Step("Searching groups (in text mode)..."))
                        {
                            results = await _groupReadService.SearchAsync(payload, User);
                            viewModel.Results = results.Items;
                            if (model.StartIndex == 0) viewModel.Count = results.Count.Value;
                        }
                    }
                }
                
                viewModel.SearchType = "Groups";

                if (viewModel.Count == 1)
                {
                    return new RedirectToRouteResult(null, new RouteValueDictionary
                                {
                                    { "action", "Details" },
                                    { "controller", "Group" },
                                    { "id", viewModel.Results.Single().GroupUID }
                                });
                }
                else viewModel.CalculatePageStats(model.PageSize);

                return View("GroupResults", viewModel);

            }
        }
        
        private async Task<ActionResult> SearchByLocalAuthority(ViewModel model)
        {
            var payload = new EstablishmentSearchPayload(nameof(SearchEstablishmentDocument.Name), model.StartIndex, model.PageSize);
            AddFilters(model, payload.Filters);
            return await ProcessSearch(model, payload);
        }

        private async Task<ActionResult> SearchByLocation(ViewModel model)
        {
            var payload = new EstablishmentSearchPayload(nameof(SearchEstablishmentDocument.Name), model.StartIndex, model.PageSize);
            var filters = payload.Filters;

            var coord = LatLon.Parse(model.LocationSearchModel.AutoSuggestValue);
            if (coord != null)
            {
                payload.GeoSearchLocation = coord;
                payload.GeoSearchMaxRadiusInKilometres = 10;
                payload.GeoSearchOrderByDistance = true;
            }
            else model.Error = "The co-ordinate could not be parsed.";

            AddFilters(model, filters);

            return await ProcessSearch(model, payload);
        }

        private async Task<ActionResult> SearchByTextSearch(ViewModel model)
        {
            var retVal = await SearchByUrnAsync(model);
            if (retVal != null) return retVal;
            
            var payload = new EstablishmentSearchPayload(nameof(SearchEstablishmentDocument.Name), model.StartIndex, model.PageSize);
            var filters = payload.Filters;
            if (model.TextSearchType == ViewModel.eTextSearchType.UKPRN)
            {
                filters.UKPRN = model.TextSearchModel.Text.ToInteger();
            }
            else if(model.TextSearchType == ViewModel.eTextSearchType.LAESTAB)
            {
                var laestab = LAESTAB.TryParse(model.TextSearchModel.Text).Value;
                filters.LocalAuthorityIds = new int[] { laestab.LocalAuthorityId };
                filters.EstablishmentNumber = laestab.EstablishmentNumber;
            }
            else if (model.TextSearchType == ViewModel.eTextSearchType.EstablishmentName)
            {
                payload.Text = model.TextSearchModel.Text;
            }
            else model.Error = "The LAESTAB, UKPRN or URN was invalid.";

            AddFilters(model, filters);

            return await ProcessSearch(model, payload);
        }

        private async Task<ActionResult> ProcessSearch(ViewModel model, EstablishmentSearchPayload payload)
        {
            if (!model.HasError)
            {
                using (MiniProfiler.Current.Step("Invoking AZS search"))
                {
                    var results = await _establishmentReadService.SearchAsync(payload, User);
                    if (payload.Skip == 0) model.Count = results.Count.GetValueOrDefault();
                    model.Results = results.Items;
                }
            }

            if (model.Count == 1) return RedirectToEstabDetail(model.Results.First().Urn.GetValueOrDefault());
            else
            {
                var permittedStatusIds = _establishmentReadService.GetPermittedStatusIds(User);

                using (MiniProfiler.Current.Step("Populate lookups from CachedLookupService"))
                {
                    var svc = new CachedLookupService();
                    model.EstablishmentTypes = (await svc.EstablishmentTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                    model.EstablishmentStatuses = (await svc.EstablishmentStatusesGetAllAsync()).Where(x => permittedStatusIds == null || permittedStatusIds.Contains(x.Id)).Select(x => new LookupItemViewModel(x));
                    model.EducationPhases = (await svc.EducationPhasesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                    model.ReligiousCharacters = (await svc.ReligiousCharactersGetAllAsync()).Select(x => new LookupItemViewModel(x));
                    model.LocalAuthorties = (await svc.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
                }
                
                return View("AdvancedSearchResults", model);
            }
        }

        private void AddFilters(ViewModel viewModel, EstablishmentSearchFilters filters)
        {
            filters.EducationPhaseIds = viewModel.SelectedEducationPhaseIds.ToArray();
            filters.StatusIds = viewModel.SelectedEstablishmentStatusIds.ToArray();
            filters.TypeIds = viewModel.SelectedEstablishmentTypeIds.ToArray();
            filters.LocalAuthorityIds = viewModel.SelectedLocalAuthorityIds.ToArray();
            filters.ReligiousCharacterIds = viewModel.SelectedReligiousCharacterIds.ToArray();
        }


        private async Task<ActionResult> SearchByUrnAsync(ViewModel model)
        {
            var urn = model.TextSearchModel.AutoSuggestValueAsInt ?? (model.TextSearchType == ViewModel.eTextSearchType.URN ? model.TextSearchModel.Text.ToInteger() : null);
            if (urn.HasValue)
            {
                if ((await _establishmentReadService.CanAccess(urn.Value, User)).ReturnValue)
                    return RedirectToEstabDetail(urn.Value);
            }
            return null;
        }
        
        [HttpGet]
        public async Task<ActionResult> Suggest(string text) => Json(await _establishmentReadService.SuggestAsync(text, User));

        [HttpGet]
        public async Task<ActionResult> SuggestGroup(string text) => Json(await _groupReadService.SuggestAsync(text, User));


        private ActionResult RedirectToEstabDetail(int urn)
            => new RedirectToRouteResult(null, new RouteValueDictionary
            {
                { "action", "Details" },
                { "controller", "Establishment" },
                { "id", urn }
            });

    }
}