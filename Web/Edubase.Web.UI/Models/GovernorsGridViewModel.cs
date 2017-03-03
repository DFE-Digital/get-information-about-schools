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
    using GR = eLookupGovernorRole;

    public class GovernorsGridViewModel
    {
        public List<GridViewModel> Grids { get; set; } = new List<GridViewModel>();

        public GovernorsDetailsDto DomainModel { get; set; }

        public bool EditMode { get; private set; }

        public int? Id { get; set; }

        public GovernorsGridViewModel(GovernorsDetailsDto dto)
            : this(dto, false, null)
        {
        }

        public GovernorsGridViewModel(GovernorsDetailsDto dto, bool editMode, int? id)
        {
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
                var grid = new GridViewModel(GetRoleName(role) + (isHistoric ? $" (in past 12 months)" : string.Empty));
                var displayPolicy = dto.RoleDisplayPolicies.Get(role);
                Guard.IsNotNull(displayPolicy, () => new Exception($"The display policy should not be null for the role '{role}'"));
                bool includeEndDate = ((isHistoric && role == GR.Member || role != GR.Member) 
                    && displayPolicy.AppointmentEndDate);

                SetupHeader(role, grid, displayPolicy, includeEndDate);
                
                var list = governors.Where(x => x.RoleId == (int)role);
                foreach (var governor in list)
                {
                    grid.AddRow(governor.Id.ToString()).AddCell(governor.GetFullName(), displayPolicy.FullName)
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

        private void SetupHeader(GR role, GridViewModel grid, GovernorDisplayPolicy displayPolicy, bool includeEndDate)
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
        

        private string GetRoleName(GR role)
        {
            if (role == GR.AccountingOfficer) return "Accounting officer";
            else if (role == GR.ChairOfGovernors) return "Chair of governors";
            else if (role == GR.ChairOfLocalGoverningBody) return "Chair of local governing body";
            else if (role == GR.ChairOfTrustees) return "Chair of trustees";
            else if (role == GR.ChiefFinancialOfficer) return "Chief financial officer";
            else if (role == GR.Governor) return "Governors";
            else if (role == GR.LocalGovernor) return "Local governors";
            else if (role == GR.Member) return "Members";
            else if (role == GR.Trustee) return "Trustees";
            return role.ToString();
        }
    }
}