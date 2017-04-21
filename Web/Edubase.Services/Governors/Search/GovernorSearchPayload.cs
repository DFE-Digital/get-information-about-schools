using Edubase.Services.Core.Search;
using Newtonsoft.Json;
using System.Collections.Generic;

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
