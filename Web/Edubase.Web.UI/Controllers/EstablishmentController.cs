using AutoMapper;
using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services.Core;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Web.Resources;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Establishments;
using StackExchange.Profiling;
using System;
using System.Collections.Specialized;
using System.Linq;
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
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly IFileDownloadFactoryService _downloadService;
        private readonly NomenclatureService _nomenclatureService;
        private readonly IResourcesHelper _resourcesHelper;

        public EstablishmentController(IEstablishmentReadService establishmentReadService, 
            IGroupReadService groupReadService, IMapper mapper, 
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService cachedLookupService,
            IGovernorsReadService governorsReadService,
            IFileDownloadFactoryService downloadService,
            NomenclatureService nomenclatureService,
            IResourcesHelper resourcesHelper)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _mapper = mapper;
            _establishmentWriteService = establishmentWriteService;
            _cachedLookupService = cachedLookupService;
            _governorsReadService = governorsReadService;
            _downloadService = downloadService;
            _nomenclatureService = nomenclatureService;
            _resourcesHelper = resourcesHelper;
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> EditDetails(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            ViewModel viewModel = await CreateEditViewModel(id);
            viewModel.SelectedTab = "details";
            return View(viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Location")]
        public async Task<ActionResult> EditLocation(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.Location) throw new PermissionDeniedException();
            viewModel.SelectedTab = "location";
            return View(viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/IEBT")]
        public async Task<ActionResult> EditIEBT(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.IEBT) throw new PermissionDeniedException();
            viewModel.SelectedTab = "iebt";
            return View("EditIEBT", viewModel);
        }
        
        private async Task<ViewModel> CreateEditViewModel(int? id)
        {
            var domainModel = (await _establishmentReadService.GetAsync(id.Value, User)).GetResult();
            var viewModel = _mapper.Map<ViewModel>(domainModel);

            viewModel.DisplayPolicy = await _establishmentReadService.GetDisplayPolicyAsync(User, domainModel);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, viewModel.DisplayPolicy, User);

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
            model.DisplayPolicy = await _establishmentReadService.GetDisplayPolicyAsync(User, domainModel);
            model.TabDisplayPolicy = new TabDisplayPolicy(domainModel, model.DisplayPolicy, User);
            await PopulateSelectLists(model);

            if (model.Action == ViewModel.eAction.SaveDetails || model.Action == ViewModel.eAction.SaveIEBT || model.Action == ViewModel.eAction.SaveLocation)
            {
                if (ModelState.IsValid)
                {
                    model.OriginalEstablishmentName = domainModel.Name;
                    await PrepareModels(model, domainModel);

                    var changes = await _establishmentReadService.GetModelChangesAsync(domainModel, User);

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

            return View(model);
        }

        private async Task PrepareModels(ViewModel model, EstablishmentModel domainModel)
        {
            model.LSOAId = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.LSOACode)?.Id;
            model.MSOAId = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.MSOACode)?.Id;

            MapToDomainModel(model, domainModel, Request.Form);
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
                viewModel.LinkedEstablishments = (await _establishmentReadService.GetLinkedEstablishmentsAsync(id, User)).Select(x => new LinkedEstabViewModel(x));
                foreach (var item in viewModel.LinkedEstablishments)
	            {
                    item.LinkTypeName = await _cachedLookupService.GetNameAsync(() => item.LinkTypeId);
                }
            }
            

            if (User.Identity.IsAuthenticated)
            {
                using (MiniProfiler.Current.Step("Retrieving ChangeHistory"))
                    viewModel.ChangeHistory = await _establishmentReadService.GetChangeHistoryAsync(id, 20, User);
            }

            using (MiniProfiler.Current.Step("Retrieving parent group records"))
                viewModel.Groups = await _groupReadService.GetAllByEstablishmentUrnAsync(id, User);
            
            using (MiniProfiler.Current.Step("Retrieving DisplayPolicy"))
                viewModel.DisplayPolicy = await _establishmentReadService.GetDisplayPolicyAsync(User, viewModel.Establishment);

            using (MiniProfiler.Current.Step("Retrieving TabDisplayPolicy"))
                viewModel.TabDisplayPolicy = new TabDisplayPolicy(viewModel.Establishment, viewModel.DisplayPolicy, User);

            viewModel.UserCanEdit = await _establishmentReadService.CanEditAsync(viewModel.Establishment.Urn.Value, User);

            // Retrieve the lookup name values
            await PopulateLookupNames(viewModel);

            viewModel.AgeRangeToolTip = _resourcesHelper.GetResourceStringForEstablishment("AgeRange", (eLookupEstablishmentTypeGroup?) viewModel.Establishment.EstablishmentTypeGroupId, User);
            viewModel.AgeRangeToolTipLink = _resourcesHelper.GetResourceStringForEstablishment("AgeRangeLink", (eLookupEstablishmentTypeGroup?)viewModel.Establishment.EstablishmentTypeGroupId, User);
            viewModel.SchoolCapacityToolTip = _resourcesHelper.GetResourceStringForEstablishment("SchoolCapacity", (eLookupEstablishmentTypeGroup?)viewModel.Establishment.EstablishmentTypeGroupId, User);
            viewModel.SchoolCapacityToolTipLink = _resourcesHelper.GetResourceStringForEstablishment("SchoolCapacityLink", (eLookupEstablishmentTypeGroup?)viewModel.Establishment.EstablishmentTypeGroupId, User);

            return View(viewModel);
        }


        [HttpGet, EdubaseAuthorize, Route("Download/ChangeHistory/csv/{id}")]
        public async Task<ActionResult> DownloadCsvChangeHistory(int id)
        {
            // todo: TEXCHANGE; wating for API: 
            throw new NotImplementedException("Not done yet; requires API '/establishment/{urn}/changes/download'");
        }

        
        [HttpGet, EdubaseAuthorize, Route("Download/ChangeHistory/xlsx/{id}")]
        public async Task<ActionResult> DownloadXlsxChangeHistory(int id)
        {
            // todo: TEXCHANGE; wating for API
            throw new NotImplementedException("Not done yet; requires API '/establishment/{urn}/changes/download'");
        }
        
        private async Task PopulateSelectLists(ViewModel viewModel)
        {
            viewModel.AccommodationChanges = (await _cachedLookupService.AccommodationChangedGetAllAsync()).ToSelectList(viewModel.AccommodationChangedId);
            viewModel.FurtherEducationTypes = (await _cachedLookupService.FurtherEducationTypesGetAllAsync()).ToSelectList(viewModel.FurtherEducationTypeId);
            viewModel.Genders = (await _cachedLookupService.GendersGetAllAsync()).ToSelectList(viewModel.GenderId);
            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.EstablishmentTypes = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).ToSelectList(viewModel.TypeId);
            viewModel.HeadTitles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.HeadTitleId);
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

        private async Task PopulateLookupNames(EstablishmentDetailViewModel vm)
        {
            var c = _cachedLookupService;
            vm.ReligiousCharacterName = await c.GetNameAsync(() => vm.Establishment.ReligiousCharacterId);
            vm.DioceseName = await c.GetNameAsync(() => vm.Establishment.DioceseId);
            vm.ReligiousEthosName = await c.GetNameAsync(() => vm.Establishment.ReligiousEthosId);
            vm.ProvisionBoardingName = await c.GetNameAsync(() => vm.Establishment.ProvisionBoardingId);
            vm.ProvisionNurseryName = await c.GetNameAsync(() => vm.Establishment.ProvisionNurseryId);
            vm.ProvisionOfficialSixthFormName = await c.GetNameAsync(() => vm.Establishment.ProvisionOfficialSixthFormId);
            vm.Section41ApprovedName = await c.GetNameAsync(() => vm.Establishment.Section41ApprovedId);
            vm.ReasonEstablishmentOpenedName = await c.GetNameAsync(() => vm.Establishment.ReasonEstablishmentOpenedId);
            vm.ReasonEstablishmentClosedName = await c.GetNameAsync(() => vm.Establishment.ReasonEstablishmentClosedId);
            vm.CCOperationalHoursName = await c.GetNameAsync(() => vm.Establishment.CCOperationalHoursId);
            vm.CCGovernanceName = await c.GetNameAsync(() => vm.Establishment.CCGovernanceId);
            vm.CCDeliveryModelName = await c.GetNameAsync(() => vm.Establishment.CCDeliveryModelId);
            vm.CCGroupLeadName = await c.GetNameAsync(() => vm.Establishment.CCGroupLeadId);
            vm.CCPhaseTypeName = await c.GetNameAsync(() => vm.Establishment.CCPhaseTypeId);
            vm.CCDisadvantagedAreaName = await c.GetNameAsync(() => vm.Establishment.CCDisadvantagedAreaId);
            vm.CCDirectProvisionOfEarlyYearsName = await c.GetNameAsync(() => vm.Establishment.CCDirectProvisionOfEarlyYearsId);
            vm.ProvisionSpecialClassesName = await c.GetNameAsync(() => vm.Establishment.ProvisionSpecialClassesId);
            //vm.SEN1Name = await c.GetNameAsync(() => vm.Establishment.SEN1Id); // TODO: TEXCHANGE: SUPPORT COMBINED PROPERTY
            //vm.SEN2Name = await c.GetNameAsync(() => vm.Establishment.SEN2Id);
            //vm.SEN3Name = await c.GetNameAsync(() => vm.Establishment.SEN3Id);
            //vm.SEN4Name = await c.GetNameAsync(() => vm.Establishment.SEN4Id);
            vm.TeenageMothersProvisionName = await c.GetNameAsync(() => vm.Establishment.TeenageMothersProvisionId);
            vm.ChildcareFacilitiesName = await c.GetNameAsync(() => vm.Establishment.ChildcareFacilitiesId);
            vm.PRUSENName = await c.GetNameAsync(() => vm.Establishment.PRUSENId);
            vm.PRUEBDName = await c.GetNameAsync(() => vm.Establishment.PRUEBDId);
            vm.PruFulltimeProvisionName = await c.GetNameAsync(() => vm.Establishment.PruFulltimeProvisionId);
            vm.PruEducatedByOthersName = await c.GetNameAsync(() => vm.Establishment.PruEducatedByOthersId);
            vm.TypeOfResourcedProvisionName = await c.GetNameAsync(() => vm.Establishment.TypeOfResourcedProvisionId);
            vm.BSOInspectorateName = await c.GetNameAsync(() => vm.Establishment.BSOInspectorateId);
            vm.InspectorateName = await c.GetNameAsync(() => vm.Establishment.InspectorateId);
            vm.IndependentSchoolTypeName = await c.GetNameAsync(() => vm.Establishment.IEBTModel.IndependentSchoolTypeId);
            vm.RSCRegionName = await c.GetNameAsync(() => vm.Establishment.RSCRegionId);
            vm.GovernmentOfficeRegionName = await c.GetNameAsync(() => vm.Establishment.GovernmentOfficeRegionId);
            vm.AdministrativeDistrictName = await c.GetNameAsync(() => vm.Establishment.AdministrativeDistrictId);
            vm.AdministrativeWardName = await c.GetNameAsync(() => vm.Establishment.AdministrativeWardId);
            vm.ParliamentaryConstituencyName = await c.GetNameAsync(() => vm.Establishment.ParliamentaryConstituencyId);
            vm.UrbanRuralName = await c.GetNameAsync(() => vm.Establishment.UrbanRuralId);
            vm.GSSLAName = await c.GetNameAsync(() => vm.Establishment.GSSLAId);
            vm.CASWardName = await c.GetNameAsync(() => vm.Establishment.CASWardId);
            vm.MSOAName = await c.GetNameAsync(() => vm.Establishment.MSOAId);
            vm.LSOAName = await c.GetNameAsync(() => vm.Establishment.LSOAId);
            vm.LocalAuthorityName = await c.GetNameAsync(() => vm.Establishment.LocalAuthorityId);
            vm.HeadTitleName = await c.GetNameAsync(() => vm.Establishment.HeadTitleId);
            vm.EducationPhaseName = await c.GetNameAsync(() => vm.Establishment.EducationPhaseId);
            vm.TypeName = await c.GetNameAsync(() => vm.Establishment.TypeId);
            vm.FurtherEducationTypeName = await c.GetNameAsync(() => vm.Establishment.FurtherEducationTypeId);
            vm.GenderName = await c.GetNameAsync(() => vm.Establishment.GenderId);
            vm.StatusName = await c.GetNameAsync(() => vm.Establishment.StatusId);
            vm.AdmissionsPolicyName = await c.GetNameAsync(() => vm.Establishment.AdmissionsPolicyId);
        }

    }
}