using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class SharedGovernorViewModel
    {
        public bool MultiSelect { get; set; }
        public bool Selected { get; set; }
        public bool PreExisting { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string AppointingBodyName { get; set; }
        public DateTime? DOB { get; set; }
        public string Nationality { get; set; }
        public string PostCode { get; set; }
        public List<EstablishmentViewModel> SharedWith { get; set; }

        [DisplayName("Date of appointment")]
        public DateTimeViewModel AppointmentStartDate { get; set; } = new DateTimeViewModel();

        [DisplayName("Date term ends")]
        public DateTimeViewModel AppointmentEndDate { get; set; } = new DateTimeViewModel();

        public class EstablishmentViewModel
        {
            public int Urn { get; set; }
            public string EstablishmentName { get; set; }
        }

        public static async Task<SharedGovernorViewModel> MapFromGovernor(GovernorModel governor, int establishmentUrn, ICachedLookupService cachedLookupService)
        {
            var dateNow = DateTime.Now.Date;
            var appointment = governor.Appointments?.SingleOrDefault(g => g.EstablishmentUrn == establishmentUrn);
            var sharedWith = governor.Appointments?
                .Where(a => a.AppointmentStartDate < dateNow && (a.AppointmentEndDate == null || a.AppointmentEndDate > dateNow))
                .Select(a => new EstablishmentViewModel { Urn = a.EstablishmentUrn.Value, EstablishmentName = a.EstablishmentName })
                .ToList();

            var appointingBodies = await cachedLookupService.GovernorAppointingBodiesGetAllAsync();
            var nationalities = await cachedLookupService.NationalitiesGetAllAsync();

            return new SharedGovernorViewModel
            {
                AppointingBodyName = appointingBodies.Single(g => g.Id == governor.AppointingBodyId).Name,
                AppointmentStartDate = appointment?.AppointmentStartDate != null ? new DateTimeViewModel(appointment.AppointmentStartDate) : new DateTimeViewModel(),
                AppointmentEndDate = appointment?.AppointmentEndDate != null ? new DateTimeViewModel(appointment.AppointmentEndDate) : new DateTimeViewModel(),
                DOB = governor.DOB,
                FullName = governor.GetFullName(),
                Id = governor.Id.Value,
                Nationality = nationalities.SingleOrDefault(n => n.Id == governor.NationalityId)?.Name,
                PostCode = governor.PostCode,
                Selected = appointment != null,
                PreExisting = appointment != null,
                SharedWith = sharedWith ?? new List<EstablishmentViewModel>(),
                MultiSelect = IsSharedGovernorRoleMultiSelect((eLookupGovernorRole)governor.RoleId)
            };
        }

        private static bool IsSharedGovernorRoleMultiSelect(eLookupGovernorRole role)
        {
            return role == eLookupGovernorRole.Establishment_SharedLocalGovernor;
        }
    }
}