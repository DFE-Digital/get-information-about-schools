using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System;

namespace Edubase.Services.Governors.Search
{
    public class GovernorsSearchIndex
    {
        public const string INDEX_NAME = "governors";
        public const string SUGGESTER_NAME = "governors";

        public SearchIndex CreateModel()
        {
            var retVal = new SearchIndex();
            retVal.Name = INDEX_NAME;
            retVal.SuggesterName = SUGGESTER_NAME;
            
            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Id",
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
                Name = "EstablishmentUrn",
                IsFacetable = false,
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
                Name = "Person_Title",
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
                Name = "Person_FirstName",
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
                Name = "Person_MiddleName",
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
                Name = "Person_LastName",
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
                Name = "PreviousPerson_Title",
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
                Name = "PreviousPerson_FirstName",
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
                Name = "PreviousPerson_MiddleName",
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
                Name = "PreviousPerson_LastName",
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
                Name = "AppointmentStartDate",
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
                Name = "AppointmentEndDate",
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
                Name = "RoleId",
                IsFacetable = false,
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
                Name = "AppointingBodyId",
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
                Name = "EmailAddress",
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
                Name = "DOB",
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
                Name = "Nationality",
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
                Name = "PostCode",
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
                Name = "GroupUID",
                IsFacetable = false,
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
