using Edubase.Services.Domain;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Lookup
{
    public class LookupApiService : ILookupService
    {
        private const string ApiPrefix = "lookup/";
        private readonly IHttpClientWrapper _httpClient;
        private readonly ISecurityService _securityService;

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
            {45,  new List<int> {11}}
        };

        public LookupApiService(IHttpClientWrapper httpClient, ISecurityService securityService)
        {
            _securityService = securityService;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync() => await GetData("local-authorities");
        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync() => await GetData("governor-roles");
        public async Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync() => await GetData("governor-appointing-bodies");
        public async Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync() => await GetData("group-types");
        public async Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync() => await GetData("establishment-type-groups");
        public async Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync() => await GetData("establishment-statuses");
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
        public async Task<IEnumerable<LookupDto>> EstablishmentAccreditedGetAllAsync() => await GetData("establishment-accredited");
        public async Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync() => await GetData("education-phases");
        public async Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync() => await GetData("establishment-link-types");
        public async Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync() => await GetData("further-education-types");
        public async Task<IEnumerable<LookupDto>> GendersGetAllAsync() => await GetData("genders");
        public async Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync() => await GetData("government-office-regions");
        public async Task<IEnumerable<LookupDto>> GSSLAGetAllAsync() => await GetData("gssla");
        public async Task<IEnumerable<LookupDto>> TitlesGetAllAsync() => await GetData("titles");
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
        public async Task<IEnumerable<LookupDto>> QualityAssuranceBodyNameGetAllAsync() => await GetData("quality-assurance-body-name");
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
        public async Task<IEnumerable<LookupDto>> CountiesGetAllAsync() => await GetData("counties");
        public async Task<IEnumerable<LookupDto>> OfstedRatingsGetAllAsync() => await GetData("ofsted-ratings");
        public async Task<IEnumerable<LookupDto>> RscRegionsGetAllAsync() => await GetData("rsc-regions");
        private async Task<IEnumerable<LookupDto>> GetData(string name) => (await _httpClient.GetAsync<List<LookupDto>>(ApiPrefix + name, _securityService.CreateAnonymousPrincipal())).GetResponse();

        // #region Synchronous methods (deprecating)
        //
        // public IEnumerable<LookupDto> LocalAuthorityGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> AccommodationChangedGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> AdministrativeDistrictsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> AdministrativeWardsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> AdmissionsPoliciesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> BoardingEstablishmentGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> CASWardsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> CCDeliveryModelsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> CCDisadvantagedAreasGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> CCGovernanceGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> CCGroupLeadsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> CCOperationalHoursGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> CCPhaseTypesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ChildcareFacilitiesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> DiocesesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> DirectProvisionOfEarlyYearsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void Dispose()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> EducationPhasesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> EstablishmentAccreditedGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> EstablishmentLinkTypesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> EstablishmentStatusesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> EstablishmentTypeGroupsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<EstablishmentLookupDto> EstablishmentTypesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> FurtherEducationTypesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> GendersGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> GovernmentOfficeRegionsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> GovernorAppointingBodiesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> GovernorRolesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> GroupStatusesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> GroupTypesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> GSSLAGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> HeadTitlesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> IndependentSchoolTypesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> InspectorateNamesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> InspectoratesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> LocalGovernorsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> LSOAsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> MSOAsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> NationalitiesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ParliamentaryConstituenciesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ProvisionBoardingGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ProvisionNurseriesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ProvisionOfficialSixthFormsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ProvisionSpecialClassesGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> PRUEBDsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> PruEducatedByOthersGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> PruFulltimeProvisionsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> PRUSENsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> QualityAssuranceBodyNameGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        // public IEnumerable<LookupDto> ReasonEstablishmentClosedGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ReasonEstablishmentOpenedGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ReligiousCharactersGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ReligiousEthosGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> ResourcedProvisionsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> Section41ApprovedGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> SpecialEducationNeedsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> TeenageMothersProvisionsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> TypeOfResourcedProvisionsGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public IEnumerable<LookupDto> UrbanRuralGetAll()
        // {
        //     throw new NotImplementedException();
        // }
        //
        //
        // #endregion
    }
}
