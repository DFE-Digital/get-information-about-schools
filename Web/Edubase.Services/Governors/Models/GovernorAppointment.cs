using System;

namespace Edubase.Services.Governors.Models
{
    public class GovernorAppointment
    {
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public int? EstablishmentUrn { get; set; }
        public string EstablishmentName { get; set; }
    }
}
