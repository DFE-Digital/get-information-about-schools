using Newtonsoft.Json;

namespace Edubase.Services.Domain
{
    public class ApiResultDto<T>
    {
        [JsonProperty("value")]
        public T Value { get; set; }
    }
}
