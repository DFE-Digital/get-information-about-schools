using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using Edubase.Common;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    using Edubase.Services.Establishments.Models;
    using Edubase.Services.Security;
    using Edubase.Web.UI.Helpers;
    using Models.Search;
    using Services.Domain;
    using Services.Enums;
    using Services.Establishments;
    using Services.Establishments.Downloads;
    using Services.Establishments.Search;
    using Services.Lookup;
    using System;
    using System.Threading.Tasks;
    using System.Web.Hosting;
    using UI.Controllers;
    using UI.Models.Search;
    using EM = Services.Establishments.Models.EstablishmentModel;
    using R = Services.Security.EdubaseRoles;

    [RouteArea("Establishments"), RoutePrefix("Search"), Route("{action=index}")]
    public class EstablishmentsSearchController : EduBaseController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentDownloadService _establishmentDownloadService;
        private readonly ICachedLookupService _lookupService;

        public EstablishmentsSearchController(IEstablishmentReadService establishmentReadService,
            IEstablishmentDownloadService establishmentDownloadService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _establishmentDownloadService = establishmentDownloadService;
            _lookupService = lookupService;
        }


        [HttpGet, Route(Name="EstabSearch")]
        public async Task<ActionResult> Index(EstablishmentSearchViewModel model)
        {
            model.SearchQueryString = Request.QueryString.ToString();

            var retVal = await SearchByUrnAsync(model);
            if (retVal != null) return retVal;

            var payload = await GetEstablishmentSearchPayload(model);
            if (!payload.Success) model.Error = payload.ErrorMessage;
            return await ProcessEstablishmentsSearch(model, payload.Object);
        }

        [HttpGet, Route("results-js")]
        public async Task<PartialViewResult> ResultsPartial(EstablishmentSearchViewModel model)
        {
            var payload = await GetEstablishmentSearchPayload(model);
            if (!payload.Success) model.Error = payload.ErrorMessage;
            await ProcessEstablishmentsSearch(model, payload.Object);
            HttpContext.Response.Headers.Add("x-count", model.Count.ToString());
            HttpContext.Response.Headers.Add("x-show-date-filter-warning", model.ShowDateFilterWarning.ToString().ToLower());
            return PartialView("Partials/_EstablishmentSearchResults", model);
        }

        [HttpGet, Route("results-json")]
        public async Task<ActionResult> JsonResults(EstablishmentSearchViewModel model)
        {
            var payload = await GetEstablishmentSearchPayload(model);
            payload.Object.Take = 100;

            if (!payload.Success) model.Error = payload.ErrorMessage;
            await ProcessEstablishmentsSearch(model, payload.Object);
            var localAuthorities = (await _lookupService.LocalAuthorityGetAllAsync());
            var establishmentTypes = await _lookupService.EstablishmentTypesGetAllAsync();
            var educationPhases = await _lookupService.EducationPhasesGetAllAsync();
            var counties = (await _lookupService.CountiesGetAllAsync()).Where(c => c.Id != 63); //remove "not recorded"
            HttpContext.Response.Headers.Add("x-count", model.Count.ToString());

            var filtered = model.Results
                                .Select(result => new { Result = result, LA = localAuthorities.SingleOrDefault(la => la.Id == result.LocalAuthorityId) })
                                .Select(a => new
            {
                Name = a.Result.Name,
                Location = a.Result.Location,
                Address = StringUtil.ConcatNonEmpties(", ", a.Result.Address_Line1, 
                                                            a.Result.Address_Locality, 
                                                            a.Result.Address_Line3, 
                                                            a.Result.Address_CityOrTown, 
                                                            counties.FirstOrDefault(c => c.Id == a.Result.Address_CountyId)?.Name,
                                                            a.Result.Address_PostCode),
                Urn = a.Result.Urn,
                LAESTAB = a.LA?.Code != null && a.Result.EstablishmentNumber.HasValue ? $"{a.LA.Code}/{a.Result.EstablishmentNumber.Value:D4}" : string.Empty,
                Status = model.EstablishmentStatuses.FirstOrDefault(x => x.Id == a.Result.StatusId)?.Name ?? "Not recorded",
                LocalAuthority = a.LA?.Name ?? "Not recorded",
                PhaseType = string.Concat(educationPhases.FirstOrDefault(x => x.Id == a.Result.EducationPhaseId)?.Name ?? "Not recorded", ", ", establishmentTypes.FirstOrDefault(x => x.Id == a.Result.TypeId)?.Name ?? "Not recorded"),
            });

            return Json(filtered);
        }

        [HttpGet, Route("PrepareDownload")]
        public async Task<ActionResult> PrepareDownload(EstablishmentSearchDownloadViewModel viewModel)
        {
            viewModel.SearchSource = eLookupSearchSource.Establishments;
            viewModel.AllowIncludeEmailAddresses = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.APT, R.AP_AOS, R.EFADO, R.FST, R.IEBT, R.ISI, R.OFSTED, R.SOU, R.EDUBASE_CHILDRENS_CENTRE_POLICY, R.EDUBASE_LACCDO, R.EFAHNS, R.edubase_ddce, R.SFC, R.DUGE);
            viewModel.AllowIncludeIEBTFields = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.IEBT, R.ISI, R.OFSTED);
            viewModel.AllowIncludeBringUpFields = User.InRole(R.IEBT);
            viewModel.AllowIncludeChildrensCentreFields = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.EDUBASE_CHILDRENS_CENTRE_POLICY, R.EDUBASE_LACCDO);

            if (!viewModel.Dataset.HasValue)
            {
                viewModel.SearchQueryString = Request.QueryString.ToString();
                return View("Downloads/SelectDataset", viewModel);
            }

            if (!viewModel.FileFormat.HasValue)
                return View("Downloads/SelectFormat", viewModel);

            var progressId = await _establishmentDownloadService.SearchWithDownloadGenerationAsync(
                new EstablishmentSearchDownloadPayload
                {
                    SearchPayload = (await GetEstablishmentSearchPayload(viewModel)).Object,
                    DataSet = viewModel.Dataset.Value,
                    FileFormat = viewModel.FileFormat.Value,
                    IncludeBringUpFields = viewModel.IncludeBringUpFields,
                    IncludeChildrensCentreFields = viewModel.IncludeChildrensCentreFields,
                    IncludeEmailAddresses = viewModel.IncludeEmailAddresses,
                    IncludeIEBTFields = viewModel.IncludeIEBTFields,
                    IncludeLinks = viewModel.IncludeLinks
                }, User);

            return RedirectToAction(nameof(Download), new { id = progressId, fileFormat = viewModel.FileFormat.Value, viewModel.SearchQueryString, viewModel.SearchSource });
        }

        [HttpGet, Route("Download")]
        public async Task<ActionResult> Download(Guid id, eFileFormat fileFormat, string searchQueryString = null, eLookupSearchSource? searchSource = null)
        {

            var model = await _establishmentDownloadService.GetDownloadGenerationProgressAsync(id, User);
            var viewModel = new EstablishmentSearchDownloadGenerationProgressViewModel(model, model.IsComplete ? 4 : 3)
            {
                FileFormat = fileFormat,
                SearchSource = searchSource,
                SearchQueryString = searchQueryString
            };

            if (model.HasErrored)
                throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");

            if (!model.IsComplete)
                return View("Downloads/PreparingFilePleaseWait", viewModel);

            return View("Downloads/ReadyToDownload", viewModel);
        }

        private async Task<EstablishmentSearchViewModel> PopulateLookups(EstablishmentSearchViewModel vm)
        {
            vm.LocalAuthorities = (await _lookupService.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.GovernorRoles = (await _lookupService.GovernorRolesGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.AdmissionsPolicies = (await _lookupService.AdmissionsPoliciesGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.BoardingProvisions = (await _lookupService.ProvisionBoardingGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.Dioceses = (await _lookupService.DiocesesGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.Districts = (await _lookupService.AdministrativeDistrictsGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.FurtherEducationTypes = (await _lookupService.FurtherEducationTypesGetAllAsync())
                .OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.Genders = (await _lookupService.GendersGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.GORs = (await _lookupService.GovernmentOfficeRegionsGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.NurseryProvisions = (await _lookupService.ProvisionNurseriesGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.ParliamentaryConstituencies = (await _lookupService.ParliamentaryConstituenciesGetAllAsync())
                .OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.ReligiousEthoses = (await _lookupService.ReligiousEthosGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.RSCRegions = (await _lookupService.RscRegionsGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.Section41Designations = (await _lookupService.Section41ApprovedGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.SixthFormProvisions = (await _lookupService.ProvisionOfficialSixthFormsGetAllAsync())
                .OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.SpecialClassesProvisions = (await _lookupService.ProvisionSpecialClassesGetAllAsync())
                .OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.TypeOfSENProvisions = (await _lookupService.SpecialEducationNeedsGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.UrbanRuralDesignations = (await _lookupService.UrbanRuralGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            vm.OfstedRatings = (await _lookupService.OfstedRatingsGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));

            return vm;
        }

        private ActionResult RedirectToEstabDetail(int urn)
            => new RedirectToRouteResult(null, new RouteValueDictionary
            {
                { "action", "Details" },
                { "controller", "Establishment" },
                { "id", urn },
                { "area", "Establishments" }
            });

        private async Task<Returns<EstablishmentSearchPayload>> GetEstablishmentSearchPayload(EstablishmentSearchViewModel model)
        {
            var retVal = new Returns<EstablishmentSearchPayload>();
            var payload = new EstablishmentSearchPayload(model.StartIndex, model.PageSize);
            var filters = payload.Filters;

            if (model.SearchType == eSearchType.Text || model.SearchType == eSearchType.EstablishmentAll)
            {
                if (model.TextSearchType == EstablishmentSearchViewModel.eTextSearchType.UKPRN)
                {
                    filters.UKPRN = model.TextSearchModel.Text;
                }
                else if (model.TextSearchType == EstablishmentSearchViewModel.eTextSearchType.LAESTAB)
                {
                    var laestab = LAESTAB.TryParse(model.TextSearchModel.Text).Value;
                    var localAuthorityId = (await _lookupService.LocalAuthorityGetAllAsync()).FirstOrDefault(x => x.Code == laestab.LocalAuthorityCode)?.Id;
                    if (localAuthorityId.HasValue) filters.LocalAuthorityIds = new int[] { localAuthorityId.Value };
                    filters.EstablishmentNumber = laestab.EstablishmentNumber;
                }
                else if (model.TextSearchType == EstablishmentSearchViewModel.eTextSearchType.EstablishmentName)
                {
                    payload.Text = model.TextSearchModel.Text;
                }
                else retVal.ErrorMessage = "The LAESTAB, UKPRN or URN was invalid.";
            }
            else if (model.SearchType == eSearchType.Location)
            {
                var coord = model.LocationSearchCoordinate;
                if (coord != null)
                {
                    payload.GeoSearchLocation = coord;
                    payload.RadiusInMiles = model.GetRadiusOption();
                }
                else retVal.ErrorMessage = "The co-ordinate could not be parsed.";
            }

            filters.EducationPhaseIds = model.SelectedEducationPhaseIds.ToArray();
            filters.StatusIds = model.SelectedEstablishmentStatusIds.ToArray();
            filters.TypeIds = model.SelectedEstablishmentTypeIds.ToArray();
            if (!filters.LocalAuthorityIds.Any()) filters.LocalAuthorityIds = model.SelectedLocalAuthorityIds.ToArray();
            filters.ReligiousCharacterIds = model.SelectedReligiousCharacterIds.ToArray();

            filters.AdmissionsPolicyIds = model.SelectedAdmissionsPolicyIds.ToArray();
            filters.ProvisionBoardingIds = model.SelectedBoardingProvisionIds.ToArray();
            filters.DioceseIds = model.SelectedDioceseIds.ToArray();
            filters.AdministrativeDistrictIds = model.SelectedDistrictIds.ToArray();
            filters.FurtherEducationTypeIds = model.SelectedFETypeIds.ToArray();
            filters.GenderIds = model.SelectedGenderIds.ToArray();
            filters.GovernmentOfficeRegionIds = model.SelectedGORIds.ToArray();
            filters.ProvisionNurseryIds = model.SelectedNurseryProvisionIds.ToArray();
            filters.ParliamentaryConstituencyIds = model.SelectedParliamentaryConstituencyIds.ToArray();
            filters.ReligiousEthosIds = model.SelectedReligiousEthosIds.ToArray();
            filters.RSCRegionIds = model.SelectedRSCRegionIds.ToArray();
            filters.Section41ApprovedIds = model.SelectedSection41Ids.ToArray();
            filters.ProvisionOfficialSixthFormIds = model.SelectedSixthFormProvisionIds.ToArray();
            filters.ProvisionSpecialClassesIds = model.SelectedSpecialClassesProvisionIds.ToArray();
            filters.SENIds = model.SelectedTypeOfSENProvisionIds.ToArray();
            filters.UrbanRuralIds = model.SelectedUrbanRuralIds.ToArray();

            filters.CloseDateMin = model.CloseDateFrom?.ToDateTime();
            filters.CloseDateMax = model.CloseDateTo?.ToDateTime();
            filters.OpenDateMin = model.OpenDateFrom?.ToDateTime();
            filters.OpenDateMax = model.OpenDateTo?.ToDateTime();
            filters.StatutoryLowAgeMin = model.AgeRangeLow?.From;
            filters.StatutoryLowAgeMax = model.AgeRangeLow?.To;
            filters.StatutoryHighAgeMin = model.AgeRangeHigh?.From;
            filters.StatutoryHighAgeMax = model.AgeRangeHigh?.To;
            filters.OfstedRatingIds = model.SelectedOfstedRatingIds.ToArray();


            payload.SortBy = model.GetSortOption();

            return retVal.Set(payload);
        }

        private ActionResult NoResults(EstablishmentSearchViewModel model)
        {
            var routeDictionary = new RouteValueDictionary
            {
                {"action", "Index"},
                {"controller", "Search"},
                {"area", string.Empty},
                {"SearchType", model.SearchType}
            };

            string queryStringToAppend = string.Empty;

            switch (model.SearchType)
            {
                case eSearchType.Text:
                    routeDictionary.Add("TextSearchModel.Text", model.TextSearchModel.Text);
                    routeDictionary.Add("NoResultsForName", true);
                    break;
                case eSearchType.Location:
                    routeDictionary.Add("LocationSearchModel.Text", model.LocationSearchModel.Text);
                    routeDictionary.Add("NoResultsForLocation", true);
                    break;
                case eSearchType.ByLocalAuthority:
                    queryStringToAppend = string.Concat("&", QueryStringHelper.ToQueryString(EstablishmentSearchViewModel.BIND_ALIAS_LAIDS, model.SelectedLocalAuthorityIds.ToArray()));
                    routeDictionary.Add("NoResultsForLA", true);
                    break;
            }

            return new RedirectResult(string.Concat(Url.RouteUrl(routeDictionary), queryStringToAppend));
        }

        private async Task<ActionResult> ProcessEstablishmentsSearch(EstablishmentSearchViewModel model,
            EstablishmentSearchPayload payload)
        {
            if (model.HasError)
            {
                return NoResults(model);
            }
            else
            {
                PopulateSelectList(payload); // select only fields we use in this context

                var results = await _establishmentReadService.SearchAsync(payload, User);

                if (payload.Skip == 0) model.Count = results.Count;
                model.Results = results.Items;

                if (results.Count == 0)
                {
                    return NoResults(model);
                }

                var localAuthorities = await _lookupService.LocalAuthorityGetAllAsync();

                foreach (var item in model.Results)
                {
                    model.Addresses.Add(item, await item.GetAddressAsync(_lookupService));
                    var laEstab = string.Empty;
                    if (item.LocalAuthorityId.HasValue && item.EstablishmentNumber.HasValue)
                    {
                        var code = localAuthorities.FirstOrDefault(x => x.Id == item.LocalAuthorityId)?.Code;
                        if (code != null) laEstab = string.Concat(code, "/", item.EstablishmentNumber?.ToString("D4"));
                    }
                    model.LAESTABs.Add(item, laEstab);
                }
            }

            if (model.Count == 1 && model.GoToDetailPageOnOneResult)
                return RedirectToEstabDetail(model.Results.First().Urn.GetValueOrDefault());
            else
            {
                var permittedStatusIds = await _establishmentReadService.GetPermittedStatusIdsAsync(User);
                var establishmentTypes = await _lookupService.EstablishmentTypesGetAllAsync();
                var establishmentGroupTypes = await _lookupService.EstablishmentTypeGroupsGetAllAsync();

                
                model.EstablishmentTypes = establishmentGroupTypes.Select(groupType => new HeirarchicalLookupItemViewModel
                    {
                        Id = groupType.Id,
                        Name = groupType.Name,
                        ChildItems = establishmentTypes.Where(c => c.GroupIds.Contains(groupType.Id))
                            .Select(e => new HeirarchicalLookupItemViewModel {Id = e.Id, Name = e.Name})
                            .ToList()
                    })
                    .ToList();

                model.EstablishmentTypeLookup = establishmentTypes.ToDictionary(e => e.Id, e => e.Name);
                
                model.EstablishmentStatuses = (await _lookupService.EstablishmentStatusesGetAllAsync())
                    .Where(x => permittedStatusIds == null || permittedStatusIds.Contains(x.Id))
                    .Select(x => new LookupItemViewModel(x));

                model.EducationPhases =
                    (await _lookupService.EducationPhasesGetAllAsync()).Select(x => new LookupItemViewModel(x));

                model.ReligiousCharacters =
                    (await _lookupService.ReligiousCharactersGetAllAsync()).Select(x => new LookupItemViewModel(x));

                await PopulateLookups(model);


                return View("Index", model);
            }
        }

        private void PopulateSelectList(EstablishmentSearchPayload payload)
        {
            payload.Select.Append(nameof(EM.Name))
                .Append(nameof(EM.Address_Line1))
                .Append(nameof(EM.Address_Line2))
                .Append(nameof(EM.Address_Line3))
                .Append(nameof(EM.Address_Locality))
                .Append(nameof(EM.Address_PostCode))
                .Append(nameof(EM.Address_CountryId))
                .Append(nameof(EM.Address_CountyId))
                .Append(nameof(EM.Address_CityOrTown))
                .Append(nameof(EM.Location))
                .Append(nameof(EM.EducationPhaseId))
                .Append(nameof(EM.TypeId))
                .Append(nameof(EM.Urn))
                .Append(nameof(EM.LocalAuthorityId))
                .Append(nameof(EM.EstablishmentNumber))
                .Append(nameof(EM.StatusId));
        }

        private async Task<ActionResult> SearchByUrnAsync(EstablishmentSearchViewModel model)
        {
            var urn = model.TextSearchModel.AutoSuggestValueAsInt
                      ?? (model.TextSearchType == EstablishmentSearchViewModel.eTextSearchType.URN
                          ? model.TextSearchModel.Text.ToInteger()
                          : null);

            if (urn.HasValue)
            {
                if ((await _establishmentReadService.CanAccess(urn.Value, User)).ReturnValue)
                    return RedirectToEstabDetail(urn.Value);
            }

            return null;
        }
    }
}