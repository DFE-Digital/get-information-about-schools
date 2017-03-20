using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Models.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    using UI.Models.Establishments;
    using Services.Governors.DisplayPolicies;
    using Services.Nomenclature;
    using UI.Models;
    using GR = eLookupGovernorRole;

    public class GovernorsGridViewModel : Groups.Models.CreateEdit.IGroupPageViewModel, IEstablishmentPageViewModel
    {
        private readonly NomenclatureService _nomenclatureService;

        public List<GridViewModel<GovernorModel>> Grids { get; set; } = new List<GridViewModel<GovernorModel>>();
        public List<LookupItemViewModel> GovernorRoles { get; internal set; }
        public GovernorsDetailsDto DomainModel { get; set; }

        public bool EditMode { get; private set; }

        public string Action { get; set; }

        public string ParentControllerName => EstablishmentUrn.HasValue ? "Establishment" : "Group";

        public string ParentAreaName => EstablishmentUrn.HasValue ? "" : "Groups";

        public string Layout { get; set; }

        public int? Id => EstablishmentUrn ?? GroupUId;

        public int? EstablishmentUrn { get; set; }

        public int? GroupUId { get; set; }

        public string ParentEntityName => GroupUId.HasValue ? "group" : "establishment";

        /// <summary>
        /// GID of the governor that's being removed.
        /// </summary>
        public int? RemovalGid { get; set; }

        public DateTimeViewModel RemovalAppointmentEndDate { get; set; } = new DateTimeViewModel();

        public string ListOfEstablishmentsPluralName { get; set; }

        public string GroupName { get; set; }

        public int? GroupTypeId { get; set; }

        public string SelectedTabName { get; set; }

        string IEstablishmentPageViewModel.SelectedTab { get; set; }

        int? IEstablishmentPageViewModel.Urn { get; set; }

        string IEstablishmentPageViewModel.Name { get; set; }

        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }

        public GovernorsGridViewModel(GovernorsDetailsDto dto, bool editMode, int? groudUId, int? establishmentUrn, NomenclatureService nomenclatureService)
        {
            _nomenclatureService = nomenclatureService;
            CreateGrids(dto, dto.CurrentGovernors, false);
            CreateGrids(dto, dto.HistoricGovernors, true);
            DomainModel = dto;
            EditMode = editMode;
            GroupUId = groudUId;
            EstablishmentUrn = establishmentUrn;            
        }

        public GovernorsGridViewModel()
        {

        }

        private void CreateGrids(GovernorsDetailsDto dto, IEnumerable<GovernorModel> governors, bool isHistoric)
        {
            foreach (var role in dto.ApplicableRoles)
            {
                var grid = new GridViewModel<GovernorModel>(_nomenclatureService.GetGovernorRoleName(role) + (isHistoric ? $" (in past 12 months)" : string.Empty));
                grid.Tag = isHistoric ? "historic" : "current";
                var displayPolicy = dto.RoleDisplayPolicies.Get(role);
                Guard.IsNotNull(displayPolicy, () => new Exception($"The display policy should not be null for the role '{role}'"));
                bool includeEndDate = ((isHistoric && role == GR.Member || role != GR.Member) 
                    && displayPolicy.AppointmentEndDate) || (role.OneOfThese(GR.ChiefFinancialOfficer, GR.AccountingOfficer) && isHistoric);

                SetupHeader(role, grid, displayPolicy, includeEndDate);
                
                var list = governors.Where(x => x.RoleId == (int)role);
                foreach (var governor in list)
                {
                    grid.AddRow(governor).AddCell(governor.GetFullName(), displayPolicy.FullName)
                        .AddCell(governor.Id, displayPolicy.Id)
                        .AddCell(governor.AppointingBodyName, displayPolicy.AppointingBodyId)
                        .AddCell(governor.AppointmentStartDate?.ToString("dd/MM/yyyy"), displayPolicy.AppointmentStartDate)
                        .AddCell(governor.AppointmentEndDate?.ToString("dd/MM/yyyy"), includeEndDate)
                        .AddCell(governor.PostCode, displayPolicy.PostCode)
                        .AddCell(governor.DOB?.ToString("dd/MM/yyyy"), displayPolicy.DOB)
                        .AddCell(governor.GetPreviousFullName(), displayPolicy.PreviousFullName)
                        .AddCell(governor.Nationality, displayPolicy.Nationality)
                        .AddCell(governor.EmailAddress, displayPolicy.EmailAddress)
                        .AddCell(governor.TelephoneNumber, displayPolicy.TelephoneNumber);
                }

                Grids.Add(grid);
            }
        }

        private void SetupHeader(GR role, GridViewModel<GovernorModel> grid, GovernorDisplayPolicy displayPolicy, bool includeEndDate)
        {
            grid.AddHeaderCell("Name", displayPolicy.FullName)
                .AddHeaderCell("GID", displayPolicy.Id)
                .AddHeaderCell("Appointed by", displayPolicy.AppointingBodyId)
                .AddHeaderCell("From", displayPolicy.AppointmentStartDate)
                .AddHeaderCell(role == GR.Member ? "Date stepped down" : "To", includeEndDate)
                .AddHeaderCell("Postcode", displayPolicy.PostCode)
                .AddHeaderCell("Date of birth", displayPolicy.DOB)
                .AddHeaderCell("Previous name", displayPolicy.PreviousFullName)
                .AddHeaderCell("Nationality", displayPolicy.Nationality)
                .AddHeaderCell("Email address", displayPolicy.EmailAddress)
                .AddHeaderCell("Telephone", displayPolicy.TelephoneNumber);
        }
        
        
    }
}