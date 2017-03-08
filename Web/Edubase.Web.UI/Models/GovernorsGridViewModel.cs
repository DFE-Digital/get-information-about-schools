using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Models.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Models
{
    using Services.Governors.DisplayPolicies;
    using Services.Nomenclature;
    using GR = eLookupGovernorRole;

    public class GovernorsGridViewModel
    {
        private readonly NomenclatureService _nomenclatureService;

        public List<GridViewModel<GovernorModel>> Grids { get; set; } = new List<GridViewModel<GovernorModel>>();

        public GovernorsDetailsDto DomainModel { get; set; }

        public bool EditMode { get; private set; }

        public int? Id { get; set; }

        public GovernorsGridViewModel(GovernorsDetailsDto dto, NomenclatureService nomenclatureService)
            : this(dto, false, null, nomenclatureService)
        {
        }

        public GovernorsGridViewModel(GovernorsDetailsDto dto, bool editMode, int? id, NomenclatureService nomenclatureService)
        {
            _nomenclatureService = nomenclatureService;
            CreateGrids(dto, dto.CurrentGovernors, false);
            CreateGrids(dto, dto.HistoricGovernors, true);
            DomainModel = dto;
            EditMode = editMode;
            Id = id;
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
                    && displayPolicy.AppointmentEndDate);

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
                        .AddCell(governor.EmailAddress, displayPolicy.EmailAddress);
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
                .AddHeaderCell("Email address", displayPolicy.EmailAddress);
        }
        
        
    }
}