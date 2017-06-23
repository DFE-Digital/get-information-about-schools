using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Establishments;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Castle.Core.Internal;
using Edubase.Services.Domain;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Validation;
using FluentValidation.Mvc;
using Newtonsoft.Json;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("Governor")]
    public class GovernorController : Controller
    {
        const string GROUP_EDIT_GOVERNANCE = "~/Groups/Group/Edit/{groupUId:int}/Governance";
        const string ESTAB_EDIT_GOVERNANCE = "~/Establishment/Edit/{establishmentUrn:int}/Governance";
        
        const string ESTAB_EDIT_GOVERNANCE_MODE = "~/Establishment/Edit/{establishmentUrn:int}/GovernanceMode";

        const string GROUP_ADD_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Add";
        const string ESTAB_ADD_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Add";

        const string GROUP_EDIT_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Edit/{gid:int}";
        const string ESTAB_EDIT_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Edit/{gid:int}";

        const string GROUP_REPLACE_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Replace/{gid:int}";
        const string ESTAB_REPLACE_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Replace/{gid:int}";

        const string VIEW_EDIT_GOV_VIEW_NAME = "~/Areas/Governors/Views/Governor/ViewEdit.cshtml";
        const string GROUPS_LAYOUT = "~/Areas/Groups/Views/Group/_EditLayoutPage.cshtml";
        const string ESTAB_LAYOUT = "~/Views/Establishment/_EditLayoutPage.cshtml";

        const string ESTAB_SELECT_SHARED_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/SelectSharedGovernor";
        const string ESTAB_EDIT_SHARED_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/EditSharedGovernor";

        private const string GROUP_EDIT_DELEGATION = "~/Groups/Group/Edit/{groupUId:int}/Governance/Delegation";

        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly NomenclatureService _nomenclatureService;
        private readonly IGovernorsWriteService _governorsWriteService;
        private readonly IGroupReadService _groupReadService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public GovernorController(
            IGovernorsReadService governorsReadService,
            NomenclatureService nomenclatureService,
            ICachedLookupService cachedLookupService,
            IGovernorsWriteService governorsWriteService,
            IGroupReadService groupReadService,
            IGroupsWriteService groupWriteService,
            IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService)
        {
            _governorsReadService = governorsReadService;
            _nomenclatureService = nomenclatureService;
            _cachedLookupService = cachedLookupService;
            _governorsWriteService = governorsWriteService;
            _groupReadService = groupReadService;
            _groupWriteService = groupWriteService;
            _establishmentReadService = establishmentReadService;
            _establishmentWriteService = establishmentWriteService;
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="establishmentUrn"></param>
        /// <returns></returns>
        [Route(ESTAB_EDIT_GOVERNANCE_MODE, Name = "EstabEditGovernanceMode"), HttpGet]
        public async Task<ActionResult> EditGovernanceMode(int? establishmentUrn, bool failed = false)
        {
            Guard.IsTrue(establishmentUrn.HasValue, () => new InvalidParameterException($"Parameter '{nameof(establishmentUrn)}' is null."));

            using (MiniProfiler.Current.Step("Retrieving Governors Details"))
            {
                if (failed) ModelState.AddModelError("", "Unable to update Governance");
                
                var viewModel = new EditGovernanceModeViewModel { Urn = establishmentUrn.Value };
                await PopulateLayoutProperties(viewModel, establishmentUrn, null, x => viewModel.GovernanceMode = x.GovernanceMode ?? eGovernanceMode.LocalGovernors);
                return View(viewModel);
            }
        }

        /// <summary>
        /// Saves the governance mode
        /// </summary>
        /// <returns></returns>
        [Route(ESTAB_EDIT_GOVERNANCE_MODE), HttpPost]
        public async Task<ActionResult> EditGovernanceMode(EditGovernanceModeViewModel viewModel)
        {
            try
            {
                await _establishmentWriteService.UpdateGovernanceModeAsync(viewModel.Urn.Value, viewModel.GovernanceMode.Value, User);
                return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = viewModel.Urn });
            }
            catch (EduSecurityException) // for some reason the API responds with a 403 for this one, even though it's nothing to do with authentication/authorization.
            {
                return RedirectToRoute("EstabEditGovernanceMode", new { establishmentUrn = viewModel.Urn, failed = true });
            }
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="groupUId"></param>
        /// <param name="establishmentUrn"></param>
        /// <param name="editMode"></param>
        /// <returns></returns>
        [Route(GROUP_EDIT_GOVERNANCE, Name = "GroupEditGovernance"), 
         Route(ESTAB_EDIT_GOVERNANCE, Name = "EstabEditGovernance"),
         HttpGet]
        public async Task<ActionResult> Edit(int? groupUId, int? establishmentUrn, int? removalGid, int? duplicateGovernorId, bool roleAlreadyExists = false)
        {
            Guard.IsTrue(groupUId.HasValue || establishmentUrn.HasValue, () => new InvalidParameterException($"Both parameters '{nameof(groupUId)}' and '{nameof(establishmentUrn)}' are null."));

            using (MiniProfiler.Current.Step("Retrieving Governors Details"))
            {
                var domainModel = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
                var viewModel = new GovernorsGridViewModel(domainModel, true, groupUId, establishmentUrn, _nomenclatureService, 
                    (await _cachedLookupService.NationalitiesGetAllAsync()), 
                    (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()));
                
                var applicableRoles = domainModel.ApplicableRoles.Cast<int>();
                viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).Where(x => applicableRoles.Contains(x.Id)).Select(x => new LookupItemViewModel(x)).ToList();

                await PopulateLayoutProperties(viewModel, establishmentUrn, groupUId, x => viewModel.GovernanceMode = x.GovernanceMode, x=>
                {
                    viewModel.ShowDelegationInformation = x.GroupTypeId.GetValueOrDefault() == (int)eLookupGroupType.MultiacademyTrust;
                    viewModel.DelegationInformation = x.DelegationInformation;
                });

                viewModel.RemovalGid = removalGid;
                viewModel.GovernorShared = false;
                if (removalGid.HasValue)
                {
                    var govToBeRemoved = domainModel.CurrentGovernors.SingleOrDefault(g => g.Id == removalGid.Value);
                    if (govToBeRemoved != null && EnumSets.SharedGovernorRoles.Contains(govToBeRemoved.RoleId.Value))
                    {
                        viewModel.GovernorShared = true;
                    }
                }

                if (duplicateGovernorId.HasValue)
                {

                    var duplicate = await _governorsReadService.GetGovernorAsync(duplicateGovernorId.Value, User);
                    ViewData.Add("DuplicateGovernor", duplicate);
                }

                if (roleAlreadyExists)
                {
                    ModelState.AddModelError("role", "The selected role already contains an appointee.");
                }
                

                return View(VIEW_EDIT_GOV_VIEW_NAME, viewModel);
            }
        }

        [Route(GROUP_EDIT_GOVERNANCE, Name = "GroupDeleteOrRetireGovernor"), Route(ESTAB_EDIT_GOVERNANCE, Name = "EstabDeleteOrRetireGovernor"), HttpPost]
        public async Task<ActionResult> DeleteOrRetireGovernor(GovernorsGridViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Action == "Save") // retire selected governor with the chosen appt. end date
                {
                    if (viewModel.GovernorShared.HasValue && viewModel.GovernorShared.Value)
                    {
                        var sharedGovernor = await _governorsReadService.GetGovernorAsync(viewModel.RemovalGid.Value, User);
                        var appointment = sharedGovernor.Appointments.Single(a => a.EstablishmentUrn == viewModel.EstablishmentUrn.Value);

                        var response = await _governorsWriteService.UpdateSharedGovernorAppointmentAsync(viewModel.RemovalGid.Value,
                                                viewModel.EstablishmentUrn.Value, appointment.AppointmentStartDate.Value,
                                                viewModel.RemovalAppointmentEndDate.ToDateTime().Value, User);

                        response.ApplyToModelState(ControllerContext);
                    }
                    else
                    {
                        var domainModel = await _governorsReadService.GetGovernorAsync(viewModel.RemovalGid.Value, User);
                        domainModel.AppointmentEndDate = viewModel.RemovalAppointmentEndDate.ToDateTime().Value;
                        var response = await _governorsWriteService.UpdateDatesAsync(viewModel.RemovalGid.Value,
                            domainModel.AppointmentStartDate.Value,
                            viewModel.RemovalAppointmentEndDate.ToDateTime().Value, User);

                        //var response = await _governorsWriteService.SaveAsync(domainModel, User);
                        if (!response.Success)
                        {
                            if (response.Errors.Length == 0)
                            {
                                throw new TexunaApiSystemException($"The TEX-API said no (but gave no details!)...");
                            }

                            response.ApplyToModelState(ControllerContext);
                        }
                    }
                }
                else if (viewModel.Action == "Remove") // mark the governor record as deleted
                {
                    if (viewModel.GovernorShared.HasValue && viewModel.GovernorShared.Value)
                    {
                        await _governorsWriteService.DeleteSharedGovernorAppointmentAsync(viewModel.RemovalGid.Value, viewModel.EstablishmentUrn.Value, User);
                    }
                    else
                    {
                        await _governorsWriteService.DeleteAsync(viewModel.RemovalGid.Value, User);
                    }
                }
                else throw new InvalidParameterException($"The parameter for action is invalid: '{viewModel.Action}'");

                if (ModelState.IsValid)
                {
                    return RedirectToRoute(
                        viewModel.EstablishmentUrn.HasValue ? "EstabEditGovernance" : "GroupEditGovernance",
                        new {viewModel.EstablishmentUrn, viewModel.GroupUId});
                }
            }

            await PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId);

            return await Edit(viewModel.GroupUId, viewModel.EstablishmentUrn, viewModel.RemovalGid, null);
        }

        [Route]
        public ActionResult View(int? groupUId, int? establishmentUrn, GovernorsGridViewModel viewModel = null)
        {
            if (viewModel != null) return View(VIEW_EDIT_GOV_VIEW_NAME, viewModel);
            else
            {
                // KHD Hack: Async child actions are not supported; but we have an async stack, so we have to wrap the async calls in an sync wrapper.  Hopefully won't deadlock.
                // Need to use ASP.NET Core really now; that supports ViewComponents which are apparently the solution.
                return Task.Run(async () =>
                {
                    using (MiniProfiler.Current.Step("Retrieving Governors Details"))
                    {
                        viewModel = await CreateGovernorsViewModel(groupUId, establishmentUrn);
                        return View(VIEW_EDIT_GOV_VIEW_NAME, viewModel);
                    }
                }).Result;
            }
        }

        internal async Task<GovernorsGridViewModel> CreateGovernorsViewModel(int? groupUId = null, int? establishmentUrn = null, EstablishmentModel establishmentModel = null)
        {
            establishmentUrn = establishmentUrn ?? establishmentModel?.Urn;

            var domainModel = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
            var viewModel = new GovernorsGridViewModel(domainModel, false, groupUId, establishmentUrn, _nomenclatureService,
                (await _cachedLookupService.NationalitiesGetAllAsync()), (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()));

            if (establishmentUrn.HasValue || establishmentModel != null)
            {
                var estabDomainModel = establishmentModel ?? (await _establishmentReadService.GetAsync(establishmentUrn.Value, User)).GetResult();
                viewModel.GovernanceMode = estabDomainModel.GovernanceMode ?? eGovernanceMode.LocalGovernors;
            }

            if (groupUId.HasValue)
            {
                var groupModel = (await _groupReadService.GetAsync(groupUId.Value, User)).GetResult();
                viewModel.ShowDelegationInformation = groupModel.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust;
                viewModel.DelegationInformation = groupModel.DelegationInformation;
            }

            return viewModel;
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <param name="gid"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        [Route(GROUP_ADD_GOVERNOR, Name = "GroupAddGovernor"), Route(ESTAB_ADD_GOVERNOR, Name = "EstabAddGovernor"),
             Route(GROUP_EDIT_GOVERNOR, Name = "GroupEditGovernor"), Route(ESTAB_EDIT_GOVERNOR, Name = "EstabEditGovernor"),
             Route(GROUP_REPLACE_GOVERNOR, Name = "GroupReplaceGovernor"),
             HttpGet]
        public async Task<ActionResult> AddEditOrReplace(int? groupUId, int? establishmentUrn, eLookupGovernorRole? role, int? gid)
        {
            var replaceMode = (ControllerContext.RouteData.Route as System.Web.Routing.Route).Url.IndexOf("/Replace/", StringComparison.OrdinalIgnoreCase) > -1;
            if (role == null && gid == null) throw new EdubaseException("Role was not supplied and no Governor ID was supplied");

            if (role.HasValue)
            {
                var existingGovernors = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
                
                if (EnumSets.eSingularGovernorRoles.Contains(role.Value) && existingGovernors.CurrentGovernors.Any(g => g.RoleId == (int)role.Value))
                {
                    return RedirectToRoute(establishmentUrn.HasValue ? "EstabEditGovernance" : "GroupEditGovernance", new { establishmentUrn, groupUId, roleAlreadyExists = true });
                }

                if (establishmentUrn.HasValue && EnumSets.eSharedGovernorRoles.Contains(role.Value))
                {
                    return RedirectToRoute("SelectSharedGovernor", new { establishmentUrn = establishmentUrn.Value, role = role.Value });
                }
            } 
            
            var viewModel = new CreateEditGovernorViewModel
            {
                GroupUId = groupUId,
                EstablishmentUrn = establishmentUrn,
                Mode = CreateEditGovernorViewModel.EditMode.Create
            };

            if (gid.HasValue)
            {
                var model = await _governorsReadService.GetGovernorAsync(gid.Value, User);
                role = (eLookupGovernorRole)model.RoleId.Value;

                if (replaceMode)
                {
                    viewModel.Mode = CreateEditGovernorViewModel.EditMode.Replace;
                    viewModel.ReplaceGovernorViewModel.AppointmentEndDate = new DateTimeViewModel(model.AppointmentEndDate);
                    viewModel.ReplaceGovernorViewModel.GID = gid;
                    viewModel.ReplaceGovernorViewModel.Name = model.GetFullName();
                }
                else
                {
                    viewModel.Mode = CreateEditGovernorViewModel.EditMode.Edit;
                    viewModel.AppointingBodyId = model.AppointingBodyId;
                    viewModel.AppointmentEndDate = new DateTimeViewModel(model.AppointmentEndDate);
                    viewModel.AppointmentStartDate = new DateTimeViewModel(model.AppointmentStartDate);
                    viewModel.DOB = new DateTimeViewModel(model.DOB);
                    viewModel.EmailAddress = model.EmailAddress;

                    viewModel.GovernorTitleId = model.Person_TitleId;
                    viewModel.FirstName = model.Person_FirstName;
                    viewModel.MiddleName = model.Person_MiddleName;
                    viewModel.LastName = model.Person_LastName;

                    viewModel.PreviousTitleId = model.PreviousPerson_TitleId;
                    viewModel.PreviousFirstName = model.PreviousPerson_FirstName;
                    viewModel.PreviousMiddleName = model.PreviousPerson_MiddleName;
                    viewModel.PreviousLastName = model.PreviousPerson_LastName;

                    viewModel.GID = model.Id;
                    viewModel.NationalityId = model.NationalityId; //todo: textchange
                    viewModel.TelephoneNumber = model.TelephoneNumber;
                    viewModel.PostCode = model.PostCode;

                    viewModel.EstablishmentUrn = model.EstablishmentUrn;
                    viewModel.GroupUId = model.GroupUId;

                    viewModel.IsHistoric = model.AppointmentEndDate.HasValue &&
                                           model.AppointmentEndDate.Value < DateTime.Now.Date;
                }
            }

            await PopulateLayoutProperties(viewModel, establishmentUrn, groupUId);

            viewModel.GovernorRoleName = _nomenclatureService.GetGovernorRoleName(role.Value);
            viewModel.GovernorRole = role.Value;
            await PopulateSelectLists(viewModel);
            viewModel.DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync(role.Value, groupUId.HasValue, User);

            ModelState.Clear();

            return View(viewModel);}


        [Route(GROUP_ADD_GOVERNOR), Route(ESTAB_ADD_GOVERNOR), 
            Route(GROUP_EDIT_GOVERNOR), Route(ESTAB_EDIT_GOVERNOR),
            Route(GROUP_REPLACE_GOVERNOR), HttpPost]
        public async Task<ActionResult> AddEditOrReplace(CreateEditGovernorViewModel viewModel)
        {
            await PopulateSelectLists(viewModel);
            viewModel.DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync(viewModel.GovernorRole, viewModel.GroupUId.HasValue, User);

            var governorModel = new GovernorModel
            {
                AppointingBodyId = viewModel.AppointingBodyId,
                AppointmentEndDate = viewModel.AppointmentEndDate.ToDateTime(),
                AppointmentStartDate = viewModel.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime()?.AddDays(1),
                DOB = viewModel.DOB.ToDateTime(),
                EmailAddress = viewModel.EmailAddress,
                GroupUId = viewModel.GroupUId,
                EstablishmentUrn = viewModel.EstablishmentUrn,
                NationalityId = viewModel.NationalityId,
                Id = viewModel.GID,
                Person_FirstName = viewModel.FirstName,
                Person_MiddleName = viewModel.MiddleName,
                Person_LastName = viewModel.LastName,
                Person_TitleId = viewModel.GovernorTitleId,
                PreviousPerson_FirstName = viewModel.PreviousFirstName,
                PreviousPerson_MiddleName = viewModel.PreviousMiddleName,
                PreviousPerson_LastName = viewModel.PreviousLastName,
                PreviousPerson_TitleId = viewModel.PreviousTitleId,
                PostCode = viewModel.PostCode,
                RoleId = (int) viewModel.GovernorRole,
                TelephoneNumber = viewModel.TelephoneNumber
            };

            var validationResults = await _governorsWriteService.ValidateAsync(governorModel, User);
            validationResults.ApplyToModelState(ControllerContext);

            if (ModelState.IsValid)
            {
                ApiResponse<int> response;
                if (!viewModel.EstablishmentUrn.HasValue &&
                    EnumSets.eSharedGovernorRoles.Contains(viewModel.GovernorRole))
                {
                    var existingGovernors = await _governorsReadService.GetGovernorListAsync(null, viewModel.GroupUId, User);
                    var duplicates = existingGovernors.CurrentGovernors.Where(g => g.RoleId == (int) viewModel.GovernorRole
                                                                                && string.Equals($"{g.Person_TitleId} {g.Person_FirstName} {g.Person_MiddleName} {g.Person_LastName}", 
                                                                                                 $"{viewModel.GovernorTitleId} {viewModel.FirstName} {viewModel.MiddleName} {viewModel.LastName}", 
                                                                                                 StringComparison.OrdinalIgnoreCase));
                    if (duplicates.Any())
                    {
                        ModelState.Clear();
                        return RedirectToRoute("GroupEditGovernance", new { groupUId = viewModel.GroupUId, duplicateGovernorId = duplicates.First().Id});
                    }
                }

                response = await _governorsWriteService.SaveAsync(governorModel, User);
                
                if (response.Success)
                {
                    viewModel.GID = response.Response;
                    ModelState.Clear();

                    return RedirectToRoute(viewModel.EstablishmentUrn.HasValue ? "EstabEditGovernance" : "GroupEditGovernance",
                        new { establishmentUrn = viewModel.EstablishmentUrn, groupUId = viewModel.GroupUId });
                }

                ErrorsToModelState<GovernorModel>(response.Errors);
            }

            await PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId);
            
            return View(viewModel);
        }

        private void ErrorsToModelState<TModel>(IEnumerable<ApiError> errors)
        {
            var type = typeof(TModel);
            var properties = type.GetProperties();
            foreach (var error in errors)
            {
                foreach (var property in properties)
                {
                    JsonPropertyAttribute attribute = null;
                    if (property.HasAttribute<JsonPropertyAttribute>())
                    {
                        attribute = property.GetAttribute<JsonPropertyAttribute>();
                    }

                    if (string.Equals(error.Fields, property.Name, StringComparison.OrdinalIgnoreCase) ||
                        (attribute != null && string.Equals(error.Fields, attribute.PropertyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError(property.Name, error.Message);
                    }
                }
            }
        }

        [HttpGet, Route(ESTAB_SELECT_SHARED_GOVERNOR, Name = "SelectSharedGovernor")]
        public async Task<ActionResult> SelectSharedGovernor(int establishmentUrn, eLookupGovernorRole role)
        {
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == (int)role).Name;
            var governors = (await _governorsReadService.GetSharedGovernorsAsync(establishmentUrn, User)).Where(g => RoleEquivalence.GetEquivalentRole(role).Contains((eLookupGovernorRole)g.RoleId)).ToList();

            var viewModel = new SelectSharedGovernorViewModel
            {
                Governors = governors.Select(g => MapGovernorToSharedGovernorViewModel(g, establishmentUrn)).ToList(),
                GovernorType = roleName.ToLowerInvariant()
            };

            await PopulateLayoutProperties(viewModel, establishmentUrn, null, null);

            return View(viewModel);
        }

        [HttpPost, Route(ESTAB_SELECT_SHARED_GOVERNOR)]
        public async Task<ActionResult> SelectSharedGovernor(SelectSharedGovernorViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var governor in model.Governors.Where(g => (g.Selected && !g.PreExisting) || string.Equals(g.Id.ToString(), model.SelectedGovernorId)))
                {
                    var response = await _governorsWriteService.AddSharedGovernorAppointmentAsync(governor.Id, model.Urn.Value, governor.AppointmentStartDate.ToDateTime().Value, governor.AppointmentEndDate.ToDateTime(), User);
                    if (!response.Success)
                    {
                        response.ApplyToModelState(ControllerContext);
                    }
                }

                if (ModelState.IsValid)
                {
                    return RedirectToRoute("EstabEditGovernance", new {establishmentUrn = model.Urn});
                }
            }

            var governors = (await _governorsReadService.GetSharedGovernorsAsync(model.Urn.Value, User))
                .Where(g => RoleEquivalence.GetEquivalentRole(model.Role).Contains((eLookupGovernorRole)g.RoleId))
                .Select(g => MapGovernorToSharedGovernorViewModel(g, model.Urn.Value))
                .ToList();

            foreach (var previousGovernor in model.Governors)
            {
                var newGovernor = governors.Single(g => g.Id == previousGovernor.Id);
                if (!newGovernor.PreExisting)
                {
                    newGovernor.Selected = previousGovernor.Selected;
                }
            }

            model.Governors = governors;
            await PopulateLayoutProperties(model, model.Urn.Value, null, null);

            return View(model);
        }

        [HttpGet, Route(ESTAB_EDIT_SHARED_GOVERNOR, Name="EditSharedGovernor")]
        public async Task<ActionResult> EditSharedGovernor(int establishmentUrn, int governorId)
        {
            var governor = await _governorsReadService.GetGovernorAsync(governorId, User);
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == governor.RoleId.Value).Name;

            var model = new EditSharedGovernorViewModel
            {
                Governor = MapGovernorToSharedGovernorViewModel(governor, establishmentUrn),
                GovernorType = roleName
            };

            await PopulateLayoutProperties(model, establishmentUrn, null, null);
            return View(model);
        }

        [HttpPost, Route(ESTAB_EDIT_SHARED_GOVERNOR)]
        public async Task<ActionResult> EditSharedGovernor(EditSharedGovernorViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _governorsWriteService.UpdateDatesAsync(model.Governor.Id, model.Governor.AppointmentStartDate.ToDateTime().Value, model.Governor.AppointmentEndDate.ToDateTime().Value, User);
                return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = model.Urn });
            }

            var governor = await _governorsReadService.GetGovernorAsync(model.Governor.Id, User);
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == governor.RoleId.Value).Name;
            model.Governor = MapGovernorToSharedGovernorViewModel(governor, model.Urn.Value);
            model.GovernorType = roleName;

            await PopulateLayoutProperties(model, model.Urn.Value, null, null);

            return View(model);
        }

        [HttpGet, Route(ESTAB_REPLACE_GOVERNOR, Name = "EstabReplaceGovernor")]
        public async Task<ActionResult> ReplaceChair(int establishmentUrn, int gid)
        {
            var governor = await _governorsReadService.GetGovernorAsync(gid, User);
            var governors = (await _governorsReadService.GetSharedGovernorsAsync(establishmentUrn, User)).Where(g => g.RoleId == governor.RoleId && g.Id != gid).ToList();

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = gid,
                GovernorFullName = governor.GetFullName(),
                DateTermEnds = new DateTimeViewModel(governor.AppointmentEndDate),
                NewLocalGovernor = new GovernorViewModel
                {
                    DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync((eLookupGovernorRole)governor.RoleId.Value, false, User)
                },
                SharedGovernors = governors.Select(g => MapGovernorToSharedGovernorViewModel(g, establishmentUrn)).ToList(),
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair
            };

            await PopulateSelectLists(model.NewLocalGovernor);
            await PopulateLayoutProperties(model, establishmentUrn, null, null);

            return View(model);
        }

        [HttpPost, Route(ESTAB_REPLACE_GOVERNOR)]
        public async Task<ActionResult> ReplaceChair(ReplaceChairViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ExistingChairType == ReplaceChairViewModel.ChairType.SharedChair)
                {
                    var existingGovernor =
                        await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User);
                    await _governorsWriteService.UpdateDatesAsync(model.ExistingGovernorId,
                        existingGovernor.Appointments.SingleOrDefault(a => a.EstablishmentUrn == model.Urn.Value).AppointmentStartDate.Value,
                        model.DateTermEnds.ToDateTime().Value, User);
                }
                else
                {
                    var existingGovernor = await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User);
                    existingGovernor.AppointmentEndDate = model.DateTermEnds.ToDateTime();
                    await _governorsWriteService.SaveAsync(existingGovernor, User);
                }

                if (model.NewChairType == ReplaceChairViewModel.ChairType.SharedChair)
                {
                    var newGovernor = model.SharedGovernors.SingleOrDefault(s => s.Id == model.SelectedGovernorId);
                    await _governorsWriteService.UpdateDatesAsync(model.SelectedGovernorId, 
                        newGovernor.AppointmentStartDate.ToDateTime().Value,
                        newGovernor.AppointmentEndDate.ToDateTime().Value, User);
                }
                else
                {
                    await _governorsWriteService.SaveAsync(new GovernorModel
                    {
                        AppointingBodyId = model.NewLocalGovernor.AppointingBodyId,
                        AppointmentEndDate = model.NewLocalGovernor.AppointmentEndDate.ToDateTime(),
                        AppointmentStartDate = model.NewLocalGovernor.AppointmentStartDate.ToDateTime(),
                        DOB = model.NewLocalGovernor.DOB.ToDateTime(),
                        EmailAddress = model.NewLocalGovernor.EmailAddress,
                        EstablishmentUrn = model.Urn,
                        NationalityId = model.NewLocalGovernor.NationalityId,
                        Person_FirstName = model.NewLocalGovernor.FirstName,
                        Person_MiddleName = model.NewLocalGovernor.MiddleName,
                        Person_LastName = model.NewLocalGovernor.LastName,
                        //Person_Title = model.NewLocalGovernor.GovernorTitle,//todo: texchange
                        PreviousPerson_FirstName = model.NewLocalGovernor.PreviousFirstName,
                        PreviousPerson_MiddleName = model.NewLocalGovernor.PreviousMiddleName,
                        PreviousPerson_LastName = model.NewLocalGovernor.PreviousLastName,
                        //PreviousPerson_Title = model.NewLocalGovernor.PreviousTitle,//todo: texchange
                        PostCode = model.NewLocalGovernor.PostCode,
                        RoleId = (int)eLookupGovernorRole.ChairOfTrustees,
                        TelephoneNumber = model.NewLocalGovernor.TelephoneNumber
                    }, User);
                }

                return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = model.Urn });
            }

            var governor = await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User) ?? await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User);
            var governors = (await _governorsReadService.GetSharedGovernorsAsync(model.Urn.Value, User)).Where(g => g.RoleId == governor.RoleId && g.Id != model.ExistingGovernorId).ToList();

            model.NewLocalGovernor.DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync((eLookupGovernorRole)governor.RoleId.Value, false, User);
            model.SharedGovernors = governors.Select(g => MapGovernorToSharedGovernorViewModel(g, model.Urn.Value)).ToList();

            await PopulateSelectLists(model.NewLocalGovernor);
            await PopulateLayoutProperties(model, model.Urn, null, null);

            return View(model);
        }

        [HttpGet]
        [Route(GROUP_EDIT_DELEGATION, Name = "GroupEditDelegation")]
        public async Task<ActionResult> GroupEditDelegation(int groupUId)
        {
            var group = await _groupReadService.GetAsync(groupUId, User);
            if (group.Success)
            {
                var model = new EditGroupDelegationInformationViewModel
                {
                    DelegationInformation = group.ReturnValue.DelegationInformation
                };

                await PopulateLayoutProperties(model, null, groupUId);

                return View(model);
            }
            return RedirectToRoute("GroupEditGovernance", new { GroupUId = groupUId });
        }

        [HttpPost]
        [Route(GROUP_EDIT_DELEGATION)]
        public async Task<ActionResult> GroupEditDelegation(EditGroupDelegationInformationViewModel model)
        {
            var result = await new EditGroupDelegationInformationViewModelValidator().ValidateAsync(model);

            if (ModelState.IsValid)
            {
                var groupResult = await _groupReadService.GetAsync(model.GroupUId.Value, User);
                if (groupResult.Success)
                {
                    var group = groupResult.ReturnValue;
                    group.DelegationInformation = model.DelegationInformation;
                    var updatedGroup = new SaveGroupDto(group);
                    await _groupWriteService.SaveAsync(updatedGroup, User);
                }
                return RedirectToRoute("GroupEditGovernance", new { GroupUId = model.GroupUId });
            }

            result.EduBaseAddToModelState(ModelState, null);
            await PopulateLayoutProperties(model, null, model.GroupUId); 
            return View(model);
        }

        private SharedGovernorViewModel MapGovernorToSharedGovernorViewModel(GovernorModel governor, int establishmentUrn)
        {
            var dateNow = DateTime.Now.Date;
            var appointment = governor.Appointments?.SingleOrDefault(g => g.EstablishmentUrn == establishmentUrn);
            var sharedWith = governor.Appointments?
                                     .Where(a => a.AppointmentStartDate < dateNow && (a.AppointmentEndDate == null || a.AppointmentEndDate > dateNow))
                                     .Select(a => new SharedGovernorViewModel.EstablishmentViewModel { Urn = a.EstablishmentUrn.Value, EstablishmentName = a.EstablishmentName })
                                     .ToList();

            return new SharedGovernorViewModel
            {
                //AppointingBodyName = governor.AppointingBodyId, // todo: texchange!
                AppointmentStartDate = appointment?.AppointmentStartDate != null ? new DateTimeViewModel(appointment.AppointmentStartDate) : new DateTimeViewModel(),
                AppointmentEndDate = appointment?.AppointmentEndDate != null ? new DateTimeViewModel(appointment.AppointmentEndDate) : new DateTimeViewModel(),
                DOB = governor.DOB,
                FullName = governor.GetFullName(),
                Id = governor.Id.Value,
                //Nationality = governor.Nationality, // todo: texchange
                PostCode = governor.PostCode,
                Selected = appointment != null,
                PreExisting = appointment != null,
                SharedWith = sharedWith ?? new List<SharedGovernorViewModel.EstablishmentViewModel>(),
                MultiSelect = IsSharedGovernorRoleMultiSelect((eLookupGovernorRole)governor.RoleId)
            };
        }

        private bool IsSharedGovernorRoleMultiSelect(eLookupGovernorRole role)
        {
            return role == eLookupGovernorRole.Group_SharedLocalGovernor;
        }

        private async Task PopulateSelectLists(GovernorViewModel viewModel)
        {
            viewModel.AppointingBodies = (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).ToSelectList(viewModel.AppointingBodyId);
            viewModel.Nationalities = (await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(viewModel.NationalityId);
            viewModel.Titles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.GovernorTitleId);
            viewModel.PreviousTitles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.PreviousTitleId);
        }
        
        private async Task PopulateLayoutProperties(object viewModel, int? establishmentUrn, int? groupUId, Action<EstablishmentModel> processEstablishment = null, Action<GroupModel> processGroup = null)
        {
            if (establishmentUrn.HasValue && groupUId.HasValue)
                throw new InvalidParameterException("Both urn and uid cannot be populated");
            else if(!establishmentUrn.HasValue && !groupUId.HasValue)
                throw new InvalidParameterException($"Both {nameof(establishmentUrn)} and {nameof(groupUId)} parameters are null");

            if (establishmentUrn.HasValue)
            {
                var domainModel = (await _establishmentReadService.GetAsync(establishmentUrn.Value, User)).GetResult();
                var displayPolicy = (await _establishmentReadService.GetDisplayPolicyAsync(domainModel, User));

                var vm = (IEstablishmentPageViewModel)viewModel;
                vm.Layout = ESTAB_LAYOUT;
                vm.Name = domainModel.Name;
                vm.SelectedTab = "governance";
                vm.Urn = domainModel.Urn;
                vm.TabDisplayPolicy = new TabDisplayPolicy(domainModel, displayPolicy, User);
                processEstablishment?.Invoke(domainModel);
            }
            else if (groupUId.HasValue)
            {
                var domainModel = (await _groupReadService.GetAsync(groupUId.Value, User)).GetResult();
                var vm = (IGroupPageViewModel)viewModel;
                vm.Layout = GROUPS_LAYOUT;
                vm.GroupName = domainModel.Name;
                vm.GroupTypeId = domainModel.GroupTypeId.Value;
                vm.GroupUId = groupUId;
                vm.SelectedTabName = "governance";
                vm.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((eLookupGroupType)vm.GroupTypeId.Value);
                processGroup?.Invoke(domainModel);
            }
        }
    }
}