using Edubase.Services.Enums;
using Newtonsoft.Json;

namespace Edubase.Services.Domain
{
    public class CreateGroupPermissionDto
    {
        [JsonProperty("groupTypeIds")]
        public eLookupGroupType[] GroupTypes { get; set; }

        [JsonProperty("ccLocalAuthorityId")]
        public int? CCLocalAuthorityId { get; set; }
    }
}
