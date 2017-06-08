using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models;
using FluentValidation.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Groups.Downloads;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    using Common;
    using Exceptions;
    using Filters;
    using Models.CreateEdit;
    using Models.Validators;
    using Services.Domain;
    using Services.Governors;
    using Services.Groups.Models;
    using Services.IntegrationEndPoints.CompaniesHouse;
    using StackExchange.Profiling;
    using UI.Models;
    using GT = Services.Enums.eLookupGroupType;
    using Services.Exceptions;
    using static Models.CreateEdit.GroupEditorViewModelBase;
    using static Models.CreateEdit.GroupEditorViewModel;
    using Services.Enums;
    using Governors.Models;
    using Services.Nomenclature;
    using Services.Governors.Models;

    [RouteArea("Groups"), RoutePrefix("Group")]
    public class GroupController : Controller
    {
        private readonly ICachedLookupService _lookup;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly ISecurityService _securityService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly ICompaniesHouseService _companiesHouseService;
        private readonly IGroupDownloadService _groupDownloadService;
        private readonly NomenclatureService _nomenclatureService;
        
        public GroupController(
            ICachedLookupService cachedLookupService, 
            ISecurityService securityService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            IGovernorsReadService governorsReadService,
            IGroupsWriteService groupWriteService,
            ICompaniesHouseService companiesHouseService,
            IGroupDownloadService groupDownloadService,
            NomenclatureService nomenclatureService)
        {
            _lookup = cachedLookupService;
            _securityService = securityService;
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _governorsReadService = governorsReadService;
            _groupWriteService = groupWriteService;
            _companiesHouseService = companiesHouseService;
            _nomenclatureService = nomenclatureService;
            _groupDownloadService = groupDownloadService;
        }


        [Route(nameof(Details) + "/{id:int}"), HttpGet]
        public async Task<ActionResult> Details(int id, string searchQueryString = "", eLookupSearchSource searchSource = eLookupSearchSource.Groups)
        {
            var model = (await _groupReadService.GetAsync(id, User)).GetResult();

            var viewModel = new GroupDetailViewModel
            {
                SearchQueryString = searchQueryString,
                SearchSource = searchSource,
                CanUserEdit = await _groupReadService.CanEditAsync(id, User),
                Group = model,
                GroupTypeName = model.GroupTypeId.HasValue ? await _lookup.GetNameAsync(() => model.GroupTypeId) : null,
                LocalAuthorityName = model.LocalAuthorityId.HasValue ? await _lookup.GetNameAsync(() => model.LocalAuthorityId) : null,
                GroupStatusName = model.StatusId.HasValue ? await _lookup.GetNameAsync(() => model.StatusId, "Group") : null,
                Address = model.GroupTypeId.OneOfThese(GT.SingleacademyTrust, GT.MultiacademyTrust, GT.ChildrensCentresGroup) ? model.Address.ToString() : null,
                IsUserLoggedOn = User.Identity.IsAuthenticated
            };

            if (viewModel.IsUserLoggedOn)
            {
                using (MiniProfiler.Current.Step("Retrieving change history"))
                    viewModel.ChangeHistory = await _groupReadService.GetChangeHistoryAsync(id, 20, User);
            }

            await PopulateEstablishmentList(viewModel.Establishments, model.GroupUId.Value);
            
            return View(viewModel);
        }

        [HttpGet]
        [Route("Create/{type}")]
        public async Task<ActionResult> CreateNewGroup(string type)
        {
            var groupTypeMode = StringUtil.ToEnum<eGroupTypeMode>(type);
            Guard.IsTrue(groupTypeMode.HasValue, () => new InvalidParameterException($"Invalid type parameter supplied"));

            var viewModel = await PopulateSelectLists(new GroupEditorViewModel(eSaveMode.Details));

            if (groupTypeMode == eGroupTypeMode.ChildrensCentre)
            {
                viewModel.GroupTypeId = (int)GT.ChildrensCentresCollaboration;
                viewModel.SaveMode = eSaveMode.DetailsAndLinks;
            }
            else if (groupTypeMode == eGroupTypeMode.Federation) viewModel.GroupTypeId = (int)GT.Federation;
            else if (groupTypeMode == eGroupTypeMode.Trust) viewModel.GroupTypeId = (int)GT.Trust;
            else if (groupTypeMode == eGroupTypeMode.Sponsor) viewModel.GroupTypeId = (int)GT.SchoolSponsor;
            
            var permission = await _securityService.GetCreateGroupPermissionAsync(User);
            if (!permission.GroupTypes.Any(x => x == viewModel.GroupType.Value))
            {
                throw new PermissionDeniedException("Current principal does not have permission to create a group of this type.");
            }

            if (viewModel.GroupTypeMode == eGroupTypeMode.ChildrensCentre)
            {
                if (permission.CCLocalAuthorityId.HasValue)
                {
                    viewModel.IsLocalAuthorityEditable = false;
                    viewModel.LocalAuthorityId = permission.CCLocalAuthorityId;
                    viewModel.LocalAuthorityName = await _lookup.GetNameAsync(() => viewModel.LocalAuthorityId);
                }
                else viewModel.IsLocalAuthorityEditable = true;
            }
            
            return View("Create", viewModel);
        }
        

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult> Create(GroupEditorViewModel viewModel)
        {
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, User, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);
            
            await PopulateSelectLists(viewModel);

            if (ModelState.IsValid)
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null) return actionResult;
            }

            return View("Create", viewModel);
        }


        [HttpGet]
        [Route("Edit/{id:int}/Details")]
        public async Task<ActionResult> EditDetails(int id)
        {
            var domainModel = (await _groupReadService.GetAsync(id, User)).GetResult();
            var viewModel = new GroupEditorViewModel(eSaveMode.Details);
            viewModel.Address = domainModel.Address.ToString();
            viewModel.ClosedDate = new DateTimeViewModel(domainModel.ClosedDate);
            viewModel.OpenDate = new DateTimeViewModel(domainModel.OpenDate);
            viewModel.LocalAuthorityId = domainModel.LocalAuthorityId;
            viewModel.GroupStatusId = domainModel.StatusId;
            viewModel.GroupTypeId = domainModel.GroupTypeId;
            viewModel.GroupManagerEmailAddress = domainModel.ManagerEmailAddress;
            viewModel.GroupName = domainModel.Name;
            viewModel.CompaniesHouseNumber = domainModel.CompaniesHouseNumber;
            viewModel.GroupUId = domainModel.GroupUId;
            viewModel.GroupId = domainModel.GroupId;
            viewModel.SelectedTabName = "details";
            viewModel.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((GT)viewModel.GroupTypeId.Value);

            await PopulateEstablishmentList(viewModel.LinkedEstablishments.Establishments, id, true);
            await PopulateSelectLists(viewModel);

            viewModel.LocalAuthorityName = await _lookup.GetNameAsync(() => viewModel.LocalAuthorityId);
            viewModel.DeriveCCLeadCentreUrn();

            if (viewModel.GroupTypeId.HasValue) viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => viewModel.GroupTypeId));

            return View("EditDetails", viewModel);
        }

        [HttpPost]
        [Route("Edit/{id:int}/Details")]
        public async Task<ActionResult> EditDetails(GroupEditorViewModel viewModel)
        {
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, User, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);

            await PopulateSelectLists(viewModel);
            if (viewModel.GroupTypeId.HasValue) viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => viewModel.GroupTypeId));

            await ValidateAsync(viewModel);

            if (ModelState.IsValid)
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null) return actionResult;
            }

            viewModel.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((GT)viewModel.GroupTypeId.Value);

            return View("EditDetails", viewModel);
        }

        /// <summary>
        /// Does 2nd-level validation
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task ValidateAsync(GroupEditorViewModel viewModel)
        {
            if (viewModel.Action == ActionSave && ModelState.IsValid)
            {
                var dto = CreateSaveDto(viewModel);
                var validationEnvelope = await _groupWriteService.ValidateAsync(dto, User);
                validationEnvelope.Warnings.ForEach(x => ModelState.AddModelError(x.Fields, x.Message));
                validationEnvelope.Errors.ForEach(x => ModelState.AddModelError(x.Fields, x.Message));
            }
        }

        [HttpGet]
        [Route("Edit/{id:int}/Links")]
        public async Task<ActionResult> EditLinks(int id)
        {
            var domainModel = (await _groupReadService.GetAsync(id, User)).GetResult();
            var viewModel = new GroupEditorViewModel
            {
                SaveMode = eSaveMode.Links,
                GroupName = domainModel.Name,
                GroupUId = domainModel.GroupUId,
                GroupTypeId = domainModel.GroupTypeId
            };
            
            await PopulateEstablishmentList(viewModel.LinkedEstablishments.Establishments, id, true);
            viewModel.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((GT)viewModel.GroupTypeId.Value);
            viewModel.SelectedTabName = "links";

            viewModel.DeriveCCLeadCentreUrn();

            if (viewModel.GroupTypeId.HasValue) viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => viewModel.GroupTypeId));

            return View(viewModel);
        }

        [HttpPost]
        [Route("Edit/{id:int}/Links")]
        public async Task<ActionResult> EditLinks(GroupEditorViewModel viewModel)
        {
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, User, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);

            await ValidateAsync(viewModel);

            if (ModelState.IsValid)
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null) return actionResult;
            }

            return View(viewModel);
        }
        

        [EdubaseAuthorize, Route(nameof(SearchCompaniesHouse))]
        public async Task<ActionResult> SearchCompaniesHouse(SearchCompaniesHouseModel viewModel)
        {
            if (!viewModel.SearchText.IsNullOrEmpty())
            {
                if (viewModel.SearchText.IsInteger()) viewModel.Results = await _companiesHouseService.SearchByCompaniesHouseNumber(viewModel.SearchText);
                else viewModel.Results = await _companiesHouseService.SearchByName(viewModel.SearchText, viewModel.StartIndex, viewModel.PageSize);
                viewModel.NotFound = !viewModel.Results.Items.Any();
            }
            return View(viewModel);
        }


        [HttpGet, EdubaseAuthorize, Route(nameof(CreateAcademyTrust) + "/{companiesHouseNumber}")]
        public async Task<ActionResult> CreateAcademyTrust(string companiesHouseNumber)
        {
            if (string.IsNullOrWhiteSpace(companiesHouseNumber)) return HttpNotFound();

            var companyProfile = await _companiesHouseService.SearchByCompaniesHouseNumber(companiesHouseNumber);
            var groupTypes = await GetAcademyTrustGroupTypes();

            var vm = new CreateAcademyTrustViewModel(companyProfile.Items.First(), groupTypes);
            vm.TrustExists = await _groupReadService.ExistsAsync(User, companiesHouseNumber: CompaniesHouseNumber.Parse(companiesHouseNumber));
            
            if (vm.Address == null) ModelState.AddModelError("", "This company record doesn't have an address");
            if (!vm.OpenDate.HasValue) ModelState.AddModelError("", "This company record doesn't have an incorporation date");

            vm.AllowSave = !vm.TrustExists && ModelState.IsValid;

            return View(vm);
        }

        [HttpPost, EdubaseAuthorize, Route(nameof(CreateAcademyTrust) + "/{companiesHouseNumber}")]
        public async Task<ActionResult> SaveNewAcademyTrust(CreateAcademyTrustViewModel viewModel)
        {
            var dto = new SaveGroupDto(new GroupModel
            {
                Address = UriHelper.DeserializeUrlToken<AddressDto>(viewModel.CompaniesHouseAddressToken),
                CompaniesHouseNumber = viewModel.CompaniesHouseNumber,
                GroupTypeId = viewModel.TypeId,
                Name = viewModel.Name,
                OpenDate = viewModel.OpenDate,
                StatusId = (int)eLookupGroupStatus.Open,
                GroupId = viewModel.GroupId
            });
            
            var validationEnvelope = await _groupWriteService.ValidateAsync(dto, User);
            validationEnvelope.Warnings.ForEach(x => ModelState.AddModelError(x.Fields, x.Message));
            validationEnvelope.Errors.ForEach(x => ModelState.AddModelError(x.Fields, x.Message));

            if (ModelState.IsValid)
            {
                var groupUId = await _groupWriteService.SaveAsync(dto, User);
                return RedirectToAction(nameof(Details), new { id = groupUId });
            }
            else viewModel.GroupTypes = await GetAcademyTrustGroupTypes(viewModel.TypeId);

            return View("CreateAcademyTrust", viewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetAcademyTrustGroupTypes(int? typeId = null) 
            => (await _lookup.GroupTypesGetAllAsync()).Where(x => x.Id == (int)GT.MultiacademyTrust || x.Id == (int)GT.SingleacademyTrust).ToSelectList(typeId);
        
        private async Task<ActionResult> ProcessCreateEditGroup(GroupEditorViewModel viewModel)
        {
            var suppressClearModelState = false;

            if (viewModel.Action == ActionLinkedEstablishmentAdd)
            {
                await AddLinkedEstablishment(viewModel);
            }
            else if (viewModel.Action == ActionLinkedEstablishmentCancelEdit)
            {
                viewModel.LinkedEstablishments.Establishments.ForEach(x => x.EditMode = false);
            }
            else if (viewModel.Action.StartsWith(ActionLinkedEstablishmentEdit, StringComparison.OrdinalIgnoreCase))
            {
                var m = viewModel.LinkedEstablishments.Establishments.Single(x => x.Urn == viewModel.ActionUrn);
                m.SetEditMode().JoinedDateEditable = new DateTimeViewModel(m.JoinedDate);
            }
            else if (viewModel.Action.StartsWith(ActionLinkedEstablishmentRemove, StringComparison.OrdinalIgnoreCase))
            {
                var index = viewModel.LinkedEstablishments.Establishments.FindIndex(x => x.Urn == viewModel.ActionUrn);
                if (index >= 0) viewModel.LinkedEstablishments.Establishments.RemoveAt(index);
            }
            else if (viewModel.Action == ActionLinkedEstablishmentSave)
            {
                var model = viewModel.LinkedEstablishments.Establishments.First(x => x.EditMode == true);
                model.SetEditMode(false).JoinedDate = model.JoinedDateEditable.ToDateTime();
            }
            else if (viewModel.Action == ActionLinkedEstablishmentSearch)
            {
                await SearchForLinkedEstablishment(viewModel);
            }
            else if (viewModel.Action == ActionSave)
            {
                suppressClearModelState = true;
                await SaveGroup(viewModel);
                return RedirectToAction(nameof(Details), new { id = viewModel.GroupUId.Value });
            }
            else throw new InvalidParameterException("The action parameter is invalid");
            
            if(!suppressClearModelState) ModelState.Clear();

            return null;
        }

        private async Task SaveGroup(GroupEditorViewModel viewModel)
        {
            SaveGroupDto dto = CreateSaveDto(viewModel);
            viewModel.GroupUId = await _groupWriteService.SaveAsync(dto, User);
        }

        private static SaveGroupDto CreateSaveDto(GroupEditorViewModel viewModel)
        {
            viewModel.SetCCLeadCentreUrn();

            Func<GroupModel> createDomainModel = () => new GroupModel
            {
                CompaniesHouseNumber = viewModel.CompaniesHouseNumber,
                GroupId = viewModel.GroupId,
                GroupTypeId = viewModel.GroupTypeId,
                GroupUId = viewModel.GroupUId,
                LocalAuthorityId = viewModel.LocalAuthorityId,
                ManagerEmailAddress = viewModel.GroupManagerEmailAddress,
                Name = viewModel.GroupName,
                OpenDate = viewModel.OpenDate.ToDateTime(),
                StatusId = viewModel.GroupStatusId,
                ClosedDate = viewModel.ClosedDate.ToDateTime()
            };

            Func<List<LinkedEstablishmentGroup>> createLinksDomainModel = 
                () => viewModel.LinkedEstablishments.Establishments.Select(x => new LinkedEstablishmentGroup
            {
                Urn = x.Urn,
                Id = x.Id,
                JoinedDate = x.JoinedDate,
                CCIsLeadCentre = x.CCIsLeadCentre
            }).ToList();

            SaveGroupDto dto = null;
            if (viewModel.SaveMode == eSaveMode.Details) dto = new SaveGroupDto(createDomainModel());
            else if (viewModel.SaveMode == eSaveMode.DetailsAndLinks) dto = new SaveGroupDto(createDomainModel(), createLinksDomainModel());
            else if (viewModel.SaveMode == eSaveMode.Links) dto = new SaveGroupDto(viewModel.GroupUId.Value, createLinksDomainModel());
            else throw new NotImplementedException($"SaveMode '{viewModel.SaveMode}' is not supported");
            return dto;
        }

        private async Task AddLinkedEstablishment(GroupEditorViewModel viewModel)
        {
            var model = (await _establishmentReadService.GetAsync(viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn.Value, User)).GetResult();
            viewModel.LinkedEstablishments.Establishments.Add(new EstablishmentGroupViewModel
            {
                Address = await model.GetAddressAsync(_lookup),
                HeadFirstName = model.HeadFirstName,
                HeadLastName = model.HeadLastName,
                HeadTitleName = await _lookup.GetNameAsync(() => model.HeadTitleId),
                JoinedDate = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate.ToDateTime(),
                Name = model.Name,
                TypeName = await _lookup.GetNameAsync(() => model.TypeId),
                Urn = model.Urn.Value
            });

            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Reset();
        }

        private async Task SearchForLinkedEstablishment(GroupEditorViewModel viewModel)
        {
            var urn = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Urn;
            var result = await _establishmentReadService.GetAsync(urn.ToInteger().Value, User);
            var model = result.GetResult();
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Name = model.Name;
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn = model.Urn;
        }

        private async Task<GroupEditorViewModel> PopulateSelectLists(GroupEditorViewModel viewModel)
        {
            viewModel.LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.CCGroupTypes = (await _lookup.GroupTypesGetAllAsync())
                    .Where(x => x.Id.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)).ToSelectList(viewModel.GroupTypeId);
            viewModel.Statuses = (await _lookup.GroupStatusesGetAllAsync()).ToSelectList(viewModel.GroupStatusId);
            return viewModel;
        }

        private async Task PopulateEstablishmentList(List<EstablishmentGroupViewModel> list, int groupUId, bool includeFutureDated = false)
        {
            var establishmentGroups = await _groupReadService.GetEstablishmentGroupsAsync(groupUId, User, includeFutureDated);
            foreach (var establishmentGroup in establishmentGroups)
            {
                list.Add(new EstablishmentGroupViewModel
                {
                    Id = establishmentGroup.Id,
                    Address = establishmentGroup.Address.ToString(),
                    HeadFirstName = establishmentGroup.HeadFirstName,
                    HeadLastName = establishmentGroup.HeadLastName,
                    Name = establishmentGroup.Name,
                    Urn = establishmentGroup.Urn.Value,
                    TypeName = establishmentGroup.TypeName,
                    HeadTitleName = establishmentGroup.HeadTitle,
                    JoinedDate = establishmentGroup.JoinedDate,
                    CCIsLeadCentre = establishmentGroup.CCIsLeadCentre ?? false
                });
            }
        }


        [HttpGet, EdubaseAuthorize, Route("Download/ChangeHistory/{downloadType}/{id}")]
        public async Task<ActionResult> DownloadChangeHistory(int id, DownloadType downloadType)
        {
            var response = await _groupDownloadService.DownloadGroupHistory(id, downloadType, User);
            return Redirect(response.Url);
        }
    }
}