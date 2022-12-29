using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Common.Text;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Services.Nomenclature;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Grid;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    using GR = eLookupGovernorRole;

    public class GovernorsGridViewModel : IGroupPageViewModel, IEstablishmentPageViewModel
    {
        private readonly NomenclatureService _nomenclatureService;

        public GovernorsGridViewModel(GovernorsDetailsDto dto, bool editMode, int? groupUId, int? establishmentUrn,
            NomenclatureService nomenclatureService, IEnumerable<LookupDto> nationalities,
            IEnumerable<LookupDto> appointingBodies, GovernorPermissions governorPermissions)
        {
            _nomenclatureService = nomenclatureService;
            DomainModel = dto;
            EditMode = editMode;
            GroupUId = groupUId;
            Nationalities = nationalities;
            AppointingBodies = appointingBodies;
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
        public IEnumerable<LookupDto> AppointingBodies { get; }

        string IEstablishmentPageViewModel.SelectedTab { get; set; }

        int? IEstablishmentPageViewModel.Urn { get; set; }
        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }

        string IEstablishmentPageViewModel.Name { get; set; }
        public string TypeName { get; set; }

        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }

        public string Layout { get; set; }

        public int? GroupUId { get; set; }

        public string ListOfEstablishmentsPluralName { get; set; }

        public string GroupName { get; set; }

        public int? GroupTypeId { get; set; }

        public string SelectedTabName { get; set; }
        public string GroupTypeName { get; set; }

        private void CreateGrids(GovernorsDetailsDto dto, IEnumerable<GovernorModel> governors, bool isHistoric,
            int? groupUid, int? establishmentUrn)
        {
            var roles = dto.ApplicableRoles.Where(role => !EnumSets.eSharedGovernorRoles.Contains(role)
                                                          ||
                                                          (RoleEquivalence.GetLocalEquivalentToSharedRole(role) != null
                                                           && !dto.ApplicableRoles.Contains(RoleEquivalence
                                                               .GetLocalEquivalentToSharedRole(role).Value)));
            foreach (var role in roles)
            {
                var equivalantRoles = RoleEquivalence.GetEquivalentToLocalRole(role).Cast<int>().ToList();
                var pluralise = !EnumSets.eSingularGovernorRoles.Contains(role);


                var grid =
                    new GovernorGridViewModel(
                        $"{_nomenclatureService.GetGovernorRoleName(role, eTextCase.SentenceCase, pluralise)}{(isHistoric ? " (in past 12 months)" : string.Empty)}")
                    {
                        Tag = isHistoric ? "historic" : "current",
                        Role = role,
                        IsSharedRole = EnumSets.eSharedGovernorRoles.Contains(role),
                        GroupUid = groupUid,
                        EstablishmentUrn = establishmentUrn,
                        IsHistoricRole = isHistoric,
                        RoleName = _nomenclatureService.GetGovernorRoleName(role)
                    };

                var displayPolicy = dto.RoleDisplayPolicies.Get(role);
                Guard.IsNotNull(displayPolicy,
                    () => new Exception($"The display policy should not be null for the role '{role}'"));
                var includeEndDate = (((isHistoric && role == GR.Member) || role != GR.Member) &&
                                      displayPolicy.AppointmentEndDate) ||
                                     (role.OneOfThese(GR.ChiefFinancialOfficer, GR.AccountingOfficer) && isHistoric);

                SetupHeader(role, grid, displayPolicy, includeEndDate);

                var list = governors.Where(x => x.RoleId.HasValue && equivalantRoles.Contains(x.RoleId.Value));
                foreach (var governor in list)
                {
                    var isShared = governor.RoleId.HasValue &&
                                   EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value);
                    var establishments = string.Join(", ",
                        governor.Appointments?.Select(a => $"{a.EstablishmentName}, URN: {a.EstablishmentUrn}") ??
                        new string[] { });
                    var appointment =
                        governor.Appointments?.SingleOrDefault(a => a.EstablishmentUrn == EstablishmentUrn);
                    var startDate = isShared && appointment != null
                        ? appointment.AppointmentStartDate
                        : governor.AppointmentStartDate;
                    var endDate = isShared && appointment != null
                        ? appointment.AppointmentEndDate
                        : governor.AppointmentEndDate;

                    // if (EnumSets.eGovernanceProfessionalRoles.Contains(role))
                    // {
                    grid.AddRow(governor, endDate)
                        .AddCell(governor.GetFullName(), displayPolicy.FullName)
                        .AddCell(governor.Id, displayPolicy.Id)
                        .AddCell(governor.DOB?.ToString("d MMMM yyyy"), displayPolicy.DOB)
                        .AddCell(governor.PostCode, displayPolicy.PostCode)
                        .AddCell(governor.TelephoneNumber, displayPolicy.TelephoneNumber)
                        .AddCell(governor.EmailAddress, displayPolicy.EmailAddress)
                        .AddCell(startDate?.ToString("d MMMM yyyy"), displayPolicy.AppointmentStartDate)
                        .AddCell(endDate?.ToString("d MMMM yyyy"), includeEndDate);
                    // }
                    // else
                    // {
                    // grid.AddRow(governor, endDate)
                    //     .AddCell(governor.GetFullName(), displayPolicy.FullName)
                    //     .AddCell(string.IsNullOrWhiteSpace(establishments) ? null : establishments,
                    //         role.OneOfThese(GR.LocalGovernor, GR.ChairOfLocalGoverningBody))
                    //     .AddCell(governor.Id, displayPolicy.Id)
                    //     .AddCell(AppointingBodies.FirstOrDefault(x => x.Id == governor.AppointingBodyId)?.Name,
                    //         displayPolicy.AppointingBodyId)
                    //     .AddCell(startDate?.ToString("d MMMM yyyy"), displayPolicy.AppointmentStartDate)
                    //     .AddCell(endDate?.ToString("d MMMM yyyy"), includeEndDate)
                    //     .AddCell(governor.PostCode, displayPolicy.PostCode)
                    //     .AddCell(governor.DOB?.ToString("d MMMM yyyy"), displayPolicy.DOB)
                    //     .AddCell(governor.GetPreviousFullName(), displayPolicy.PreviousFullName)
                    //     .AddCell(governor.EmailAddress, displayPolicy.EmailAddress)
                    //     .AddCell(governor.TelephoneNumber, displayPolicy.TelephoneNumber);
                    // }

                    if (isHistoric)
                    {
                        var gov = new HistoricGovernorViewModel
                        {
                            AppointingBodyId = governor.AppointingBodyId,
                            AppointingBody =
                                AppointingBodies.FirstOrDefault(x => x.Id == governor.AppointingBodyId)?.Name,
                            AppointmentEndDate = new DateTimeViewModel(governor.AppointmentEndDate),
                            AppointmentStartDate = new DateTimeViewModel(governor.AppointmentStartDate),
                            FullName = governor.GetFullName(),
                            RoleName = _nomenclatureService.GetGovernorRoleName(role),
                            GID = governor.Id
                        };

                        HistoricGovernors.Add(gov);
                    }
                }

                grid.Rows = grid.Rows.OrderByDescending(x => x.SortValue).ThenBy(x => x.Model.GetFullName()).ToList();
                HistoricGovernors = HistoricGovernors.OrderByDescending(x => x.AppointmentEndDate.ToDateTime())
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
            // if (EnumSets.eGovernanceProfessionalRoles.Contains(role))
            // {
            grid.AddHeaderCell("Name", displayPolicy.FullName, "name", "sortText")
                .AddHeaderCell("Governance role identifier (GID)", displayPolicy.Id, "gid")
                .AddHeaderCell("Date of birth", displayPolicy.DOB)
                .AddHeaderCell("Home postcode", displayPolicy.PostCode)
                .AddHeaderCell("Telephone", displayPolicy.TelephoneNumber)
                .AddHeaderCell("Email address", displayPolicy.EmailAddress)
                .AddHeaderCell("Date of appointment", displayPolicy.AppointmentStartDate, "fromDate", "sortDate")
                .AddHeaderCell("Date appointment ended", includeEndDate, "toDate", "sortDate");
            // }
            // else
            // {
            //     grid.AddHeaderCell("Name", displayPolicy.FullName, "name", "sortText")
            //         .AddHeaderCell("Shared with", role.OneOfThese(GR.LocalGovernor, GR.ChairOfLocalGoverningBody),
            //             "shared", "sortText")
            //         .AddHeaderCell("GID", displayPolicy.Id, "gid")
            //         .AddHeaderCell("Appointed by", displayPolicy.AppointingBodyId, "appointed", "sortText")
            //         .AddHeaderCell("From", displayPolicy.AppointmentStartDate, "fromDate", "sortDate")
            //         .AddHeaderCell(role == GR.Member ? "Date stepped down" : "To", includeEndDate, "toDate", "sortDate")
            //         .AddHeaderCell("Postcode", displayPolicy.PostCode)
            //         .AddHeaderCell("Date of birth", displayPolicy.DOB)
            //         .AddHeaderCell("Previous name", displayPolicy.PreviousFullName)
            //         .AddHeaderCell("Email address", displayPolicy.EmailAddress)
            //         .AddHeaderCell("Telephone", displayPolicy.TelephoneNumber);
            // }
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
                            new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = "sortText" });
                        foreach (var row in grid.Rows)
                        {
                            row.Cells.Add(
                                new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = "sortText" });
                        }
                    }
                    else
                    {
                        grid.HeaderCells.Insert(index.Value,
                            new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = "sortText" });
                        foreach (var row in grid.Rows)
                        {
                            row.Cells.Insert(index.Value,
                                new GridCellViewModel(string.Empty) { SortKey = sortKey, SortType = "sortText" });
                        }
                    }

                    addedCount++;
                }
            }
        }
    }
}
