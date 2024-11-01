using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Core.Internal;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Factories;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Validation;
using Newtonsoft.Json;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("Governor")]
    public class GovernorController : Controller
    {
        private const string GROUP_EDIT_GOVERNANCE = "~/Groups/Group/Edit/{groupUId:int}/Governance";
        private const string ESTAB_EDIT_GOVERNANCE = "~/Establishment/Edit/{establishmentUrn:int}/Governance";
        private const string GROUP_ADD_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Add";
        private const string ESTAB_ADD_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Add";
        private const string GROUP_EDIT_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Edit/{gid:int}";
        private const string ESTAB_EDIT_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Edit/{gid:int}";
        private const string GROUP_REPLACE_GOVERNOR = "~/Groups/Group/Edit/{groupUId:int}/Governance/Replace/{gid:int}";
        private const string ESTAB_REPLACE_GOVERNOR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/Replace/{gid:int}";
        private const string ESTAB_REPLACE_CHAIR = "~/Establishment/Edit/{establishmentUrn:int}/Governance/ReplaceChair/{gid:int}";
        private const string VIEW_EDIT_GOV_VIEW_NAME = "~/Areas/Governors/Views/Governor/ViewEdit.cshtml";
        private const string EstablishmentDetails = "EstabDetails";

        private readonly ICachedLookupService _cachedLookupService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly IGovernorsWriteService _governorsWriteService;
        private readonly IGroupReadService _groupReadService;
        private readonly ILayoutHelper _layoutHelper;

        public GovernorController(
            IGovernorsReadService governorsReadService,
            ICachedLookupService cachedLookupService,
            IGovernorsWriteService governorsWriteService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            ILayoutHelper layoutHelper)
        {
            _governorsReadService = governorsReadService;
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
         HttpGet, EdubaseAuthorize]
        public async Task<ActionResult> Edit(int? groupUId, int? establishmentUrn, int? removalGid,
            int? duplicateGovernorId, bool roleAlreadyExists = false, eLookupGovernorRole? selectedRole = null)
        {
            Guard.IsTrue(groupUId.HasValue || establishmentUrn.HasValue,
                () => new InvalidParameterException(
                    $"Both parameters '{nameof(groupUId)}' and '{nameof(establishmentUrn)}' are null."));

            var domainModel = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
            var governorPermissions =
                await _governorsReadService.GetGovernorPermissions(establishmentUrn, groupUId, User);

            var viewModel = new GovernorsGridViewModel(domainModel,
                true,
                groupUId,
                establishmentUrn,
                await _cachedLookupService.NationalitiesGetAllAsync(),
                await _cachedLookupService.GovernorAppointingBodiesGetAllAsync(),
                await _cachedLookupService.TitlesGetAllAsync(),
                governorPermissions);

            var applicableRoles = domainModel.ApplicableRoles.Cast<int>();
            viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync())
                .Where(x => applicableRoles.Contains(x.Id)).Select(x => new LookupItemViewModel(x)).ToList();

            await _layoutHelper.PopulateLayoutProperties(viewModel, establishmentUrn, groupUId, User,
                x => viewModel.GovernanceMode = x.GovernanceMode, x =>
                {
                    viewModel.ShowDelegationAndCorpContactInformation = x.GroupTypeId.GetValueOrDefault() ==
                                                                        (int) eLookupGroupType.MultiacademyTrust;
                    viewModel.DelegationInformation = x.DelegationInformation;
                    viewModel.CorporateContact = x.CorporateContact;
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

            // Set the selected selectedRole (used to pre-fill the drop-down where there is an error)
            ViewData.Add("SelectedGovernorRole", selectedRole);

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

        [Route(GROUP_EDIT_GOVERNANCE, Name = "GroupDeleteOrRetireGovernor")]
        [Route(ESTAB_EDIT_GOVERNANCE, Name = "EstabDeleteOrRetireGovernor")]
        [HttpPost]
        [EdubaseAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteOrRetireGovernor(GovernorsGridViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Action == "Save") // retire selected governor with the chosen appt. end date
                {
                    if (viewModel.GovernorShared.HasValue && viewModel.GovernorShared.Value &&
                        viewModel.EstablishmentUrn.HasValue)
                    {
                        var sharedGovernor =
                            await _governorsReadService.GetGovernorAsync(viewModel.RemovalGid.Value, User);
                        var appointment = sharedGovernor.Appointments.Single(a =>
                            a.EstablishmentUrn == viewModel.EstablishmentUrn.Value);

                        var response = await _governorsWriteService.UpdateSharedGovernorAppointmentAsync(
                            viewModel.RemovalGid.Value,
                            viewModel.EstablishmentUrn.Value, appointment.AppointmentStartDate.Value,
                            viewModel.RemovalAppointmentEndDate.ToDateTime().Value, User);

                        response.ApplyToModelState(ControllerContext);
                    }
                    else
                    {
                        var response = await _governorsWriteService.UpdateDatesAsync(viewModel.RemovalGid.Value,
                            viewModel.RemovalAppointmentEndDate.ToDateTime().Value, User);

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
                    if (viewModel.GovernorShared.GetValueOrDefault() && viewModel.EstablishmentUrn.HasValue)
                    {
                        await _governorsWriteService.DeleteSharedGovernorAppointmentAsync(viewModel.RemovalGid.Value,
                            viewModel.EstablishmentUrn.Value, User);
                    }
                    else
                    {
                        await _governorsWriteService.DeleteAsync(viewModel.RemovalGid.Value, User);
                    }
                }
                else throw new InvalidParameterException($"The parameter for action is invalid: '{viewModel.Action}'");

                if (ModelState.IsValid)
                {
                    var url = viewModel.EstablishmentUrn.HasValue
                        ? $"{Url.RouteUrl(EstablishmentDetails, new { id = viewModel.EstablishmentUrn, saved = true })}#school-governance"
                        : $"{Url.RouteUrl("GroupDetails", new { id = viewModel.GroupUId, saved = true })}#governance";

                    return Redirect(url);
                }
            }

            await _layoutHelper.PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId,
                User);

            return await Edit(viewModel.GroupUId, viewModel.EstablishmentUrn, viewModel.RemovalGid, null);
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupUId"></param>
        /// <param name="establishmentUrn"></param>
        /// <param name="role"></param>
        /// <param name="gid"></param>
        /// <returns></returns>
        [Route(GROUP_ADD_GOVERNOR, Name = "GroupAddGovernor"), Route(ESTAB_ADD_GOVERNOR, Name = "EstabAddGovernor"),
         Route(GROUP_EDIT_GOVERNOR, Name = "GroupEditGovernor"), Route(ESTAB_EDIT_GOVERNOR, Name = "EstabEditGovernor"),
         Route(GROUP_REPLACE_GOVERNOR, Name = "GroupReplaceGovernor"),
         Route(ESTAB_REPLACE_GOVERNOR, Name = "EstabReplaceGovernor"),
         HttpGet, EdubaseAuthorize]
        public async Task<ActionResult> AddEditOrReplace(int? groupUId, int? establishmentUrn,
            eLookupGovernorRole? role, int? gid)
        {
            var replaceGovernorState = new
            {
                ReplacementGovernorId = Request.QueryString["gid2"].ToInteger(),
                AppointmentEndDateDay = Request.QueryString["d"].ToInteger(),
                AppointmentEndDateMonth = Request.QueryString["m"].ToInteger(),
                AppointmentEndDateYear = Request.QueryString["y"].ToInteger(),
                Reinstate = Request.QueryString["rag"] == "true"
            };

            var replaceMode =
                ((Route) ControllerContext.RouteData.Route).Url.IndexOf("/Replace/",
                    StringComparison.OrdinalIgnoreCase) > -1;
            if (role == null && gid == null)
            {
                throw new EdubaseException("Role was not supplied and no Governor ID was supplied");
            }

            if (role.HasValue)
            {
                if (!await RoleAllowed(role.Value, groupUId, establishmentUrn, User))
                {
                    return RedirectToRoute(establishmentUrn.HasValue ? "EstabEditGovernance" : "GroupEditGovernance",
                        new { establishmentUrn, groupUId, roleAlreadyExists = true, selectedRole = role });
                }

                if (establishmentUrn.HasValue && EnumSets.eSharedGovernorRoles.Contains(role.Value))
                {
                    return RedirectToRoute("SelectSharedGovernor",
                        new { establishmentUrn = establishmentUrn.Value, role = role.Value });
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
                role = (eLookupGovernorRole) model.RoleId.Value;

                if (replaceMode)
                {
                    viewModel.Mode = CreateEditGovernorViewModel.EditMode.Replace;
                    viewModel.ReplaceGovernorViewModel.AppointmentEndDate =
                        new DateTimeViewModel(model.AppointmentEndDate);
                    viewModel.ReplaceGovernorViewModel.GID = gid;
                    viewModel.ReplaceGovernorViewModel.Name = model.GetFullName();

                    if (establishmentUrn.HasValue && role.OneOfThese(eLookupGovernorRole.ChairOfTrustees,
                            eLookupGovernorRole.ChairOfGovernors))
                    {
                        var models =
                            await _governorsReadService.GetGovernorListAsync(establishmentUrn, principal: User);
                        var governorsOrTrustees = models.CurrentGovernors
                            .Where(x => x.RoleId == (int) eLookupGovernorRole.Governor ||
                                        x.RoleId == (int) eLookupGovernorRole.Trustee).OrderBy(x => x.Person_LastName)
                            .ToArray();

                        if (replaceGovernorState.ReplacementGovernorId.HasValue)
                        {
                            viewModel.SelectedGovernor =
                                governorsOrTrustees.FirstOrDefault(x =>
                                    x.Id == replaceGovernorState.ReplacementGovernorId);
                            PrepopulateFields(viewModel.SelectedGovernor, viewModel);
                        }

                        viewModel.ExistingGovernors = governorsOrTrustees.Select(x => new SelectListItem
                        {
                            Text = x.Person_FirstName + " " + x.Person_LastName,
                            Value = x.Id.ToString(),
                            Selected = replaceGovernorState.ReplacementGovernorId.HasValue &&
                                       replaceGovernorState.ReplacementGovernorId.Value == x.Id
                        });
                    }
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

                    viewModel.IsOriginalSignatoryMember = model.IsOriginalSignatoryMember;
                    viewModel.IsOriginalChairOfTrustees = model.IsOriginalChairOfTrustees;

                    viewModel.PreviousTitleId = model.PreviousPerson_TitleId;
                    viewModel.PreviousFirstName = model.PreviousPerson_FirstName;
                    viewModel.PreviousMiddleName = model.PreviousPerson_MiddleName;
                    viewModel.PreviousLastName = model.PreviousPerson_LastName;

                    viewModel.GID = model.Id;
                    viewModel.TelephoneNumber = model.TelephoneNumber;
                    viewModel.PostCode = model.PostCode;

                    viewModel.EstablishmentUrn = model.EstablishmentUrn;
                    viewModel.GroupUId = model.GroupUId;

                    viewModel.IsHistoric = model.AppointmentEndDate.HasValue &&
                                           model.AppointmentEndDate.Value < DateTime.Now.Date;
                }
            }

            await _layoutHelper.PopulateLayoutProperties(viewModel, establishmentUrn, groupUId, User);

            viewModel.GovernorRoleName = GovernorRoleNameFactory.Create(role.Value, removeGroupEstablishmentSuffix: true);
            viewModel.GovernorRoleNameMidSentence = GovernorRoleNameFactory.Create(role.Value, isMidSentence: true, removeGroupEstablishmentSuffix: true);
            viewModel.GovernorRole = role.Value;
            await PopulateSelectLists(viewModel);
            viewModel.DisplayPolicy =
                await _governorsReadService.GetEditorDisplayPolicyAsync(role.Value, groupUId.HasValue, User);

            if (viewModel.GroupTypeId != 11)
            {
                viewModel.DisplayPolicy.IsOriginalChairOfTrustees = false;
                viewModel.DisplayPolicy.IsOriginalSignatoryMember = false;
            }

            ModelState.Clear();

            if (replaceGovernorState.AppointmentEndDateDay.HasValue)
            {
                viewModel.ReinstateAsGovernor = replaceGovernorState.Reinstate;
                viewModel.ReplaceGovernorViewModel.AppointmentEndDate.Day = replaceGovernorState.AppointmentEndDateDay;
                viewModel.ReplaceGovernorViewModel.AppointmentEndDate.Month =
                    replaceGovernorState.AppointmentEndDateMonth;
                viewModel.ReplaceGovernorViewModel.AppointmentEndDate.Year =
                    replaceGovernorState.AppointmentEndDateYear;
            }

            return View(viewModel);
        }

        /// <summary>
        /// When replacing an existing chair of governor with data from an existing governor, this method prepopulates the fields from the governor record.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        private void PrepopulateFields(GovernorModel model, CreateEditGovernorViewModel viewModel)
        {
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

            viewModel.TelephoneNumber = model.TelephoneNumber;
            viewModel.PostCode = model.PostCode;

            viewModel.EstablishmentUrn = model.EstablishmentUrn;
            viewModel.GroupUId = model.GroupUId;

            viewModel.SelectedPreviousGovernorId = model.Id;
        }

        /// <summary>
        /// When replacing an existing chair of trustees/local govs with data from an existing governor, this method prepopulates the fields from the governor record.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        private void PrepopulateFields(GovernorModel model, ReplaceChairViewModel viewModel)
        {
            viewModel.NewLocalGovernor = viewModel.NewLocalGovernor ?? new GovernorViewModel();

            viewModel.NewLocalGovernor.AppointingBodyId = model.AppointingBodyId;
            viewModel.NewLocalGovernor.AppointmentEndDate = new DateTimeViewModel(model.AppointmentEndDate);
            viewModel.NewLocalGovernor.AppointmentStartDate = new DateTimeViewModel(model.AppointmentStartDate);
            viewModel.NewLocalGovernor.DOB = new DateTimeViewModel(model.DOB);
            viewModel.NewLocalGovernor.EmailAddress = model.EmailAddress;

            viewModel.NewLocalGovernor.GovernorTitleId = model.Person_TitleId;
            viewModel.NewLocalGovernor.FirstName = model.Person_FirstName;
            viewModel.NewLocalGovernor.MiddleName = model.Person_MiddleName;
            viewModel.NewLocalGovernor.LastName = model.Person_LastName;

            viewModel.NewLocalGovernor.PreviousTitleId = model.PreviousPerson_TitleId;
            viewModel.NewLocalGovernor.PreviousFirstName = model.PreviousPerson_FirstName;
            viewModel.NewLocalGovernor.PreviousMiddleName = model.PreviousPerson_MiddleName;
            viewModel.NewLocalGovernor.PreviousLastName = model.PreviousPerson_LastName;

            viewModel.NewLocalGovernor.TelephoneNumber = model.TelephoneNumber;
            viewModel.NewLocalGovernor.PostCode = model.PostCode;

            viewModel.Urn = model.EstablishmentUrn;

            viewModel.SelectedPreviousExistingNonChairId = model.Id;
        }

        public async Task<bool> RoleAllowed(eLookupGovernorRole newRole, int? groupUId, int? establishmentUrn,
            IPrincipal user)
        {
            var existingGovernors = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, user);
            var existingGovernorRoleIds = existingGovernors
                .CurrentGovernors
                .Select(g => g.RoleId)
                .OfType<int>()
                .ToHashSet();

            // Only a single chair of a local governing body may be attached (either directly, or via shared role)
            if (IsEquivalentRoleAlreadyPresent(newRole, EnumSets.eChairOfLocalGoverningBodyRoles, existingGovernorRoleIds))
            {
                return false;
            }

            // Where the new governor is a role which permits only a single appointee, forbid if an exact match is found
            var isRoleWhichPermitsOnlySingleAppointee = EnumSets.eSingularGovernorRoles.Contains(newRole);
            var exactCurrentGovernorTypeMatchFound = existingGovernorRoleIds.Contains((int) newRole);
            if (isRoleWhichPermitsOnlySingleAppointee && exactCurrentGovernorTypeMatchFound)
            {
                return false;
            }

            // As a general rule, in addition to the also subject to the "single person only" rules above,
            // only one governance professional "type" is permitted per establishment/group.
            // There are some exceptions to this, which are defined in EnumSets.PermittedGovernanceProfessionalCombinations.
            var isForbiddenGovernanceProfessionalCombination = EnumSets
                .ForbiddenCombinationsOfGovernanceProfessionalRoles
                .Any(combination =>
                {
                    var preExistingRole = combination[0];
                    var proposedNewRole = combination[1];
                    return proposedNewRole == newRole && existingGovernorRoleIds.Contains((int) preExistingRole);
                });

            if (isForbiddenGovernanceProfessionalCombination)
            {
                return false;
            }

            // Allow, if no rule met to forbid creating a new governor of this type
            return true;
        }

        /// <param name="governorRole">The role under consideration</param>
        /// <param name="rolesToConsiderEquivalent">Roles which must not co-exist for the purposes of this check - for example, we must not (simultaneously) have a chairperson and a "shared" chairperson.</param>
        /// <param name="existingRoleIds">The collection of governors to search for equivalent</param>
        /// <returns><c>true</c>, where the role ID is match exactly or with an existing role which it may not co-exist with, else <c>false</c>.</returns>
        private static bool IsEquivalentRoleAlreadyPresent(
            eLookupGovernorRole governorRole,
            IEnumerable<eLookupGovernorRole> rolesToConsiderEquivalent,
            IEnumerable<int> existingRoleIds
        )
        {
            var equivalentRoleIds = rolesToConsiderEquivalent
                .Select(role => (int) role)
                .ToHashSet();

            var isRoleUnderConsideration = equivalentRoleIds.Contains((int) governorRole);
            var existingMatchFound = existingRoleIds
                .Any(existingRoleId => equivalentRoleIds.Contains(existingRoleId));

            var isGovernanceProfessionalDuplicate = isRoleUnderConsideration && existingMatchFound;
            return isGovernanceProfessionalDuplicate;
        }

        [Route(GROUP_ADD_GOVERNOR), Route(ESTAB_ADD_GOVERNOR),
         Route(GROUP_EDIT_GOVERNOR), Route(ESTAB_EDIT_GOVERNOR),
         Route(GROUP_REPLACE_GOVERNOR), Route(ESTAB_REPLACE_GOVERNOR),
         HttpPost, EdubaseAuthorize, ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEditOrReplace(CreateEditGovernorViewModel viewModel)
        {
            await PopulateSelectLists(viewModel);
            viewModel.DisplayPolicy =
                await _governorsReadService.GetEditorDisplayPolicyAsync(viewModel.GovernorRole,
                    viewModel.GroupUId.HasValue, User);

            if (viewModel.GroupTypeId != 11)
            {
                viewModel.DisplayPolicy.IsOriginalChairOfTrustees = false;
                viewModel.DisplayPolicy.IsOriginalSignatoryMember = false;
            }

            var governorModel = new GovernorModel
            {
                AppointingBodyId = viewModel.AppointingBodyId,
                AppointmentEndDate = viewModel.AppointmentEndDate.ToDateTime(),
                AppointmentStartDate =
                    viewModel.Mode == CreateEditGovernorViewModel.EditMode.Replace
                        ? viewModel.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime()?.AddDays(1)
                        : viewModel.AppointmentStartDate.ToDateTime(),
                DOB = viewModel.DOB.ToDateTime(),
                EmailAddress = viewModel.EmailAddress,
                GroupUId = viewModel.GroupUId,
                EstablishmentUrn = viewModel.EstablishmentUrn,
                Id = viewModel.GID,
                Person_FirstName = viewModel.FirstName,
                Person_MiddleName = viewModel.MiddleName,
                Person_LastName = viewModel.LastName,
                IsOriginalSignatoryMember = viewModel.IsOriginalSignatoryMember,
                IsOriginalChairOfTrustees = viewModel.IsOriginalChairOfTrustees,
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
            validationResults = CheckEndDateNotBeforeStartDate(governorModel, validationResults);
            validationResults.ApplyToModelState(ControllerContext, true);

            if (ModelState.IsValid)
            {
                if (!viewModel.EstablishmentUrn.HasValue
                    && !viewModel.GID.HasValue
                    && EnumSets.eSharedGovernorRoles.Contains(viewModel.GovernorRole))
                {
                    var existingGovernors =
                        await _governorsReadService.GetGovernorListAsync(null, viewModel.GroupUId, User);
                    var duplicates = existingGovernors.CurrentGovernors.Where(g =>
                        g.RoleId == (int) viewModel.GovernorRole
                        && string.Equals(
                            $"{g.Person_TitleId} {g.Person_FirstName} {g.Person_MiddleName} {g.Person_LastName}",
                            $"{viewModel.GovernorTitleId} {viewModel.FirstName} {viewModel.MiddleName} {viewModel.LastName}",
                            StringComparison.OrdinalIgnoreCase));
                    if (duplicates.Any())
                    {
                        ModelState.Clear();
                        return RedirectToRoute("GroupEditGovernance",
                            new { groupUId = viewModel.GroupUId, duplicateGovernorId = duplicates.First().Id });
                    }
                }

                GovernorModel oldGovernorModel = null;
                if (viewModel.ReinstateAsGovernor &&
                    (viewModel.ReplaceGovernorViewModel?.GID.HasValue).GetValueOrDefault())
                {
                    oldGovernorModel =
                        await _governorsReadService.GetGovernorAsync(viewModel.ReplaceGovernorViewModel.GID.Value,
                            User);
                }

                var response = await _governorsWriteService.SaveAsync(governorModel, User);

                if (response.Success)
                {
                    viewModel.GID = response.Response;
                    ModelState.Clear();

                    if (viewModel.SelectedPreviousGovernorId.HasValue)
                    {
                        await RetireGovernorAsync(viewModel.SelectedPreviousGovernorId.Value,
                            viewModel.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime().GetValueOrDefault());
                        if (viewModel.ReinstateAsGovernor && viewModel.ReplaceGovernorViewModel.GID.HasValue)
                        {
                            await ReInstateChairAsNonChairAsync(viewModel.ReplaceGovernorViewModel.GID.Value,
                                governorModel.AppointmentStartDate.GetValueOrDefault(),
                                (oldGovernorModel?.AppointmentEndDate).GetValueOrDefault(),
                                viewModel.GovernorRole);
                        }
                    }

                    var url = viewModel.EstablishmentUrn.HasValue
                        ? $"{Url.RouteUrl(EstablishmentDetails, new { id = viewModel.EstablishmentUrn, saved = true })}#school-governance"
                        : $"{Url.RouteUrl("GroupDetails", new { id = viewModel.GroupUId, saved = true })}#governance";

                    return Redirect(url);
                }

                ErrorsToModelState<GovernorModel>(response.Errors);
            }

            await _layoutHelper.PopulateLayoutProperties(viewModel, viewModel.EstablishmentUrn, viewModel.GroupUId,
                User);

            return View(viewModel);
        }

        public async Task RetireGovernorAsync(int gid, DateTime endDate)
        {
            await _governorsWriteService.UpdateDatesAsync(gid, endDate, User);
        }

        /// <summary>
        /// Re-instates a chair of governors as just a simple lowly governor
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="appointmentStartDate"></param>
        /// <param name="appointmentEndDate"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task ReInstateChairAsNonChairAsync(int gid, DateTime appointmentStartDate,
            DateTime appointmentEndDate, eLookupGovernorRole currentRole)
        {
            eLookupGovernorRole targetRole;
            if (currentRole == eLookupGovernorRole.ChairOfGovernors)
            {
                targetRole = eLookupGovernorRole.Governor;
            }
            else if (currentRole == eLookupGovernorRole.ChairOfLocalGoverningBody)
            {
                targetRole = eLookupGovernorRole.LocalGovernor;
            }
            else if (currentRole == eLookupGovernorRole.ChairOfTrustees)
            {
                targetRole = eLookupGovernorRole.Trustee;
            }
            else
            {
                throw new Exception("You cannot demote from role " + currentRole);
            }

            var model = await _governorsReadService.GetGovernorAsync(gid, User);
            model.RoleId = (int) targetRole;
            model.Id = null;
            model.AppointmentStartDate = appointmentStartDate;
            model.AppointmentEndDate = appointmentEndDate;
            model.EmailAddress = null;
            model.TelephoneNumber = null;
            await _governorsWriteService.SaveAsync(model, User);
        }

        [HttpGet, Route(ESTAB_REPLACE_CHAIR, Name = "EstabReplaceChair"), EdubaseAuthorize]
        public async Task<ActionResult> ReplaceChair(int establishmentUrn, int gid)
        {
            var replaceGovernorState = new
            {
                ReplacementGovernorId = Request.QueryString["rgid"].ToInteger(),
                DateTermEndsDay = Request.QueryString["d"].ToInteger(),
                DateTermEndsMonth = Request.QueryString["m"].ToInteger(),
                DateTermEndsYear = Request.QueryString["y"].ToInteger(),
                Reinstate = Request.QueryString["ri"] == "true"
            };

            var governor = await _governorsReadService.GetGovernorAsync(gid, User);
            var roles = new List<eLookupGovernorRole> { (eLookupGovernorRole) governor.RoleId };

            if (EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value))
            {
                var localEquivalent =
                    RoleEquivalence.GetLocalEquivalentToSharedRole((eLookupGovernorRole) governor.RoleId);
                if (localEquivalent != null)
                {
                    roles.Add(localEquivalent.Value);
                }
            }
            else
            {
                roles.AddRange(RoleEquivalence.GetEquivalentToLocalRole((eLookupGovernorRole) governor.RoleId));
            }

            var governors = (await _governorsReadService.GetSharedGovernorsAsync(establishmentUrn, User))
                .Where(g => roles.Contains((eLookupGovernorRole) g.RoleId) && g.Id != gid).ToList();

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = gid,
                GovernorFullName = governor.GetFullName(),
                DateTermEnds = new DateTimeViewModel(governor.AppointmentEndDate),
                NewLocalGovernor =
                    new GovernorViewModel
                    {
                        DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync(
                            RoleEquivalence.GetLocalEquivalentToSharedRole(
                                (eLookupGovernorRole) governor.RoleId.Value) ??
                            (eLookupGovernorRole) governor.RoleId.Value, false, User)
                    },
                SharedGovernors =
                    (await Task.WhenAll(governors.Select(async g =>
                        await SharedGovernorViewModel.MapFromGovernor(g, establishmentUrn, _cachedLookupService))))
                    .ToList(),
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                Role = (eLookupGovernorRole) governor.RoleId,
                RoleName = GovernorRoleNameFactory.Create((eLookupGovernorRole) governor.RoleId, isMidSentence: true)
            };

            var models = await _governorsReadService.GetGovernorListAsync(establishmentUrn, principal: User);
            var localGovernors = models.CurrentGovernors.Where(x => x.RoleId == (int) eLookupGovernorRole.LocalGovernor)
                .OrderBy(x => x.Person_LastName).ToArray();

            if (replaceGovernorState.ReplacementGovernorId.HasValue)
            {
                model.SelectedNonChair =
                    localGovernors.FirstOrDefault(x => x.Id == replaceGovernorState.ReplacementGovernorId);
                PrepopulateFields(model.SelectedNonChair, model);
            }

            model.ExistingNonChairs = localGovernors.Select(x => new SelectListItem
            {
                Text = x.Person_FirstName + " " + x.Person_LastName,
                Value = x.Id.ToString(),
                Selected = replaceGovernorState.ReplacementGovernorId.HasValue &&
                           replaceGovernorState.ReplacementGovernorId.Value == x.Id
            });

            await PopulateSelectLists(model.NewLocalGovernor);
            await _layoutHelper.PopulateLayoutProperties(model, establishmentUrn, null, User);

            if (replaceGovernorState.DateTermEndsDay.HasValue)
            {
                model.Reinstate = replaceGovernorState.Reinstate;
                model.DateTermEnds.Day = replaceGovernorState.DateTermEndsDay;
                model.DateTermEnds.Month = replaceGovernorState.DateTermEndsMonth;
                model.DateTermEnds.Year = replaceGovernorState.DateTermEndsYear;
            }

            return View(model);
        }

        [HttpPost, Route(ESTAB_REPLACE_CHAIR), EdubaseAuthorize, ValidateAntiForgeryToken]
        public async Task<ActionResult> ReplaceChair(ReplaceChairViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewChairType == ReplaceChairViewModel.ChairType.SharedChair)
                {
                    var newGovernor = model.SharedGovernors.SingleOrDefault(s => s.Id == model.SelectedGovernorId);

                    var validation = await _governorsWriteService.AddSharedGovernorAppointmentAsync(
                        model.SelectedGovernorId,
                        model.Urn.Value,
                        model.DateTermEnds.ToDateTime().Value.AddDays(1),
                        newGovernor.AppointmentEndDate.ToDateTime(), User);

                    if (!validation.HasErrors)
                    {
                        var url =
                            $"{Url.RouteUrl(EstablishmentDetails, new { id = model.Urn, saved = true })}#school-governance";
                        return Redirect(url);
                    }

                    validation.ApplyToModelState(ControllerContext);
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
                        Person_FirstName = model.NewLocalGovernor.FirstName,
                        Person_MiddleName = model.NewLocalGovernor.MiddleName,
                        Person_LastName = model.NewLocalGovernor.LastName,
                        Person_TitleId = model.NewLocalGovernor.GovernorTitleId,
                        PreviousPerson_FirstName = model.NewLocalGovernor.PreviousFirstName,
                        PreviousPerson_MiddleName = model.NewLocalGovernor.PreviousMiddleName,
                        PreviousPerson_LastName = model.NewLocalGovernor.PreviousLastName,
                        PreviousPerson_TitleId = model.NewLocalGovernor.PreviousTitleId,
                        PostCode = model.NewLocalGovernor.PostCode,
                        RoleId = (int) (RoleEquivalence.GetLocalEquivalentToSharedRole(model.Role) ?? model.Role),
                        TelephoneNumber = model.NewLocalGovernor.TelephoneNumber
                    };

                    var validation = await _governorsWriteService.ValidateAsync(newGovernor, User);

                    if (!validation.HasErrors)
                    {
                        GovernorModel oldGovernorModel = null;
                        if (model.Reinstate)
                        {
                            oldGovernorModel =
                                await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User);
                        }

                        await _governorsWriteService.SaveAsync(newGovernor, User);

                        if (model.SelectedPreviousExistingNonChairId.HasValue)
                        {
                            await RetireGovernorAsync(model.SelectedPreviousExistingNonChairId.Value,
                                model.DateTermEnds.ToDateTime().GetValueOrDefault());
                        }

                        if (model.Reinstate) // re-instate the old chair to be the non-chair equivalent role.
                        {
                            await ReInstateChairAsNonChairAsync(model.ExistingGovernorId,
                                newGovernor.AppointmentStartDate.GetValueOrDefault(),
                                (oldGovernorModel?.AppointmentEndDate).GetValueOrDefault(),
                                eLookupGovernorRole.ChairOfLocalGoverningBody);
                        }

                        var url =
                            $"{Url.RouteUrl(EstablishmentDetails, new { id = model.Urn, saved = true })}#school-governance";
                        return Redirect(url);
                    }

                    validation.ApplyToModelState(ControllerContext, nameof(model.NewLocalGovernor), true);
                }
            }

            var governor = await _governorsReadService.GetGovernorAsync(model.ExistingGovernorId, User);
            var roles = new List<eLookupGovernorRole> { (eLookupGovernorRole) governor.RoleId };

            if (EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value))
            {
                var localEquivalent =
                    RoleEquivalence.GetLocalEquivalentToSharedRole((eLookupGovernorRole) governor.RoleId);
                if (localEquivalent != null)
                    roles.Add(localEquivalent.Value);
            }
            else
            {
                roles.AddRange(RoleEquivalence.GetEquivalentToLocalRole((eLookupGovernorRole) governor.RoleId));
            }

            var governors = (await _governorsReadService.GetSharedGovernorsAsync(model.Urn.Value, User)).Where(g =>
                roles.Contains((eLookupGovernorRole) g.RoleId) && g.Id != model.ExistingGovernorId).ToList();

            model.NewLocalGovernor.DisplayPolicy = await _governorsReadService.GetEditorDisplayPolicyAsync(
                (RoleEquivalence.GetLocalEquivalentToSharedRole((eLookupGovernorRole) governor.RoleId.Value) ??
                 (eLookupGovernorRole) governor.RoleId.Value), false, User);

            var sourceGovernors = (await Task.WhenAll(governors.Select(async g =>
                await SharedGovernorViewModel.MapFromGovernor(g, model.Urn.Value, _cachedLookupService)))).ToList();
            if (model.SharedGovernors == null)
            {
                model.SharedGovernors = sourceGovernors;
            }
            else
            {
                for (var i = 0; i < model.SharedGovernors?.Count; i++)
                {
                    // if this is the one the user selected, we dont want to change any of the values they entered
                    if (model.SharedGovernors[i].Selected)
                    {
                        model.SharedGovernors[i].SharedWith =
                            sourceGovernors.First(x => x.Id == model.SharedGovernors[i].Id).SharedWith;
                    }
                    else
                    {
                        model.SharedGovernors[i] = sourceGovernors.First(x => x.Id == model.SharedGovernors[i].Id);
                        model.SharedGovernors[i].Selected = false;
                    }
                }
            }

            await PopulateSelectLists(model.NewLocalGovernor);
            await _layoutHelper.PopulateLayoutProperties(model, model.Urn, null, User);


            var models = await _governorsReadService.GetGovernorListAsync(model.Urn, principal: User);
            var localGovernors = models.CurrentGovernors.Where(x => x.RoleId == (int) eLookupGovernorRole.LocalGovernor)
                .OrderBy(x => x.Person_LastName).ToArray();

            if (model.SelectedPreviousExistingNonChairId.HasValue)
            {
                model.SelectedNonChair =
                    localGovernors.FirstOrDefault(x => x.Id == model.SelectedPreviousExistingNonChairId);
            }

            model.ExistingNonChairs = localGovernors.Select(x => new SelectListItem
            {
                Text = x.Person_FirstName + " " + x.Person_LastName,
                Value = x.Id.ToString(),
                Selected = model.SelectedPreviousExistingNonChairId.HasValue &&
                           model.SelectedPreviousExistingNonChairId.Value == x.Id
            });


            return View(model);
        }

        internal async Task<GovernorsGridViewModel> CreateGovernorsViewModel(int? groupUId = null,
            int? establishmentUrn = null, EstablishmentModel establishmentModel = null, IPrincipal user = null)
        {
            user = user ?? User;
            establishmentUrn = establishmentUrn ?? establishmentModel?.Urn;
            GovernorsGridViewModel viewModel;

            var domainModelTask = _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, user);
            var governorPermissionsTask =
                _governorsReadService.GetGovernorPermissions(establishmentUrn, groupUId, user);

            await Task.WhenAll(domainModelTask, governorPermissionsTask);

            var domainModel = domainModelTask.Result;
            var governorPermissions = governorPermissionsTask.Result;

            viewModel = new GovernorsGridViewModel(domainModel,
                false,
                groupUId,
                establishmentUrn,
                await _cachedLookupService.NationalitiesGetAllAsync(),
                await _cachedLookupService.GovernorAppointingBodiesGetAllAsync(),
                await _cachedLookupService.TitlesGetAllAsync(),
                    governorPermissions);

            if (establishmentUrn.HasValue || establishmentModel != null) // governance view for an establishment
            {
                var estabDomainModel = establishmentModel ??
                                       (await _establishmentReadService.GetAsync(establishmentUrn.Value, user))
                                       .GetResult();
                var items = await _establishmentReadService.GetPermissibleLocalGovernorsAsync(establishmentUrn.Value,
                    user); // The API uses 1 as a default value, hence we have to call another API to deduce whether to show the Governance mode UI section
                viewModel.GovernanceMode = items.Any() ? estabDomainModel.GovernanceMode : null;
            }

            if (groupUId.HasValue) // governance view for a group
            {
                var groupModel = (await _groupReadService.GetAsync(groupUId.Value, user)).GetResult();
                viewModel.ShowDelegationAndCorpContactInformation =
                    groupModel.GroupTypeId == (int) eLookupGroupType.MultiacademyTrust;
                viewModel.DelegationInformation = groupModel.DelegationInformation;
                viewModel.CorporateContact = groupModel.CorporateContact;
                viewModel.GroupTypeId = groupModel.GroupTypeId;
            }

            return viewModel;
        }

        private async Task PopulateSelectLists(GovernorViewModel viewModel)
        {
            viewModel.AppointingBodies =
                (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).ToSelectList(viewModel
                    .AppointingBodyId);
            viewModel.Titles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.GovernorTitleId);
            viewModel.PreviousTitles =
                (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.PreviousTitleId);
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
                    attribute = property.GetAttribute<JsonPropertyAttribute>();

                    if (string.Equals(error.Fields, property.Name, StringComparison.OrdinalIgnoreCase) ||
                        (attribute != null && string.Equals(error.Fields, attribute.PropertyName,
                            StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError(property.Name, error.Message);
                    }
                }
            }
        }

        private ValidationEnvelopeDto CheckEndDateNotBeforeStartDate(GovernorModel governorModel, ValidationEnvelopeDto validationResults)
        {

            if (validationResults.HasErrors)
            {
                return validationResults;
            }

            if (governorModel.AppointmentStartDate != null && governorModel.AppointmentEndDate != null &&
                governorModel.AppointmentStartDate >= governorModel.AppointmentEndDate)
            {
                validationResults = new ValidationEnvelopeDto
                {
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Code = "error.wrongValue.stepdownDate.validate.field.edit.governor_appointmentEndDate",
                            Fields = "appointmentEndDate",
                            Message = "Date appointment ended must not be before the date of appointment"
                        }
                    }
                };
            }

            return validationResults;
        }
    }
}
