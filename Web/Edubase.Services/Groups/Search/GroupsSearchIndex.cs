using Edubase.Data.Entity;
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
                Name = nameof(GroupCollection.GroupUID),
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
                Name = nameof(GroupCollection.Name),
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });

            retVal.Fields.Add(new SearchIndexField
            {
                Name = nameof(GroupCollection.NameDistilled),
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
                Name = nameof(GroupCollection.CompaniesHouseNumber),
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
                Name = nameof(GroupCollection.GroupTypeId),
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
                Name = nameof(GroupCollection.ClosedDate),
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
                Name = nameof(GroupCollection.StatusId),
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
                Name = nameof(GroupCollection.OpenDate),
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
                Name = nameof(GroupCollection.Address),
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
                Name = nameof(GroupCollection.ManagerEmailAddress),
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
                Name = nameof(GroupCollection.GroupId),
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
                Name = nameof(GroupCollection.CreatedUtc),
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
                Name = nameof(GroupCollection.LastUpdatedUtc),
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
                Name = nameof(GroupCollection.EstablishmentCount),
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(int)
            });

            retVal.Fields.Add(new SearchIndexField
            {
                Name = nameof(GroupCollection.LocalAuthorityId),
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(int)
            });

            retVal.Fields.Add(new SearchIndexField
            {
                Name = nameof(GroupCollection.IsDeleted),
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
