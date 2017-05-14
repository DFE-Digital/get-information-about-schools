using System;

namespace Edubase.Services.Governors.Models
{
    /// <summary>
    /// TODO: TEXCHANGE; what is this.
    /// </summary>
    public class GovernorAppointment
    {
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public int? EstablishmentUrn { get; set; }
        public string EstablishmentName { get; set; }
    }
}
