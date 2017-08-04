using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupChangeRequestType
    {
        [EnumMember(Value = "Group change")]
        GroupChange,

        [EnumMember(Value = "New link")]
        NewLink,

        [EnumMember(Value = "Remove link")]
        RemoveLink,

        [EnumMember(Value = "New group")]
        NewGroup
    }
}
