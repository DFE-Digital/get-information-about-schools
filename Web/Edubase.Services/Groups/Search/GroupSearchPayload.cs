using System.Collections.Generic;

namespace Edubase.Services.Groups.Search
{
    public class GroupSearchPayload
    {
        public enum eSortBy
        {
            /// <summary>
            /// Alphabetical on the group name (ascending)
            /// </summary>
            NameAlphabeticalAZ,

            /// <summary>
            /// Alphabetical on the group name (descending)
            /// </summary>
            NameAlphabeticalZA
        }

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
