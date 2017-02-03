using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Mvc;
using MoreLinq;

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    using Common;
    using Exceptions;
    using Helpers;
    using Models.CreateEdit;
    using Models.Validators;
    using Services.Exceptions;
    using Services.Governors;
    using Services.Groups.Models;
    using UI.Models;
    using static GroupDetailViewModel;
    using GT = Services.Enums.eLookupGroupType;

    [RouteArea("Groups"), RoutePrefix("Group")]
    public class GroupController : Controller
    {
        ICachedLookupService _lookup;
        IEstablishmentReadService _establishmentReadService;
        IGroupReadService _groupReadService;
        ISecurityService _securityService;
        IGovernorsReadService _governorsReadService;
        IGroupsWriteService _groupWriteService;

        public GroupController(
            ICachedLookupService cachedLookupService, 
            ISecurityService securityService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            IGovernorsReadService governorsReadService,
            IGroupsWriteService groupWriteService)
        {
            _lookup = cachedLookupService;
            _securityService = securityService;
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _governorsReadService = governorsReadService;
            _groupWriteService = groupWriteService;
        }


        [Route(nameof(Details) + "/{id:int}"), HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var viewModel = new GroupDetailViewModel();
            var result = await _groupReadService.GetAsync(id, User);

            var model = result.GetResult();
            viewModel.Group = model;

            if (model.GroupTypeId.HasValue) viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => model.GroupTypeId));
            if (model.LocalAuthorityId.HasValue) viewModel.LocalAuthorityName = (await _lookup.GetNameAsync(() => model.LocalAuthorityId));
            if (model.StatusId.HasValue) viewModel.GroupStatusName = (await _lookup.GetNameAsync(() => model.StatusId, "Group"));

            if (model.GroupTypeId.Equals((int)GT.ChildrensCentresGroup)) viewModel.Address = "!FROM LEAD CENTRE!"; // TODO: get from the 'lead centre'; need IsLeadCentre on the estabgroup table
            else if(model.GroupTypeId.OneOfThese(GT.SingleacademyTrust, GT.MultiacademyTrust)) viewModel.Address = model.Address;

            viewModel.CanUserEdit = _securityService.GetEditGroupPermission(User).CanEdit(model.GroupUID.Value, model.GroupTypeId.Value, model.LocalAuthorityId);
            viewModel.IsUserLoggedOn = User.Identity.IsAuthenticated;

            await PopulateEstablishmentList(viewModel, model);

            viewModel.HistoricalGovernors = await _governorsReadService.GetHistoricalByGroupUID(id);
            viewModel.Governors = await _governorsReadService.GetCurrentByGroupUID(id);

            return View(viewModel);
        }

        [HttpGet]
        [Route("Create/{type}")]
        public async Task<ActionResult> CreateNewGroup(string type)
        {
            var mode = StringUtil.ToEnum<GroupEditorViewModel.eGroupTypeMode>(type);
            Guard.IsTrue(mode.HasValue, () => new InvalidParameterException($"Invalid type parameter supplied"));

            var viewModel = await PopulateSelectLists(new GroupEditorViewModel(mode.Value));
            
            if (viewModel.GroupTypeMode == GroupEditorViewModel.eGroupTypeMode.ChildrensCentre)
            {
                var permission = _securityService.GetCreateGroupPermission(User);
                if (permission.LocalAuthorityIds.Any())
                {
                    viewModel.IsLocalAuthorityEditable = false;
                    viewModel.LocalAuthorityId = permission.LocalAuthorityIds[0];
                    viewModel.LocalAuthorityName = await _lookup.GetNameAsync(() => viewModel.LocalAuthorityId);
                }
                else viewModel.IsLocalAuthorityEditable = true;
            }

            viewModel.Statuses = (await _lookup.GroupStatusesGetAllAsync()).ToSelectList(viewModel.GroupStatusId);
            
            return View("CreateEdit", viewModel);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult> Create(GroupEditorViewModel viewModel)
        {
            var result = await new GroupEditorViewModelValidator(_groupReadService, _establishmentReadService, _securityService).ValidateAsync(viewModel);
            result.AddToModelState(ModelState, string.Empty);
            ViewBag.FVErrors = result;

            await PopulateSelectLists(viewModel);

            if (ModelState.IsValid)
            {
                var actionResult = await ProcessCreateEditGroup(viewModel);
                if (actionResult != null) return actionResult;
            }

            return View("CreateEdit", viewModel);
        }

        private async Task<ActionResult> ProcessCreateEditGroup(GroupEditorViewModel viewModel)
        {
            var suppressClearModelState = false;

            if (viewModel.Action == GroupEditorViewModel.ActionLinkedEstablishmentAdd)
            {
                await AddLinkedEstablishment(viewModel);
            }
            else if (viewModel.Action == GroupEditorViewModel.ActionLinkedEstablishmentCancelEdit)
            {
                viewModel.LinkedEstablishments.Establishments.ForEach(x => x.EditMode = false);
            }
            else if (viewModel.Action.StartsWith(GroupEditorViewModel.ActionLinkedEstablishmentEdit, StringComparison.OrdinalIgnoreCase))
            {
                var m = viewModel.LinkedEstablishments.Establishments.Single(x => x.Urn == viewModel.ActionUrn);
                m.SetEditMode().JoinedDateEditable = new DateTimeViewModel(m.JoinedDate);
            }
            else if (viewModel.Action.StartsWith(GroupEditorViewModel.ActionLinkedEstablishmentRemove, StringComparison.OrdinalIgnoreCase))
            {
                var index = viewModel.LinkedEstablishments.Establishments.FindIndex(x => x.Urn == viewModel.ActionUrn);
                if (index >= 0) viewModel.LinkedEstablishments.Establishments.RemoveAt(index);
            }
            else if (viewModel.Action == GroupEditorViewModel.ActionLinkedEstablishmentSave)
            {
                var model = viewModel.LinkedEstablishments.Establishments.First(x => x.EditMode == true);
                model.SetEditMode(false).JoinedDate = model.JoinedDateEditable.ToDateTime();
            }
            else if (viewModel.Action == GroupEditorViewModel.ActionLinkedEstablishmentSearch)
            {
                await SearchForLinkedEstablishment(viewModel);
            }
            else if (viewModel.Action == GroupEditorViewModel.ActionSave)
            {
                suppressClearModelState = true;
                await SaveGroup(viewModel);
                return RedirectToAction(nameof(Details), new { id = viewModel.GroupUID.Value });
            }
            else throw new InvalidParameterException("The action parameter is invalid");
            
            if(!suppressClearModelState) ModelState.Clear();

            return null;
        }

        private async Task SaveGroup(GroupEditorViewModel viewModel)
        {
            viewModel.SetCCLeadCentreUrn();

            var dto = new SaveGroupDto(new GroupModel
            {
                Address = viewModel.Address,
                CompaniesHouseNumber = viewModel.CompaniesHouseNumber,
                GroupId = viewModel.GroupId,
                GroupTypeId = viewModel.GroupTypeId,
                GroupUID = viewModel.GroupUID,
                LocalAuthorityId = viewModel.LocalAuthorityId,
                ManagerEmailAddress = viewModel.GroupManagerEmailAddress,
                Name = viewModel.Name,
                OpenDate = viewModel.OpenDate.ToDateTime(),
                StatusId = viewModel.GroupStatusId,
                ClosedDate = viewModel.ClosedDate.ToDateTime()

            }, viewModel.LinkedEstablishments.Establishments.Select(x => new EstablishmentGroupModel
            {
                EstablishmentUrn = x.Urn,
                Id = x.Id,
                JoinedDate = x.JoinedDate,
                CCIsLeadCentre = x.CCIsLeadCentre
            }).ToList());
            
            await _groupWriteService.SaveAsync(dto, User);
        }

        private async Task AddLinkedEstablishment(GroupEditorViewModel viewModel)
        {
            var model = (await _establishmentReadService.GetAsync(viewModel.LinkedEstablishments.LinkedEstablishmentSearch.FoundUrn.Value, User)).GetResult();
            viewModel.LinkedEstablishments.Establishments.Add(new EstablishmentGroupViewModel
            {
                Address = model.GetAddress(),
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
                    .Where(x => ((GT)x.Id).OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)).ToSelectList(viewModel.GroupTypeId);
            viewModel.Statuses = (await _lookup.GroupStatusesGetAllAsync()).ToSelectList(viewModel.GroupStatusId);
            return viewModel;
        }


        private async Task PopulateEstablishmentList(GroupDetailViewModel viewModel, GroupModel model)
        {
            var establishmentGroups = await _groupReadService.GetEstablishmentGroupsAsync(model.GroupUID.Value);
            foreach (var establishmentGroup in establishmentGroups)
            {
                var result = await _establishmentReadService.GetAsync(establishmentGroup.EstablishmentUrn, User);
                if (result.Success)
                {
                    var estabModel = result.GetResult();
                    viewModel.Establishments.Add(new EstablishmentGroupViewModel
                    {
                        Address = estabModel.GetAddress(),
                        HeadFirstName = estabModel.HeadFirstName,
                        HeadLastName = estabModel.HeadLastName,
                        Name = estabModel.Name,
                        Urn = estabModel.Urn.Value,
                        TypeName = await _lookup.GetNameAsync(() => estabModel.LocalAuthorityId),
                        HeadTitleName = await _lookup.GetNameAsync(() => estabModel.HeadTitleId),
                        JoinedDate = establishmentGroup.JoinedDate
                    });
                }
            }
        }
    }
}