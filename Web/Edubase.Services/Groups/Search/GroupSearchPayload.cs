using Edubase.Services.Enums;
using Newtonsoft.Json;

namespace Edubase.Services.Groups.Search
{
    public class GroupSearchPayload
    {
        public GroupSearchPayload()
        {

        }

        public GroupSearchPayload(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public int[] GroupTypeIds { get; set; }

        [JsonIgnore] // TODO: TEXCHANGE:  support GroupStatusIds
        public int[] GroupStatusIds { get; set; }

        public string Text { get; set; }
        
        public int Skip { get; set; }

        public int Take { get; set; } = 10;

        [JsonIgnore]
        public eSortBy SortBy { get; set; }

        [JsonProperty("sortBy")]
        internal string SortByDescriptor => SortBy.ToString();
    }
}
