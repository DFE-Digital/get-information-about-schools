using System;
using Edubase.Services.Enums;
using Newtonsoft.Json;

namespace Edubase.Services.Domain
{
    public class GroupChangeDto : EdubaseChangeDto
    {
        [JsonProperty("uid")]
        public int GroupUId { get; set; }

        public GroupChangeRequestType? RequestType { get; set; }

        public GroupChangeLinkType? LinkType { get; set; }

        public DateTime? LinkDateUtc { get; set; }

        public int? LinkUrn { get; set; }

        public string LinkEstablishmentName { get; set; }
    }
}