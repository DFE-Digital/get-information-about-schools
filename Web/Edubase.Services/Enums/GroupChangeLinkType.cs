using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupChangeLinkType
    {
        Hard,
        Soft
    }
}
