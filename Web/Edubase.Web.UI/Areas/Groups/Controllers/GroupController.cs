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

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    using Common;
    using Exceptions;
    using Filters;
    using Models.CreateEdit;
    using Models.Validators;
    using MoreLinq;
    using Services.Domain;
    using Services.Enums;
    using Services.Exceptions;
    using Services.Groups.Models;
    using Services.IntegrationEndPoints.CompaniesHouse;
    using Services.Nomenclature;
    using UI.Models;
    using static Models.CreateEdit.GroupEditorViewModel;
    using static Models.CreateEdit.GroupEditorViewModelBase;
    using GT = Services.Enums.eLookupGroupType;
    using GS = Services.Enums.eLookupGroupStatus;
    using R = EdubaseRoles;

    [RouteArea("Groups"), RoutePrefix("Group")]
    public class GroupController : Controller
    {
        private readonly ICompaniesHouseService _companiesHouseService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly ICachedLookupService _lookup;
        private readonly NomenclatureService _nomenclatureService;
        private readonly ISecurityService _securityService;
        public GroupController(
            ICachedLookupService cachedLookupService, 
            ISecurityService securityService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            IGroupsWriteService groupWriteService,
            ICompaniesHouseService companiesHouseService,
            NomenclatureService nomenclatureService)
        {
            _lookup = cachedLookupService;
            _securityService = securityService;
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _groupWriteService = groupWriteService;
            _companiesHouseService = companiesHouseService;
            _nomenclatureService = nomenclatureService;
        }


        [HttpGet, EdubaseAuthorize, Route("Convert", Name = "GroupConvertSAT2MAT")]
        public ActionResult Convert() => View(new ConvertSATViewModel());

        [HttpPost, EdubaseAuthorize, Route("Convert", Name = "PostGroupConvertSAT2MAT"), ValidateAntiForgeryToken]
        public async Task<ActionResult> Convert(ConvertSATViewModel viewModel)
        {
            if (viewModel.ActionName == "find" && ModelState.IsValid)
            {
                var result = (await _groupReadService.SearchByIdsAsync(viewModel.Text, viewModel.Text.ToInteger(), viewModel.Text, User)).Items.FirstOrDefault();
                if (result == null) ModelState.AddModelError(nameof(viewModel.Text), "We were unable to find a SAT matching those details");
                else if (result.GroupTypeId != (int) GT.SingleacademyTrust) ModelState.AddModelError(nameof(viewModel.Text), "That's an invalid group because it's of the wrong type.");
                else
                {
                    viewModel.Details = result;
                    viewModel.CountryName = await _lookup.GetNameAsync(() => result.Address.CountryId);
                    viewModel.CountyName = await _lookup.GetNameAsync(() => result.Address.CountyId);
                    viewModel.Token = UriHelper.SerializeToUrlToken(result);
                }
            }
            else if (viewModel.ActionName == "confirm")
            {
                viewModel.Details = UriHelper.DeserializeUrlToken<SearchGroupDocument>(viewModel.Token);
                var apiResponse = await _groupWriteService.ConvertSAT2MAT(viewModel.Details.GroupUId, viewModel.CopyGovernanceInfo, User);
                if (apiResponse.HasErrors)
                {
                    apiResponse.Errors.ForEach(x => ModelState.AddModelError("", x.GetMessage()));
                }
                else
                {
                    return RedirectToRoute("GroupDetails", new { id = apiResponse.GetResponse().Value });
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("Create"), EdubaseAuthorize]
        public async Task<ActionResult> Create(GroupEditorViewModel viewModel)
        {
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, User, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);

            await PopulateSelectLists(viewModel);

            await ValidateAsync(viewModel);

            if (ModelState.IsValid && !viewModel.WarningsToProcess.Any())
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null) return actionResult;
            }

            return viewModel.GroupTypeMode == eGroupTypeMode.ChildrensCentre
                ? View("CreateChildrensCentre", viewModel)
                : View("Create", viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route(nameof(CreateAcademyTrust) + "/{companiesHouseNumber}")]
        public async Task<ActionResult> CreateAcademyTrust(string companiesHouseNumber)
        {
            if (string.IsNullOrWhiteSpace(companiesHouseNumber)) return HttpNotFound();

            var companyProfile = await _companiesHouseService.SearchByCompaniesHouseNumber(companiesHouseNumber);
            var groupTypes = await GetAcademyTrustGroupTypes();

            var vm = new CreateAcademyTrustViewModel(companyProfile.Items.First(), groupTypes);

            var existingTrust = await _groupReadService.SearchByIdsAsync(null, null, companiesHouseNumber, User);
            if (existingTrust != null && existingTrust.Items.Any())
            {
                vm.TrustExists = true;
                vm.TypeId = existingTrust.Items.First().GroupTypeId;
                vm.GroupId = existingTrust.Items.First().GroupId;
            }

            if (vm.Address == null)
            {
                ModelState.AddModelError("", "This company record doesn't have an address");
            }

            if (!vm.OpenDate.HasValue)
            {
                ModelState.AddModelError("", "This company record doesn't have an incorporation date");
            }

            vm.AllowSave = !vm.TrustExists && ModelState.IsValid;

            return View(vm);
        }

        [HttpGet]
        [Route("Create/{type}"), EdubaseAuthorize]
        public async Task<ActionResult> CreateNewGroup(string type)
        {
            var groupTypeMode = StringUtil.ToEnum<eGroupTypeMode>(type);
            Guard.IsTrue(groupTypeMode.HasValue, () => new InvalidParameterException($"Invalid type parameter supplied"));

            var viewModel = await PopulateSelectLists(new GroupEditorViewModel(eSaveMode.Details));

            if (groupTypeMode == eGroupTypeMode.ChildrensCentre)
            {
                viewModel.GroupTypeId = (int) GT.ChildrensCentresCollaboration;
                viewModel.SaveMode = eSaveMode.DetailsAndLinks;
            }
            else if (groupTypeMode == eGroupTypeMode.Federation)
            {
                viewModel.GroupTypeId = (int) GT.Federation;
                viewModel.SaveMode = User.IsInRole(EdubaseRoles.ESTABLISHMENT) ? eSaveMode.DetailsAndLinks : eSaveMode.Details;
            }
            else if (groupTypeMode == eGroupTypeMode.Trust)
            {
                viewModel.GroupTypeId = (int) GT.Trust;
            }
            else if (groupTypeMode == eGroupTypeMode.Sponsor)
            {
                viewModel.GroupTypeId = (int) GT.SchoolSponsor;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

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

                return View("CreateChildrensCentre", viewModel);
            }

            return View("Create", viewModel);
        }

        [Route(nameof(Details) + "/{id:int}", Name = "GroupDetails"), HttpGet]
        public async Task<ActionResult> Details(int id, string searchQueryString = "", eLookupSearchSource searchSource = eLookupSearchSource.Groups, int skip = 0, string sortBy = null, bool saved = false)
        {
            ViewBag.ShowSaved = saved;

            var model = (await _groupReadService.GetAsync(id, User)).GetResult();

            var viewModel = new GroupDetailViewModel
            {
                SearchQueryString = searchQueryString,
                SearchSource = searchSource,
                CanUserEdit = await _groupReadService.CanEditAsync(id, User),
                CanUserEditGovernance = await _groupReadService.CanEditGovernanceAsync(id, User),
                Group = model,
                GroupTypeName = model.GroupTypeId.HasValue ? await _lookup.GetNameAsync(() => model.GroupTypeId) : null,
                LocalAuthorityName = model.LocalAuthorityId.HasValue ? await _lookup.GetNameAsync(() => model.LocalAuthorityId) : null,
                GroupStatusName = model.StatusId.HasValue ? await _lookup.GetNameAsync(() => model.StatusId, "Group") : null,
                Address = model.GroupTypeId.OneOfThese(GT.SingleacademyTrust, GT.MultiacademyTrust, GT.ChildrensCentresGroup) ? model.Address.ToString() : null,
                IsUserLoggedOn = User.Identity.IsAuthenticated,
                GroupTypeId = model.GroupTypeId ?? -1,
                IsClosed = model.StatusId == (int)eLookupGroupStatus.Closed || model.StatusId == (int)eLookupGroupStatus.CreatedInError,
                IsClosedInError = model.StatusId == (int)eLookupGroupStatus.CreatedInError,
                CloseDate = model.ClosedDate
            };

            if (viewModel.IsUserLoggedOn)
            {
                viewModel.ChangeHistory = await _groupReadService.GetChangeHistoryAsync(id, skip, 100, sortBy.Clean() ?? "requestedDateUtc-desc", User);
            }

            viewModel.Links = await _groupReadService.GetLinksAsync(id, User);

            await PopulateEstablishmentList(viewModel.Establishments, model.GroupUId.Value, true);
            
            return View(viewModel);
        }
        [HttpGet]
        [Route("Edit/{id:int}/Details"), EdubaseAuthorize]
        public async Task<ActionResult> EditDetails(int id)
        {
            var domainModel = (await _groupReadService.GetAsync(id, User)).GetResult();
            var viewModel = new GroupEditorViewModel(eSaveMode.Details)
            {
                Address = domainModel.Address.ToString(),
                AddressJsonToken = UriHelper.SerializeToUrlToken(domainModel.Address),
                ClosedDate = new DateTimeViewModel(domainModel.ClosedDate),
                OpenDate = new DateTimeViewModel(domainModel.OpenDate),
                LocalAuthorityId = domainModel.LocalAuthorityId,
                GroupTypeId = domainModel.GroupTypeId,
                ManagerEmailAddress = domainModel.ManagerEmailAddress,
                GroupName = domainModel.Name,
                CompaniesHouseNumber = domainModel.CompaniesHouseNumber,
                GroupUId = domainModel.GroupUId,
                GroupId = domainModel.GroupId,
                SelectedTabName = "details",
                StatusId = domainModel.StatusId,
                OriginalStatusId = domainModel.StatusId
            };
            viewModel.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((GT)viewModel.GroupTypeId.Value);

            await PopulateEstablishmentList(viewModel.LinkedEstablishments.Establishments, id, true);
            await PopulateSelectLists(viewModel);

            viewModel.LocalAuthorityName = await _lookup.GetNameAsync(() => viewModel.LocalAuthorityId);
            viewModel.DeriveCCLeadCentreUrn();

            if (viewModel.GroupTypeId.HasValue)
            {
                viewModel.GroupTypeName = await _lookup.GetNameAsync(() => viewModel.GroupTypeId);
            }

            viewModel.CanUserCloseMATAndMarkAsCreatedInError = viewModel.GroupType.OneOfThese(GT.MultiacademyTrust) 
                && !viewModel.StatusId.OneOfThese(GS.CreatedInError, GS.Closed) 
                && User.InRole(R.ROLE_BACKOFFICE);

            viewModel.IsLocalAuthorityEditable = viewModel.GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup) 
                && viewModel.LinkedEstablishments.Establishments.Count == 0 && User.InRole(R.ROLE_BACKOFFICE);


            if(User.InRole(R.EDUBASE, R.ROLE_BACKOFFICE, R.EDUBASE_CMT, R.AP_AOS) && viewModel.GroupType.OneOfThese(GT.MultiacademyTrust))
            {
                viewModel.CanUserEditClosedDate = true;
                viewModel.CanUserEditStatus = true;
                PopulateStatusSelectList(viewModel);
            }

            return View("EditDetails", viewModel);
        }

        [HttpPost]
        [Route("Edit/{id:int}/Details"), EdubaseAuthorize]
        public async Task<ActionResult> EditDetails(GroupEditorViewModel viewModel)
        {
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, User, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);

            await PopulateSelectLists(viewModel);

            if (viewModel.CanUserEditStatus)
            {
                PopulateStatusSelectList(viewModel);
            }

            if (viewModel.GroupTypeId.HasValue)
            {
                viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => viewModel.GroupTypeId));
            }

            await ValidateAsync(viewModel);

            if (ModelState.IsValid && !viewModel.WarningsToProcess.Any())
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null) return actionResult;
            }

            viewModel.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((GT) viewModel.GroupTypeId.Value);

            return View("EditDetails", viewModel);
        }

        [HttpGet]
        [Route("Edit/{id:int}/Links"), EdubaseAuthorize]
        public async Task<ActionResult> EditLinks(int id, bool saved = false)
        {
            ViewBag.ShowSaved = saved;
            var domainModel = (await _groupReadService.GetAsync(id, User)).GetResult();
            var viewModel = new GroupEditorViewModel
            {
                SaveMode = eSaveMode.Links,
                GroupName = domainModel.Name,
                GroupUId = domainModel.GroupUId,
                GroupTypeId = domainModel.GroupTypeId
            };

            await PopulateEstablishmentList(viewModel.LinkedEstablishments.Establishments, id, true);
            viewModel.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((GT) viewModel.GroupTypeId.Value);
            viewModel.SelectedTabName = "links";

            viewModel.DeriveCCLeadCentreUrn();

            if (viewModel.GroupTypeId.HasValue) viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => viewModel.GroupTypeId));

            return View(viewModel);
        }

        [HttpPost]
        [Route("Edit/{id:int}/Links"), EdubaseAuthorize]
        public async Task<ActionResult> EditLinks(GroupEditorViewModel viewModel)
        {
            var model = (await _groupReadService.GetAsync(viewModel.GroupUId.Value, User)).GetResult();
            viewModel.OpenDate = new DateTimeViewModel(model.OpenDate);
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
                StatusId = (int) eLookupGroupStatus.Open,
                GroupId = viewModel.GroupId
            });

            if (ModelState.IsValid)
            {
                var validationEnvelope = await _groupWriteService.ValidateAsync(dto, User);
                if (validationEnvelope.HasErrors)
                {
                    validationEnvelope.Errors.ForEach(x => ModelState.AddModelError(x.Fields, x.GetMessage()));
                }
            }

            if (ModelState.IsValid)
            {
                var apiResponse = (await _groupWriteService.SaveNewAsync(dto, User));
                if (!apiResponse.HasErrors)
                {
                    return RedirectToAction(nameof(Details), new { id = apiResponse.GetResponse().Value });
                }
                else
                {
                    apiResponse.Errors.ForEach(x => ModelState.AddModelError(x.Fields, x.GetMessage()));
                }
            }
            else
            {
                viewModel.GroupTypes = await GetAcademyTrustGroupTypes(viewModel.TypeId);
            }

            return View("CreateAcademyTrust", viewModel);
        }

        [EdubaseAuthorize, Route(nameof(SearchCompaniesHouse)), EdubaseAuthorize]
        public async Task<ActionResult> SearchCompaniesHouse(SearchCompaniesHouseModel viewModel)
        {
            if (!viewModel.SearchText.IsNullOrEmpty())
            {
                if (viewModel.SearchText.IsInteger())
                {
                    viewModel.Results = await _companiesHouseService.SearchByCompaniesHouseNumber(viewModel.SearchText);
                }
                else
                {
                    viewModel.Results = await _companiesHouseService.SearchByName(viewModel.SearchText, viewModel.StartIndex, viewModel.PageSize);
                }

                viewModel.NotFound = !viewModel.Results.Items.Any();

                if (viewModel.Results.Count == 1)
                {
                    return RedirectToAction("CreateAcademyTrust", "Group", new { companiesHouseNumber = viewModel.Results.Items.First().Number, area = "Groups" });
                }
            }
            return View(viewModel);
        }

        private static void PopulateStatusSelectList(GroupEditorViewModel viewModel)
        {
            viewModel.Statuses = new[] { new SelectListItem { Value = ((int) GS.Open).ToString(), Text = "Open" }, new SelectListItem { Value = ((int) GS.Closed).ToString(), Text = "Closed" } };
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

        private SaveGroupDto CreateSaveDto(GroupEditorViewModel viewModel)
        {
            viewModel.SetCCLeadCentreUrn();

            GroupModel createDomainModel() => new GroupModel
            {
                CompaniesHouseNumber = viewModel.CompaniesHouseNumber,
                GroupId = viewModel.GroupId,
                GroupTypeId = viewModel.GroupTypeId,
                GroupUId = viewModel.GroupUId,
                LocalAuthorityId = viewModel.LocalAuthorityId,
                ManagerEmailAddress = viewModel.ManagerEmailAddress,
                Name = viewModel.GroupName,
                OpenDate = viewModel.OpenDate.ToDateTime(),
                ClosedDate = viewModel.ClosedDate.ToDateTime(),
                Address = UriHelper.TryDeserializeUrlToken<AddressDto>(viewModel.AddressJsonToken),
                StatusId = viewModel.StatusId
            };

            List<LinkedEstablishmentGroup> createLinksDomainModel() => viewModel.LinkedEstablishments.Establishments.Select(x => new LinkedEstablishmentGroup
            {
                Urn = x.Urn,
                Id = x.Id,
                JoinedDate = x.JoinedDate ?? x.JoinedDateEditable.ToDateTime(),
                CCIsLeadCentre = x.CCIsLeadCentre
            }).ToList();

            SaveGroupDto dto = null;
            if (viewModel.SaveMode == eSaveMode.Details)
            {
                dto = new SaveGroupDto(createDomainModel());
            }
            else if (viewModel.SaveMode == eSaveMode.DetailsAndLinks)
            {
                dto = new SaveGroupDto(createDomainModel(), createLinksDomainModel());
            }
            else if (viewModel.SaveMode == eSaveMode.Links)
            {
                dto = new SaveGroupDto(viewModel.GroupUId.Value, createLinksDomainModel());
            }
            else
            {
                throw new NotImplementedException($"SaveMode '{viewModel.SaveMode}' is not supported");
            }

            if (viewModel.CanUserCloseMATAndMarkAsCreatedInError
                && viewModel.CloseMATAndMarkAsCreatedInError
                && dto.Group != null)
            {
                dto.Group.StatusId = (int) GS.CreatedInError;
            }

            return dto;
        }

        private async Task<IEnumerable<SelectListItem>> GetAcademyTrustGroupTypes(int? typeId = null)
                    => (await _lookup.GroupTypesGetAllAsync()).Where(x => x.Id == (int) GT.MultiacademyTrust || x.Id == (int) GT.SingleacademyTrust).ToSelectList(typeId);

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
                    CCIsLeadCentre = establishmentGroup.CCIsLeadCentre ?? false,
                    LAESTAB = establishmentGroup.LAESTAB,
                    LocalAuthorityName = establishmentGroup.LocalAuthorityName,
                    PhaseName = establishmentGroup.PhaseName,
                    StatusName = establishmentGroup.StatusName,
                    Location = establishmentGroup.Location
                });
            }
        }

        private async Task<GroupEditorViewModel> PopulateSelectLists(GroupEditorViewModel viewModel)
        {
            viewModel.LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.CCGroupTypes = (await _lookup.GroupTypesGetAllAsync())
                    .Where(x => x.Id.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)).OrderBy(x => x.Id).ToSelectList(viewModel.GroupTypeId);
            return viewModel;
        }

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
                var apiResponse = await SaveGroup(viewModel);
                if (apiResponse.HasErrors)
                {
                    apiResponse.Errors.ForEach(x => ModelState.AddModelError(x.Fields, x.GetMessage()));
                }
                else
                {
                    return RedirectToAction(nameof(Details), new { id = viewModel.GroupUId.Value, saved = true });
                }
            }
            else
            {
                throw new InvalidParameterException("The action parameter is invalid");
            }

            if (!suppressClearModelState) ModelState.Clear();

            return null;
        }

        private async Task<ApiResponse> SaveGroup(GroupEditorViewModel viewModel)
        {
            var dto = CreateSaveDto(viewModel);
            if (dto.IsNewEntity)
            {
                var resp = await _groupWriteService.SaveNewAsync(dto, User);
                if (!resp.HasErrors) viewModel.GroupUId = resp.GetResponse().Value;
                return resp;
            }
            else
            {
                return await _groupWriteService.SaveAsync(dto, User);
            }
        }

        private async Task SearchForLinkedEstablishment(GroupEditorViewModel viewModel)
        {
            var urn = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Urn;
            var result = await _establishmentReadService.GetAsync(urn.ToInteger().Value, User);
            var model = result.GetResult();
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Name = model?.Name;
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn = model?.Urn;
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
                validationEnvelope.Errors.ForEach(x => ModelState.AddModelError(x.Fields?.Replace("Unmapped field: group.closedDate", nameof(viewModel.ClosedDate)) ?? string.Empty, x.GetMessage()));
                viewModel.SetWarnings(validationEnvelope);
                ModelState.Remove(nameof(viewModel.ProcessedWarnings));
            }
        }
    }
}
