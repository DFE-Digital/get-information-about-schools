using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using Newtonsoft.Json;
using System.Security.Principal;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class EstablishmentDisplayPolicy : EstablishmentFieldList
    {
        public string HeadteacherLabel { get; set; } = "Headteacher/Principal";
        public string HeadEmailAddressLabel { get; set; } = "Headteacher/Principal email";
        
        public string EstablishmentTypeLabel { get; set; } = "School type";
        public string MainEmailAddressLabel { get; set; } = "Email";
        protected EstablishmentModel Establishment { get; private set; }
        protected IPrincipal Principal { get; private set; }
        public bool IsUserLoggedIn { get; set; }
        public bool IsSchoolClosed { get; set; }

        [JsonProperty("iebtDetail")]
        public IEBTDetailDisplayPolicy IEBTDetail { get; set; }
    }
}