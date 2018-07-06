using System;

namespace Edubase.Services.Establishments.Models
{
    public class Establishment2JsonPropertyMap
    {
        private static readonly string[] _properties = new[]
        {
            "urn",
            "localAuthorityId",
            "establishmentNumber",
            "name",
            "statusId",
            "reasonEstablishmentOpenedId",
            "reasonEstablishmentClosedId",
            "openDate",
            "closeDate",
            "educationPhaseId",
            "statutoryLowAge",
            "statutoryHighAge",
            "provisionBoardingId",
            "provisionNurseryId",
            "provisionOfficialSixthFormId",
            "genderId",
            "religiousCharacterId",
            "religiousEthosId",
            "dioceseId",
            "admissionsPolicyId",
            "capacity",
            "provisionSpecialClassesId",
            "UKPRN",
            "lastChangedDate",
            "address_Line1",
            "address_Line2",
            "address_Line3",
            "address_CityOrTown",
            "address_CountyId",
            "address_CountryId",
            "address_Locality",
            "address_PostCode",
            "headFirstName",
            "headLastName",
            "headTitleId",
            "headEmailAddress",
            "headPreferredJobTitle",
            "contact_TelephoneNumber",
            "contact_EmailAddress",
            "contact_WebsiteAddress",
            "contact_FaxNumber",
            "contactAlt_TelephoneNumber",
            "contactAlt_EmailAddress",
            "contactAlt_WebsiteAddress",
            "contactAlt_FaxNumber",
            "typeId",
            "easting",
            "northing",
            "latLon",
            "establishmentTypeGroupId",
            "ofstedRating",
            "ofstedInspectionDate",
            "inspectorateId",
            "section41ApprovedId",
            "proprietorName",
            "SENStat",
            "SENNoStat",
            "SENIds",
            "teenageMothersProvisionId",
            "teenageMothersCapacity",
            "childcareFacilitiesId",
            "PRUSENId",
            "PRUEBDId",
            "placesPRU",
            "pruFulltimeProvisionId",
            "pruEducatedByOthersId",
            "typeOfResourcedProvisionId",
            "resourcedProvisionOnRoll",
            "resourcedProvisionCapacity",
            "governmentOfficeRegionId",
            "administrativeDistrictId",
            "administrativeWardId",
            "parliamentaryConstituencyId",
            "urbanRuralId",
            "GSSLAId",
            "casWardId",
            "MSOAId",
            "LSOAId",
            "furtherEducationTypeId",
            "ccGovernanceId",
            "ccGovernanceDetail",
            "ccOperationalHoursId",
            "ccPhaseTypeId",
            "ccGroupLeadId",
            "ccDisadvantagedAreaId",
            "ccDirectProvisionOfEarlyYearsId",
            "ccDeliveryModelId",
            "ccUnder5YearsOfAgeCount",
            "senUnitOnRoll",
            "senUnitCapacity",
            "rscRegionId",
            "bsoInspectorateId",
            "bsoInspectorateReportUrl",
            "bsoDateOfLastInspectionVisit",
            "bsoDateOfNextInspectionVisit",
            "createdUtc",
            "lastUpdatedUtc",
            "predecessorName",
            "predecessorUrn",
            "nextGeneralActionRequired",
            "nextActionRequiredByWEL",
            "numberOfPupilsOnRoll"
        };

        public static string Convert(string propertyName)
        {
            if (propertyName == nameof(EstablishmentModel.Location)) propertyName = "latLon";

            try
            {
                return _properties.SingleOrThrow(x => x.Equals(propertyName, StringComparison.OrdinalIgnoreCase), () => new Exception("The field was not found in JsonPropertyMap"));
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't find item '{propertyName}' in JsonPropertyMap", ex);
            }
            
        }
    }
}
