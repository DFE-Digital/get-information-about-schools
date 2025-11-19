using Newtonsoft.Json;
using System;

namespace Edubase.Services.Establishments.Search
{
    public abstract class EstablishmentSearchFiltersLookups
    {
        public int[] LocalAuthorityIds { get; set; } = new int[0];

        public int[] StatusIds { get; set; } = new int[0];

        public int[] ReasonEstablishmentOpenedIds { get; set; } = new int[0];

        public int[] ReasonEstablishmentClosedIds { get; set; } = new int[0];

        public int[] EducationPhaseIds { get; set; } = new int[0];

        public int[] ProvisionBoardingIds { get; set; } = new int[0];

        public int[] ProvisionNurseryIds { get; set; } = new int[0];

        public int[] ProvisionOfficialSixthFormIds { get; set; } = new int[0];

        public int[] GenderIds { get; set; } = new int[0];

        public int[] RegistrationSuspendedIds { get; set; } = new int[0];

        public int[] ReligiousCharacterIds { get; set; } = new int[0];

        public int[] ReligiousEthosIds { get; set; } = new int[0];

        public int[] DioceseIds { get; set; } = new int[0];

        public int[] AdmissionsPolicyIds { get; set; } = new int[0];

        public int[] ProvisionSpecialClassesIds { get; set; } = new int[0];

        public int[] HeadTitleIds { get; set; } = new int[0];

        public int[] TypeIds { get; set; } = new int[0];

        public int[] EstablishmentTypeGroupIds { get; set; } = new int[0];

        public int[] InspectorateIds { get; set; } = new int[0];

        public int[] Section41ApprovedIds { get; set; } = new int[0];

        [JsonProperty("SENIds")]
        public int[] SENIds { get; set; } = new int[0];

        public int[] TeenageMothersProvisionIds { get; set; } = new int[0];

        public int[] ChildcareFacilitiesIds { get; set; } = new int[0];

        [JsonProperty("PRUSENIds"), JsonIgnore]
        public int[] PRUSENIds { get; set; } = new int[0];

        [JsonProperty("PRUEBDIds"), JsonIgnore]
        public int[] PRUEBDIds { get; set; } = new int[0];

        public int[] PruFulltimeProvisionIds { get; set; } = new int[0];

        public int[] PruEducatedByOthersIds { get; set; } = new int[0];

        public int[] TypeOfResourcedProvisionIds { get; set; } = new int[0];

        public int[] GovernmentOfficeRegionIds { get; set; } = new int[0];

        public int[] AdministrativeDistrictIds { get; set; } = new int[0];

        public int[] AdministrativeWardIds { get; set; } = new int[0];

        public int[] ParliamentaryConstituencyIds { get; set; } = new int[0];

        public int[] UrbanRuralIds { get; set; } = new int[0];

        [JsonProperty("GSSLAIds"), JsonIgnore]
        public int[] GSSLAIds { get; set; } = new int[0];

        [JsonProperty("MSOAIds"), JsonIgnore]
        public int[] MSOAIds { get; set; } = new int[0];

        [JsonProperty("LSOAIds"), JsonIgnore]
        public int[] LSOAIds { get; set; } = new int[0];

        public int[] FurtherEducationTypeIds { get; set; } = new int[0];

        public int[] CCGovernanceIds { get; set; } = new int[0];

        public int[] CCOperationalHoursIds { get; set; } = new int[0];

        public int[] CCPhaseTypeIds { get; set; } = new int[0];

        public int[] CCGroupLeadIds { get; set; } = new int[0];

        public int[] CCDisadvantagedAreaIds { get; set; } = new int[0];

        public int[] CCDirectProvisionOfEarlyYearsIds { get; set; } = new int[0];

        public int[] CCDeliveryModelIds { get; set; } = new int[0];

        public int[] RSCRegionIds { get; set; } = new int[0];

        public int[] BSOInspectorateIds { get; set; } = new int[0];

    }
}
