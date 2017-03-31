using Edubase.Services.Domain;
using Edubase.Services.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Lookup
{
    public class LookupApiService : ILookupService
    {
        private const string ApiPrefix = "lookup/";
        private const string ApiLocalAuthorityLookupPath = ApiPrefix + "local-authorities";

        /* 
         * THE FOLLOWING APIs SHOULD HAVE IMMUTABLE IDs
         */
        private const string ApiGovernorRolesLookupPath = ApiPrefix + "governor-roles";
        private const string ApiGovernorAppointingBodiesLookupPath = ApiPrefix + "governor-appointing-bodies";
        private const string ApiGroupTypesLookupPath = ApiPrefix + "group-types";
        private const string ApiEstablishmentTypeGroupsLookupPath = ApiPrefix + "establishment-type-groups";
        private const string ApiEstablishmentStatusesLookupPath = ApiPrefix + "establishment-statuses";
        private const string ApiEstablishmentTypesLookupPath = ApiPrefix + "establishment-types";
        private const string ApiGroupStatusesLookupPath = ApiPrefix + "group-statuses";


        private readonly HttpClientWrapper _httpClient;
        
        
        public LookupApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiLocalAuthorityLookupPath);
        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiGovernorRolesLookupPath);
        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiGovernorAppointingBodiesLookupPath);
        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiGroupTypesLookupPath);
        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiEstablishmentTypeGroupsLookupPath);
        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiEstablishmentStatusesLookupPath);
        public async Task<IEnumerable<LookupDto>> EstablishmentTypesGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiEstablishmentTypesLookupPath);
        public async Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync() => await _httpClient.GetAsync<List<LookupDto>>(ApiGroupStatusesLookupPath);




        public IEnumerable<LookupDto> LocalAuthorityGetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> AccommodationChangedGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> AdministrativeDistrictsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> AdministrativeWardsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> AdmissionsPoliciesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> BoardingEstablishmentGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> CASWardsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> CASWardsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> CCDeliveryModelsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> CCDeliveryModelsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> CCDisadvantagedAreasGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> CCGovernanceGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> CCGroupLeadsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> CCGroupLeadsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> CCOperationalHoursGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> CCPhaseTypesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ChildcareFacilitiesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> DiocesesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> DiocesesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> DirectProvisionOfEarlyYearsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> EducationPhasesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> EstablishmentLinkTypesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> EstablishmentStatusesGetAll()
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<LookupDto> EstablishmentTypeGroupsGetAll()
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<LookupDto> EstablishmentTypesGetAll()
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<LookupDto> FurtherEducationTypesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> GendersGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> GendersGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> GovernmentOfficeRegionsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> GovernorAppointingBodiesGetAll()
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<LookupDto> GovernorRolesGetAll()
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<LookupDto> GroupStatusesGetAll()
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<LookupDto> GroupTypesGetAll()
        {
            throw new NotImplementedException();
        }

        

        public IEnumerable<LookupDto> GSSLAGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> GSSLAGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> HeadTitlesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> HeadTitlesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> IndependentSchoolTypesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> InspectorateNamesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> InspectoratesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> LocalGovernorsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> LSOAsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> LSOAsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> MSOAsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> MSOAsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> NationalitiesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ParliamentaryConstituenciesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ProvisionBoardingGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ProvisionNurseriesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ProvisionOfficialSixthFormsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ProvisionSpecialClassesGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> PRUEBDsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> PruEducatedByOthersGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> PruFulltimeProvisionsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> PRUSENsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ReasonEstablishmentClosedGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ReasonEstablishmentOpenedGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ReligiousCharactersGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ReligiousEthosGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> ResourcedProvisionsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> Section41ApprovedGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> SpecialEducationNeedsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> TeenageMothersProvisionsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> TypeOfResourcedProvisionsGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> UrbanRuralGetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
