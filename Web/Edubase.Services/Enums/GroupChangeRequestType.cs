using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupChangeRequestType
    {
        [EnumMember(Value = "Group Change")]
        GroupChange,

        [EnumMember(Value = "New Link")]
        NewLink,

        [EnumMember(Value = "Remove Link")]
        RemoveLink,

        [EnumMember(Value = "New Group")]
        NewGroup
    }
}
