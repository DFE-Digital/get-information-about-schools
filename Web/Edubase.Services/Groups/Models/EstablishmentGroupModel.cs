using Edubase.Services.Domain;
using Newtonsoft.Json;
using System;

namespace Edubase.Services.Groups.Models
{
    public class EstablishmentGroupModel
    {
        public int? Id { get; set; }
        
        public int EstablishmentUrn { get; set; }
        public DateTime? JoinedDate { get; set; }
        public bool? CCIsLeadCentre { get; set; }
        
        public AddressDto Address { get; set; } = new AddressDto();
        public string HeadTitle { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public int? Urn { get; set; }

        [JsonProperty("phaseName")]
        public string PhaseName { get; set; }

        [JsonProperty("laestab")]
        public string LAESTAB { get; set; }

        [JsonProperty("localAuthorityName")]
        public string LocalAuthorityName { get; set; }

        [JsonProperty("statusName")]
        public string StatusName { get; set; }
    }
}
