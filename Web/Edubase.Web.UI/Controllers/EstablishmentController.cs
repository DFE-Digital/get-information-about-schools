using AutoMapper;
using Edubase.Common.Reflection;
using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
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
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel = Edubase.Web.UI.Models.CreateEditEstablishmentModel;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Establishment")]
    public class EstablishmentController : EduBaseController
    {
        private IEstablishmentReadService _establishmentReadService;
        private IGroupReadService _groupReadService;
        private IMapper _mapper;
        private ILAESTABService _laEstabService;
        private IEstablishmentWriteService _establishmentWriteService;
        private ICachedLookupService _cachedLookupService;
        private IGovernorsReadService _governorsReadService;

        public EstablishmentController(IEstablishmentReadService establishmentReadService, 
            IGroupReadService groupReadService, IMapper mapper, 
            ILAESTABService laEstabService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService cachedLookupService,
            IGovernorsReadService governorsReadService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _mapper = mapper;
            _laEstabService = laEstabService;
            _establishmentWriteService = establishmentWriteService;
            _cachedLookupService = cachedLookupService;
            _governorsReadService = governorsReadService;
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue) return HttpNotFound();

            var domainModel = (await _establishmentReadService.GetAsync(id.Value, User)).GetResult();
            var viewModel = _mapper.Map<ViewModel>(domainModel);
            
            viewModel.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, domainModel);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, User);

            viewModel.AllowHidingOfAddress = User.InRole(EdubaseRoles.Admin, EdubaseRoles.IEBT);

            await PopulateSelectLists(viewModel);
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> Edit(ViewModel model)
        {
            var domainModel = (await _establishmentReadService.GetAsync(model.Urn.Value, User)).GetResult();
            model.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, domainModel);
            model.TabDisplayPolicy = new TabDisplayPolicy(domainModel, User);
            await PopulateSelectLists(model);

            if (model.Action == ViewModel.eAction.Save)
            {
                if (ModelState.IsValid)
                {
                    model.LSOAId = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.LSOACode)?.Id;
                    model.MSOAId = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.MSOACode)?.Id;

                    MapToDomainModel(model, domainModel, Request.Form);

                    domainModel.AdditionalAddresses = model.AdditionalAddresses;
                    domainModel.OpenDate = model.OpenDate.ToDateTime();
                    domainModel.CloseDate = model.CloseDate.ToDateTime();

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
        
        /// <summary>
        /// Transfers property values from the view model to the domain model
        /// where the value is passed into the form; i.e., the key is present.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="domainModel"></param>
        /// <param name="form"></param>
        private void MapToDomainModel(ViewModel viewModel, EstablishmentModel domainModel, NameValueCollection form)
        {
            var properties = ReflectionHelper.GetProperties(domainModel, writeableOnly: true);
            properties = properties.Where(x => form.AllKeys.Contains(x)).ToList();

            var viewModelProperties = ReflectionHelper.GetProperties(viewModel);

            foreach (var item in properties.Intersect(viewModelProperties))
            {
                var value = ReflectionHelper.GetPropertyValue(viewModel, item);
                ReflectionHelper.SetProperty(domainModel, item, value);
            }


        }
        
        [HttpGet, EdubaseAuthorize]
        public ActionResult Create() => RedirectToAction("Index", "Prototype", new { viewName = "Placeholder" });
        
        //[HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize]
        //public ActionResult Create([CustomizeValidator(RuleSet = "oncreate")] ViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (var dc = ApplicationDbContext.Create())
        //        {
        //            var dataModel = _mapper.Map<Establishment>(model);
        //            var pol = _laEstabService.GetEstabNumberEntryPolicy(dataModel.TypeId.Value, dataModel.EducationPhaseId.Value);
        //            if(pol == EstabNumberEntryPolicy.SystemGenerated)
        //            {
        //                dataModel.EstablishmentNumber = _laEstabService.GenerateEstablishmentNumber(dataModel.TypeId.Value, dataModel.EducationPhaseId.Value, dataModel.LocalAuthorityId.Value);
        //            }

        //            dc.Establishments.Add(dataModel);
        //            dc.SaveChanges();
        //            return RedirectToAction("Details", "Establishment", new { id = dataModel.Urn });
        //        }
        //    }
        //    else return View(model);
        //}

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

                using (MiniProfiler.Current.Step("Retrieving UserHasPendingApprovals flag"))
                    viewModel.UserHasPendingApprovals = new ApprovalService().Any(User as ClaimsPrincipal, id);
            }

            using (MiniProfiler.Current.Step("Retrieving Group record"))
                viewModel.Group = await _groupReadService.GetByEstablishmentUrnAsync(id);
            

            using (MiniProfiler.Current.Step("Retrieving HistoricalGovernors"))
                viewModel.HistoricalGovernors = await _governorsReadService.GetHistoricalByUrn(id);

            using (MiniProfiler.Current.Step("Retrieving Governors"))
                viewModel.Governors = await _governorsReadService.GetCurrentByUrn(id);

            using (MiniProfiler.Current.Step("Retrieving DisplayPolicy"))
                viewModel.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, viewModel.Establishment);

            using (MiniProfiler.Current.Step("Retrieving TabDisplayPolicy"))
                viewModel.TabDisplayPolicy = new TabDisplayPolicy(viewModel.Establishment, User);


            viewModel.UserCanEdit = ((ClaimsPrincipal)User).GetEditEstablishmentPermissions()
                .CanEdit(viewModel.Establishment.Urn.Value, 
                    viewModel.Establishment.TypeId,
                    viewModel.Group != null ? new [] { viewModel.Group.GroupUID.Value } : null as int[], 
                    viewModel.Establishment.LocalAuthorityId, 
                    viewModel.Establishment.EstablishmentTypeGroupId);

            return View(viewModel);
        }

        private async Task PopulateSelectLists(ViewModel viewModel)
        {
            viewModel.FurtherEducationTypes = (await _cachedLookupService.FurtherEducationTypesGetAllAsync()).ToSelectList(viewModel.FurtherEducationTypeId);
            viewModel.Genders = (await _cachedLookupService.GendersGetAllAsync()).ToSelectList(viewModel.GenderId);
            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.EstablishmentTypes = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).ToSelectList(viewModel.TypeId);
            viewModel.HeadTitles = (await _cachedLookupService.HeadTitlesGetAllAsync()).ToSelectList(viewModel.HeadTitleId);
            viewModel.Statuses = (await _cachedLookupService.EstablishmentStatusesGetAllAsync()).ToSelectList(viewModel.StatusId);
            viewModel.AdmissionsPolicies = (await _cachedLookupService.AdmissionsPoliciesGetAllAsync()).ToSelectList(viewModel.AdmissionsPolicyId);
            viewModel.Inspectorates = (await _cachedLookupService.InspectoratesGetAllAsync()).ToSelectList(viewModel.InspectorateId);
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
            viewModel.RSCRegionLocalAuthorites = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.RSCRegionId);
            viewModel.GovernmentOfficeRegions = (await _cachedLookupService.GovernmentOfficeRegionsGetAllAsync()).ToSelectList(viewModel.GovernmentOfficeRegionId);
            viewModel.AdministrativeDistricts = (await _cachedLookupService.AdministrativeDistrictsGetAllAsync()).ToSelectList(viewModel.AdministrativeDistrictId);
            viewModel.AdministrativeWards = (await _cachedLookupService.AdministrativeWardsGetAllAsync()).ToSelectList(viewModel.AdministrativeWardId);
            viewModel.ParliamentaryConstituencies = (await _cachedLookupService.ParliamentaryConstituenciesGetAllAsync()).ToSelectList(viewModel.ParliamentaryConstituencyId);
            viewModel.UrbanRuralLookup = (await _cachedLookupService.UrbanRuralGetAllAsync()).ToSelectList(viewModel.UrbanRuralId);
            viewModel.GSSLALookup = (await _cachedLookupService.GSSLAGetAllAsync()).ToSelectList(viewModel.GSSLAId);
            viewModel.CASWards = (await _cachedLookupService.CASWardsGetAllAsync()).ToSelectList(viewModel.CASWardId);

            if (viewModel.MSOAId.HasValue) viewModel.MSOACode = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Id == viewModel.MSOAId.Value)?.Code;
            if (viewModel.LSOAId.HasValue) viewModel.LSOACode = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Id == viewModel.LSOAId.Value)?.Code;
        }

    }
}