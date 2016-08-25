using System.Collections.Generic;
using Web.Services.Api.Models;

namespace Web.Services.Api
{
    public interface IApiService
    {
        dynamic GetSchoolById(string id);
        dynamic GetSchoolsByIds(IEnumerable<string> ids, string fields);

        dynamic GetAssociatedSchools(string id, string laestab);

        dynamic GetLocalAuthorityById(string id);

        dynamic GetNationalData();

        dynamic GetSchoolKeystageData(string id, string stage, string fields);

        dynamic SearchKeystageData(string stage, string filter = null, string orderby = null, string skip = null,
            string take = null);

        dynamic SearchSchoolByName(string q, string filter = null, string orderby = null, string skip = null,
            string take = null, bool disambiguate = false);

        dynamic SearchSchoolByRegionOrLa(string region, string la, string filter = null, string orderby = null,
            string skip = null,
            string take = null);

        dynamic SearchSchoolByAcademySponsor(string academySponsorCode, string filter = null, string orderby = null,
            string skip = null,
            string take = null);

        dynamic SearchSchoolLocation(string q, decimal distance, string filter = null, string orderby = null,
            string skip = null,
            string take = null);

        dynamic SearchSchoolByTownOrCityId(string id, decimal distance, string filter = null, string orderby = null,
            string skip = null,
            string take = null);

        dynamic SearchSchoolByPostcode(string q, decimal distance, string filter = null, string orderby = null,
            string skip = null,
            string take = null);

        dynamic SearchSchoolByLatLong(string lat, string lon, decimal distance, string filter = null,
            string orderby = null, string skip = null,
            string take = null);

        dynamic SearchSchoolByParliamentaryConstituency(string parliamentaryConstituency, string filter = null,
            string orderby = null, string skip = null,
            string take = null);

        DownloadResponse DonwloadDataFile(string regions, string filters, string fileFormat, string year, bool meta);
        DownloadResponse DonwloadSchoolDataFile(string urn, string format);

        dynamic GetFileAttachments(string urn);

        dynamic DownloadDataYears();
        dynamic DownloadDataRegionsForYear(string year);

        dynamic SearchSchool(string filter = null, string orderby = null,
            string skip = null,
            string take = null);

        dynamic SuggestSchool(string url, string query);

        dynamic SuggestTown(string url, string query);

        dynamic GetSimilarSchools(string urn, string phase);
    }
}