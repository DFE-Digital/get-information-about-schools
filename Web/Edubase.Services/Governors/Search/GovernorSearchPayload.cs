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


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? RoleId { get; set; }
        public bool IncludeHistoric { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; } = 10;
        public IList<string> OrderBy { get; set; } = new List<string>() { nameof(SearchGovernorDocument.Person_LastName) };
    }
}
