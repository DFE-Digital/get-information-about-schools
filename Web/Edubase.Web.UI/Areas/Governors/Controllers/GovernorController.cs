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
using System.Web;
using System.Web.Http.Routing;
using System.Web.Mvc;
using Edubase.Services;
using Edubase.Services.Governors.Search;
using Edubase.Web.UI.Helpers;

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
        const string ESTAB_ADD_SHARED_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/AddSharedGovernor";
        const string ESTAB_EDIT_SHARED_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/EditSharedGovernor";

        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly NomenclatureService _nomenclatureService;
        private readonly IGovernorsWriteService _governorsWriteService;
        private readonly IGroupReadService _groupReadService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;

        public GovernorController(
            IGovernorsReadService governorsReadService,
            NomenclatureService nomenclatureService,
            ICachedLookupService cachedLookupService,
            IGovernorsWriteService governorsWriteService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService)
        {
            _governorsReadService = governorsReadService;
            _nomenclatureService = nomenclatureService;
            _cachedLookupService = cachedLookupService;
            _governorsWriteService = governorsWriteService;
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _establishmentWriteService = establishmentWriteService;
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="establishmentUrn"></param>
        /// <returns></returns>
        [Route(ESTAB_EDIT_GOVERNANCE_MODE, Name = "EstabEditGovernanceMode"), HttpGet]
        public async Task<ActionResult> EditGovernanceMode(int? establishmentUrn)
        {
            Guard.IsTrue(establishmentUrn.HasValue, () => new InvalidParameterException($"Parameter '{nameof(establishmentUrn)}' is null."));

            using (MiniProfiler.Current.Step("Retrieving Governors Details"))
            {
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
            var domainModel = (await _establishmentReadService.GetAsync(viewModel.Urn.Value, User)).GetResult();
            domainModel.GovernanceMode = viewModel.GovernanceMode;
            await _establishmentWriteService.SaveAsync(domainModel, User);
            return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = viewModel.Urn });
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="groupUId"></param>
        /// <param name="establishmentUrn"></param>
        /// <param name="editMode"></param>
        /// <returns></returns>
        [Route(GROUP_EDIT_GOVERNANCE, Name = "GroupEditGovernance"), Route(ESTAB_EDIT_GOVERNANCE, Name = "EstabEditGovernance"), HttpGet]
        public async Task<ActionResult> Edit(int? groupUId, int? establishmentUrn, int? removalGid, int? duplicateGovernorId)
        {
            Guard.IsTrue(groupUId.HasValue || establishmentUrn.HasValue, () => new InvalidParameterException($"Both parameters '{nameof(groupUId)}' and '{nameof(establishmentUrn)}' are null."));

            using (MiniProfiler.Current.Step("Retrieving Governors Details"))
            {
                var domainModel = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
                var viewModel = new GovernorsGridViewModel(domainModel, true, groupUId, establishmentUrn, _nomenclatureService);

                var applicableRoles = domainModel.ApplicableRoles.Cast<int>();
                viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).Where(x => applicableRoles.Contains(x.Id)).Select(x => new LookupItemViewModel(x)).ToList();

                await PopulateLayoutProperties(viewModel, establishmentUrn, groupUId, x => viewModel.GovernanceMode = x.GovernanceMode);

                viewModel.RemovalGid = removalGid;
                if (duplicateGovernorId.HasValue)
                {

                    var duplicate = await _governorsReadService.GetGovernorAsync(duplicateGovernorId.Value);
                    ViewData.Add("DuplicateGovernor", duplicate);
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
                    var domainModel = await _governorsReadService.GetGovernorAsync(viewModel.RemovalGid.Value, User);
                    domainModel.AppointmentEndDate = viewModel.RemovalAppointmentEndDate.ToDateTime().Value;
                    await _governorsWriteService.SaveAsync(domainModel, User);
                }
                else if (viewModel.Action == "Remove") // mark the governor record as deleted
                {
                    await _governorsWriteService.DeleteAsync(viewModel.RemovalGid.Value, User);
                }
                else throw new InvalidParameterException($"The parameter for action is invalid: '{viewModel.Action}'");

                return RedirectToRoute(viewModel.EstablishmentUrn.HasValue ? "EstabEditGovernance" : "GroupEditGovernance", new { viewModel.EstablishmentUrn, viewModel.GroupUId });
            }

            await PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId);

            return await Edit(viewModel.GroupUId, viewModel.EstablishmentUrn, viewModel.RemovalGid, null);
        }

        [Route]
        public ActionResult View(int? groupUId, int? establishmentUrn)
        {
            // KHD Hack: Async child actions are not supported; but we have an async stack, so we have to wrap the async calls in an sync wrapper.  Hopefully won't deadlock.
            // Need to use ASP.NET Core really now; that supports ViewComponents which are apparently the solution.
            return Task.Run(async () =>
            {
                using (MiniProfiler.Current.Step("Retrieving Governors Details"))
                {
                    var domainModel = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
                    var viewModel = new GovernorsGridViewModel(domainModel, false, groupUId, establishmentUrn, _nomenclatureService);

                    if (establishmentUrn.HasValue)
                    {
                        var estabDomainModel = await _establishmentReadService.GetAsync(establishmentUrn.Value, User);
                        viewModel.GovernanceMode = estabDomainModel.GetResult().GovernanceMode ?? eGovernanceMode.LocalGovernors;
                    }

                    return View(VIEW_EDIT_GOV_VIEW_NAME, viewModel);
                }
            }).Result;
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

            if (establishmentUrn.HasValue && role.HasValue &&
                role.Value.OneOfThese(eLookupGovernorRole.SharedLocalGovernor,
                    eLookupGovernorRole.SharedChairOfLocalGoverningBody))
            {
                return RedirectToRoute("SelectSharedGovernor", new {establishmentUrn = establishmentUrn.Value, role = role.Value});
            }

            if (role == null && gid == null) throw new EdubaseException("Role was not supplied and no Governor ID was supplied");
            var viewModel = new CreateEditGovernorViewModel
            {
                GroupUId = groupUId,
                EstablishmentUrn = establishmentUrn
            };

            if (gid.HasValue)
            {
                var model = await _governorsReadService.GetGovernorAsync(gid.Value, User);
                role = (eLookupGovernorRole)model.RoleId.Value;

                if (replaceMode)
                {
                    viewModel.ReplaceGovernorViewModel.AppointmentEndDate = new DateTimeViewModel(model.AppointmentEndDate);
                    viewModel.ReplaceGovernorViewModel.GID = gid;
                    viewModel.ReplaceGovernorViewModel.Name = model.GetFullName();
                }
                else
                {
                    viewModel.AppointingBodyId = model.AppointingBodyId;
                    viewModel.AppointmentEndDate = new DateTimeViewModel(model.AppointmentEndDate);
                    viewModel.AppointmentStartDate = new DateTimeViewModel(model.AppointmentStartDate);
                    viewModel.DOB = new DateTimeViewModel(model.DOB);
                    viewModel.EmailAddress = model.EmailAddress;

                    viewModel.GovernorTitle = model.Person_Title;
                    viewModel.FirstName = model.Person_FirstName;
                    viewModel.MiddleName = model.Person_MiddleName;
                    viewModel.LastName = model.Person_LastName;

                    viewModel.PreviousTitle = model.PreviousPerson_Title;
                    viewModel.PreviousFirstName = model.PreviousPerson_FirstName;
                    viewModel.PreviousMiddleName = model.PreviousPerson_MiddleName;
                    viewModel.PreviousLastName = model.PreviousPerson_LastName;

                    viewModel.GID = model.Id;
                    viewModel.NationalityId = !string.IsNullOrWhiteSpace(model.Nationality) ? (await _cachedLookupService.NationalitiesGetAllAsync()).SingleOrDefault(x => x.Name == model.Nationality)?.Id : null as int?;
                    viewModel.TelephoneNumber = model.TelephoneNumber;
                    viewModel.PostCode = model.PostCode;

                    viewModel.EstablishmentUrn = model.EstablishmentUrn;
                    viewModel.GroupUId = model.GroupUID;
                }
            }

            await PopulateLayoutProperties(viewModel, establishmentUrn, groupUId);

            viewModel.GovernorRoleName = _nomenclatureService.GetGovernorRoleName(role.Value);
            viewModel.GovernorRole = role.Value;
            await PopulateSelectLists(viewModel);
            viewModel.DisplayPolicy = _governorsReadService.GetEditorDisplayPolicy(role.Value, User);

            ModelState.Clear();

            return View(viewModel);}


        [Route(GROUP_ADD_GOVERNOR), Route(ESTAB_ADD_GOVERNOR), 
            Route(GROUP_EDIT_GOVERNOR), Route(ESTAB_EDIT_GOVERNOR),
            Route(GROUP_REPLACE_GOVERNOR), HttpPost]
        public async Task<ActionResult> AddEditOrReplace(CreateEditGovernorViewModel viewModel)
        {
            await PopulateSelectLists(viewModel);
            viewModel.DisplayPolicy = _governorsReadService.GetEditorDisplayPolicy(viewModel.GovernorRole, User);

            if (ModelState.IsValid)
            {
                if (!viewModel.EstablishmentUrn.HasValue &&
                    (viewModel.GovernorRole == eLookupGovernorRole.SharedChairOfLocalGoverningBody ||
                    viewModel.GovernorRole == eLookupGovernorRole.SharedLocalGovernor))
                {
                    var existingGovernors = await _governorsReadService.GetGovernorListAsync(null, viewModel.GroupUId, User);
                    var duplicates = existingGovernors.CurrentGovernors.Where(g => g.RoleId == (int) viewModel.GovernorRole
                                                                                && string.Equals($"{g.Person_Title} {g.Person_FirstName} {g.Person_MiddleName} {g.Person_LastName}", 
                                                                                                 $"{viewModel.GovernorTitle} {viewModel.FirstName} {viewModel.MiddleName} {viewModel.LastName}", 
                                                                                                 StringComparison.OrdinalIgnoreCase));
                    if (duplicates.Any())
                    {
                        ModelState.Clear();
                        return RedirectToRoute("GroupEditGovernance", new { groupUId = viewModel.GroupUId, duplicateGovernorId = duplicates.First().Id});
                    }
                }

                if (viewModel.ReplaceGovernorViewModel.GID.HasValue)
                {
                    var governorBeingReplaced = await _governorsReadService.GetGovernorAsync(viewModel.ReplaceGovernorViewModel.GID.Value, User);
                    governorBeingReplaced.AppointmentEndDate = viewModel.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime();
                    await _governorsWriteService.SaveAsync(governorBeingReplaced, User);
                }

                viewModel.GID = await _governorsWriteService.SaveAsync(new GovernorModel
                {
                    AppointingBodyId = viewModel.AppointingBodyId,
                    AppointmentEndDate = viewModel.AppointmentEndDate.ToDateTime(),
                    AppointmentStartDate = viewModel.AppointmentStartDate.ToDateTime(),
                    DOB = viewModel.DOB.ToDateTime(),
                    EmailAddress = viewModel.EmailAddress,
                    GroupUID = viewModel.GroupUId,
                    EstablishmentUrn = viewModel.EstablishmentUrn,
                    Nationality = viewModel.NationalityId.HasValue ? viewModel.Nationalities.FirstOrDefault(x => x.Value == viewModel.NationalityId.ToString())?.Text : null as string,
                    Id = viewModel.GID,
                    Person_FirstName = viewModel.FirstName,
                    Person_MiddleName = viewModel.MiddleName,
                    Person_LastName = viewModel.LastName,
                    Person_Title = viewModel.GovernorTitle,
                    PreviousPerson_FirstName = viewModel.PreviousFirstName,
                    PreviousPerson_MiddleName = viewModel.PreviousMiddleName,
                    PreviousPerson_LastName = viewModel.PreviousLastName,
                    PreviousPerson_Title = viewModel.PreviousTitle,
                    PostCode = viewModel.PostCode,
                    RoleId = (int)viewModel.GovernorRole,
                    TelephoneNumber = viewModel.TelephoneNumber
                }, User);

                ModelState.Clear();
                

                return RedirectToRoute(viewModel.EstablishmentUrn.HasValue ? "EstabEditGovernance" : "GroupEditGovernance", 
                    new { establishmentUrn = viewModel.EstablishmentUrn, groupUId = viewModel.GroupUId });
            }

            await PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId);
            
            return View(viewModel);
        }

        [HttpGet, Route(ESTAB_SELECT_SHARED_GOVERNOR, Name = "SelectSharedGovernor")]
        public async Task<ActionResult> SelectSharedGovernor(int establishmentUrn, eLookupGovernorRole role)
        {
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == (int)role).Name;
            var governors = (await _governorsReadService.GetSharedGovernorsAsync(establishmentUrn)).Where(g => g.RoleId == (int?)role).ToList();

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
                    await _governorsWriteService.AddUpdateEstablishmentToSharedGovernor(governor.Id, model.Urn.Value, governor.AppointmentStartDate.ToDateTime().Value, governor.AppointmentEndDate.ToDateTime().Value);
                }
                return RedirectToRoute("EstabEditGovernance", new {establishmentUrn = model.Urn});
            }

            await PopulateLayoutProperties(model, model.Urn.Value, null, null);

            return View(model);
        }

        [HttpGet, Route(ESTAB_EDIT_SHARED_GOVERNOR, Name="EditSharedGovernor")]
        public async Task<ActionResult> EditSharedGovernor(int establishmentUrn, int governorId)
        {
            var governor = await _governorsReadService.GetSharedGovernorAsync(governorId, establishmentUrn);
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
                await _governorsWriteService.AddUpdateEstablishmentToSharedGovernor(model.Governor.Id, model.Urn.Value, model.Governor.AppointmentStartDate.ToDateTime().Value, model.Governor.AppointmentEndDate.ToDateTime().Value);
                return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = model.Urn });
            }

            var governor = await _governorsReadService.GetSharedGovernorAsync(model.Governor.Id, model.Urn.Value);
            var roleName = (await _cachedLookupService.GovernorRolesGetAllAsync()).Single(x => x.Id == governor.RoleId.Value).Name;
            model.Governor = MapGovernorToSharedGovernorViewModel(governor, model.Urn.Value);
            model.GovernorType = roleName;

            await PopulateLayoutProperties(model, model.Urn.Value, null, null);

            return View(model);
        }

        [HttpGet, Route(ESTAB_REPLACE_GOVERNOR, Name = "EstabReplaceGovernor")]
        public async Task<ActionResult> ReplaceChair(int establishmentUrn, int gid)
        {
            var governor = await _governorsReadService.GetSharedGovernorAsync(gid, establishmentUrn) ?? await _governorsReadService.GetGovernorAsync(gid);
            var governors = (await _governorsReadService.GetSharedGovernorsAsync(establishmentUrn)).Where(g => g.RoleId == governor.RoleId && g.Id != gid).ToList();

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = gid,
                GovernorFullName = governor.GetFullName(),
                DateTermEnds = new DateTimeViewModel(governor.AppointmentEndDate),
                NewLocalGovernor = new GovernorViewModel
                {
                    DisplayPolicy = _governorsReadService.GetEditorDisplayPolicy((eLookupGovernorRole)governor.RoleId.Value)
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
                        await _governorsReadService.GetSharedGovernorAsync(model.ExistingGovernorId, model.Urn.Value);
                    await _governorsWriteService.AddUpdateEstablishmentToSharedGovernor(model.ExistingGovernorId,
                        model.Urn.Value, existingGovernor.Appointments.SingleOrDefault(a => a.EstablishmentUrn == model.Urn.Value).AppointmentStartDate.Value,
                        model.DateTermEnds.ToDateTime().Value);
                }
                else
                {
                    var existingGovernor = await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId);
                    existingGovernor.AppointmentEndDate = model.DateTermEnds.ToDateTime();
                    await _governorsWriteService.SaveAsync(existingGovernor, User);
                }

                if (model.NewChairType == ReplaceChairViewModel.ChairType.SharedChair)
                {
                    var newGovernor = model.SharedGovernors.SingleOrDefault(s => s.Id == model.SelectedGovernorId);
                    await _governorsWriteService.AddUpdateEstablishmentToSharedGovernor(model.SelectedGovernorId,
                        model.Urn.Value, newGovernor.AppointmentStartDate.ToDateTime().Value,
                        newGovernor.AppointmentEndDate.ToDateTime().Value);
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
                        Nationality = model.NewLocalGovernor.NationalityId.HasValue ? model.NewLocalGovernor.Nationalities.FirstOrDefault(x => x.Value == model.NewLocalGovernor.NationalityId.ToString())?.Text : null,
                        Person_FirstName = model.NewLocalGovernor.FirstName,
                        Person_MiddleName = model.NewLocalGovernor.MiddleName,
                        Person_LastName = model.NewLocalGovernor.LastName,
                        Person_Title = model.NewLocalGovernor.GovernorTitle,
                        PreviousPerson_FirstName = model.NewLocalGovernor.PreviousFirstName,
                        PreviousPerson_MiddleName = model.NewLocalGovernor.PreviousMiddleName,
                        PreviousPerson_LastName = model.NewLocalGovernor.PreviousLastName,
                        PreviousPerson_Title = model.NewLocalGovernor.PreviousTitle,
                        PostCode = model.NewLocalGovernor.PostCode,
                        RoleId = (int)model.NewLocalGovernor.GovernorRole,
                        TelephoneNumber = model.NewLocalGovernor.TelephoneNumber
                    }, User);
                }

                return RedirectToRoute("EstabEditGovernance", new { establishmentUrn = model.Urn });
            }

            return View(model);
        }

        private SharedGovernorViewModel MapGovernorToSharedGovernorViewModel(GovernorModel governor, int establishmentUrn)
        {
            var dateNow = DateTime.Now.Date;
            var appointment = governor.Appointments?.SingleOrDefault(g => g.EstablishmentUrn == establishmentUrn);
            var sharedWith = governor.Appointments
                                     .Where(a => a.AppointmentStartDate < dateNow && (a.AppointmentEndDate == null || a.AppointmentEndDate > dateNow))
                                     .Select(a => new SharedGovernorViewModel.EstablishmentViewModel { Urn = a.EstablishmentUrn.Value, EstablishmentName = a.EstablishmentName })
                                     .ToList();

            return new SharedGovernorViewModel
            {
                AppointingBodyName = governor.AppointingBodyName,
                AppointmentStartDate = appointment?.AppointmentStartDate != null ? new DateTimeViewModel(appointment.AppointmentStartDate) : new DateTimeViewModel(),
                AppointmentEndDate = appointment?.AppointmentEndDate != null ? new DateTimeViewModel(appointment.AppointmentEndDate) : new DateTimeViewModel(),
                DOB = governor.DOB,
                FullName = governor.GetFullName(),
                Id = governor.Id.Value,
                Nationality = governor.Nationality,
                PostCode = governor.PostCode,
                Selected = appointment != null,
                PreExisting = appointment != null,
                SharedWith = sharedWith,
                MultiSelect = IsSharedGovernorRoleMultiSelect((eLookupGovernorRole)governor.RoleId)
            };
        }

        private bool IsSharedGovernorRoleMultiSelect(eLookupGovernorRole role)
        {
            return role == eLookupGovernorRole.SharedLocalGovernor;
        }

        private async Task PopulateSelectLists(GovernorViewModel viewModel)
        {
            viewModel.AppointingBodies = (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).ToSelectList(viewModel.AppointingBodyId);
            viewModel.Nationalities = (await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(viewModel.NationalityId);
            viewModel.Titles = viewModel.GetTitles();
            viewModel.PreviousTitles = viewModel.GetTitles();
        }
        
        private async Task PopulateLayoutProperties(object viewModel, int? establishmentUrn, int? groupUId, Action<EstablishmentModel> processEstablishment = null)
        {
            if (establishmentUrn.HasValue && groupUId.HasValue)
                throw new InvalidParameterException("Both urn and uid cannot be populated");
            else if(!establishmentUrn.HasValue && !groupUId.HasValue)
                throw new InvalidParameterException($"Both {nameof(establishmentUrn)} and {nameof(groupUId)} parameters are null");

            if (establishmentUrn.HasValue)
            {
                var domainModel = (await _establishmentReadService.GetAsync(establishmentUrn.Value, User)).GetResult();

                var vm = (IEstablishmentPageViewModel)viewModel;
                vm.Layout = ESTAB_LAYOUT;
                vm.Name = domainModel.Name;
                vm.SelectedTab = "governance";
                vm.Urn = domainModel.Urn;
                vm.TabDisplayPolicy = new TabDisplayPolicy(domainModel, User);
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
            }
        }
    }
}