using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System;

namespace Edubase.Services.Groups.Search
{
    public class GroupsSearchIndex
    {
        public const string INDEX_NAME = "groups";
        public const string SUGGESTER_NAME = "groups";

        public SearchIndex CreateModel()
        {
            var retVal = new SearchIndex();
            retVal.Name = INDEX_NAME;
            retVal.SuggesterName = SUGGESTER_NAME;

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "GroupUID",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = true,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Name",
                IsFacetable = false,
                IsSearchable = true,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = true,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "CompaniesHouseNumber",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "GroupTypeId",
                IsFacetable = true,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(int)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "ClosedDate",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(DateTime)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "StatusId",
                IsFacetable = true,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(int)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "OpenDate",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(DateTime)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Head_Title",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Head_FirstName",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Head_MiddleName",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Head_LastName",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Address",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "ManagerEmailAddress",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "GroupId",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "CreatedUtc",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(DateTime)
            });
            

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "LastUpdatedUtc",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(DateTime)
            });

            retVal.Fields.Add(new SearchIndexField
            {
                Name = "IsDeleted",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(bool)
            });

            return retVal;
        }
    }
}
