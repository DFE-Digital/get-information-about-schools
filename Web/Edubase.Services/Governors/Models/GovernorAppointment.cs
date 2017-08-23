using System;
using Newtonsoft.Json;

namespace Edubase.Services.Governors.Models
{
    public class GovernorAppointment
    {
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public int? EstablishmentUrn { get; set; }

        /// <summary>
        /// NOTE: Texuna do not want to supply this field in the governors API response due to 'caching reasons'
        /// </summary>
        [JsonIgnore]
        public string EstablishmentName { get; set; }
    }
}
