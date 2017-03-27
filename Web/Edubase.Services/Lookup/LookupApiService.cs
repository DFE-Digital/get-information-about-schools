using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Edubase.Data.DbContext;

namespace Edubase.Services.Lookup
{
    public class LookupApiService : IDisposable, ILookupService
    {
        private const string ApiLocalAuthorityLookupPath = "/lookup/local-authorities";
        private const string ApiGovernorRolesLookupPath = "/lookup/governor-roles";

        private Lazy<ApplicationDbContext> _dc = new Lazy<ApplicationDbContext>(ApplicationDbContext.Create);
        private ApplicationDbContext DataContext => _dc.Value;
        private HttpClient _httpClient;

        public LookupApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync()
        {
            var result = await _httpClient.GetAsync(ApiLocalAuthorityLookupPath);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsAsync<List<LookupDto>>();
        }

        public IEnumerable<LookupDto> LocalAuthorityGetAll()
        {
            var result = _httpClient.GetAsync(ApiLocalAuthorityLookupPath).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<List<LookupDto>>().Result;
        }

        public async Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync() => (await DataContext.LookupAdmissionsPolicies.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> AdmissionsPoliciesGetAll() => DataContext.LookupAdmissionsPolicies.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync() => (await DataContext.LookupEducationPhases.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> EducationPhasesGetAll() => DataContext.LookupEducationPhases.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync() => (await DataContext.LookupEstablishmentStatuses.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> EstablishmentStatusesGetAll() => DataContext.LookupEstablishmentStatuses.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> EstablishmentTypesGetAllAsync() => (await DataContext.LookupEstablishmentTypes.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> EstablishmentTypesGetAll() => DataContext.LookupEstablishmentTypes.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> GendersGetAllAsync() => (await DataContext.LookupGenders.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> GendersGetAll() => DataContext.LookupGenders.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync() => (await DataContext.LookupGroupTypes.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> GroupTypesGetAll() => DataContext.LookupGroupTypes.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> HeadTitlesGetAllAsync() => (await DataContext.LookupHeadTitles.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> HeadTitlesGetAll() => DataContext.LookupHeadTitles.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync() => (await DataContext.LookupProvisionBoarding.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ProvisionBoardingGetAll() => DataContext.LookupProvisionBoarding.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync() => (await DataContext.LookupProvisionNurseries.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ProvisionNurseriesGetAll() => DataContext.LookupProvisionNurseries.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync() => (await DataContext.LookupProvisionOfficialSixthForms.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ProvisionOfficialSixthFormsGetAll() => DataContext.LookupProvisionOfficialSixthForms.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync() => (await DataContext.LookupProvisionSpecialClasses.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ProvisionSpecialClassesGetAll() => DataContext.LookupProvisionSpecialClasses.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync() => (await DataContext.LookupReasonEstablishmentClosed.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ReasonEstablishmentClosedGetAll() => DataContext.LookupReasonEstablishmentClosed.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync() => (await DataContext.LookupReasonEstablishmentOpened.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ReasonEstablishmentOpenedGetAll() => DataContext.LookupReasonEstablishmentOpened.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync() => (await DataContext.LookupReligiousCharacters.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ReligiousCharactersGetAll() => DataContext.LookupReligiousCharacters.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync() => (await DataContext.LookupReligiousEthos.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ReligiousEthosGetAll() => DataContext.LookupReligiousEthos.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync()
        {
            var result = await _httpClient.GetAsync(ApiGovernorRolesLookupPath);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsAsync<List<LookupDto>>();
        }

        public IEnumerable<LookupDto> GovernorRolesGetAll()
        {
            var result = _httpClient.GetAsync(ApiGovernorRolesLookupPath).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<List<LookupDto>>().Result;
        }

        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync() => (await DataContext.LookupGovernorAppointingBodies.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> GovernorAppointingBodiesGetAll() => DataContext.LookupGovernorAppointingBodies.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync() => (await DataContext.LookupAccommodationChanged.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> AccommodationChangedGetAll() => DataContext.LookupAccommodationChanged.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync() => (await DataContext.LookupBoardingEstablishment.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> BoardingEstablishmentGetAll() => DataContext.LookupBoardingEstablishment.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync() => (await DataContext.LookupCCGovernance.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CCGovernanceGetAll() => DataContext.LookupCCGovernance.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync() => (await DataContext.LookupCCOperationalHours.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CCOperationalHoursGetAll() => DataContext.LookupCCOperationalHours.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync() => (await DataContext.LookupCCDisadvantagedAreas.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CCDisadvantagedAreasGetAll() => DataContext.LookupCCDisadvantagedAreas.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync() => (await DataContext.LookupCCPhaseTypes.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CCPhaseTypesGetAll() => DataContext.LookupCCPhaseTypes.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> CCDeliveryModelsGetAllAsync() => (await DataContext.LookupCCDeliveryModels.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CCDeliveryModelsGetAll() => DataContext.LookupCCDeliveryModels.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> CCGroupLeadsGetAllAsync() => (await DataContext.LookupCCGroupLeads.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CCGroupLeadsGetAll() => DataContext.LookupCCGroupLeads.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> DiocesesGetAllAsync() => (await DataContext.LookupDioceses.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> DiocesesGetAll() => DataContext.LookupDioceses.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync() => (await DataContext.LookupChildcareFacilities.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ChildcareFacilitiesGetAll() => DataContext.LookupChildcareFacilities.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync() => (await DataContext.LookupDirectProvisionOfEarlyYears.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> DirectProvisionOfEarlyYearsGetAll() => DataContext.LookupDirectProvisionOfEarlyYears.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync() => (await DataContext.LookupFurtherEducationTypes.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> FurtherEducationTypesGetAll() => DataContext.LookupFurtherEducationTypes.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync() => (await DataContext.LookupIndependentSchoolTypes.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> IndependentSchoolTypesGetAll() => DataContext.LookupIndependentSchoolTypes.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync() => (await DataContext.LookupInspectorates.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> InspectoratesGetAll() => DataContext.LookupInspectorates.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync() => (await DataContext.LookupInspectorateNames.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> InspectorateNamesGetAll() => DataContext.LookupInspectorateNames.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync() => (await DataContext.LookupLocalGovernors.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> LocalGovernorsGetAll() => DataContext.LookupLocalGovernors.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync() => (await DataContext.LookupNationalities.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> NationalitiesGetAll() => DataContext.LookupNationalities.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync() => (await DataContext.LookupPRUEBDs.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> PRUEBDsGetAll() => DataContext.LookupPRUEBDs.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync() => (await DataContext.LookupPruEducatedByOthers.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> PruEducatedByOthersGetAll() => DataContext.LookupPruEducatedByOthers.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync() => (await DataContext.LookupPruFulltimeProvisions.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> PruFulltimeProvisionsGetAll() => DataContext.LookupPruFulltimeProvisions.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync() => (await DataContext.LookupPRUSENs.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> PRUSENsGetAll() => DataContext.LookupPRUSENs.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync() => (await DataContext.LookupResourcedProvisions.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ResourcedProvisionsGetAll() => DataContext.LookupResourcedProvisions.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync() => (await DataContext.LookupSection41Approved.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> Section41ApprovedGetAll() => DataContext.LookupSection41Approved.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync() => (await DataContext.LookupSpecialEducationNeeds.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> SpecialEducationNeedsGetAll() => DataContext.LookupSpecialEducationNeeds.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync() => (await DataContext.LookupTeenageMothersProvisions.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> TeenageMothersProvisionsGetAll() => DataContext.LookupTeenageMothersProvisions.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync() => (await DataContext.LookupTypeOfResourcedProvisions.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> TypeOfResourcedProvisionsGetAll() => DataContext.LookupTypeOfResourcedProvisions.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync() => (await DataContext.LookupEstablishmentLinkTypes.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> EstablishmentLinkTypesGetAll() => DataContext.LookupEstablishmentLinkTypes.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync() => (await DataContext.LookupEstablishmentTypeGroups.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> EstablishmentTypeGroupsGetAll() => DataContext.LookupEstablishmentTypeGroups.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync() => (await DataContext.LookupGovernmentOfficeRegions.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> GovernmentOfficeRegionsGetAll() => DataContext.LookupGovernmentOfficeRegions.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync() => (await DataContext.LookupAdministrativeDistricts.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> AdministrativeDistrictsGetAll() => DataContext.LookupAdministrativeDistricts.ToArray().Select(x => new LookupDto(x));
        public async Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync() => (await DataContext.LookupAdministrativeDistricts.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> AdministrativeWardsGetAll() => DataContext.LookupAdministrativeDistricts.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync() => (await DataContext.LookupParliamentaryConstituencies.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> ParliamentaryConstituenciesGetAll() => DataContext.LookupParliamentaryConstituencies.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync() => (await DataContext.LookupUrbanRural.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> UrbanRuralGetAll() => DataContext.LookupUrbanRural.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> GSSLAGetAllAsync() => (await DataContext.LookupGSSLA.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> GSSLAGetAll() => DataContext.LookupGSSLA.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> CASWardsGetAllAsync() => (await DataContext.LookupCASWards.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CASWardsGetAll() => DataContext.LookupCASWards.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> MSOAsGetAllAsync() => (await DataContext.LookupMSOAs.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> MSOAsGetAll() => DataContext.LookupMSOAs.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> LSOAsGetAllAsync() => (await DataContext.LookupLSOAs.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> LSOAsGetAll() => DataContext.LookupLSOAs.ToArray().Select(x => new LookupDto(x));

        public async Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync() => (await DataContext.LookupGroupStatuses.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> GroupStatusesGetAll() => DataContext.LookupGroupStatuses.ToArray().Select(x => new LookupDto(x));


        public void Dispose()
        {
            if (_dc.IsValueCreated)
                DataContext.Dispose();
        }

        
    }
}
