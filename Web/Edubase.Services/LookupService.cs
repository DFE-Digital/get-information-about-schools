using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public class LookupService : IDisposable
    {
        private Lazy<ApplicationDbContext> _dc = new Lazy<ApplicationDbContext>(ApplicationDbContext.Create);
        private ApplicationDbContext DataContext => _dc.Value;
        
        public LookupService() { }

        public async Task<IEnumerable<LookupDto>> LocalAuthorityGetAllAsync() => (await DataContext.LocalAuthorities.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> LocalAuthorityGetAll() => DataContext.LocalAuthorities.ToArray().Select(x => new LookupDto(x));
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
        public async Task<IEnumerable<LookupDto>> GovernorRolesGetAllAsync() => (await DataContext.LookupGovernorRoles.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> GovernorRolesGetAll() => DataContext.LookupGovernorRoles.ToArray().Select(x => new LookupDto(x));
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
        public async Task<IEnumerable<LookupDto>> CCPhaseTypesGetAllAsync() => (await DataContext.LookupCCPhaseTypes.ToArrayAsync()).Select(x => new LookupDto(x));
        public IEnumerable<LookupDto> CCPhaseTypesGetAll() => DataContext.LookupCCPhaseTypes.ToArray().Select(x => new LookupDto(x));
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



        public void Dispose()
        {
            if (_dc.IsValueCreated)
                DataContext.Dispose();
        }

        
    }
}
