//using Edubase.Common;
//using Edubase.Web.UI.Models;
//using System.Linq;
//using System.Web.Mvc;
//using System.Web.Routing;
//using System;

//namespace Edubase.Web.UI.Controllers
//{
//    using Common.Spatial;
//    using Models.Search;
//    using Services;
//    using Services.Domain;
//    using Services.Establishments;
//    using Services.Establishments.Search;
//    using Services.Groups;
//    using Services.Groups.Models;
//    using Services.Groups.Search;
//    using Services.Lookup;
//    using StackExchange.Profiling;
//    using System;
//    using System.Threading.Tasks;
//    using ViewModel = Models.Search.AdvancedSearchViewModel;

//    public partial class SearchController : EduBaseController
//    {
//        private Returns<EstablishmentSearchPayload> GetEstablishmentSearchPayload(ViewModel model)
//        {
//            var retVal = new Returns<EstablishmentSearchPayload>();
//            var payload = new EstablishmentSearchPayload(model.StartIndex, model.PageSize);
//            var filters = payload.Filters;

//            if (model.SearchType == ViewModel.eSearchType.Text)
//            {
//                if (model.TextSearchType == ViewModel.eTextSearchType.UKPRN)
//                {
//                    filters.UKPRN = model.TextSearchModel.Text.ToInteger();
//                }
//                else if (model.TextSearchType == ViewModel.eTextSearchType.LAESTAB)
//                {
//                    var laestab = LAESTAB.TryParse(model.TextSearchModel.Text).Value;
//                    filters.LocalAuthorityIds = new int[] { laestab.LocalAuthorityId };
//                    filters.EstablishmentNumber = laestab.EstablishmentNumber;
//                }
//                else if (model.TextSearchType == ViewModel.eTextSearchType.EstablishmentName)
//                {
//                    payload.Text = model.TextSearchModel.Text;
//                }
//                else retVal.ErrorMessage = "The LAESTAB, UKPRN or URN was invalid.";
//            }
//            else if (model.SearchType == ViewModel.eSearchType.Location)
//            {
//                var coord = model.LocationSearchCoordinate;
//                if (coord != null)
//                {
//                    payload.GeoSearchLocation = coord;
//                    payload.RadiusInMiles = model.GetRadiusOption();
//                }
//                else retVal.ErrorMessage = "The co-ordinate could not be parsed."; // todo: need to support Location-disambiguation page and non-JS scenario.
//            }

//            filters.EducationPhaseIds = model.SelectedEducationPhaseIds.ToArray();
//            filters.StatusIds = model.SelectedEstablishmentStatusIds.ToArray();
//            filters.TypeIds = model.SelectedEstablishmentTypeIds.ToArray();
//            filters.LocalAuthorityIds = model.SelectedLocalAuthorityIds.ToArray();
//            filters.ReligiousCharacterIds = model.SelectedReligiousCharacterIds.ToArray();

//            filters.AdmissionsPolicyIds = model.SelectedAdmissionsPolicyIds.ToArray();
//            filters.ProvisionBoardingIds = model.SelectedBoardingProvisionIds.ToArray();
//            filters.DioceseIds = model.SelectedDioceseIds.ToArray();
//            filters.AdministrativeDistrictIds = model.SelectedDistrictIds.ToArray();
//            filters.FurtherEducationTypeIds = model.SelectedGenderIds.ToArray();
//            filters.GovernmentOfficeRegionIds = model.SelectedGORIds.ToArray();
//            filters.ProvisionNurseryIds = model.SelectedNurseryProvisionIds.ToArray();
//            filters.ParliamentaryConstituencyIds = model.SelectedParliamentaryConstituencyIds.ToArray();
//            filters.ReligiousEthosIds = model.SelectedReligiousEthosIds.ToArray();
//            filters.RSCRegionIds = model.SelectedRSCRegionIds.ToArray();
//            filters.Section41ApprovedIds = model.SelectedSection41Ids.ToArray();
//            filters.ProvisionOfficialSixthFormIds = model.SelectedSixthFormProvisionIds.ToArray();
//            filters.ProvisionSpecialClassesIds = model.SelectedSpecialClassesProvisionIds.ToArray();
            
//            payload.SENIds = model.SelectedTypeOfSENProvisionIds.ToArray();

//            filters.UrbanRuralIds = model.SelectedUrbanRuralIds.ToArray();
//            filters.AdministrativeWardIds = model.SelectedWardIds.ToArray();

//            payload.SortBy = model.GetSortOption();

//            return retVal.Set(payload);
//        }

//        private async Task<ActionResult> ProcessEstablishmentsSearch(ViewModel model, EstablishmentSearchPayload payload)
//        {
//            if (!model.HasError)
//            {
//                using (MiniProfiler.Current.Step("Invoking AZS search"))
//                {
//                    try
//                    {
//                        var results = await _establishmentReadService.SearchAsync(payload, User);
//                        if (payload.Skip == 0) model.Count = results.Count.GetValueOrDefault();
//                        model.Results = results.Items;
//                    }
//                    catch (Services.Exceptions.SearchQueryTooLargeException) // expected domain exception when over 800 filters are selected; very much an edge case.
//                    {
//                        model.Error = "You have selected too many filters; please reduce the number of filters.";
//                        model.Count = 0;
//                    }
                    
//                }
//            }

//            if (model.Count == 1 && model.GoToDetailPageOnOneResult) return RedirectToEstabDetail(model.Results.First().Urn.GetValueOrDefault());
//            else
//            {
//                var permittedStatusIds = _establishmentReadService.GetPermittedStatusIds(User);

//                using (MiniProfiler.Current.Step("Populate filter lookups from CachedLookupService"))
//                {
//                    model.EstablishmentTypes = (await _lookupService.EstablishmentTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));
//                    model.EstablishmentStatuses = (await _lookupService.EstablishmentStatusesGetAllAsync()).Where(x => permittedStatusIds == null || permittedStatusIds.Contains(x.Id)).Select(x => new LookupItemViewModel(x));
//                    model.EducationPhases = (await _lookupService.EducationPhasesGetAllAsync()).Select(x => new LookupItemViewModel(x));
//                    model.ReligiousCharacters = (await _lookupService.ReligiousCharactersGetAllAsync()).Select(x => new LookupItemViewModel(x));
//                    await PopulateLookups(model);
//                }
                
//                return View("AdvancedSearchResults", model);
//            }
//        }
        
//        private async Task<ActionResult> SearchByUrnAsync(ViewModel model)
//        {
//            var urn = model.TextSearchModel.AutoSuggestValueAsInt 
//                ?? (model.TextSearchType == ViewModel.eTextSearchType.URN 
//                ? model.TextSearchModel.Text.ToInteger() : null);

//            if (urn.HasValue)
//            {
//                if ((await _establishmentReadService.CanAccess(urn.Value, User)).ReturnValue)
//                    return RedirectToEstabDetail(urn.Value);
//            }

//            return null;
//        }
        
//        private ActionResult RedirectToEstabDetail(int urn)
//            => new RedirectToRouteResult(null, new RouteValueDictionary
//            {
//                { "action", "Details" },
//                { "controller", "Establishment" },
//                { "id", urn }
//            });

//        private async Task<ViewModel> PopulateLookups(ViewModel vm)
//        {
//            using (MiniProfiler.Current.Step($"{GetType().Name}.{nameof(PopulateLookups)}"))
//            {
//                vm.LocalAuthorities = (await _lookupService.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.GovernorRoles = (await _lookupService.GovernorRolesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.AdmissionsPolicies = (await _lookupService.AdmissionsPoliciesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.BoardingProvisions = (await _lookupService.ProvisionBoardingGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.Dioceses = (await _lookupService.DiocesesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.Districts = (await _lookupService.AdministrativeDistrictsGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.FurtherEducationTypes = (await _lookupService.FurtherEducationTypesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.Genders = (await _lookupService.GendersGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.GORs = (await _lookupService.GovernmentOfficeRegionsGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.NurseryProvisions = (await _lookupService.ProvisionNurseriesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.ParliamentaryConstituencies = (await _lookupService.ParliamentaryConstituenciesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.ReligiousEthoses = (await _lookupService.ReligiousEthosGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.RSCRegions = (await _lookupService.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.Section41Designations = (await _lookupService.Section41ApprovedGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.SixthFormProvisions = (await _lookupService.ProvisionOfficialSixthFormsGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.SpecialClassesProvisions = (await _lookupService.ProvisionSpecialClassesGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.TypeOfSENProvisions = (await _lookupService.SpecialEducationNeedsGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.UrbanRuralDesignations = (await _lookupService.UrbanRuralGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//                vm.Wards = (await _lookupService.AdministrativeWardsGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
//            }
//            return vm;
//        }
        

//    }
//}