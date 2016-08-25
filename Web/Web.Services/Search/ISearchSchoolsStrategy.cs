using System.Collections.Generic;

namespace Web.Services.Search
{
    public interface ISearchSchoolsStrategy
    {
        int Priority { get; }
        string Description { get; }
        IEnumerable<dynamic> Search(string searchTerm);
    }
}
