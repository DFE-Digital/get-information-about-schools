using Newtonsoft.Json;

namespace Edubase.Services.Domain
{
    public class ChangeDescriptorDto
    {
        [JsonProperty("fieldName")]
        public string Name { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
