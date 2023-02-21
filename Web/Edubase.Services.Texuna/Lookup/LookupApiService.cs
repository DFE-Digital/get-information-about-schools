using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Lookup;
using Edubase.Services.Security;

namespace Edubase.Services.Texuna.Lookup
{
    public class LookupApiService : ILookupService
    {
        private const string ApiPrefix = "lookup/";
        private readonly IHttpClientWrapper _httpClient;
        private readonly ISecurityService _securityService;

        //Mapping EstablishmentType.id to EstablishmentTypeGroup.code
        private readonly Dictionary<int, List<int>> establishmentTypeToGroup = new Dictionary<int, List<int>>
        {
            {39, new List<int> {2}},
            {40, new List<int> {2}},
            {36, new List<int> {2}},
            {37, new List<int> {2}},
            {28, new List<int> {2}},
            {38, new List<int> {2, 8}},
            {27, new List<int> {2, 8}},
            {22, new List<int> {2}},
            {31, new List<int> {10}},
            {41, new List<int> {4}},
            {42, new List<int> {4}},
            {5,  new List<int> {6}},
            {1,  new List<int> {7}},
            {6,  new List<int> {7, 8}},
            {14, new List<int> {10}},
            {16, new List<int> {10}},
            {4,  new List<int> {7}},
            {11, new List<int> {7, 8}},
            {29, new List<int> {3}},
            {33, new List<int> {3}},
            {32, new List<int> {3}},
            {30, new List<int> {3, 8}},
            {15, new List<int> {1}},
            {23, new List<int> {5}},
            {8,  new List<int> {8}},
            {43, new List<int> {10}},
            {13, new List<int> {7}},
            {44, new List<int> {10}},
            {21, new List<int> {10}},
            {7,  new List<int> {8}},
            {19, new List<int> {10}},
            {10, new List<int> {6}},
            {9,  new List<int> {8}},
            {24, new List<int> {9}},
            {17, new List<int> {10}},
            {12, new List<int> {7}},
            {18, new List<int> {10}},
            {20, new List<int> {10}},
            {25, new List<int> {1}},
            {26, new List<int> {10}},
            {35, new List<int> {3}},
            {34, new List<int> {3}},
            {2,  new List<int> {7}},
            {3,  new List<int> {7}},
            {45,  new List<int> {11}},
            {46, new List<int> {10}}
        };

        public LookupApiService(IHttpClientWrapper httpClient, ISecurityService securityService)
        {
            _securityService = securityService;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync()
        {
            return await GetData("local-authorities");
        }

        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync()
        {
            return await GetData("governor-roles");
        }

        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync()
        {
            return await GetData("governor-appointing-bodies");
        }

        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync()
        {
            return await GetData("group-types");
        }

        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync()
        {
            return await GetData("establishment-type-groups");
        }

        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync()
        {
            return await GetData("establishment-statuses");
        }

        public async Task<IEnumerable<EstablishmentLookupDto>> EstablishmentTypesGetAllAsync()
        {
            var establishmentTypes =
                (await _httpClient.GetAsync<List<LookupDto>>($"{ApiPrefix}establishment-types",
                    _securityService.CreateAnonymousPrincipal())).Response;
            return establishmentTypes.Join(establishmentTypeToGroup, e => e.Id, et => et.Key,
                (e, et) => new EstablishmentLookupDto
                {
                    Code = e.Code,
                    Id = e.Id,
                    Name = e.Name,
                    DisplayOrder = e.DisplayOrder,
                    GroupIds = et.Value
                });
        }

        public async Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync()
        {
            return await GetData("group-statuses");
        }

        public async Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync()
        {
            return await GetData("accommodation-changed");
        }

        public async Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync()
        {
            return await GetData("administrative-districts");
        }

        public async Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync()
        {
            return await GetData("administrative-wards");
        }

        public async Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync()
        {
            return await GetData("admissions-policies");
        }

        public async Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync()
        {
            return await GetData("boarding-establishment");
        }

        public async Task<IEnumerable<LookupDto>> CCDeliveryModelsGetAllAsync()
        {
            return await GetData("cc-delivery-models");
        }

        public async Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync()
        {
            return await GetData("cc-disadvantaged-areas");
        }

        public async Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync()
        {
            return await GetData("cc-governance");
        }

        public async Task<IEnumerable<LookupDto>> CCGroupLeadsGetAllAsync()
        {
            return await GetData("cc-group-leads");
        }

        public async Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync()
        {
            return await GetData("cc-operational-hours");
        }

        public async Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync()
        {
            return await GetData("cc-phase-types");
        }

        public async Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync()
        {
            return await GetData("childcare-facilities");
        }

        public async Task<IEnumerable<LookupDto>> DiocesesGetAllAsync()
        {
            return await GetData("dioceses");
        }

        public async Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync()
        {
            return await GetData("direct-provision-of-early-years");
        }

        public async Task<IEnumerable<LookupDto>> EstablishmentAccreditedGetAllAsync()
        {
            return await GetData("establishment-accredited");
        }

        public async Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync()
        {
            return await GetData("education-phases");
        }

        public async Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync()
        {
            return await GetData("establishment-link-types");
        }

        public async Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync()
        {
            return await GetData("further-education-types");
        }

        public async Task<IEnumerable<LookupDto>> GendersGetAllAsync()
        {
            return await GetData("genders");
        }

        public async Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync()
        {
            return await GetData("government-office-regions");
        }

        public async Task<IEnumerable<LookupDto>> GSSLAGetAllAsync()
        {
            return await GetData("gssla");
        }

        public async Task<IEnumerable<LookupDto>> TitlesGetAllAsync()
        {
            return await GetData("titles");
        }

        public async Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync()
        {
            return await GetData("independent-school-types");
        }

        public async Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync()
        {
            return await GetData("inspectorate-names");
        }

        public async Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync()
        {
            return await GetData("inspectorates");
        }

        public async Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync()
        {
            return await GetData("local-governors");
        }

        public async Task<IEnumerable<LookupDto>> LSOAsGetAllAsync()
        {
            return await GetData("lsoas");
        }

        public async Task<IEnumerable<LookupDto>> MSOAsGetAllAsync()
        {
            return await GetData("msoas");
        }

        public async Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync()
        {
            return await GetData("nationalities");
        }

        public async Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync()
        {
            return await GetData("parliamentary-constituencies");
        }

        public async Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync()
        {
            return await GetData("provision-boarding");
        }

        public async Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync()
        {
            return await GetData("provision-nurseries");
        }

        public async Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync()
        {
            return await GetData("provision-official-sixth-forms");
        }

        public async Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync()
        {
            return await GetData("provision-special-classes");
        }

        public async Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync()
        {
            return await GetData("pruebds");
        }

        public async Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync()
        {
            return await GetData("pru-educated-by-others");
        }

        public async Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync()
        {
            return await GetData("pru-fulltime-provisions");
        }

        public async Task<IEnumerable<LookupDto>> QualityAssuranceBodyNameGetAllAsync()
        {
            return await GetData("quality-assurance-body-name");
        }

        public async Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync()
        {
            return await GetData("prusens");
        }

        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync()
        {
            return await GetData("reason-establishment-closed");
        }

        public async Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync()
        {
            return await GetData("reason-establishment-opened");
        }

        public async Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync()
        {
            return await GetData("religious-characters");
        }

        public async Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync()
        {
            return await GetData("religious-ethos");
        }

        public async Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync()
        {
            return await GetData("resourced-provisions");
        }

        public async Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync()
        {
            return await GetData("section41-approved");
        }

        public async Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync()
        {
            return await GetData("special-education-needs");
        }

        public async Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync()
        {
            return await GetData("teenage-mothers-provisions");
        }

        public async Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync()
        {
            return await GetData("type-of-resourced-provisions");
        }

        public async Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync()
        {
            return await GetData("urban-rural");
        }

        public async Task<IEnumerable<LookupDto>> CountiesGetAllAsync()
        {
            return await GetData("counties");
        }

        public async Task<IEnumerable<LookupDto>> OfstedRatingsGetAllAsync()
        {
            return await GetData("ofsted-ratings");
        }

        public async Task<IEnumerable<LookupDto>> RscRegionsGetAllAsync()
        {
            return await GetData("rsc-regions");
        }

        public async Task<IEnumerable<LookupDto>> CASWardsGetAllAsync()
        {
            return await GetData("cas-wards");
        }

        private async Task<IEnumerable<LookupDto>> GetData(string name)
        {
            return (await _httpClient.GetAsync<List<LookupDto>>(ApiPrefix + name,
                _securityService.CreateAnonymousPrincipal())).GetResponse();
        }
    }
}
