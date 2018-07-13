using Newtonsoft.Json;

namespace Edubase.Services.Domain
{
    public class ChangeDescriptorDto
    {
        [JsonProperty("fieldName")]
        public string Name { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Id { get; set; }
        public string Tag { get; set; }
        public bool RequiresApproval { get; set; }
    }
}
