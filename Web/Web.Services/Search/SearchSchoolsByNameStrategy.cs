using System.Collections.Generic;
using Web.Services.Api;

namespace Web.Services.Search
{
    public class SearchSchoolsByNameStrategy : ISearchSchoolsStrategy
    {
        private readonly IApiService _apiService;
        public int Priority => 3;
        public string Description => "School name";

        public SearchSchoolsByNameStrategy(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IEnumerable<dynamic> Search(string searchTerm)
        {
            var results = _apiService.SearchSchoolByName(searchTerm);
            return results.NumberOfResults > 0 ? results.Results : null;
        }
    }
}