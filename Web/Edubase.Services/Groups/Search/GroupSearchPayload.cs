using Edubase.Services.Core.Search;
using System.Collections.Generic;

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

        public string Text { get; set; }
        
        public int Skip { get; set; }

        public int Take { get; set; } = 10;

        public eSortBy SortBy { get; set; }
    }
}
