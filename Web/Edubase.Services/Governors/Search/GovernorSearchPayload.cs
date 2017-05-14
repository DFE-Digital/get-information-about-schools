using Edubase.Services.Enums;
using Newtonsoft.Json;

namespace Edubase.Services.Governors.Search
{
    public class GovernorSearchPayload
    {
        public GovernorSearchPayload()
        {

        }

        public GovernorSearchPayload(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        [JsonIgnore] // TODO: TEXCHANGE:  support Gid search
        public string Gid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int[] RoleIds { get; set; }
        public bool IncludeHistoric { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; } = 10;
        [JsonIgnore]
        public eSortBy SortBy { get; set; }

        [JsonProperty("sortBy")]
        internal string SortByDescriptor => SortBy.ToString();
    }
}
