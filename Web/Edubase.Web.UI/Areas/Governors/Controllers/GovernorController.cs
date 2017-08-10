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
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Castle.Core.Internal;
using Edubase.Services.Domain;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Validation;
using Newtonsoft.Json;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("Governor")]
    public class GovernorController : Controller
    {
        const string GROUP_EDIT_GOVERNANCE = "~/Groups/Group/Edit/{groupUId:int}/Governance";
        const string ESTAB_EDIT_GOVERNANCE = "~/Establishment/Edit/{establishmentUrn:int}/Governance";

        const string GROUP_ADD_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Add";
        const string ESTAB_ADD_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Add";

        const string GROUP_EDIT_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Edit/{gid:int}";
        const string ESTAB_EDIT_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Edit/{gid:int}";

        const string GROUP_REPLACE_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Replace/{gid:int}";
        const string ESTAB_REPLACE_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Replace/{gid:int}";

        const string VIEW_EDIT_GOV_VIEW_NAME = "~/Areas/Governors/Views/Governor/ViewEdit.cshtml";

        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly NomenclatureService _nomenclatureService;
        private readonly IGovernorsWriteService _governorsWriteService;
        private readonly IGroupReadService _groupReadService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly LayoutHelper _layoutHelper;

        public GovernorController(
            IGovernorsReadService governorsReadService,
            NomenclatureService nomenclatureService,
            ICachedLookupService cachedLookupService,
            IGovernorsWriteService governorsWriteService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            LayoutHelper layoutHelper)
        {
            _governorsReadService = governorsReadService;
            _nomenclatureService = nomenclatureService;
            _cachedLookupService = cachedLookupService;
            _governorsWriteService = governorsWriteService;
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _layoutHelper = layoutHelper;
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

            
            var domainModel = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
            var viewModel = new GovernorsGridViewModel(domainModel, true, groupUId, establishmentUrn, _nomenclatureService, 
                (await _cachedLookupService.NationalitiesGetAllAsync()), 
                (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()));
                
                var applicableRoles = domainModel.ApplicableRoles.Cast<int>();
                viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).Where(x => applicableRoles.Contains(x.Id)).Select(x => new LookupItemViewModel(x)).ToList();

            await _layoutHelper.PopulateLayoutProperties(viewModel, establishmentUrn, groupUId, User, x => viewModel.GovernanceMode = x.GovernanceMode, x=>
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

        [Route(GROUP_EDIT_GOVERNANCE, Name = "GroupDeleteOrRetireGovernor"), Route(ESTAB_EDIT_GOVERNANCE, Name = "EstabDeleteOrRetireGovernor"), HttpPost]
        public async Task<ActionResult> DeleteOrRetireGovernor(GovernorsGridViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Action == "Save") // retire selected governor with the chosen appt. end date
                {
                    if (viewModel.GovernorShared.HasValue && viewModel.GovernorShared.Value && viewModel.EstablishmentUrn.HasValue)
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

            await _layoutHelper.PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId, User);

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
                    viewModel = await CreateGovernorsViewModel(groupUId, establishmentUrn);
                    return View(VIEW_EDIT_GOV_VIEW_NAME, viewModel);
                    
                }).Result;
            }
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
                if (!await RoleAllowed(role.Value, groupUId, establishmentUrn, User))
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

            await _layoutHelper.PopulateLayoutProperties(viewModel, establishmentUrn, groupUId, User);

            viewModel.GovernorRoleName = _nomenclatureService.GetGovernorRoleName(role.Value);
            viewModel.GovernorRole = role.Value;
            await PopulateSelectLists(viewModel);
            viewModel.DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync(role.Value, groupUId.HasValue, User);

            ModelState.Clear();

            return View(viewModel);
        }

        private async Task<bool> RoleAllowed(eLookupGovernorRole roleId, int? groupUId, int? establishmentUrn, IPrincipal user)
        {
            var existingGovernors = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, user);

            if (EnumSets.eSingularGovernorRoles.Contains(roleId))
            {
                if (roleId == eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody ||
                    roleId == eLookupGovernorRole.ChairOfLocalGoverningBody)
                {
                    return !existingGovernors.CurrentGovernors.Any(
                        g => g.RoleId == (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody ||
                             g.RoleId == (int) eLookupGovernorRole.ChairOfLocalGoverningBody);
                }

                return existingGovernors.CurrentGovernors.All(g => g.RoleId != (int) roleId);
            }

            return true;
        }

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
                AppointmentStartDate = viewModel.Mode == CreateEditGovernorViewModel.EditMode.Replace ? viewModel.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime()?.AddDays(1) : viewModel.AppointmentStartDate.ToDateTime(),
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

                var response = await _governorsWriteService.SaveAsync(governorModel, User);
                
                if (response.Success)
                {
                    viewModel.GID = response.Response;
                    ModelState.Clear();

                    return RedirectToRoute(viewModel.EstablishmentUrn.HasValue ? "EstabEditGovernance" : "GroupEditGovernance",
                        new { establishmentUrn = viewModel.EstablishmentUrn, groupUId = viewModel.GroupUId });
                }

                ErrorsToModelState<GovernorModel>(response.Errors);
            }

            await _layoutHelper.PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId, User);
            
            return View(viewModel);
        }

        [HttpGet, Route(ESTAB_REPLACE_GOVERNOR, Name = "EstabReplaceGovernor")]
        public async Task<ActionResult> ReplaceChair(int establishmentUrn, int gid)
        {
            var governor = await _governorsReadService.GetGovernorAsync(gid, User);
            var roles = new List<eLookupGovernorRole>
            {
                (eLookupGovernorRole)governor.RoleId
            };

            if (EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value))
            {
                var localEquivalent =
                    RoleEquivalence.GetLocalEquivalentToSharedRole((eLookupGovernorRole) governor.RoleId);
                if (localEquivalent != null)
                    roles.Add(localEquivalent.Value);

            }
            else
            {
                roles.AddRange(RoleEquivalence.GetEquivalentToLocalRole((eLookupGovernorRole)governor.RoleId));
            }
            
            var governors = (await _governorsReadService.GetSharedGovernorsAsync(establishmentUrn, User)).Where(g => roles.Contains((eLookupGovernorRole)g.RoleId) && g.Id != gid).ToList();

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = gid,
                GovernorFullName = governor.GetFullName(),
                DateTermEnds = new DateTimeViewModel(governor.AppointmentEndDate),
                NewLocalGovernor = new GovernorViewModel
                {
                    DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync((eLookupGovernorRole)governor.RoleId.Value, false, User)
                },
                SharedGovernors = (await Task.WhenAll(governors.Select(async g => await SharedGovernorViewModel.MapFromGovernor(g, establishmentUrn, _cachedLookupService)))).ToList(),
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                Role = (eLookupGovernorRole)governor.RoleId
            };

            await PopulateSelectLists(model.NewLocalGovernor);
            await _layoutHelper.PopulateLayoutProperties(model, establishmentUrn, null, User);

            return View(model);
        }

        [HttpPost, Route(ESTAB_REPLACE_GOVERNOR)]
        public async Task<ActionResult> ReplaceChair(ReplaceChairViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewChairType == ReplaceChairViewModel.ChairType.SharedChair)
                {
                    var newGovernor = model.SharedGovernors.SingleOrDefault(s => s.Id == model.SelectedGovernorId);

                    await _governorsWriteService.AddSharedGovernorAppointmentAsync(model.SelectedGovernorId, model.Urn.Value,
                        model.DateTermEnds.ToDateTime().Value.AddDays(1),
                        newGovernor.AppointmentEndDate.ToDateTime(), User);

                    return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = model.Urn });
                }
                else
                {
                    var newGovernor = new GovernorModel
                    {
                        AppointingBodyId = model.NewLocalGovernor.AppointingBodyId,
                        AppointmentEndDate = model.NewLocalGovernor.AppointmentEndDate.ToDateTime(),
                        AppointmentStartDate = model.DateTermEnds.ToDateTime().Value.AddDays(1),
                        DOB = model.NewLocalGovernor.DOB.ToDateTime(),
                        EmailAddress = model.NewLocalGovernor.EmailAddress,
                        EstablishmentUrn = model.Urn,
                        NationalityId = model.NewLocalGovernor.NationalityId,
                        Person_FirstName = model.NewLocalGovernor.FirstName,
                        Person_MiddleName = model.NewLocalGovernor.MiddleName,
                        Person_LastName = model.NewLocalGovernor.LastName,
                        Person_TitleId = model.NewLocalGovernor.GovernorTitleId,
                        PreviousPerson_FirstName = model.NewLocalGovernor.PreviousFirstName,
                        PreviousPerson_MiddleName = model.NewLocalGovernor.PreviousMiddleName,
                        PreviousPerson_LastName = model.NewLocalGovernor.PreviousLastName,
                        PreviousPerson_TitleId = model.NewLocalGovernor.PreviousTitleId,
                        PostCode = model.NewLocalGovernor.PostCode,
                        RoleId = (int) model.Role,
                        TelephoneNumber = model.NewLocalGovernor.TelephoneNumber
                    };

                    var validation = await _governorsWriteService.ValidateAsync(newGovernor, User);

                    if (!validation.HasErrors)
                    {
                        await _governorsWriteService.SaveAsync(newGovernor, User);
                        return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = model.Urn });
                    }
                    
                    validation.ApplyToModelState(ControllerContext, nameof(model.NewLocalGovernor));
                }
            }

            var governor = await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User) ?? await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User);
            var roles = new List<eLookupGovernorRole>
            {
                (eLookupGovernorRole)governor.RoleId
            };

            if (EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value))
            {
                var localEquivalent =
                    RoleEquivalence.GetLocalEquivalentToSharedRole((eLookupGovernorRole)governor.RoleId);
                if (localEquivalent != null)
                    roles.Add(localEquivalent.Value);

            }
            else
            {
                roles.AddRange(RoleEquivalence.GetEquivalentToLocalRole((eLookupGovernorRole)governor.RoleId));
            }

            var governors = (await _governorsReadService.GetSharedGovernorsAsync(model.Urn.Value, User)).Where(g => roles.Contains((eLookupGovernorRole)g.RoleId) && g.Id != model.ExistingGovernorId).ToList();

            model.NewLocalGovernor.DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync((eLookupGovernorRole)governor.RoleId.Value, false, User);
            model.SharedGovernors = (await Task.WhenAll(governors.Select(async g => await SharedGovernorViewModel.MapFromGovernor(g, model.Urn.Value, _cachedLookupService)))).ToList();

            await PopulateSelectLists(model.NewLocalGovernor);
            await _layoutHelper.PopulateLayoutProperties(model, model.Urn, null, User);

            return View(model);
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
                var items = await _establishmentReadService.GetPermissibleLocalGovernorsAsync(establishmentUrn.Value, User); // The API uses 1 as a default value, hence we have to call another API to deduce whether to show the Governance mode UI section
                viewModel.GovernanceMode = items.Any() ? estabDomainModel.GovernanceMode : null;
            }

            if (groupUId.HasValue)
            {
                var groupModel = (await _groupReadService.GetAsync(groupUId.Value, User)).GetResult();
                viewModel.ShowDelegationInformation = groupModel.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust;
                viewModel.DelegationInformation = groupModel.DelegationInformation;
            }

            return viewModel;
        }

        private async Task PopulateSelectLists(GovernorViewModel viewModel)
        {
            viewModel.AppointingBodies = (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).ToSelectList(viewModel.AppointingBodyId);
            viewModel.Nationalities = (await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(viewModel.NationalityId);
            viewModel.Titles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.GovernorTitleId);
            viewModel.PreviousTitles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.PreviousTitleId);
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
    }
}