using AutoMapper;
using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services;
using Edubase.Services.Core;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Establishments;
using FluentValidation.Mvc;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel = Edubase.Web.UI.Models.EditEstablishmentModel;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Establishment")]
    public class EstablishmentController : EduBaseController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly IMapper _mapper;
        private readonly ILAESTABService _laEstabService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly IFileDownloadFactoryService _downloadService;

        public EstablishmentController(IEstablishmentReadService establishmentReadService, 
            IGroupReadService groupReadService, IMapper mapper, 
            ILAESTABService laEstabService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService cachedLookupService,
            IGovernorsReadService governorsReadService,
            IFileDownloadFactoryService downloadService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _mapper = mapper;
            _laEstabService = laEstabService;
            _establishmentWriteService = establishmentWriteService;
            _cachedLookupService = cachedLookupService;
            _governorsReadService = governorsReadService;
            _downloadService = downloadService;
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> EditDetails(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            ViewModel viewModel = await CreateEditViewModel(id);
            return View(viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Location")]
        public async Task<ActionResult> EditLocation(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.Location) throw new PermissionDeniedException();
            return View(viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/IEBT")]
        public async Task<ActionResult> EditIEBT(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.IEBT) throw new PermissionDeniedException();
            return View("EditIEBT", viewModel);
        }

        private async Task<ViewModel> CreateEditViewModel(int? id)
        {
            var domainModel = (await _establishmentReadService.GetAsync(id.Value, User)).GetResult();
            var viewModel = _mapper.Map<ViewModel>(domainModel);

            viewModel.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, domainModel);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, User);

            viewModel.AllowHidingOfAddress = User.InRole(EdubaseRoles.Admin, EdubaseRoles.IEBT);

            await PopulateSelectLists(viewModel);
            return viewModel;
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> EditDetails(ViewModel model)
        {
            return await SaveEstablishment(model);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/Location")]
        public async Task<ActionResult> EditLocation(ViewModel model)
        {
            return await SaveEstablishment(model);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/IEBT")]
        public async Task<ActionResult> EditIEBT(ViewModel model)
        {
            return await SaveEstablishment(model);
        }

        private async Task<ActionResult> SaveEstablishment(ViewModel model)
        {
            var domainModel = (await _establishmentReadService.GetAsync(model.Urn.Value, User)).GetResult();
            model.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, domainModel);
            model.TabDisplayPolicy = new TabDisplayPolicy(domainModel, User);
            await PopulateSelectLists(model);

            if (model.Action == ViewModel.eAction.SaveDetails || model.Action == ViewModel.eAction.SaveIEBT || model.Action == ViewModel.eAction.SaveLocation)
            {
                if (ModelState.IsValid)
                {
                    model.OriginalEstablishmentName = domainModel.Name;
                    await PrepareModels(model, domainModel);

                    var changes = await _establishmentReadService.GetModelChangesAsync(domainModel);

                    if (model.RequireConfirmationOfChanges && changes.Any()) model.ChangesSummary = changes;
                    else
                    {
                        await _establishmentWriteService.SaveAsync(domainModel, User);
                        return RedirectToAction("Details", "Establishment", new { id = model.Urn.Value });
                    }
                }
            }
            else if (model.Action == ViewModel.eAction.Confirm)
            {
                if (ModelState.IsValid)
                {
                    await PrepareModels(model, domainModel);
                    await _establishmentWriteService.SaveAsync(domainModel, User);
                    return RedirectToAction("Details", "Establishment", new { id = model.Urn.Value });
                }
            }
            else if (model.AddressToRemoveId.HasValue)
            {
                var item = model.AdditionalAddresses.FirstOrDefault(x => x.Id == model.AddressToRemoveId.Value);
                if (item != null)
                {
                    model.AdditionalAddresses.Remove(item);
                    model.AdditionalAddressesCount = model.AdditionalAddressesCount - 1;
                }

                ModelState.Clear();
            }
            else if (model.Action == ViewModel.eAction.AddAddress)
            {
                model.AdditionalAddressesCount = model.AdditionalAddressesCount + 1;
                model.AdditionalAddresses.Add(new AdditionalAddressModel());

                ModelState.Clear();
            }

            return View(model);
        }

        private async Task PrepareModels(ViewModel model, EstablishmentModel domainModel)
        {
            model.LSOAId = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.LSOACode)?.Id;
            model.MSOAId = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.MSOACode)?.Id;

            MapToDomainModel(model, domainModel, Request.Form);

            domainModel.AdditionalAddresses = model.AdditionalAddresses;
        }

        /// <summary>
        /// Transfers property values from the view model to the domain model
        /// where the value is passed into the form; i.e., the key is present.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="domainModel"></param>
        /// <param name="form"></param>
        private void MapToDomainModel(ViewModel viewModel, EstablishmentModel domainModel, NameValueCollection form)
        {
            var keys = form.AllKeys.Select(x => x.GetPart(".")).Distinct();

            var properties = ReflectionHelper.GetProperties(domainModel, writeableOnly: true);
            properties = properties.Where(x => keys.Contains(x)).ToList();

            var viewModelProperties = ReflectionHelper.GetProperties(viewModel);

            foreach (var item in properties.Intersect(viewModelProperties))
            {
                var info = ReflectionHelper.GetPropertyInfo(viewModel, item);
                if(info.Type == typeof(DateTimeViewModel))
                {
                    var value = (ReflectionHelper.GetPropertyValue(viewModel, item) as DateTimeViewModel).ToDateTime()?.Date;
                    ReflectionHelper.SetProperty(domainModel, item, value);
                }
                else
                {
                    var value = ReflectionHelper.GetPropertyValue(viewModel, item);
                    ReflectionHelper.SetProperty(domainModel, item, value);
                }
            }
        }
        
        [HttpGet, EdubaseAuthorize]
        public ActionResult Create() => RedirectToAction("Index", "Prototype", new { viewName = "Placeholder" });
        

        [HttpGet, Route("Details/{id}")]
        public async Task<ActionResult> Details(int id)
        {
            var viewModel = new EstablishmentDetailViewModel()
            {
                IsUserLoggedOn = User.Identity.IsAuthenticated
            };

            using (MiniProfiler.Current.Step("Retrieving establishment"))
            {
                var result = await _establishmentReadService.GetAsync(id, User);
                if (!result.Success) return HttpNotFound();
                viewModel.Establishment = result.ReturnValue;
            }
            

            using (MiniProfiler.Current.Step("Retrieving LinkedEstablishments"))
            {
                viewModel.LinkedEstablishments = (await _establishmentReadService.GetLinkedEstablishments(id)).Select(x => new LinkedEstabViewModel(x));
            }
            

            if (User.Identity.IsAuthenticated)
            {
                using (MiniProfiler.Current.Step("Retrieving ChangeHistory"))
                    viewModel.ChangeHistory = await _establishmentReadService.GetChangeHistoryAsync(id, 20, User);
            }

            using (MiniProfiler.Current.Step("Retrieving parent group records"))
                viewModel.Groups = await _groupReadService.GetAllByEstablishmentUrnAsync(id);
            
            using (MiniProfiler.Current.Step("Retrieving Governors Details"))
                viewModel.GovernorsDetails = new GovernorsGridViewModel(await _governorsReadService.GetGovernorListAsync(urn: id, principal: User));

            using (MiniProfiler.Current.Step("Retrieving DisplayPolicy"))
                viewModel.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, viewModel.Establishment);

            using (MiniProfiler.Current.Step("Retrieving TabDisplayPolicy"))
                viewModel.TabDisplayPolicy = new TabDisplayPolicy(viewModel.Establishment, User);


            viewModel.UserCanEdit = ((ClaimsPrincipal)User).GetEditEstablishmentPermissions()
                .CanEdit(viewModel.Establishment.Urn.Value,
                    viewModel.Establishment.TypeId,
                    viewModel.Groups.Select(x => x.GroupUID.Value).ToArray(),
                    viewModel.Establishment.LocalAuthorityId,
                    viewModel.Establishment.EstablishmentTypeGroupId);

            return View(viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Download/ChangeHistory/csv/{id}")]
        public async Task<ActionResult> DownloadCsvChangeHistory(int id)
        {
            var model = (await _establishmentReadService.GetAsync(id, User)).GetResult();
            var data = await GetChangeHistoryDownloadDataAsync(id);
            var csvStream = await _downloadService.CreateCsvStreamAsync(data.Item1, data.Item2);
            return File(csvStream, "text/csv", string.Concat(model.Name, $"({id})", "-change-history.csv"));
        }

        
        [HttpGet, EdubaseAuthorize, Route("Download/ChangeHistory/xlsx/{id}")]
        public async Task<ActionResult> DownloadXlsxChangeHistory(int id)
        {
            var model = (await _establishmentReadService.GetAsync(id, User)).GetResult();
            var data = await GetChangeHistoryDownloadDataAsync(id);
            var xlsxStream = _downloadService.CreateXlsxStream($"Change history for {model.Name} ({model.Urn})", $"Change history for {model.Name} ({model.Urn})", data.Item1, data.Item2);
            return File(xlsxStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                string.Concat(model.Name, $"({id})", "-change-history.xlsx"));
        }

        private async Task<Tuple<List<string>, List<List<string>>>> GetChangeHistoryDownloadDataAsync(int urn)
        {
            var headers = new List<string>
            {
                "Updated field",
                "New value",
                "Old value",
                "Date changed",
                "Effective date",
                "Date requested",
                "Suggested by",
                "Approved by",
                "Reason"
            };

            var changes = await _establishmentReadService.GetChangeHistoryAsync(urn, 200, User);

            var data = changes.Select(x => new List<string>
            {
                x.Name,
                x.NewValue,
                x.OldValue,
                x.RequestedDateUtc?.ToString("dd/MM/yyyy"),
                x.EffectiveDateUtc?.ToString("dd/MM/yyyy"),
                x.RequestedDateUtc?.ToString("dd/MM/yyyy"),
                x.OriginatorUserName,
                string.Empty, 
                string.Empty
            }).ToList();

            return new Tuple<List<string>, List<List<string>>>(headers, data);
        }

        private async Task PopulateSelectLists(ViewModel viewModel)
        {
            viewModel.AccommodationChanges = (await _cachedLookupService.AccommodationChangedGetAllAsync()).ToSelectList(viewModel.AccommodationChangedId);
            viewModel.FurtherEducationTypes = (await _cachedLookupService.FurtherEducationTypesGetAllAsync()).ToSelectList(viewModel.FurtherEducationTypeId);
            viewModel.Genders = (await _cachedLookupService.GendersGetAllAsync()).ToSelectList(viewModel.GenderId);
            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.EstablishmentTypes = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).ToSelectList(viewModel.TypeId);
            viewModel.HeadTitles = (await _cachedLookupService.HeadTitlesGetAllAsync()).ToSelectList(viewModel.HeadTitleId);
            viewModel.Statuses = (await _cachedLookupService.EstablishmentStatusesGetAllAsync()).ToSelectList(viewModel.StatusId);
            viewModel.AdmissionsPolicies = (await _cachedLookupService.AdmissionsPoliciesGetAllAsync()).ToSelectList(viewModel.AdmissionsPolicyId);
            viewModel.Inspectorates = (await _cachedLookupService.InspectoratesGetAllAsync()).ToSelectList(viewModel.InspectorateId);
            viewModel.IndependentSchoolTypes = (await _cachedLookupService.IndependentSchoolTypesGetAllAsync()).ToSelectList(viewModel.IndependentSchoolTypeId);
            viewModel.BSOInspectorates = (await _cachedLookupService.InspectorateNamesGetAllAsync()).ToSelectList(viewModel.BSOInspectorateId);
            viewModel.ReligiousCharacters = (await _cachedLookupService.ReligiousCharactersGetAllAsync()).ToSelectList(viewModel.ReligiousCharacterId);
            viewModel.ReligiousEthoses = (await _cachedLookupService.ReligiousEthosGetAllAsync()).ToSelectList(viewModel.ReligiousEthosId);
            viewModel.Dioceses = (await _cachedLookupService.DiocesesGetAllAsync()).ToSelectList(viewModel.DioceseId);
            viewModel.BoardingProvisions = (await _cachedLookupService.ProvisionBoardingGetAllAsync()).ToSelectList(viewModel.ProvisionBoardingId);
            viewModel.NurseryProvisions = (await _cachedLookupService.ProvisionNurseriesGetAllAsync()).ToSelectList(viewModel.ProvisionNurseryId);
            viewModel.OfficialSixthFormProvisions = (await _cachedLookupService.ProvisionOfficialSixthFormsGetAllAsync()).ToSelectList(viewModel.ProvisionOfficialSixthFormId);
            viewModel.Section41ApprovedItems = (await _cachedLookupService.Section41ApprovedGetAllAsync()).ToSelectList(viewModel.Section41ApprovedId);
            viewModel.EducationPhases = (await _cachedLookupService.EducationPhasesGetAllAsync()).ToSelectList(viewModel.EducationPhaseId);
            viewModel.ReasonsEstablishmentOpened = (await _cachedLookupService.ReasonEstablishmentOpenedGetAllAsync()).ToSelectList(viewModel.ReasonEstablishmentOpenedId);
            viewModel.ReasonsEstablishmentClosed = (await _cachedLookupService.ReasonEstablishmentClosedGetAllAsync()).ToSelectList(viewModel.ReasonEstablishmentClosedId);
            viewModel.SpecialClassesProvisions = (await _cachedLookupService.ProvisionSpecialClassesGetAllAsync()).ToSelectList(viewModel.ProvisionSpecialClassesId);
            viewModel.SENProvisions1 = (await _cachedLookupService.SpecialEducationNeedsGetAllAsync()).ToSelectList(viewModel.SEN1Id);
            viewModel.SENProvisions2 = (await _cachedLookupService.SpecialEducationNeedsGetAllAsync()).ToSelectList(viewModel.SEN2Id);
            viewModel.SENProvisions3 = (await _cachedLookupService.SpecialEducationNeedsGetAllAsync()).ToSelectList(viewModel.SEN3Id);
            viewModel.SENProvisions4 = (await _cachedLookupService.SpecialEducationNeedsGetAllAsync()).ToSelectList(viewModel.SEN4Id);
            viewModel.TypeOfResourcedProvisions = (await _cachedLookupService.TypeOfResourcedProvisionsGetAllAsync()).ToSelectList(viewModel.TypeOfResourcedProvisionId);
            viewModel.TeenageMothersProvisions = (await _cachedLookupService.TeenageMothersProvisionsGetAllAsync()).ToSelectList(viewModel.TeenageMothersProvisionId);
            viewModel.ChildcareFacilitiesProvisions = (await _cachedLookupService.ChildcareFacilitiesGetAllAsync()).ToSelectList(viewModel.ChildcareFacilitiesId);
            viewModel.RSCRegionLocalAuthorites = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.RSCRegionId);
            viewModel.GovernmentOfficeRegions = (await _cachedLookupService.GovernmentOfficeRegionsGetAllAsync()).ToSelectList(viewModel.GovernmentOfficeRegionId);
            viewModel.AdministrativeDistricts = (await _cachedLookupService.AdministrativeDistrictsGetAllAsync()).ToSelectList(viewModel.AdministrativeDistrictId);
            viewModel.AdministrativeWards = (await _cachedLookupService.AdministrativeWardsGetAllAsync()).ToSelectList(viewModel.AdministrativeWardId);
            viewModel.ParliamentaryConstituencies = (await _cachedLookupService.ParliamentaryConstituenciesGetAllAsync()).ToSelectList(viewModel.ParliamentaryConstituencyId);
            viewModel.UrbanRuralLookup = (await _cachedLookupService.UrbanRuralGetAllAsync()).ToSelectList(viewModel.UrbanRuralId);
            viewModel.GSSLALookup = (await _cachedLookupService.GSSLAGetAllAsync()).ToSelectList(viewModel.GSSLAId);
            viewModel.CASWards = (await _cachedLookupService.CASWardsGetAllAsync()).ToSelectList(viewModel.CASWardId);
            viewModel.PruFulltimeProvisionOptions = (await _cachedLookupService.PruFulltimeProvisionsGetAllAsync()).ToSelectList(viewModel.PruFulltimeProvisionId);
            viewModel.PruEducatedByOthersOptions = (await _cachedLookupService.PruEducatedByOthersGetAllAsync()).ToSelectList(viewModel.PruEducatedByOthersId);
            viewModel.PRUEBDOptions = (await _cachedLookupService.PRUEBDsGetAllAsync()).ToSelectList(viewModel.PRUEBDId);
            viewModel.PRUSENOptions = (await _cachedLookupService.PRUSENsGetAllAsync()).ToSelectList(viewModel.PRUSENId);

            if (viewModel.MSOAId.HasValue) viewModel.MSOACode = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Id == viewModel.MSOAId.Value)?.Code;
            if (viewModel.LSOAId.HasValue) viewModel.LSOACode = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Id == viewModel.LSOAId.Value)?.Code;
        }

    }
}