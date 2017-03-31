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
        private readonly HttpClientWrapper _httpClient;

        //private Dictionary<string, Func<Task<IEnumerable<LookupDto>>>> _map = new Dictionary<string, Func<Task<IEnumerable<LookupDto>>>>();

        public LookupApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;

            // TODO: remove all this!
            //_map.Add("local-authorities", LocalAuthorityGetAllAsync);
            //_map.Add("governor-roles", GovernorRolesGetAllAsync);
            //_map.Add("governor-appointing-bodies", GovernorAppointingBodiesGetAllAsync);
            //_map.Add("group-types", GroupTypesGetAllAsync);
            //_map.Add("establishment-type-groups", EstablishmentTypeGroupsGetAllAsync);
            //_map.Add("establishment-statuses", EstablishmentStatusesGetAllAsync);
            //_map.Add("establishment-types", EstablishmentTypesGetAllAsync);
            //_map.Add("group-statuses", GroupStatusesGetAllAsync);
            //_map.Add("accommodation-changed", AccommodationChangedGetAllAsync);
            //_map.Add("administrative-districts", AdministrativeDistrictsGetAllAsync);
            //_map.Add("administrative-wards", AdministrativeWardsGetAllAsync);
            //_map.Add("admissions-policies", AdmissionsPoliciesGetAllAsync);
            //_map.Add("boarding-establishment", BoardingEstablishmentGetAllAsync);
            //_map.Add("cas-wards", CASWardsGetAllAsync);
            //_map.Add("cc-delivery-models", CCDeliveryModelsGetAllAsync);
            //_map.Add("cc-disadvantaged-areas", CCDisadvantagedAreasGetAllAsync);
            //_map.Add("cc-governance", CCGovernanceGetAllAsync);
            //_map.Add("cc-group-leads", CCGroupLeadsGetAllAsync);
            //_map.Add("cc-operational-hours", CCOperationalHoursGetAllAsync);
            //_map.Add("cc-phase-types", CCPhaseTypesGetAllAsync);
            //_map.Add("childcare-facilities", ChildcareFacilitiesGetAllAsync);
            //_map.Add("dioceses", DiocesesGetAllAsync);
            //_map.Add("direct-provision-of-early-years", DirectProvisionOfEarlyYearsGetAllAsync);
            //_map.Add("education-phases", EducationPhasesGetAllAsync);
            //_map.Add("establishment-link-types", EstablishmentLinkTypesGetAllAsync);
            //_map.Add("further-education-types", FurtherEducationTypesGetAllAsync);
            //_map.Add("genders", GendersGetAllAsync);
            //_map.Add("government-office-regions", GovernmentOfficeRegionsGetAllAsync);
            //_map.Add("gssla", GSSLAGetAllAsync);
            //_map.Add("head-titles", HeadTitlesGetAllAsync);
            //_map.Add("independent-school-types", IndependentSchoolTypesGetAllAsync);
            //_map.Add("inspectorate-names", InspectorateNamesGetAllAsync);
            //_map.Add("inspectorates", InspectoratesGetAllAsync);
            //_map.Add("local-governors", LocalGovernorsGetAllAsync);
            //_map.Add("lsoas", LSOAsGetAllAsync);
            //_map.Add("msoas", MSOAsGetAllAsync);
            //_map.Add("nationalities", NationalitiesGetAllAsync);
            //_map.Add("parliamentary-constituencies", ParliamentaryConstituenciesGetAllAsync);
            //_map.Add("provision-boarding", ProvisionBoardingGetAllAsync);
            //_map.Add("provision-nurseries", ProvisionNurseriesGetAllAsync);
            //_map.Add("provision-official-sixth-forms", ProvisionOfficialSixthFormsGetAllAsync);
            //_map.Add("provision-special-classes", ProvisionSpecialClassesGetAllAsync);
            //_map.Add("pruebds", PRUEBDsGetAllAsync);
            //_map.Add("pru-educated-by-others", PruEducatedByOthersGetAllAsync);
            //_map.Add("pru-fulltime-provisions", PruFulltimeProvisionsGetAllAsync);
            //_map.Add("prusens", PRUSENsGetAllAsync);
            //_map.Add("reason-establishment-closed", ReasonEstablishmentClosedGetAllAsync);
            //_map.Add("reason-establishment-opened", ReasonEstablishmentOpenedGetAllAsync);
            //_map.Add("religious-characters", ReligiousCharactersGetAllAsync);
            //_map.Add("religious-ethos", ReligiousEthosGetAllAsync);
            //_map.Add("resourced-provisions", ResourcedProvisionsGetAllAsync);
            //_map.Add("section41-approved", Section41ApprovedGetAllAsync);
            //_map.Add("special-education-needs", SpecialEducationNeedsGetAllAsync);
            //_map.Add("teenage-mothers-provisions", TeenageMothersProvisionsGetAllAsync);
            //_map.Add("type-of-resourced-provisions", TypeOfResourcedProvisionsGetAllAsync);
            //_map.Add("urban-rural", UrbanRuralGetAllAsync);

        }



        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync() => await GetData("local-authorities");
        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync() => await GetData("governor-roles");
        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync() => await GetData("governor-appointing-bodies");
        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync() => await GetData("group-types");
        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync() => await GetData("establishment-type-groups");
        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync() => await GetData("establishment-statuses");
        public async Task<IEnumerable<LookupDto>> EstablishmentTypesGetAllAsync() => await GetData("establishment-types");
        public async Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync() => await GetData("group-statuses");
        public async Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync() => await GetData("accommodation-changed");
        public async Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync() => await GetData("administrative-districts");
        public async Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync() => await GetData("administrative-wards");
        public async Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync() => await GetData("admissions-policies");
        public async Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync() => await GetData("boarding-establishment");
        public async Task<IEnumerable<LookupDto>> CASWardsGetAllAsync() => await GetData("cas-wards");
        public async Task<IEnumerable<LookupDto>> CCDeliveryModelsGetAllAsync() => await GetData("cc-delivery-models");
        public async Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync() => await GetData("cc-disadvantaged-areas");
        public async Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync() => await GetData("cc-governance");
        public async Task<IEnumerable<LookupDto>> CCGroupLeadsGetAllAsync() => await GetData("cc-group-leads");
        public async Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync() => await GetData("cc-operational-hours");
        public async Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync() => await GetData("cc-phase-types");
        public async Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync() => await GetData("childcare-facilities");
        public async Task<IEnumerable<LookupDto>> DiocesesGetAllAsync() => await GetData("dioceses");
        public async Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync() => await GetData("direct-provision-of-early-years");
        public async Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync() => await GetData("education-phases");
        public async Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync() => await GetData("establishment-link-types");
        public async Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync() => await GetData("further-education-types");
        public async Task<IEnumerable<LookupDto>> GendersGetAllAsync() => await GetData("genders");
        public async Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync() => await GetData("government-office-regions");
        public async Task<IEnumerable<LookupDto>> GSSLAGetAllAsync() => await GetData("gssla");
        public async Task<IEnumerable<LookupDto>> HeadTitlesGetAllAsync() => await GetData("head-titles");
        public async Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync() => await GetData("independent-school-types");
        public async Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync() => await GetData("inspectorate-names");
        public async Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync() => await GetData("inspectorates");
        public async Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync() => await GetData("local-governors");
        public async Task<IEnumerable<LookupDto>> LSOAsGetAllAsync() => await GetData("lsoas");
        public async Task<IEnumerable<LookupDto>> MSOAsGetAllAsync() => await GetData("msoas");
        public async Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync() => await GetData("nationalities");
        public async Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync() => await GetData("parliamentary-constituencies");
        public async Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync() => await GetData("provision-boarding");
        public async Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync() => await GetData("provision-nurseries");
        public async Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync() => await GetData("provision-official-sixth-forms");
        public async Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync() => await GetData("provision-special-classes");
        public async Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync() => await GetData("pruebds");
        public async Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync() => await GetData("pru-educated-by-others");
        public async Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync() => await GetData("pru-fulltime-provisions");
        public async Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync() => await GetData("prusens");
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync() => await GetData("reason-establishment-closed");
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync() => await GetData("reason-establishment-opened");
        public async Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync() => await GetData("religious-characters");
        public async Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync() => await GetData("religious-ethos");
        public async Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync() => await GetData("resourced-provisions");
        public async Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync() => await GetData("section41-approved");
        public async Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync() => await GetData("special-education-needs");
        public async Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync() => await GetData("teenage-mothers-provisions");
        public async Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync() => await GetData("type-of-resourced-provisions");
        public async Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync() => await GetData("urban-rural");
        private async Task<IEnumerable<LookupDto>> GetData(string name) => await _httpClient.GetAsync<List<LookupDto>>(ApiPrefix + name);

        #region Synchronous methods (deprecating)

        public IEnumerable<LookupDto> LocalAuthorityGetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> AccommodationChangedGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> AdministrativeDistrictsGetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> AdministrativeWardsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> AdmissionsPoliciesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> BoardingEstablishmentGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> CASWardsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> CCDeliveryModelsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> CCDisadvantagedAreasGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> CCGovernanceGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> CCGroupLeadsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> CCOperationalHoursGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> CCPhaseTypesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ChildcareFacilitiesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> DiocesesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> DirectProvisionOfEarlyYearsGetAll()
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
        
        public IEnumerable<LookupDto> EstablishmentLinkTypesGetAll()
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
        
        public IEnumerable<LookupDto> GendersGetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LookupDto> GovernmentOfficeRegionsGetAll()
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
        
        public IEnumerable<LookupDto> HeadTitlesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> IndependentSchoolTypesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> InspectorateNamesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> InspectoratesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> LocalGovernorsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> LSOAsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> MSOAsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> NationalitiesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ParliamentaryConstituenciesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ProvisionBoardingGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ProvisionNurseriesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ProvisionOfficialSixthFormsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ProvisionSpecialClassesGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> PRUEBDsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> PruEducatedByOthersGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> PruFulltimeProvisionsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> PRUSENsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ReasonEstablishmentClosedGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ReasonEstablishmentOpenedGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ReligiousCharactersGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ReligiousEthosGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> ResourcedProvisionsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> Section41ApprovedGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> SpecialEducationNeedsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> TeenageMothersProvisionsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> TypeOfResourcedProvisionsGetAll()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<LookupDto> UrbanRuralGetAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
