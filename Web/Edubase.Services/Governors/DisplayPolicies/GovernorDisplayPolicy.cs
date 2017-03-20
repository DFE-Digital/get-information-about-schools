using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Governors.DisplayPolicies
{
    public class GovernorDisplayPolicy
    {
        /// <summary>
        /// GID / Governor ID
        /// </summary>
        public bool Id { get; internal set; }
        public bool FullName { get; private set; } = true;
        public bool AppointmentStartDate { get; private set; } = true;
        public bool AppointmentEndDate { get; internal set; } = true;
        public bool RoleId { get; private set; } = true;
        public bool AppointingBodyId { get; internal set; } = true;
        public bool EmailAddress { get; internal set; }
        public bool DOB { get; internal set; }
        public bool Nationality { get; internal set; }
        public bool PostCode { get; internal set; }
        public bool PreviousFullName { get; internal set; }
        public bool TelephoneNumber { get; internal set; }

        internal GovernorDisplayPolicy SetFullAccess(bool flag = false)
        {
            Id = EmailAddress = DOB = Nationality = PostCode = PreviousFullName = TelephoneNumber = flag;
            return this;
        }

        internal GovernorDisplayPolicy()
        {
            
        }

        public GovernorDisplayPolicy Clone() => MemberwiseClone() as GovernorDisplayPolicy;
        
    }
}
