using Edubase.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Governors.Models
{
    public class GovernorModel
    {
        public int? Id { get; set; }
        public int? EstablishmentUrn { get; set; }
        
        [JsonProperty("titleId")]
        public int? Person_TitleId { get; set; }

        [JsonProperty("firstName")]
        public string Person_FirstName { get; set; }

        [JsonProperty("middleName")]
        public string Person_MiddleName { get; set; }

        [JsonProperty("lastName")]
        public string Person_LastName { get; set; }

        public bool? IsOriginalSignatoryMember { get; set; }

        public bool? IsOriginalChairOfTrustees { get; set; }

        [JsonProperty("previousTitleId")]
        public int? PreviousPerson_TitleId { get; set; }

        [JsonProperty("previousFirstName")]
        public string PreviousPerson_FirstName { get; set; }

        [JsonProperty("previousMiddleName")]
        public string PreviousPerson_MiddleName { get; set; }

        [JsonProperty("previousLastName")]
        public string PreviousPerson_LastName { get; set; }
        
        public DateTime? AppointmentStartDate { get; set; }

        public DateTime? AppointmentEndDate { get; set; }

        public int? RoleId { get; set; }

        public int? AppointingBodyId { get; set; }

        public string EmailAddress { get; set; }

        [JsonProperty("DOB")]
        public DateTime? DOB { get; set; }

        public string PostCode { get; set; }

        public string TelephoneNumber { get; set; }

        [JsonProperty("groupUId")]
        public int? GroupUId { get; set; }
        
        public string GetFullName() => StringUtil.ConcatNonEmpties(" ", Person_FirstName, Person_MiddleName, Person_LastName);

        public string GetPreviousFullName() => StringUtil.ConcatNonEmpties(" ", PreviousPerson_FirstName, PreviousPerson_MiddleName, PreviousPerson_LastName);

        [JsonIgnore]
        public bool IsNewEntity => !Id.HasValue;

        [JsonProperty("establishments")]
        public IEnumerable<GovernorAppointment> Appointments { get; set; }
    }
}
