using System.Collections.Generic;
using System.Text.RegularExpressions;
using Web.Services.Api;

namespace Web.Services.Search
{
    public class SearchSchoolsByUrnStrategy : ISearchSchoolsStrategy
    {
        private static readonly Regex UrnRegex = new Regex(@"^\d{6}$", RegexOptions.Compiled);
        private readonly IApiService _apiService;
        public int Priority => 1;
        public string Description => "URN";

        public SearchSchoolsByUrnStrategy(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IEnumerable<dynamic> Search(string searchTerm)
        {
            if (searchTerm == null || !UrnRegex.IsMatch(searchTerm))
            {
                return null;
            }

            var result = _apiService.GetSchoolById(searchTerm);
            if (result == null)
            {
                throw new SearchException(SearchExceptionTypes.InvalidUrn);
            }

            return new[] { result };
        }
    }
}