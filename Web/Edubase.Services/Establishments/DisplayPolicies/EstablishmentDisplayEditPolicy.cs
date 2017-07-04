using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Newtonsoft.Json;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class EstablishmentDisplayEditPolicy : EstablishmentFieldList
    {
        public string HeadteacherLabel { get; set; } = "Headteacher/Principal";
        public string HeadEmailAddressLabel { get; set; } = "Headteacher/Principal email";
        public string EstablishmentTypeLabel { get; set; } = "School type";
        public string MainEmailAddressLabel { get; set; } = "Email";
        
        [JsonProperty("iebtDetail")]
        public IEBTDetailDisplayEditPolicy IEBTDetail { get; set; }

        public EstablishmentDisplayEditPolicy Initialise(EstablishmentModel establishment)
        {
            if(establishment.EstablishmentTypeGroupId == (int)eLookupEstablishmentTypeGroup.ChildrensCentres)
            {
                HeadteacherLabel = "Manager";
                HeadEmailAddressLabel = "Manager email";
                MainEmailAddressLabel = "Centre email";
                EstablishmentTypeLabel = "Provider type";
            }

            return this;
        }
    }
}