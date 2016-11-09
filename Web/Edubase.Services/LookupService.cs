using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
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
        
        public LookupService()
        {

        }

        public async Task<List<LocalAuthority>> LocalAuthorityGetAllAsync() => await ApplicationDbContext.OperationAsync(dc => dc.LocalAuthorities.ToListAsync());
        public List<LocalAuthority> LocalAuthorityGetAll() => ApplicationDbContext.Operation(dc => dc.LocalAuthorities.ToList());

        public async Task<List<LookupAdmissionsPolicy>> AdmissionsPoliciesGetAllAsync() => await DataContext.LookupAdmissionsPolicies.ToListAsync();
        public List<LookupAdmissionsPolicy> AdmissionsPoliciesGetAll() => DataContext.LookupAdmissionsPolicies.ToList();

        public async Task<List<LookupEducationPhase>> EducationPhasesGetAllAsync() => await DataContext.LookupEducationPhases.ToListAsync();
        public List<LookupEducationPhase> EducationPhasesGetAll() => DataContext.LookupEducationPhases.ToList();

        public async Task<List<LookupEstablishmentStatus>> EstablishmentStatusesGetAllAsync() => await DataContext.LookupEstablishmentStatuses.ToListAsync();
        public List<LookupEstablishmentStatus> EstablishmentStatusesGetAll() => DataContext.LookupEstablishmentStatuses.ToList();

        public async Task<List<LookupEstablishmentType>> EstablishmentTypesGetAllAsync() => await DataContext.LookupEstablishmentTypes.ToListAsync();
        public List<LookupEstablishmentType> EstablishmentTypesGetAll() => DataContext.LookupEstablishmentTypes.ToList();

        public async Task<List<LookupGender>> GendersGetAllAsync() => await DataContext.LookupGenders.ToListAsync();
        public List<LookupGender> GendersGetAll() => DataContext.LookupGenders.ToList();

        public async Task<List<LookupGroupType>> GroupTypesGetAllAsync() => await DataContext.LookupGroupTypes.ToListAsync();
        public List<LookupGroupType> GroupTypesGetAll() => DataContext.LookupGroupTypes.ToList();

        public async Task<List<LookupHeadTitle>> HeadTitlesGetAllAsync() => await DataContext.LookupHeadTitles.ToListAsync();
        public List<LookupHeadTitle> HeadTitlesGetAll() => DataContext.LookupHeadTitles.ToList();

        public async Task<List<LookupProvisionBoarding>> ProvisionBoardingGetAllAsync() => await DataContext.LookupProvisionBoarding.ToListAsync();
        public List<LookupProvisionBoarding> ProvisionBoardingGetAll() => DataContext.LookupProvisionBoarding.ToList();

        public async Task<List<LookupProvisionNursery>> ProvisionNurseriesGetAllAsync() => await DataContext.LookupProvisionNurseries.ToListAsync();
        public List<LookupProvisionNursery> ProvisionNurseriesGetAll() => DataContext.LookupProvisionNurseries.ToList();

        public async Task<List<LookupProvisionOfficialSixthForm>> ProvisionOfficialSixthFormsGetAllAsync() => await DataContext.LookupProvisionOfficialSixthForms.ToListAsync();
        public List<LookupProvisionOfficialSixthForm> ProvisionOfficialSixthFormsGetAll() => DataContext.LookupProvisionOfficialSixthForms.ToList();

        public async Task<List<LookupProvisionSpecialClasses>> ProvisionSpecialClassesGetAllAsync() => await DataContext.LookupProvisionSpecialClasses.ToListAsync();
        public List<LookupProvisionSpecialClasses> ProvisionSpecialClassesGetAll() => DataContext.LookupProvisionSpecialClasses.ToList();

        public async Task<List<LookupReasonEstablishmentClosed>> ReasonEstablishmentClosedGetAllAsync() => await DataContext.LookupReasonEstablishmentClosed.ToListAsync();
        public List<LookupReasonEstablishmentClosed> ReasonEstablishmentClosedGetAll() => DataContext.LookupReasonEstablishmentClosed.ToList();

        public async Task<List<LookupReasonEstablishmentOpened>> ReasonEstablishmentOpenedGetAllAsync() => await DataContext.LookupReasonEstablishmentOpened.ToListAsync();
        public List<LookupReasonEstablishmentOpened> ReasonEstablishmentOpenedGetAll() => DataContext.LookupReasonEstablishmentOpened.ToList();

        public async Task<List<LookupReligiousCharacter>> ReligiousCharactersGetAllAsync() => await DataContext.LookupReligiousCharacters.ToListAsync();
        public List<LookupReligiousCharacter> ReligiousCharactersGetAll() => DataContext.LookupReligiousCharacters.ToList();

        public async Task<List<LookupReligiousEthos>> ReligiousEthosGetAllAsync() => await DataContext.LookupReligiousEthos.ToListAsync();
        public List<LookupReligiousEthos> ReligiousEthosGetAll() => DataContext.LookupReligiousEthos.ToList();

        public async Task<List<LookupGovernorRole>> GovernorRolesGetAllAsync() => await DataContext.LookupGovernorRoles.ToListAsync();
        public List<LookupGovernorRole> GovernorRolesGetAll() => DataContext.LookupGovernorRoles.ToList();

        public async Task<List<LookupGovernorAppointingBody>> GovernorAppointingBodiesGetAllAsync() => await DataContext.LookupGovernorAppointingBodies.ToListAsync();
        public List<LookupGovernorAppointingBody> GovernorAppointingBodiesGetAll() => DataContext.LookupGovernorAppointingBodies.ToList();

        public async Task<List<LookupAccommodationChanged>> AccommodationChangedGetAllAsync() => await DataContext.LookupAccommodationChanged.ToListAsync();
        public List<LookupAccommodationChanged> AccommodationChangedGetAll() => DataContext.LookupAccommodationChanged.ToList();

        public async Task<List<LookupBoardingEstablishment>> BoardingEstablishmentGetAllAsync() => await DataContext.LookupBoardingEstablishment.ToListAsync();
        public List<LookupBoardingEstablishment> BoardingEstablishmentGetAll() => DataContext.LookupBoardingEstablishment.ToList();

        public async Task<List<LookupCCGovernance>> CCGovernanceGetAllAsync() => await DataContext.LookupCCGovernance.ToListAsync();
        public List<LookupCCGovernance> CCGovernanceGetAll() => DataContext.LookupCCGovernance.ToList();

        public async Task<List<LookupCCOperationalHours>> CCOperationalHoursGetAllAsync() => await DataContext.LookupCCOperationalHours.ToListAsync();
        public List<LookupCCOperationalHours> CCOperationalHoursGetAll() => DataContext.LookupCCOperationalHours.ToList();

        public async Task<List<LookupCCPhaseType>> CCPhaseTypesGetAllAsync() => await DataContext.LookupCCPhaseTypes.ToListAsync();
        public List<LookupCCPhaseType> CCPhaseTypesGetAll() => DataContext.LookupCCPhaseTypes.ToList();

        public async Task<List<LookupDiocese>> DiocesesGetAllAsync() => await DataContext.LookupDioceses.ToListAsync();
        public List<LookupDiocese> DiocesesGetAll() => DataContext.LookupDioceses.ToList();

        public async Task<List<LookupChildcareFacilities>> ChildcareFacilitiesGetAllAsync() => await DataContext.LookupChildcareFacilities.ToListAsync();
        public List<LookupChildcareFacilities> ChildcareFacilitiesGetAll() => DataContext.LookupChildcareFacilities.ToList();

        public async Task<List<LookupDirectProvisionOfEarlyYears>> DirectProvisionOfEarlyYearsGetAllAsync() => await DataContext.LookupDirectProvisionOfEarlyYears.ToListAsync();
        public List<LookupDirectProvisionOfEarlyYears> DirectProvisionOfEarlyYearsGetAll() => DataContext.LookupDirectProvisionOfEarlyYears.ToList();

        public async Task<List<LookupFurtherEducationType>> FurtherEducationTypesGetAllAsync() => await DataContext.LookupFurtherEducationTypes.ToListAsync();
        public List<LookupFurtherEducationType> FurtherEducationTypesGetAll() => DataContext.LookupFurtherEducationTypes.ToList();

        public async Task<List<LookupIndependentSchoolType>> IndependentSchoolTypesGetAllAsync() => await DataContext.LookupIndependentSchoolTypes.ToListAsync();
        public List<LookupIndependentSchoolType> IndependentSchoolTypesGetAll() => DataContext.LookupIndependentSchoolTypes.ToList();

        public async Task<List<LookupInspectorate>> InspectoratesGetAllAsync() => await DataContext.LookupInspectorates.ToListAsync();
        public List<LookupInspectorate> InspectoratesGetAll() => DataContext.LookupInspectorates.ToList();

        public async Task<List<LookupInspectorateName>> InspectorateNamesGetAllAsync() => await DataContext.LookupInspectorateNames.ToListAsync();
        public List<LookupInspectorateName> InspectorateNamesGetAll() => DataContext.LookupInspectorateNames.ToList();

        public async Task<List<LookupLocalGovernors>> LocalGovernorsGetAllAsync() => await DataContext.LookupLocalGovernors.ToListAsync();
        public List<LookupLocalGovernors> LocalGovernorsGetAll() => DataContext.LookupLocalGovernors.ToList();

        public async Task<List<LookupNationality>> NationalitysGetAllAsync() => await DataContext.LookupNationalities.ToListAsync();
        public List<LookupNationality> NationalitysGetAll() => DataContext.LookupNationalities.ToList();

        public async Task<List<LookupPRUEBD>> PRUEBDsGetAllAsync() => await DataContext.LookupPRUEBDs.ToListAsync();
        public List<LookupPRUEBD> PRUEBDsGetAll() => DataContext.LookupPRUEBDs.ToList();

        public async Task<List<LookupPruEducatedByOthers>> PruEducatedByOtherssGetAllAsync() => await DataContext.LookupPruEducatedByOthers.ToListAsync();
        public List<LookupPruEducatedByOthers> PruEducatedByOtherssGetAll() => DataContext.LookupPruEducatedByOthers.ToList();

        public async Task<List<LookupPruFulltimeProvision>> PruFulltimeProvisionsGetAllAsync() => await DataContext.LookupPruFulltimeProvisions.ToListAsync();
        public List<LookupPruFulltimeProvision> PruFulltimeProvisionsGetAll() => DataContext.LookupPruFulltimeProvisions.ToList();

        public async Task<List<LookupPRUSEN>> PRUSENsGetAllAsync() => await DataContext.LookupPRUSENs.ToListAsync();
        public List<LookupPRUSEN> PRUSENsGetAll() => DataContext.LookupPRUSENs.ToList();

        public async Task<List<LookupResourcedProvision>> ResourcedProvisionsGetAllAsync() => await DataContext.LookupResourcedProvisions.ToListAsync();
        public List<LookupResourcedProvision> ResourcedProvisionsGetAll() => DataContext.LookupResourcedProvisions.ToList();

        public async Task<List<LookupSection41Approved>> Section41ApprovedsGetAllAsync() => await DataContext.LookupSection41Approved.ToListAsync();
        public List<LookupSection41Approved> Section41ApprovedsGetAll() => DataContext.LookupSection41Approved.ToList();

        public async Task<List<LookupSpecialEducationNeeds>> SpecialEducationNeedsGetAllAsync() => await DataContext.LookupSpecialEducationNeeds.ToListAsync();
        public List<LookupSpecialEducationNeeds> SpecialEducationNeedsGetAll() => DataContext.LookupSpecialEducationNeeds.ToList();

        public async Task<List<LookupTeenageMothersProvision>> TeenageMothersProvisionsGetAllAsync() => await DataContext.LookupTeenageMothersProvisions.ToListAsync();
        public List<LookupTeenageMothersProvision> TeenageMothersProvisionsGetAll() => DataContext.LookupTeenageMothersProvisions.ToList();

        public async Task<List<LookupTypeOfResourcedProvision>> TypeOfResourcedProvisionsGetAllAsync() => await DataContext.LookupTypeOfResourcedProvisions.ToListAsync();
        public List<LookupTypeOfResourcedProvision> TypeOfResourcedProvisionsGetAll() => DataContext.LookupTypeOfResourcedProvisions.ToList();
        
        public async Task<List<LookupEstablishmentLinkType>> EstablishmentLinkTypesGetAllAsync() => await DataContext.LookupEstablishmentLinkTypes.ToListAsync();
        public List<LookupEstablishmentLinkType> EstablishmentLinkTypesGetAll() => DataContext.LookupEstablishmentLinkTypes.ToList();



        public void Dispose()
        {
            if (_dc.IsValueCreated)
                DataContext.Dispose();
        }

        
    }
}
