using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Establishments;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Governors;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models;
using Edubase.Web.UI.Helpers;
using FluentValidation.Mvc;

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    using Common;
    using Edubase.Web.UI.Areas.Groups.ViewRulesHandlers;
    using Exceptions;
    using Filters;
    using Microsoft.IdentityModel.Tokens;
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
    using GS = Services.Enums.eLookupGroupStatus;
    using GT = Services.Enums.eLookupGroupType;

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
        private readonly IGovernorsReadService _governorsReadService;
        private readonly IExternalLookupService _externalLookupService;

        public GroupController(
            ICachedLookupService cachedLookupService,
            ISecurityService securityService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            IGroupsWriteService groupWriteService,
            ICompaniesHouseService companiesHouseService,
            NomenclatureService nomenclatureService,
            IGovernorsReadService governorsReadApiService,
            IExternalLookupService externalLookupService)
        {
            _lookup = cachedLookupService;
            _securityService = securityService;
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _groupWriteService = groupWriteService;
            _companiesHouseService = companiesHouseService;
            _nomenclatureService = nomenclatureService;
            _governorsReadService = governorsReadApiService;
            _externalLookupService = externalLookupService;
        }


        [HttpGet, EdubaseAuthorize, Route("Convert", Name = "GroupConvertSAT2MAT"), MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyTrusts)]
        public ActionResult Convert() => View(new ConvertSATViewModel());

        [HttpPost, EdubaseAuthorize, Route("Convert", Name = "PostGroupConvertSAT2MAT"), MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyTrusts), ValidateAntiForgeryToken]
        public async Task<ActionResult> Convert(ConvertSATViewModel viewModel)
        {
            if (viewModel.ActionName == "find" && ModelState.IsValid)
            {
                var result = (await _groupReadService.SearchByIdsAsync(viewModel.Text, viewModel.Text.ToInteger(), viewModel.Text, viewModel.Text.ToInteger(), User)).Items.FirstOrDefault();
                if (result == null)
                {
                    ModelState.AddModelError(nameof(viewModel.Text), "We were unable to find a single-academy trust matching those details");
                }
                else if (result.StatusId == ((int) GS.Closed))
                {
                    ModelState.AddModelError(nameof(viewModel.Text), "Closed single-academy trusts can not be converted");
                }
                else if (result.GroupTypeId != (int) GT.SingleacademyTrust)
                {
                    ModelState.AddModelError(nameof(viewModel.Text), "That's an invalid group because it's of the wrong type.");
                }
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
                    TempData["ShowSaved"] = "saved";
                    return RedirectToRoute("GroupDetails", new { id = apiResponse.GetResponse().Value });
                }
            }

            return View(viewModel);
        }

        [HttpPost, Route("Create/{type}"), EdubaseAuthorize]
        public async Task<ActionResult> Create(GroupEditorViewModel viewModel, string type, bool? jsDisabled = false)
        {
            await PopulateSelectLists(viewModel);
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, User, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);

            await ValidateAsync(viewModel);

            if (viewModel.Action == ActionSave && ModelState.IsValid && !viewModel.WarningsToProcess.Any())
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null)
                {
                    return actionResult;
                }
            }

            if (ModelState.IsValid || viewModel.Action == ActionLinkedEstablishmentCancelEdit)
            {
                ModelState.Remove("ActionName");

                // because some of the lookups contain the action urn as part of the action string, we want to strip that back out for the comparison
                var actionLookup = viewModel.Action.IndexOf('-') == -1 ? viewModel.Action : string.Concat(viewModel.Action.Split('-').First(),"-");

                switch (actionLookup)
                {
                    case ActionCcCreate:
                        viewModel.ActionName = eChildrensCentreActions.Step2;
                        break;
                    case ActionSave:
                    case ActionDetails:
                        viewModel.ActionName = eChildrensCentreActions.Step3;
                        break;
                    case ActionLinkedEstablishmentSearch:
                        viewModel.ActionName = eChildrensCentreActions.Step4;
                        break;
                    case ActionLinkedEstablishmentAdd:
                        foreach (var msKey in ModelState.Keys.Where(x => x.StartsWith("LinkedEstablishments")).ToList())
                        {
                            ModelState.Remove(msKey);
                        }
                        viewModel.ActionName = eChildrensCentreActions.Step3;
                        break;
                    case ActionLinkedEstablishmentRemove:
                        foreach (var msKey in ModelState.Keys.Where(x => x.StartsWith("LinkedEstablishments")).ToList())
                        {
                            ModelState.Remove(msKey);
                        }
                        break;
                    case ActionLinkedEstablishmentEdit:
                        foreach (var msKey in ModelState.Keys.Where(x => x.StartsWith("LinkedEstablishments")).ToList())
                        {
                            ModelState.Remove(msKey);
                        }
                        viewModel.ActionName = eChildrensCentreActions.Step4;
                        break;
                    case ActionLinkedEstablishmentCancelEdit:
                        foreach (var msKey in ModelState.Keys.Where(x => x.StartsWith("LinkedEstablishments")).ToList())
                        {
                            ModelState.Remove(msKey);
                        }
                        viewModel.ActionName = eChildrensCentreActions.Step3;
                        break;
                }
            }

            ViewBag.JsDisabled = jsDisabled;

            return viewModel.GroupTypeMode == eGroupTypeMode.ChildrensCentre
                ? View("CreateChildrensCentre", viewModel)
                : View("Create", viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route(nameof(CreateAcademyTrust) + "/{companiesHouseNumber}/{academyTrustRoute}")]
        public async Task<ActionResult> CreateAcademyTrust(string companiesHouseNumber, string academyTrustRoute)
        {
            var permission = await _securityService.GetCreateGroupPermissionAsync(User);

            if (HasCreatePermissionForGroupType(permission, academyTrustRoute))
            {
                throw new PermissionDeniedException("Current principal does not have the required permissions.");
            }

            if (string.IsNullOrWhiteSpace(companiesHouseNumber))
            {
                return HttpNotFound();
            }

            ViewData["academyTrustRoute"] = academyTrustRoute;
            var companyProfile = await _companiesHouseService.SearchByCompaniesHouseNumber(companiesHouseNumber);
            var groupTypes = await GetAcademyTrustGroupTypes(academyTrustRoute);

            var vm = new CreateAcademyTrustViewModel(companyProfile.Items.First(), groupTypes.ToSelectList());

            var existingTrust = await _groupReadService.SearchByIdsAsync(null, null, companiesHouseNumber, null, User);
            if (existingTrust != null && existingTrust.Items.Any())
            {
                vm.TrustExists = true;
                vm.TrustName = existingTrust.Items.First().Name;
                vm.TypeId = existingTrust.Items.First().GroupTypeId;
                vm.GroupId = existingTrust.Items.First().GroupId;
                vm.GroupUid = existingTrust.Items.First().GroupUId;
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

        [HttpGet, Route("Create/{type}"), EdubaseAuthorize]
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
                else
                {
                    viewModel.IsLocalAuthorityEditable = true;
                }

                return View("CreateChildrensCentre", viewModel);
            }

            return View("Create", viewModel);
        }

        [HttpGet, Route(nameof(Details) + "/{id:int}", Name = "GroupDetails")]
        public async Task<ActionResult> Details(int id, string searchQueryString = "", eLookupSearchSource searchSource = eLookupSearchSource.Groups, int skip = 0, string sortBy = null, bool saved = false)
        {
            ViewBag.ShowSaved = saved;
            if (TempData["showSaved"] != null)
            {
                ViewBag.ShowSaved = true;
            }

            var result = await _groupReadService.GetAsync(id, User);
            if (result.ReturnValue == null)
            {
                return HttpNotFound();
            }

            var model = result.ReturnValue;

            var viewModel = new GroupDetailViewModel(_externalLookupService)
            {
                SearchQueryString = searchQueryString,
                SearchSource = searchSource,
                CanUserEdit = await _groupReadService.CanEditAsync(id, User),
                CanUserEditGovernance = await _groupReadService.CanEditGovernanceAsync(id, User),
                Group = model,
                GroupTypeName = model.GroupTypeId.HasValue ? await _lookup.GetNameAsync(() => model.GroupTypeId) : null,
                LocalAuthorityName = model.LocalAuthorityId.HasValue ? await _lookup.GetNameAsync(() => model.LocalAuthorityId) : null,
                GroupStatusName = model.StatusId.HasValue ? await _lookup.GetNameAsync(() => model.StatusId, "Group") : null,
                Address = model.GroupTypeId.OneOfThese(GT.SingleacademyTrust, GT.MultiacademyTrust, GT.ChildrensCentresGroup, GT.SecureSingleAcademyTrust) ? model.Address.ToString() : null,
                IsUserLoggedOn = User.Identity.IsAuthenticated,
                GroupTypeId = model.GroupTypeId ?? -1,
                IsClosed = model.StatusId == (int)eLookupGroupStatus.Closed || model.StatusId == (int)eLookupGroupStatus.CreatedInError,
                IsClosedInError = model.StatusId == (int)eLookupGroupStatus.CreatedInError,
                CloseDate = model.ClosedDate,
                UKPRN = model.UKPRN.ToInteger(),
            };

            if (viewModel.IsUserLoggedOn)
            {
                viewModel.GovernorPermissions = await _governorsReadService.GetGovernorPermissions(null, id, User);
                viewModel.ChangeHistory = await _groupReadService.GetChangeHistoryAsync(id, skip, 100, sortBy.Clean() ?? "requestedDateUtc-desc", User);
            }

            viewModel.Links = await _groupReadService.GetLinksAsync(id, User);

            await PopulateEstablishmentList(viewModel.Establishments, model.GroupUId.Value, true);

            return View(viewModel);
        }

        [HttpGet, Route("Details/{id:int}/Governance/Changes", Name = "GroupDetailGovChangeHistory"), EdubaseAuthorize]
        public async Task<ActionResult> GovernanceChangeHistoryAsync(int id, int skip = 0, string sortBy = null)
        {
            var result = await _groupReadService.GetAsync(id, User);
            if (result.ReturnValue == null)
            {
                return HttpNotFound();
            }
            var model = result.ReturnValue;

            var viewModel = new GroupDetailViewModel(_externalLookupService)
            {
                Group = model,
                GroupTypeName = model.GroupTypeId.HasValue ? await _lookup.GetNameAsync(() => model.GroupTypeId) : null,
                LocalAuthorityName = await _lookup.GetNameAsync(() => model.LocalAuthorityId),
                GroupStatusName =  await _lookup.GetNameAsync(() => model.StatusId, "Group"),
                Address = model.GroupTypeId.OneOfThese(GT.SingleacademyTrust, GT.MultiacademyTrust, GT.ChildrensCentresGroup, GT.SecureSingleAcademyTrust) ? model.Address.ToString() : null,
                IsUserLoggedOn = User.Identity.IsAuthenticated,
                GroupTypeId = model.GroupTypeId ?? -1,
                IsClosed = model.StatusId == (int) GS.Closed || model.StatusId == (int) GS.CreatedInError,
                IsClosedInError = model.StatusId == (int) GS.CreatedInError,
                CloseDate = model.ClosedDate,
                ChangeHistory = await _groupReadService.GetGovernanceChangeHistoryAsync(id, skip, 100, sortBy, User)
            };

            return View("GovernanceChangeHistory", viewModel);
        }

        [HttpGet, Route("Edit/{id:int}/Details"), EdubaseAuthorize]
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
                OriginalStatusId = domainModel.StatusId,
                UKPRN = domainModel.UKPRN.ToInteger(),
                GroupStatusName = domainModel.StatusId.HasValue ? await _lookup.GetNameAsync(() => domainModel.StatusId, "Group") : null
            };
            viewModel.ListOfEstablishmentsPluralName = NomenclatureService.GetEstablishmentsPluralName((GT)viewModel.GroupTypeId.Value);

            await PopulateEstablishmentList(viewModel.LinkedEstablishments.Establishments, id, true);
            await PopulateSelectLists(viewModel);

            viewModel.LocalAuthorityName = await _lookup.GetNameAsync(() => viewModel.LocalAuthorityId);
            viewModel.DeriveCCLeadCentreUrn();

            if (viewModel.GroupTypeId.HasValue)
            {
                viewModel.GroupTypeName = await _lookup.GetNameAsync(() => viewModel.GroupTypeId);
            }

            SetEditPermissions(viewModel);

            return View("EditDetails", viewModel);
        }

        [HttpPost, Route("Edit/{id:int}/Details"), EdubaseAuthorize]
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
                var dto = CreateSaveDto(viewModel).Group;
                var changes = await _groupReadService.GetModelChangesAsync(dto, User);

                if (changes.Any() && GroupEditorViewModelRulesHandler.ShowChangesReviewScreen(viewModel, User))
                {
                    viewModel.ChangesSummary = changes;
                    return View("EditDetails", viewModel);
                }
                else
                {
                    var actionResult = await ProcessCreateEditGroup(viewModel);
                    if (actionResult != null)
                    {
                        return actionResult;
                    }
                }
            }
            else
            {
                if (viewModel.GroupUId.HasValue)
                {
                    var domainModel = (await _groupReadService.GetAsync((int)viewModel.GroupUId, User)).GetResult();
                    viewModel.OriginalGroupName = domainModel.Name;
                    viewModel.OriginalGroupTypeName = await _lookup.GetNameAsync(() => domainModel.GroupTypeId);
                }
            }

            viewModel.ListOfEstablishmentsPluralName = NomenclatureService.GetEstablishmentsPluralName((GT) viewModel.GroupTypeId.Value);
            SetEditPermissions(viewModel);

            return View("EditDetails", viewModel);
        }

        [HttpGet, Route("Edit/{id:int}/Links"), EdubaseAuthorize]
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
            viewModel.ListOfEstablishmentsPluralName = NomenclatureService.GetEstablishmentsPluralName((GT) viewModel.GroupTypeId.Value);
            viewModel.SelectedTabName = "links";

            viewModel.DeriveCCLeadCentreUrn();

            if (viewModel.GroupTypeId.HasValue)
            {
                viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => viewModel.GroupTypeId));
            }

            return View(viewModel);
        }

        [HttpPost, Route("Edit/{id:int}/Links"), EdubaseAuthorize]
        public async Task<ActionResult> EditLinks(GroupEditorViewModel viewModel)
        {
            if (viewModel.Action == ActionLinkedEstablishmentStartSearch)
            {
                ModelState.Clear();
            }

            var model = (await _groupReadService.GetAsync(viewModel.GroupUId.Value, User)).GetResult();
            viewModel.OpenDate = new DateTimeViewModel(model.OpenDate);
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, User, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);

            await ValidateAsync(viewModel);

            if (ModelState.IsValid)
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null)
                {
                    return actionResult;
                }
            }

            return View(viewModel);
        }

        [HttpPost, EdubaseAuthorize, Route(nameof(CreateAcademyTrust) + "/{companiesHouseNumber}/{academyTrustRoute}")]
        public async Task<ActionResult> SaveNewAcademyTrust(CreateAcademyTrustViewModel viewModel, string academyTrustRoute)
        {
            var permission = await _securityService.GetCreateGroupPermissionAsync(User);
            if ((viewModel.TypeId != null) && (!permission.GroupTypes.Any(x => (int)x == viewModel.TypeId)))
            {
                throw new PermissionDeniedException("Current principal does not have permission to create a group of this type.");
            }

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
                viewModel.GroupTypes = (await GetAcademyTrustGroupTypes(academyTrustRoute)).ToSelectList(viewModel.TypeId);
            }

            return View("CreateAcademyTrust", viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route(nameof(SearchCompaniesHouse) + "/{academyTrustRoute}")]
        public async Task<ActionResult> SearchCompaniesHouse(SearchCompaniesHouseModel viewModel, string academyTrustRoute)
        {
            var permission = await _securityService.GetCreateGroupPermissionAsync(User);

            if (HasCreatePermissionForGroupType(permission, academyTrustRoute))
            {
                throw new PermissionDeniedException("Current principal does not have the required permissions.");
            }

            ViewData["academyTrustRoute"] = academyTrustRoute;

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

                if (viewModel.Results.Count == 0)
                {
                    ModelState.AddModelError(nameof(viewModel.SearchText), "We could not find any companies matching your search criteria");
                }
                else if (viewModel.Results.Count == 1)
                {
                    return RedirectToAction("CreateAcademyTrust", "Group", new { companiesHouseNumber = viewModel.Results.Items.First().Number, area = "Groups", academyTrustRoute = academyTrustRoute });
                }
            }

            if (viewModel.Search && viewModel.SearchText.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.SearchText), "The SearchText field is required.");
            }
            
            return View(viewModel);
        }

        private static void PopulateStatusSelectList(GroupEditorViewModel viewModel) => viewModel.Statuses = new[] { new SelectListItem { Value = ((int) GS.Open).ToString(), Text = "Open" }, new SelectListItem { Value = ((int) GS.Closed).ToString(), Text = "Closed" } };

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
                StatusId = viewModel.StatusId,
                UKPRN = viewModel.UKPRN?.ToString()
            };

            List<LinkedEstablishmentGroup> createLinksDomainModel()
            {
                if (viewModel.Action.StartsWith(ActionLinkedEstablishmentRemove))
                {
                    viewModel.LinkedEstablishments.Establishments.RemoveAll(x => x.Urn == viewModel.ActionUrn);
                }


                var domainList = viewModel.LinkedEstablishments.Establishments.Select(x => new LinkedEstablishmentGroup
                    {
                        Urn = x.Urn,
                        Id = x.Id,
                        JoinedDate = x.JoinedDate ?? x.JoinedDateEditable.ToDateTime(),
                        CCIsLeadCentre = x.CCIsLeadCentre
                    }).ToList();

                if (domainList.Any() && !domainList.Any(x => x.CCIsLeadCentre))
                {
                    domainList.First().CCIsLeadCentre = true;
                    viewModel.LinkedEstablishments.Establishments.First().CCIsLeadCentre = true;
                }
                return domainList;
            }

            List<LinkedEstablishmentGroup> createLinkedEstablishmentFromSearch(bool isSearch = false) => new List<LinkedEstablishmentGroup>
            {
                new LinkedEstablishmentGroup
                {
                    Urn = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Urn.ToInteger(),
                    JoinedDate =
                        viewModel.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate
                            .ToDateTime() ?? viewModel.OpenDate.ToDateTime(),
                    CCIsLeadCentre = isSearch || !createLinksDomainModel().Any(x => x.CCIsLeadCentre)
                }
            };

            List<LinkedEstablishmentGroup> createLinkedEstablishmentFromEdit() => new List<LinkedEstablishmentGroup>
            {
                new LinkedEstablishmentGroup
                {
                    Urn = viewModel.LinkedEstablishments.Establishments.First(x => x.Urn == viewModel.ActionUrn).Urn,
                    JoinedDate = viewModel.LinkedEstablishments.Establishments.First(x => x.Urn == viewModel.ActionUrn).JoinedDate,
                    CCIsLeadCentre = true // the validation always needs this to be set to true when in edit
                }
            };

            List<LinkedEstablishmentGroup> createLinkedEstablishmentFromAdd()
            {
                var domainList = createLinksDomainModel();
                if (domainList.Any(x => x.Urn == viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn))
                {
                    // edit
                    domainList.First(x => x.Urn == viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn)
                        .JoinedDate = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate.ToDateTime();

                    viewModel.LinkedEstablishments.Establishments.First(x =>
                            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn != null &&
                            x.Urn == viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn.Value).JoinedDate
                        = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate.ToDateTime();
                }
                else
                {
                    // add
                    domainList.AddRange(createLinkedEstablishmentFromSearch(false));
                    if (viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn != null)
                    {
                        viewModel.LinkedEstablishments.Establishments.Add(new EstablishmentGroupViewModel
                        {
                            Name = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Name,
                            JoinedDate =
                                viewModel.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate
                                    .ToDateTime(),
                            Urn = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn.Value,
                            CCIsLeadCentre = viewModel.LinkedEstablishments.Establishments.All(x => x.CCIsLeadCentre == false)
                        });
                    }
                }
                viewModel.LinkedEstablishments.LinkedEstablishmentSearch = new LinkedEstablishmentSearchViewModel();
                return domainList;
            }

            SaveGroupDto dto = null;
            if (viewModel.SaveMode == eSaveMode.Details)
            {
                dto = new SaveGroupDto(createDomainModel());
            }
            else if (viewModel.SaveMode == eSaveMode.DetailsAndLinks)
            {
                if (viewModel.Action == ActionLinkedEstablishmentSearch)
                {
                    dto = new SaveGroupDto(createDomainModel(), createLinkedEstablishmentFromSearch(true));
                    _ = SearchForLinkedEstablishment(viewModel);
                }
                else if (viewModel.Action.StartsWith(ActionLinkedEstablishmentEdit))
                {
                    dto = new SaveGroupDto(createDomainModel(), createLinkedEstablishmentFromEdit());
                    EditLinkedEstablishment(viewModel);
                }
                else if (viewModel.Action == ActionLinkedEstablishmentAdd)
                {
                    dto = new SaveGroupDto(createDomainModel(), createLinkedEstablishmentFromAdd());
                }
                else
                {
                    dto = new SaveGroupDto(createDomainModel(), createLinksDomainModel());
                }
            }
            else
            {
                dto = viewModel.SaveMode == eSaveMode.Links
                    ? new SaveGroupDto(viewModel.GroupUId.Value, createLinksDomainModel())
                    : throw new ArgumentOutOfRangeException(nameof(viewModel.SaveMode),
                        $"SaveMode '{viewModel.SaveMode}' is not supported");
            }

            if (viewModel.CanUserCloseAndMarkAsCreatedInError
                && viewModel.CloseAndMarkAsCreatedInError
                && dto.Group != null)
            {
                dto.Group.StatusId = (int) GS.CreatedInError;
            }

            viewModel.DeriveCCLeadCentreUrn();

            return dto;
        }

        private IEnumerable<eLookupGroupType> GroupTypesFromRouteName(string routeName)
        {
            switch (routeName)
            {
                case "academy-trust":
                    return new List<eLookupGroupType> { GT.MultiacademyTrust, GT.SingleacademyTrust };
                case "secure-academy-trust":
                    return new List<eLookupGroupType> { GT.SecureSingleAcademyTrust };
                default: return new List<eLookupGroupType>();
            };
        }
        private bool HasCreatePermissionForGroupType(CreateGroupPermissionDto userPermissions, string academyTrustRoute)
        {
            var query = from groupType in GroupTypesFromRouteName(academyTrustRoute)
                        join permissionType in userPermissions.GroupTypes on groupType equals permissionType
                        select new { groupType };
            return query.ToList().IsNullOrEmpty();
        }

        private async Task<IEnumerable<LookupDto>> GetAcademyTrustGroupTypes(string academyTrustRoute)
        {
            var allGroupTypes = await _lookup.GroupTypesGetAllAsync();
            var query = from groupType in GroupTypesFromRouteName(academyTrustRoute)
                        join lookup in allGroupTypes on (int) groupType equals lookup.Id
                        select lookup;
            return query;
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
                    CCIsLeadCentre = establishmentGroup.CCIsLeadCentre ?? false,
                    LAESTAB = establishmentGroup.LAESTAB,
                    LocalAuthorityName = establishmentGroup.LocalAuthorityName,
                    PhaseName = establishmentGroup.PhaseName,
                    StatusName = establishmentGroup.StatusName,
                    Location = establishmentGroup.Location,
                    UKPRN = establishmentGroup.UKPRN
                });
            }
        }

        private async Task<GroupEditorViewModel> PopulateSelectLists(GroupEditorViewModel viewModel)
        {
            viewModel.LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.CCGroupTypes = (await _lookup.GroupTypesGetAllAsync())
                    .Where(x => x.Id.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)).OrderBy(x => x.Id).ToSelectList(viewModel.GroupTypeId);
            if (viewModel.GroupTypeId.HasValue)
            {
                viewModel.GroupTypeName = await _lookup.GetNameAsync(() => viewModel.GroupTypeId);
            }
            return viewModel;
        }

        private GroupEditorViewModel SetEditPermissions(GroupEditorViewModel viewModel)
        {
            viewModel = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, User);
            if (GroupEditorViewModelRulesHandler.UserCanEditClosedDateAndStatus(viewModel, User))
            {
                PopulateStatusSelectList(viewModel);
            }
            return viewModel;
        }

        private async Task<ActionResult> ProcessCreateEditGroup(GroupEditorViewModel viewModel)
        {
            var suppressClearModelState = false;

            if (viewModel.Action == ActionLinkedEstablishmentAdd)
            {
                await AddLinkedEstablishment(viewModel);
            }
            else if (viewModel.Action == ActionLinkedEstablishmentCancelAdd)
            {
                return null;
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
                if (index >= 0)
                {
                    viewModel.LinkedEstablishments.Establishments.RemoveAt(index);
                }

                if (viewModel.LinkedEstablishments.Establishments.Any() && !viewModel.LinkedEstablishments.Establishments.Any(x => x.CCIsLeadCentre))
                {
                    viewModel.LinkedEstablishments.Establishments.First().CCIsLeadCentre = true;
                    viewModel.DeriveCCLeadCentreUrn();
                }
            }
            else if (viewModel.Action == ActionLinkedEstablishmentSave)
            {
                var model = viewModel.LinkedEstablishments.Establishments.First(x => x.EditMode == true);
                model.SetEditMode(false).JoinedDate = model.JoinedDateEditable.ToDateTime();
            }
            else if (viewModel.Action == ActionLinkedEstablishmentStartSearch)
            {
                viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Reset();
                return null;
            }
            else if (viewModel.Action == ActionLinkedEstablishmentSearch)
            {
                await SearchForLinkedEstablishment(viewModel);
            }
            else if (viewModel.Action == ActionSaveLinks)
            {
                suppressClearModelState = true;
                var apiResponse = await SaveGroup(viewModel);
                if (apiResponse.HasErrors)
                {
                    apiResponse.Errors.ForEach(x => ModelState.AddModelError(x.Fields, x.GetMessage()));
                }
                else
                {
                    return new RedirectResult(Url.Action(nameof(Details)) + "#list");
                    //return RedirectToAction(nameof(Details) , new { id = viewModel.GroupUId.Value, saved = true } );
                }
            }
            else if (viewModel.Action == ActionSave || viewModel.Action == ActionDetails)
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
            else if (viewModel.Action == ActionCcCreate)
            {
                viewModel.GroupTypeName = viewModel.GroupTypeId.HasValue
                    ? await _lookup.GetNameAsync(() => viewModel.GroupTypeId)
                    : null;
            }
            else
            {
                throw new InvalidParameterException("The action parameter is invalid");
            }

            if (!suppressClearModelState)
            {
                ModelState.Clear();
            }

            return null;
        }

        private async Task<ApiResponse> SaveGroup(GroupEditorViewModel viewModel)
        {
            var dto = CreateSaveDto(viewModel);
            if (dto.IsNewEntity)
            {
                var resp = await _groupWriteService.SaveNewAsync(dto, User);
                if (!resp.HasErrors)
                {
                    viewModel.GroupUId = resp.GetResponse().Value;
                }

                return resp;
            }
            else
            {
                return await _groupWriteService.SaveAsync(dto, User);
            }
        }

        private void EditLinkedEstablishment(GroupEditorViewModel viewModel)
        {
            var model = viewModel.LinkedEstablishments.Establishments.First(x => x.Urn == viewModel.ActionUrn);
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Name = model.Name;
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn = model.Urn;
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.JoinedDate = new DateTimeViewModel(model.JoinedDate);
        }

        private async Task SearchForLinkedEstablishment(GroupEditorViewModel viewModel)
        {
            var urn = viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Urn;
            var result = await _establishmentReadService.GetAsync(urn.ToInteger().Value, User);
            var model = result.GetResult();
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Urn = urn;
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Name = model?.Name;
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn = model?.Urn;
            viewModel.LinkedEstablishments.LinkedEstablishmentSearch.Address = await model?.GetAddressAsync(_lookup);
        }

        /// <summary>
        /// Does 2nd-level validation
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task  ValidateAsync(GroupEditorViewModel viewModel)
        {
            if ((viewModel.Action == ActionSave
                 || viewModel.Action == ActionDetails
                 || viewModel.Action.StartsWith(ActionLinkedEstablishmentRemove)
                 || viewModel.Action == ActionLinkedEstablishmentSearch
                 || viewModel.Action == ActionLinkedEstablishmentAdd
                 || viewModel.Action == ActionLinkedEstablishmentCancelAdd
                 || viewModel.Action.StartsWith(ActionLinkedEstablishmentEdit)
                 || viewModel.Action == ActionLinkedEstablishmentCancelEdit
                ) && ModelState.IsValid)
            {
                var dto = CreateSaveDto(viewModel);
                var validationEnvelope = await _groupWriteService.ValidateAsync(dto, User);

                if (viewModel.Action.StartsWith(ActionLinkedEstablishmentRemove) ||
                    viewModel.Action == ActionLinkedEstablishmentSearch ||
                    viewModel.Action == ActionLinkedEstablishmentAdd ||
                    viewModel.Action == ActionLinkedEstablishmentCancelAdd ||
                    viewModel.Action.StartsWith(ActionLinkedEstablishmentEdit) ||
                    viewModel.Action == ActionLinkedEstablishmentCancelEdit)
                {
                    // ignore the message about the number of establishments in the group, as per JS behaviour
                    for (var i = 0; i < validationEnvelope.Errors.Count; i++)
                    {
                        if (validationEnvelope.Errors[i].Code.Equals("error.validation.link.cc.one.linked.school"))
                        {
                            validationEnvelope.Errors.RemoveAt(i);
                        }
                    }
                }

                if (viewModel.Action == ActionSave || viewModel.Action == ActionDetails)
                {
                    // we want to rebuild the screen once the removal has completed, so set the viewstate back to default
                    viewModel.ClearWarnings();
                }

                if (viewModel.Action.StartsWith(ActionLinkedEstablishmentRemove) || viewModel.Action == ActionLinkedEstablishmentCancelEdit || viewModel.Action == ActionLinkedEstablishmentCancelAdd)
                {
                    // we want to rebuild the screen once the removal has completed, so set the viewstate back to default
                    viewModel.ClearWarnings();
                    viewModel.ProcessedWarnings = false;
                }

                validationEnvelope.Errors.ForEach(x => ModelState.AddModelError(x.Fields?.Replace("Unmapped field: group.closedDate", nameof(viewModel.ClosedDate)) ?? string.Empty, x.GetMessage()));
                viewModel.SetWarnings(validationEnvelope);
                ModelState.Remove(nameof(viewModel.ProcessedWarnings));
            }
        }
    }
}
