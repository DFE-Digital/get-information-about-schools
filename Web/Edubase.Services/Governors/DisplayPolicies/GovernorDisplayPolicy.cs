using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Governors.DisplayPolicies
{
    public class GovernorDisplayPolicy
    {
        public bool Id { get; set; }
        public bool FullName { get; set; }
        public bool IsOriginalSignatoryMember { get; set; }
        public bool IsOriginalChairOfTrustees { get; set; }
        public bool AppointmentStartDate { get; set; }
        public bool AppointmentEndDate { get; set; }
        public bool RoleId { get; set; } = true;
        public bool AppointingBodyId { get; set; }
        public bool EmailAddress { get; set; }
        public bool DOB { get; set; }
        public bool PostCode { get; set; }
        public bool PreviousFullName { get; set; }
        public bool TelephoneNumber { get; set; }
        
        public GovernorDisplayPolicy Clone() => MemberwiseClone() as GovernorDisplayPolicy;
        
    }
}
