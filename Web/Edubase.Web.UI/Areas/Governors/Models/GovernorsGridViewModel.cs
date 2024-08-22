using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Factories;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Grid;
using GR = Edubase.Services.Enums.eLookupGovernorRole;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorsGridViewModel : IGroupPageViewModel, IEstablishmentPageViewModel
    {
        public GovernorsGridViewModel(GovernorsDetailsDto dto, bool editMode, int? groupUId, int? establishmentUrn,
            IEnumerable<LookupDto> nationalities, IEnumerable<LookupDto> appointingBodies,
            IEnumerable<LookupDto> titles, GovernorPermissions governorPermissions)
        {
            DomainModel = dto;
            EditMode = editMode;
            GroupUId = groupUId;
            Nationalities = nationalities;
            AppointingBodies = appointingBodies;
            Titles = titles;
            EstablishmentUrn = establishmentUrn;
            GovernorPermissions = governorPermissions;
            CreateGrids(dto, dto.CurrentGovernors, false, groupUId, establishmentUrn);
            CreateGrids(dto, dto.HistoricalGovernors, true, groupUId, establishmentUrn);
        }

        public GovernorsGridViewModel()
        {
        }

        public List<GovernorGridViewModel> Grids { get; set; } = new List<GovernorGridViewModel>();
        public List<GovernorGridViewModel> HistoricGrids { get; set; } = new List<GovernorGridViewModel>();
        public List<HistoricGovernorViewModel> HistoricGovernors { get; set; } = new List<HistoricGovernorViewModel>();
        public List<LookupItemViewModel> GovernorRoles { get; internal set; }
        public GovernorsDetailsDto DomainModel { get; set; }

        public bool EditMode { get; }

        public GovernorPermissions GovernorPermissions { get; set; }

        public string Action { get; set; }

        public int? Id => EstablishmentUrn ?? GroupUId;

        public int? EstablishmentUrn { get; set; }

        public string ParentEntityName => GroupUId.HasValue ? "group" : "establishment";

        public string DelegationInformation { get; set; }
        public string CorporateContact { get; set; }

        public bool ShowDelegationAndCorpContactInformation { get; set; }

        /// <summary>
        ///     GID of the governor that's being removed.
        /// </summary>
        public int? RemovalGid { get; set; }

        public bool? GovernorShared { get; set; }

        public DateTimeViewModel RemovalAppointmentEndDate { get; set; } = new DateTimeViewModel();

        public eGovernanceMode? GovernanceMode { get; set; }

        public IEnumerable<LookupDto> Nationalities { get; }

        public IEnumerable<LookupDto> Titles { get; }

        string IEstablishmentPageViewModel.SelectedTab { get; set; }

        int? IEstablishmentPageViewModel.Urn { get; set; }
        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }

        string IEstablishmentPageViewModel.Name { get; set; }
        public string TypeName { get; set; }

        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }

        public string Layout { get; set; }

        public int? GroupUId { get; set; }
        public IEnumerable<LookupDto> AppointingBodies { get; private set; }

        public string ListOfEstablishmentsPluralName { get; set; }

        public string GroupName { get; set; }

        public int? GroupTypeId { get; set; }

        public string SelectedTabName { get; set; }
        public string GroupTypeName { get; set; }

        private const string SortText = "sortText";
        private const string SortDate = "sortDate";
        private const string DateFormat = "d MMMM yyyy";

        private void CreateGrids(GovernorsDetailsDto dto, IEnumerable<GovernorModel> governors, bool isHistoric,
            int? groupUid, int? establishmentUrn)
        {
            //add the roles from the api to the applicable roles
            //this allows roles that are not valid but exist to be displayed
            var allRoles = dto.ApplicableRoles.Union(governors.Select(g => (GR) g.RoleId).Distinct());

            var roles = allRoles.Where(role =>
            {
                if (!EnumSets.eSharedGovernorRoles.Contains(role))
                {
                    return true;
                }
                var localEquivalent = RoleEquivalence.GetLocalEquivalentToSharedRole(role);
                if (localEquivalent != null)
                {
                    if (!dto.ApplicableRoles.Contains(localEquivalent.Value) || role == GR.Establishment_SharedGovernanceProfessional)
                    {
                        return true;
                    }
                }
                return false;
            }).ToList();

            foreach (var role in roles)
            {
                var equivalentRoles = RoleEquivalence.GetEquivalentToLocalRole(role).Cast<int>().ToList();
                var shouldPluralise = !EnumSets.eSingularGovernorRoles.Contains(role);

                var governorRoleNameTitle = GovernorRoleNameFactory.Create(
                    role,
                    pluraliseLabelIfApplicable: shouldPluralise,
                    removeMemberPrefix: true,
                    removeGroupEstablishmentSuffix: true
                );
                var governorRoleNameSingular = GovernorRoleNameFactory.Create(
                    role,
                    pluraliseLabelIfApplicable: false,
                    removeMemberPrefix: true,
                    removeGroupEstablishmentSuffix: true
                );

                var grid = new GovernorGridViewModel($"{governorRoleNameTitle}{(isHistoric ? " (in past 12 months)" : string.Empty)}")
                {
                    Tag = isHistoric ? "historic" : "current",
                    Role = role,
                    IsSharedRole = EnumSets.eSharedGovernorRoles.Contains(role),
                    GroupUid = groupUid,
                    EstablishmentUrn = establishmentUrn,
                    IsHistoricRole = isHistoric,
                    RoleName = governorRoleNameSingular
                };

                var displayPolicy = dto.RoleDisplayPolicies.Get(role);
                Guard.IsNotNull(displayPolicy, () => new Exception($"The display policy should not be null for the role '{role}'"));

                var includeEndDate = (((isHistoric && role == GR.Member) || role != GR.Member) && displayPolicy.AppointmentEndDate)
                                     || (role.OneOfThese(GR.ChiefFinancialOfficer, GR.AccountingOfficer) && isHistoric);

                SetupHeader(role, grid, displayPolicy, includeEndDate);


                var list = governors
                    .Where(x => x.RoleId.HasValue && equivalentRoles.Contains(x.RoleId.Value));
                foreach (var governor in list)
                {
                    var isShared = governor.RoleId.HasValue && EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value);
                    var establishments = string.Join(
                        ", ",
                        (governor.Appointments?.Select(a => $"{a.EstablishmentName}, URN: {a.EstablishmentUrn}") ?? new string[] { })
                    );
                    var appointment = governor.Appointments?
                        .SingleOrDefault(a => a.EstablishmentUrn == EstablishmentUrn);
                    var startDate = isShared && appointment != null
                        ? appointment.AppointmentStartDate
                        : governor.AppointmentStartDate;
                    var endDate = isShared && appointment != null
                        ? appointment.AppointmentEndDate
                        : governor.AppointmentEndDate;

                    var getFullnameWithTitle = FullNameWithTitle(governor);

                    if (EnumSets.eGovernanceProfessionalRoles.Contains(role))
                    {
                        if (role == (GR) governor.RoleId)
                        {
                            grid.AddRow(governor, endDate)
                                .AddCell(getFullnameWithTitle, displayPolicy.FullName)
                                .AddCell(string.IsNullOrWhiteSpace(establishments) ? null : establishments, role.OneOfThese(GR.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool))
                                .AddCell(governor.Id, displayPolicy.Id)
                                .AddCell(governor.DOB?.ToString(DateFormat), displayPolicy.DOB)
                                .AddCell(governor.PostCode, displayPolicy.PostCode)
                                .AddCell(governor.TelephoneNumber, displayPolicy.TelephoneNumber)
                                .AddCell(governor.EmailAddress, displayPolicy.EmailAddress)
                                .AddCell(startDate?.ToString(DateFormat), displayPolicy.AppointmentStartDate)
                                .AddCell(endDate?.ToString(DateFormat), includeEndDate);
                        }
                    }
                    else
                    {
                        grid.AddRow(governor, endDate)
                            .AddCell(governor.GetFullName(), displayPolicy.FullName)
                            .AddCell(string.IsNullOrWhiteSpace(establishments) ? null : establishments,
                                role.OneOfThese(GR.LocalGovernor, GR.ChairOfLocalGoverningBody))
                            .AddCell(governor.Id, displayPolicy.Id)
                            .AddCell(AppointingBodies.FirstOrDefault(x => x.Id == governor.AppointingBodyId)?.Name,
                                displayPolicy.AppointingBodyId)
                            .AddCell(startDate?.ToString(DateFormat), displayPolicy.AppointmentStartDate)
                            .AddCell(endDate?.ToString(DateFormat), includeEndDate)
                            .AddCell(governor.PostCode, displayPolicy.PostCode)
                            .AddCell(governor.DOB?.ToString(DateFormat), displayPolicy.DOB)
                            .AddCell(governor.GetPreviousFullName(), displayPolicy.PreviousFullName)
                            .AddCell(governor.EmailAddress, displayPolicy.EmailAddress)
                            .AddCell(governor.TelephoneNumber, displayPolicy.TelephoneNumber);
                    }

                    if (isHistoric)
                    {
                        var governorRoleNameSingularActual = GovernorRoleNameFactory.Create(
                            (GR) governor.RoleId,
                            pluraliseLabelIfApplicable: false,
                            removeMemberPrefix: true,
                            removeGroupEstablishmentSuffix: false
                        );

                        var gov = new HistoricGovernorViewModel
                        {
                            AppointingBodyId = governor.AppointingBodyId,
                            AppointingBody = AppointingBodies.FirstOrDefault(x => x.Id == governor.AppointingBodyId)?.Name,
                            AppointmentEndDate = new DateTimeViewModel(governor.AppointmentEndDate),
                            AppointmentStartDate = new DateTimeViewModel(governor.AppointmentStartDate),
                            FullName = governor.GetFullName(),
                            RoleName = governorRoleNameSingularActual,
                            GID = governor.Id
                        };

                        HistoricGovernors.Add(gov);
                    }
                }

                grid.Rows = grid.Rows
                    .OrderByDescending(x => x.SortValue)
                    .ThenBy(x => x.Model.GetFullName())
                    .ToList();

                HistoricGovernors = HistoricGovernors
                    .OrderByDescending(x => x.AppointmentEndDate.ToDateTime())
                    .ThenBy(x => x.FullName).ToList();

                if (isHistoric)
                {
                    HistoricGrids.Add(grid);
                }
                else
                {
                    Grids.Add(grid);
                }
            }
        }

        private void SetupHeader(GR role, GridViewModel<GovernorModel> grid, GovernorDisplayPolicy displayPolicy,
            bool includeEndDate)
        {
            if (EnumSets.eGovernanceProfessionalRoles.Contains(role))
            {
                grid.AddHeaderCell("Name", displayPolicy.FullName, "name", SortText)
                    .AddHeaderCell("Shared with", role.OneOfThese(GR.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool), "shared", SortText)
                    .AddHeaderCell("Governance role identifier (GID)", displayPolicy.Id, "gid")
                    .AddHeaderCell("Date of birth", displayPolicy.DOB)
                    .AddHeaderCell("Home postcode", displayPolicy.PostCode)
                    .AddHeaderCell("Telephone number", displayPolicy.TelephoneNumber)
                    .AddHeaderCell("Email address", displayPolicy.EmailAddress)
                    .AddHeaderCell("Date of appointment", displayPolicy.AppointmentStartDate, "fromDate", SortDate)
                    .AddHeaderCell("Date appointment ended", includeEndDate, "toDate", SortDate);
            }
            else
            {
                grid.AddHeaderCell("Name", displayPolicy.FullName, "name", SortText)
                    .AddHeaderCell("Shared with", role.OneOfThese(GR.LocalGovernor, GR.ChairOfLocalGoverningBody),
                        "shared", SortText)
                    .AddHeaderCell("Governance role identifier (GID)", displayPolicy.Id, "gid")
                    .AddHeaderCell("Appointed by", displayPolicy.AppointingBodyId, "appointed", SortText)
                    .AddHeaderCell("From", displayPolicy.AppointmentStartDate, "fromDate", SortDate)
                    .AddHeaderCell(role == GR.Member ? "Date stepped down" : "To", includeEndDate, "toDate", SortDate)
                    .AddHeaderCell("Postcode", displayPolicy.PostCode)
                    .AddHeaderCell("Date of birth", displayPolicy.DOB)
                    .AddHeaderCell("Previous name", displayPolicy.PreviousFullName)
                    .AddHeaderCell("Email address", displayPolicy.EmailAddress)
                    .AddHeaderCell("Telephone", displayPolicy.TelephoneNumber);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="viewModel">Target grid</param>
        /// <param name="maxAmount">Hard max of how many to add</param>
        /// <param name="targetCount"></param>
        /// <param name="index"></param>
        public void AddEmptyCells(GovernorGridViewModel grid, int targetCount, int? maxAmount = null, int? index = null)
        {
            var addedCount = 0;
            var lastIndex = grid.HeaderCells.Count - 1;

            if (index.HasValue) // validate the index
            {
                if (index > lastIndex)
                {
                    index = null; // change to an 'Add' operation
                }
                else if (index < 0)
                {
                    index = 0;
                }
            }

            while (grid.HeaderCells.Count < targetCount && (!maxAmount.HasValue || addedCount < maxAmount.Value))
            {
                if (grid.HeaderCells.Count < targetCount)
                {
                    var sortKey = "col_" + (grid.HeaderCells.Count + 1);
                    if (index == null)
                    {
                        grid.HeaderCells.Add(
                            new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = SortText });
                        foreach (var row in grid.Rows)
                        {
                            row.Cells.Add(
                                new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = SortText });
                        }
                    }
                    else
                    {
                        grid.HeaderCells.Insert(index.Value,
                            new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = SortText });
                        foreach (var row in grid.Rows)
                        {
                            row.Cells.Insert(index.Value,
                                new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = SortText });
                        }
                    }

                    addedCount++;
                }
            }
        }

        private string FullNameWithTitle(GovernorModel governor)
        {
            var title = Titles
                .Where(x => !x.Name.ToLowerInvariant().Equals("n/a"))
                .Where(x => !x.Name.ToLowerInvariant().Equals("not applicable"))
                .Where(x => !x.Name.ToLowerInvariant().Equals("not recorded"))
                .FirstOrDefault(x => x.Id == governor.Person_TitleId);

            var titleName = title?.Name;
            var fullNameWithTitle = StringUtil.ConcatNonEmpties(" ", titleName, governor.GetFullName());

            return fullNameWithTitle;
        }
    }
}
