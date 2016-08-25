using System.Collections.Generic;
using System.Text.RegularExpressions;
using Web.Services.Api;

namespace Web.Services.Search
{
    public class SearchSchoolsByLaEstabStrategy : ISearchSchoolsStrategy
    {
        private static readonly Regex LaEstabRegex = new Regex(@"^\d{7}$", RegexOptions.Compiled);
        private readonly IApiService _apiService;
        public int Priority => 2;
        public string Description => "LAESTAB";

        public SearchSchoolsByLaEstabStrategy(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IEnumerable<dynamic> Search(string searchTerm)
        {
            if (searchTerm == null || !LaEstabRegex.IsMatch(searchTerm))
            {
                return null;
            }

            var result = _apiService.SearchSchool($"LAESTAB eq '{searchTerm}'");
            if (result == null || result.NumberOfResults == 0)
            {
                throw new SearchException(SearchExceptionTypes.InvalidLaEstab);
            }

            return result.Results;
        }
    }
}