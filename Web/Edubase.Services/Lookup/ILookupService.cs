using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services.Lookup
{
    public interface ILookupService
    {
        IEnumerable<LookupDto> AccommodationChangedGetAll();
        Task<IEnumerable<LookupDto>> AccommodationChangedGetAllAsync();
        IEnumerable<LookupDto> AdministrativeDistrictsGetAll();
        Task<IEnumerable<LookupDto>> AdministrativeDistrictsGetAllAsync();
        IEnumerable<LookupDto> AdministrativeWardsGetAll();
        Task<IEnumerable<LookupDto>> AdministrativeWardsGetAllAsync();
        IEnumerable<LookupDto> AdmissionsPoliciesGetAll();
        Task<IEnumerable<LookupDto>> AdmissionsPoliciesGetAllAsync();
        IEnumerable<LookupDto> BoardingEstablishmentGetAll();
        Task<IEnumerable<LookupDto>> BoardingEstablishmentGetAllAsync();
        IEnumerable<LookupDto> CASWardsGetAll();
        Task<IEnumerable<LookupDto>> CASWardsGetAllAsync();
        IEnumerable<LookupDto> CCDeliveryModelsGetAll();
        Task<IEnumerable<LookupDto>> CCDeliveryModelsGetAllAsync();
        IEnumerable<LookupDto> CCDisadvantagedAreasGetAll();
        Task<IEnumerable<LookupDto>> CCDisadvantagedAreasGetAllAsync();
        IEnumerable<LookupDto> CCGovernanceGetAll();
        Task<IEnumerable<LookupDto>> CCGovernanceGetAllAsync();
        IEnumerable<LookupDto> CCGroupLeadsGetAll();
        Task<IEnumerable<LookupDto>> CCGroupLeadsGetAllAsync();
        IEnumerable<LookupDto> CCOperationalHoursGetAll();
        Task<IEnumerable<LookupDto>> CCOperationalHoursGetAllAsync();
        IEnumerable<LookupDto> CCPhaseTypesGetAll();
        Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync();
        IEnumerable<LookupDto> ChildcareFacilitiesGetAll();
        Task<IEnumerable<LookupDto>> ChildcareFacilitiesGetAllAsync();
        IEnumerable<LookupDto> DiocesesGetAll();
        Task<IEnumerable<LookupDto>> DiocesesGetAllAsync();
        IEnumerable<LookupDto> DirectProvisionOfEarlyYearsGetAll();
        Task<IEnumerable<LookupDto>> DirectProvisionOfEarlyYearsGetAllAsync();
        void Dispose();
        IEnumerable<LookupDto> EducationPhasesGetAll();
        Task<IEnumerable<LookupDto>> EducationPhasesGetAllAsync();
        IEnumerable<LookupDto> EstablishmentAccreditedGetAll();
        Task<IEnumerable<LookupDto>> EstablishmentAccreditedGetAllAsync();
        IEnumerable<LookupDto> EstablishmentLinkTypesGetAll();
        Task<IEnumerable<LookupDto>> EstablishmentLinkTypesGetAllAsync();
        IEnumerable<LookupDto> EstablishmentStatusesGetAll();
        Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync();
        IEnumerable<LookupDto> EstablishmentTypeGroupsGetAll();
        Task<IEnumerable<LookupDto>> EstablishmentTypeGroupsGetAllAsync();
        IEnumerable<EstablishmentLookupDto> EstablishmentTypesGetAll();
        Task<IEnumerable<EstablishmentLookupDto>> EstablishmentTypesGetAllAsync();
        IEnumerable<LookupDto> FurtherEducationTypesGetAll();
        Task<IEnumerable<LookupDto>> FurtherEducationTypesGetAllAsync();
        IEnumerable<LookupDto> GendersGetAll();
        Task<IEnumerable<LookupDto>> GendersGetAllAsync();
        IEnumerable<LookupDto> GovernmentOfficeRegionsGetAll();
        Task<IEnumerable<LookupDto>> GovernmentOfficeRegionsGetAllAsync();
        IEnumerable<LookupDto> GovernorAppointingBodiesGetAll();
        Task<IEnumerable<LookupDto>> GovernorAppointingBodiesGetAllAsync();
        IEnumerable<LookupDto> GovernorRolesGetAll();
        Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync();
        IEnumerable<LookupDto> GroupStatusesGetAll();
        Task<IEnumerable<LookupDto>> GroupStatusesGetAllAsync();
        IEnumerable<LookupDto> GroupTypesGetAll();
        Task<IEnumerable<LookupDto>> GroupTypesGetAllAsync();
        IEnumerable<LookupDto> GSSLAGetAll();
        Task<IEnumerable<LookupDto>> GSSLAGetAllAsync();
        IEnumerable<LookupDto> HeadTitlesGetAll();
        Task<IEnumerable<LookupDto>> TitlesGetAllAsync();
        IEnumerable<LookupDto> IndependentSchoolTypesGetAll();
        Task<IEnumerable<LookupDto>> IndependentSchoolTypesGetAllAsync();
        IEnumerable<LookupDto> InspectorateNamesGetAll();
        Task<IEnumerable<LookupDto>> InspectorateNamesGetAllAsync();
        IEnumerable<LookupDto> InspectoratesGetAll();
        Task<IEnumerable<LookupDto>> InspectoratesGetAllAsync();
        IEnumerable<LookupDto> LocalAuthorityGetAll();
        Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync();
        IEnumerable<LookupDto> LocalGovernorsGetAll();
        Task<IEnumerable<LookupDto>> LocalGovernorsGetAllAsync();
        IEnumerable<LookupDto> LSOAsGetAll();
        Task<IEnumerable<LookupDto>> LSOAsGetAllAsync();
        IEnumerable<LookupDto> MSOAsGetAll();
        Task<IEnumerable<LookupDto>> MSOAsGetAllAsync();
        IEnumerable<LookupDto> NationalitiesGetAll();
        Task<IEnumerable<LookupDto>> NationalitiesGetAllAsync();
        IEnumerable<LookupDto> ParliamentaryConstituenciesGetAll();
        Task<IEnumerable<LookupDto>> ParliamentaryConstituenciesGetAllAsync();
        IEnumerable<LookupDto> ProvisionBoardingGetAll();
        Task<IEnumerable<LookupDto>> ProvisionBoardingGetAllAsync();
        IEnumerable<LookupDto> ProvisionNurseriesGetAll();
        Task<IEnumerable<LookupDto>> ProvisionNurseriesGetAllAsync();
        IEnumerable<LookupDto> ProvisionOfficialSixthFormsGetAll();
        Task<IEnumerable<LookupDto>> ProvisionOfficialSixthFormsGetAllAsync();
        IEnumerable<LookupDto> ProvisionSpecialClassesGetAll();
        Task<IEnumerable<LookupDto>> ProvisionSpecialClassesGetAllAsync();
        IEnumerable<LookupDto> PRUEBDsGetAll();
        Task<IEnumerable<LookupDto>> PRUEBDsGetAllAsync();
        IEnumerable<LookupDto> PruEducatedByOthersGetAll();
        Task<IEnumerable<LookupDto>> PruEducatedByOthersGetAllAsync();
        IEnumerable<LookupDto> PruFulltimeProvisionsGetAll();
        Task<IEnumerable<LookupDto>> PruFulltimeProvisionsGetAllAsync();
        IEnumerable<LookupDto> PRUSENsGetAll();
        Task<IEnumerable<LookupDto>> PRUSENsGetAllAsync();
        IEnumerable<LookupDto> QualityAssuranceBodyNameGetAll();
        Task<IEnumerable<LookupDto>> QualityAssuranceBodyNameGetAllAsync();
        IEnumerable<LookupDto> ReasonEstablishmentClosedGetAll();
        Task<IEnumerable<LookupDto>> ReasonEstablishmentClosedGetAllAsync();
        IEnumerable<LookupDto> ReasonEstablishmentOpenedGetAll();
        Task<IEnumerable<LookupDto>> ReasonEstablishmentOpenedGetAllAsync();
        IEnumerable<LookupDto> ReligiousCharactersGetAll();
        Task<IEnumerable<LookupDto>> ReligiousCharactersGetAllAsync();
        IEnumerable<LookupDto> ReligiousEthosGetAll();
        Task<IEnumerable<LookupDto>> ReligiousEthosGetAllAsync();
        IEnumerable<LookupDto> ResourcedProvisionsGetAll();
        Task<IEnumerable<LookupDto>> ResourcedProvisionsGetAllAsync();
        IEnumerable<LookupDto> Section41ApprovedGetAll();
        Task<IEnumerable<LookupDto>> Section41ApprovedGetAllAsync();
        IEnumerable<LookupDto> SpecialEducationNeedsGetAll();
        Task<IEnumerable<LookupDto>> SpecialEducationNeedsGetAllAsync();
        IEnumerable<LookupDto> TeenageMothersProvisionsGetAll();
        Task<IEnumerable<LookupDto>> TeenageMothersProvisionsGetAllAsync();
        IEnumerable<LookupDto> TypeOfResourcedProvisionsGetAll();
        Task<IEnumerable<LookupDto>> TypeOfResourcedProvisionsGetAllAsync();
        IEnumerable<LookupDto> UrbanRuralGetAll();
        Task<IEnumerable<LookupDto>> UrbanRuralGetAllAsync();
        Task<IEnumerable<LookupDto>> CountiesGetAllAsync();
        Task<IEnumerable<LookupDto>> OfstedRatingsGetAllAsync();
        Task<IEnumerable<LookupDto>> RscRegionsGetAllAsync();
    }
}
