using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Models.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Common.Text;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    using UI.Models.Establishments;
    using Services.Governors.DisplayPolicies;
    using Services.Nomenclature;
    using UI.Models;
    using GR = Services.Enums.eLookupGovernorRole;
    using Services.Enums;

    public class GovernorsGridViewModel : Groups.Models.CreateEdit.IGroupPageViewModel, IEstablishmentPageViewModel
    {
        private readonly NomenclatureService _nomenclatureService;
        private readonly Dictionary<int, string> _appointingBodies;

        public List<GovernorGridViewModel> Grids { get; set; } = new List<GovernorGridViewModel>();
        public List<GovernorGridViewModel> HistoricGrids { get; set; } = new List<GovernorGridViewModel>();
        public List<LookupItemViewModel> GovernorRoles { get; internal set; }
        public GovernorsDetailsDto DomainModel { get; set; }

        public bool EditMode { get; private set; }

        public string Action { get; set; }

        public string Layout { get; set; }

        public int? Id => EstablishmentUrn ?? GroupUId;

        public int? EstablishmentUrn { get; set; }

        public int? GroupUId { get; set; }

        public string ParentEntityName => GroupUId.HasValue ? "group" : "establishment";

        public string DelegationInformation { get; set; }

        public bool ShowDelegationInformation { get; set; }

        /// <summary>
        /// GID of the governor that's being removed.
        /// </summary>
        public int? RemovalGid { get; set; }

        public bool? GovernorShared { get; set; }

        public DateTimeViewModel RemovalAppointmentEndDate { get; set; } = new DateTimeViewModel();

        public string ListOfEstablishmentsPluralName { get; set; }

        public string GroupName { get; set; }

        public int? GroupTypeId { get; set; }

        public string SelectedTabName { get; set; }

        string IEstablishmentPageViewModel.SelectedTab { get; set; }

        int? IEstablishmentPageViewModel.Urn { get; set; }

        string IEstablishmentPageViewModel.Name { get; set; }

        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }

        public eGovernanceMode? GovernanceMode { get; set; }

        public GovernorsGridViewModel(GovernorsDetailsDto dto, bool editMode, int? groupUId, int? establishmentUrn, NomenclatureService nomenclatureService, Dictionary<int, string> appointingBodies)
        {
            _nomenclatureService = nomenclatureService;
            _appointingBodies = appointingBodies;
            DelegationInformation = dto.GroupDelegationInformation;
            ShowDelegationInformation = dto.ShowDelegationInformation;
            DomainModel = dto;
            EditMode = editMode;
            GroupUId = groupUId;
            EstablishmentUrn = establishmentUrn;
            CreateGrids(dto, dto.CurrentGovernors, false, groupUId, establishmentUrn);
            CreateGrids(dto, dto.HistoricalGovernors, true, groupUId, establishmentUrn);
        }

        public GovernorsGridViewModel()
        {

        }

        private void CreateGrids(GovernorsDetailsDto dto, IEnumerable<GovernorModel> governors, bool isHistoric, int? groupUid, int? establishmentUrn)
        {
            foreach (var role in dto.ApplicableRoles.Where(role => !EnumSets.eSharedGovernorRoles.Contains(role)))
            {
                var equivalantRoles = SharedLocalRoleEquivalence.GetEquivalentLocalRole(role).Cast<int>().ToList();

                var grid = new GovernorGridViewModel($"{_nomenclatureService.GetGovernorRoleName(role, eTextCase.SentenceCase, true)}{(isHistoric ? " (in past 12 months)" : string.Empty)}")
                {
                    Tag = isHistoric ? "historic" : "current",
                    Role = role,
                    IsSharedRole = EnumSets.eSharedGovernorRoles.Contains(role),
                    GroupUid = groupUid,
                    EstablishmentUrn = establishmentUrn,
                    IsHistoricRole = isHistoric
                };

                var displayPolicy = dto.RoleDisplayPolicies.Get(role);
                Guard.IsNotNull(displayPolicy, () => new Exception($"The display policy should not be null for the role '{role}'"));
                bool includeEndDate = ((isHistoric && role == GR.Member || role != GR.Member) && displayPolicy.AppointmentEndDate) || 
                                       (role.OneOfThese(GR.ChiefFinancialOfficer, GR.AccountingOfficer) && isHistoric);

                SetupHeader(role, grid, displayPolicy, includeEndDate);

                var list = governors.Where(x => x.RoleId.HasValue && equivalantRoles.Contains(x.RoleId.Value));
                foreach (var governor in list)
                {
                    var isShared = governor.RoleId.HasValue && EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value);
                    var establishments = string.Join(", ",
                        governor.Appointments?.Select(a => $"{a.EstablishmentName}, URN: {a.EstablishmentUrn}") ??
                        new string[] { });
                    var appointment = governor.Appointments?.SingleOrDefault(a => a.EstablishmentUrn == EstablishmentUrn);
                    var startDate = (isShared && appointment != null) ? appointment.AppointmentStartDate : governor.AppointmentStartDate;
                    var endDate = (isShared && appointment != null) ? appointment.AppointmentEndDate : governor.AppointmentEndDate;

                    var row = grid.AddRow(governor).AddCell(governor.GetFullName(), displayPolicy.FullName)
                                                   .AddCell(string.IsNullOrWhiteSpace(establishments) ? null : establishments, role.OneOfThese(GR.Establishment_SharedChairOfLocalGoverningBody, GR.Establishment_SharedLocalGovernor, GR.Group_SharedChairOfLocalGoverningBody, GR.Group_SharedLocalGovernor))
                                                   .AddCell(governor.Id, displayPolicy.Id)
                                                   .AddCell((governor.AppointingBodyId.HasValue && _appointingBodies.ContainsKey(governor.AppointingBodyId.Value)) ? _appointingBodies[governor.AppointingBodyId.Value] : null, displayPolicy.AppointingBodyId)
                                                   .AddCell(startDate?.ToString("dd/MM/yyyy"), displayPolicy.AppointmentStartDate)
                                                   .AddCell(endDate?.ToString("dd/MM/yyyy"), includeEndDate)
                                                   .AddCell(governor.PostCode, displayPolicy.PostCode)
                                                   .AddCell(governor.DOB?.ToString("dd/MM/yyyy"), displayPolicy.DOB)
                                                   .AddCell(governor.GetPreviousFullName(), displayPolicy.PreviousFullName)
                                                   //.AddCell(governor.Nationality, displayPolicy.Nationality) // todo: texchange: use lookup to get text label
                                                   .AddCell(governor.EmailAddress, displayPolicy.EmailAddress)
                                                   .AddCell(governor.TelephoneNumber, displayPolicy.TelephoneNumber);
                }

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

        private void SetupHeader(GR role, GridViewModel<GovernorModel> grid, GovernorDisplayPolicy displayPolicy, bool includeEndDate)
        {
            grid.AddHeaderCell("Name", displayPolicy.FullName)
                .AddHeaderCell("Shared with", role.OneOfThese(GR.Establishment_SharedChairOfLocalGoverningBody, GR.Establishment_SharedLocalGovernor, GR.Group_SharedChairOfLocalGoverningBody, GR.Group_SharedLocalGovernor))
                .AddHeaderCell("GID", displayPolicy.Id)
                .AddHeaderCell("Appointed by", displayPolicy.AppointingBodyId)
                .AddHeaderCell("From", displayPolicy.AppointmentStartDate)
                .AddHeaderCell(role == GR.Member ? "Date stepped down" : "To", includeEndDate)
                .AddHeaderCell("Postcode", displayPolicy.PostCode)
                .AddHeaderCell("Date of birth", displayPolicy.DOB)
                .AddHeaderCell("Previous name", displayPolicy.PreviousFullName)
                //.AddHeaderCell("Nationality", displayPolicy.Nationality)
                .AddHeaderCell("Email address", displayPolicy.EmailAddress)
                .AddHeaderCell("Telephone", displayPolicy.TelephoneNumber);
        }
    }
}