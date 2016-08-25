using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Web.Services.Api.Models;

namespace Web.Services.Api
{
    public class ApiService : IApiService
    {
        private readonly IHttpService _httpService;

        public ApiService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public dynamic GetSchoolById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int schoolId;
                return int.TryParse(id, out schoolId)
                    ? _httpService.ExecuteGet($"/api/school/{id}")
                    : null;
            }

            return new ExpandoObject();
        }

        public dynamic GetSchoolsByIds(IEnumerable<string> ids, string fields)
        {
            var includedFields = fields != null ? "?fields=" + fields : null;
            return _httpService.ExecutePost($"/api/school{includedFields}", new { Urns = ids });
        }

        public dynamic GetAssociatedSchools(string id, string laestab)
        {
            var query = $"/api/school/search?filter=(LAESTAB eq '{laestab}') and (id ne '{id}')";
            return _httpService.ExecuteGet(query);
        }

        public dynamic GetLocalAuthorityById(string id)
        {
            return _httpService.ExecuteGet($"/api/la_region?lea={id}");
        }

        public dynamic GetNationalData()
        {
            return _httpService.ExecuteGet("/api/la_region?lea=0");
        }

        public dynamic GetSchoolKeystageData(string id, string stage, string fields)
        {
            return _httpService.ExecuteGet($"/api/keystage/{id}/{stage}/{fields}");
        }

        public dynamic SearchKeystageData(string stage, string filter = null, string orderby = null, string skip = null,
            string take = null)
        {
            var query = QueryBuilder($"/api/keystage/search/{stage}?filter={filter}", null, orderby, skip, take);
            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolByName(string q, string filter = null, string orderby = null, string skip = null,
            string take = null, bool disambiguate = false)
        {
            q = System.Web.HttpUtility.UrlEncode(q);
            var query = string.Concat(
                QueryBuilder("/api/school/search?name=" + q, filter, orderby, skip, take),
                $"&disambiguate={disambiguate}");

            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolByRegionOrLa(string regionCode, string laCode, string filter = null, string orderby = null, string skip = null,
            string take = null)
        {
            var query = QueryBuilder("/api/school/search?regionCode=" + regionCode + "&laCode=" + laCode, filter, orderby, skip, take);
            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolByAcademySponsor(string academySponsorCode, string filter = null, string orderby = null, string skip = null,
            string take = null)
        {
            var query = QueryBuilder("/api/school/search?academySponsorCode=" + academySponsorCode, filter, orderby, skip, take);
            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolLocation(string q, decimal distance, string filter = null, string orderby = null,
            string skip = null,
            string take = null)
        {
            var query = QueryBuilder("/api/school/search/geo?locationOrPostcode=" + q, filter, orderby, skip, take, distance);
            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolByTownOrCityId(string id, decimal distance, string filter = null, string orderby = null,
            string skip = null,
            string take = null)
        {
            var query = QueryBuilder("/api/school/search/geo?townOrCityId=" + id, filter, orderby, skip, take, distance);
            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolByPostcode(string q, decimal distance, string filter = null, string orderby = null,
            string skip = null,
            string take = null)
        {
            var query = QueryBuilder("/api/school/search/geo?postcode=" + q, filter, orderby, skip, take, distance);
            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolByLatLong(string lat, string lon, decimal distance, string filter = null,
            string orderby = null, string skip = null,
            string take = null)
        {
            var query = QueryBuilder("/api/school/search/geo?lat=" + lat + "&lon=" + lon, filter, orderby, skip, take,
                distance);
            return _httpService.ExecuteGet(query);
        }

        public dynamic SearchSchoolByParliamentaryConstituency(string parliamentaryConstituency, string filter = null,
            string orderby = null, string skip = null,
            string take = null)
        {
            var initialFilter = $"(EDUB_PARLIAMENTARYCONSTITUENCY eq '{parliamentaryConstituency}')";
            var queryFilter = !string.IsNullOrEmpty(filter) ? CombineFilters(initialFilter, filter) : initialFilter;

            var query = QueryBuilder("/api/school/search?", queryFilter, orderby, skip, take);
            return _httpService.ExecuteGet(query);
        }

        public DownloadResponse DonwloadDataFile(string regions, string filters, string fileFormat, string year, bool meta)
        {
            var query = QueryBuilder("/api/blobdownload?regions=" + regions + "&filters=" + filters + "&fileformat=" + fileFormat + "&year=" + year + "&meta=" + meta);
            return _httpService.DownloadFile(query);
        }

        public dynamic GetFileAttachments(string urn)
        {
            return _httpService.ExecuteGet("/api/school/search/files/" + urn);
        }

        public DownloadResponse DonwloadSchoolDataFile(string urn, string format)
        {
            var url = string.IsNullOrEmpty(format)
                ? "/api/download/" + urn
                : "/api/download/" + urn + "?format=" + format;

            return _httpService.DownloadFile(url);
        }

        public dynamic DownloadDataYears()
        {
            return _httpService.ExecuteGet("/api/downloaddatainfo");
        }

        public dynamic DownloadDataRegionsForYear(string year)
        {
            return _httpService.ExecuteGet("/api/blobexistence?year=" + year);
        }

        public DownloadResponse DonwloadFinanceDocuments(string urn, string type)
        {
            var query = QueryBuilder("/api/blobdownload?urn=" + urn + "&type=" + type);
            return _httpService.DownloadFile(query);
        }

        public dynamic SuggestSchool(string url, string query)
        {
            query = System.Web.HttpUtility.UrlEncode(query);
            return _httpService.ExecuteGet(url + query);
        }

        public dynamic SuggestTown(string url, string query)
        {
            return _httpService.ExecuteGet(url + query);
        }

        public dynamic SearchSchool(string filter = null, string orderby = null,
            string skip = null,
            string take = null)
        {
            var query = QueryBuilder("/api/school/search?filter=" + filter, null, orderby, skip, take);
            return _httpService.ExecuteGet(query);
        }

        public dynamic GetSimilarSchools(string urn, string phase)
        {
            var query = $"/api/school/similar/{urn}?phase={phase}";
            return _httpService.ExecuteGet(query);
        }

        public RegionsAndAuthorities GetRegionsAndAuthorities()
        {
            var allLARegions = _httpService.ExecuteGet<LARegion[]>("/api/la_region");
            if (allLARegions == null)
            {
                throw new ApplicationException("Could not retrieve Region data from API service. Path=/api/la_region");
            }

            var distinctRegions = RegionGroup
                .ListAll()
                .Select(x => new LARegion { REGION = x.Value, REGIONNAME = x.Key })
                .ToArray();

            var distinctLAs = allLARegions
                .Where(record => !string.IsNullOrEmpty(record.LANAME))
                .GroupBy(record => record.LANAME)
                .OrderBy(grouped => grouped.Key)
                .Select(grouped => grouped.First())
                .ToArray();

            return new RegionsAndAuthorities
            {
                Regions = distinctRegions,
                LocalAuthorities = distinctLAs
            };
        }

        public ParliamentaryConstituency[] GetParliamentaryConstituencies()
        {
            return _httpService.ExecuteGet<ParliamentaryConstituency[]>("/api/parliamentaryconstituencies");
        }

        public AcademySponsor[] GetAcademySponsors()
        {
            return _httpService.ExecuteGet<AcademySponsor[]>("/api/academysponsors");
        }

        private static string QueryBuilder(string query, string filter = null, string orderby = null, string skip = null,
            string take = null, decimal? distance = null)
        {
            var distanceInMiles = (distance ?? 1) * 1.6m;

            var queryBuilder = new StringBuilder(query);
            queryBuilder.AppendFormat("&$count={0}", "true");

            if (!string.IsNullOrEmpty(filter))
            {
                queryBuilder.AppendFormat("&filter={0}", filter);
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                queryBuilder.AppendFormat("&orderby={0}", orderby);
            }
            if (!string.IsNullOrEmpty(skip))
            {
                queryBuilder.AppendFormat("&skip={0}", skip);
            }
            if (!string.IsNullOrEmpty(take))
            {
                queryBuilder.AppendFormat("&take={0}", take);
            }
            if (distance != null)
            {
                queryBuilder.AppendFormat("&distance={0}", distanceInMiles);
            }
            return queryBuilder.ToString();
        }

        private static string CombineFilters(params string[] filters)
        {
            return string.Join(" and ", filters);
        }
    }
}