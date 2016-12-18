using System.Collections.Generic;

namespace Edubase.Services.Groups.Search
{
    public class GroupSearchPayload
    {
        public GroupSearchPayload()
        {

        }

        public GroupSearchPayload(string orderBy, int skip, int take)
        {
            OrderBy = new List<string> { orderBy };
            Skip = skip;
            Take = take;
        }

        public string Text { get; set; }
        
        public int Skip { get; set; }

        public int Take { get; set; } = 10;
        

        public IList<string> OrderBy { get; set; } = new List<string>();
    }
}
