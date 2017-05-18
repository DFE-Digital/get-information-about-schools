using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Domain
{
    public class GroupChangeDto : EdubaseChangeDto
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum GroupChangeRequestType
        {
            [EnumMember(Value = "Group Change")]
            GroupChange,

            [EnumMember(Value = "New Link")]
            NewLink,

            [EnumMember(Value = "Remove Link")]
            RemoveLink
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum GroupChangeLinkType
        {
            Hard,
            Soft
        }

        [JsonProperty("uid")]
        public int GroupUId { get; set; }

        public GroupChangeRequestType? RequestType { get; set; }

        public GroupChangeLinkType? LinkType { get; set; }

        public DateTime? LinkDateUtc { get; set; }

        public int? LinkUrn { get; set; }

        public string LinkEstablishmentName { get; set; }
    }
}