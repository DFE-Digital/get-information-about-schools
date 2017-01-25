using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentsSearchIndex
    {
        public const string INDEX_NAME = "establishments";
        public const string SUGGESTER_NAME = "establishments";

        public SearchIndex CreateModel()
        {
            var retVal = new SearchIndex();
            retVal.Name = INDEX_NAME;
            retVal.SuggesterName = SUGGESTER_NAME;


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Urn",
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
                Name = "LocalAuthorityId",
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
                Name = "EstablishmentNumber",
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
                Name = "Name",
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
                Name = "NameDistilled",
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
                Name = "StatusId",
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
                Name = "ReasonEstablishmentOpenedId",
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
                Name = "ReasonEstablishmentClosedId",
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
                Name = "CloseDate",
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
                Name = "EducationPhaseId",
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
                Name = "StatutoryLowAge",
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
                Name = "StatutoryHighAge",
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
                Name = "ProvisionBoardingId",
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
                Name = "ProvisionNurseryId",
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
                Name = "ProvisionOfficialSixthFormId",
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
                Name = "GenderId",
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
                Name = "ReligiousCharacterId",
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
                Name = "ReligiousEthosId",
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
                Name = "DioceseId",
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
                Name = "AdmissionsPolicyId",
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
                Name = "Capacity",
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
                Name = "ProvisionSpecialClassesId",
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
                Name = "UKPRN",
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
                Name = "LastChangedDate",
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
                Name = "Address_Line1",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Address_Line2",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Address_Line3",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Address_CityOrTown",
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
                Name = "Address_County",
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
                Name = "Address_Country",
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
                Name = "Address_Locality",
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
                Name = "Address_PostCode",
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
                Name = "HeadFirstName",
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
                Name = "HeadLastName",
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
                Name = "HeadTitleId",
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
                Name = "HeadEmailAddress",
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
                Name = nameof(Data.Entity.Establishment.HeadPreferredJobTitle),
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
                Name = "Contact_TelephoneNumber",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Contact_EmailAddress",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Contact_WebsiteAddress",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Contact_FaxNumber",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "ContactAlt_TelephoneNumber",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "ContactAlt_EmailAddress",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "ContactAlt_WebsiteAddress",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "ContactAlt_FaxNumber",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "TypeId",
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
                Name = "Easting",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(int)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Northing",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(int)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "Location",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(DbGeography)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "EstablishmentTypeGroupId",
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
                Name = "OfstedRating",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = true,
                IsRetrievable = true,
                IsSortable = true,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(byte)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "OfstedInspectionDate",
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
                Name = "InspectorateId",
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
                Name = "Section41ApprovedId",
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
                Name = "ProprietorName",
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
                Name = "SENStat",
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
                Name = "SENNoStat",
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
                Name = "SEN1Id",
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
                Name = "SEN2Id",
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
                Name = "SEN3Id",
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
                Name = "SEN4Id",
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
                Name = "TeenageMothersProvisionId",
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
                Name = "TeenageMothersCapacity",
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
                Name = "ChildcareFacilitiesId",
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
                Name = "PRUSENId",
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
                Name = "PRUEBDId",
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
                Name = "PlacesPRU",
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
                Name = "PruFulltimeProvisionId",
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
                Name = "PruEducatedByOthersId",
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
                Name = "TypeOfResourcedProvisionId",
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
                Name = "ResourcedProvisionOnRoll",
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
                Name = "ResourcedProvisionCapacity",
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
                Name = "GovernmentOfficeRegionId",
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
                Name = "AdministrativeDistrictId",
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
                Name = "AdministrativeWardId",
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
                Name = "ParliamentaryConstituencyId",
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
                Name = "UrbanRuralId",
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
                Name = "GSSLAId",
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
                Name = "CASWardId",
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
                Name = "MSOAId",
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
                Name = "LSOAId",
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
                Name = "FurtherEducationTypeId",
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
                Name = "CCGovernanceId",
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
                Name = "CCGovernanceDetail",
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
                Name = "CCOperationalHoursId",
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
                Name = "CCPhaseTypeId",
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
                Name = "CCGroupLeadId",
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
                Name = "CCDisadvantagedAreaId",
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
                Name = "CCDirectProvisionOfEarlyYearsId",
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
                Name = "CCDeliveryModelId",
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
                Name = "CCUnder5YearsOfAgeCount",
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
                Name = "SenUnitOnRoll",
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
                Name = "SenUnitCapacity",
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
                Name = "RSCRegionId",
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
                Name = "BSOInspectorateId",
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
                Name = "BSOInspectorateReportUrl",
                IsFacetable = false,
                IsSearchable = false,
                IsFilterable = false,
                IsRetrievable = true,
                IsSortable = false,
                IsKey = false,
                IncludeInSuggester = false,
                ClrType = typeof(string)
            });


            retVal.Fields.Add(new SearchIndexField
            {
                Name = "BSODateOfLastInspectionVisit",
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
                Name = "BSODateOfNextInspectionVisit",
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
