using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using Web.Services.Api;
using Web.Services.Api.Models;
using System.Linq;
using System;
using Web.Services.Exceptions;

namespace Web.Services.Search
{
    public class SearchSchoolsByLaNameStrategy : ISearchSchoolsStrategy
    {
        private readonly IApiService _apiService;
        public int Priority => 2;
        public string Description => "Local authority name";

        public SearchSchoolsByLaNameStrategy(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref=""></exception>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> Search(string searchTerm)
        {
            if (searchTerm == null)  return null;

            var localAuthorities = MemoryCache.Default.Get("las") as RegionsAndAuthorities;
            if (localAuthorities == null)
            {
                localAuthorities = _apiService.GetRegionsAndAuthorities();
                MemoryCache.Default.Set("las", localAuthorities, new CacheItemPolicy());
            }

            var la = localAuthorities.LocalAuthorities.Where(x => x.LANAME.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > -1).FirstOrDefault();
            if (la == null) throw new LaNameNotFoundException();

            return _apiService.SearchSchool($"LA eq '{la.id}'")?.Results;
        }
    }
}