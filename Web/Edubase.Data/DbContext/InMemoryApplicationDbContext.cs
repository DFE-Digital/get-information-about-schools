using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Edubase.Data.DbContext
{
    public class InMemoryApplicationDbContext : IApplicationDbContext, IInMemoryApplicationDbContext
    {
        public DbSet<EstablishmentApprovalQueue> EstablishmentApprovalQueue { get; set; } = new InMemoryDbSet<EstablishmentApprovalQueue>();
        public DbSet<EstablishmentChangeHistory> EstablishmentChangeHistories { get; set; } = new InMemoryDbSet<EstablishmentChangeHistory>();
        public DbSet<EstablishmentLink> EstablishmentLinks { get; set; } = new InMemoryDbSet<EstablishmentLink>();
        public DbSet<Establishment> Establishments { get; set; } = new InMemoryDbSet<Establishment>();
        public DbSet<EstablishmentGroup> EstablishmentGroups { get; set; } = new InMemoryDbSet<EstablishmentGroup>();
        public DbSet<Governor> Governors { get; set; } = new InMemoryDbSet<Governor>();
        public DbSet<GroupCollection> Groups { get; set; } = new InMemoryDbSet<GroupCollection>();
        public DbSet<LocalAuthority> LocalAuthorities { get; set; } = new InMemoryDbSet<LocalAuthority>();
        public DbSet<LookupAccommodationChanged> LookupAccommodationChanged { get; set; } = new InMemoryDbSet<LookupAccommodationChanged>();
        public DbSet<LookupDistrictAdministrative> LookupAdministrativeDistricts { get; set; } = new InMemoryDbSet<LookupDistrictAdministrative>();
        public DbSet<LookupAdministrativeWard> LookupAdministrativeWards { get; set; } = new InMemoryDbSet<LookupAdministrativeWard>();
        public DbSet<LookupAdmissionsPolicy> LookupAdmissionsPolicies { get; set; } = new InMemoryDbSet<LookupAdmissionsPolicy>();
        public DbSet<LookupBoardingEstablishment> LookupBoardingEstablishment { get; set; } = new InMemoryDbSet<LookupBoardingEstablishment>();
        public DbSet<LookupCASWard> LookupCASWards { get; set; } = new InMemoryDbSet<LookupCASWard>();
        public DbSet<LookupCCGovernance> LookupCCGovernance { get; set; } = new InMemoryDbSet<LookupCCGovernance>();
        public DbSet<LookupCCOperationalHours> LookupCCOperationalHours { get; set; } = new InMemoryDbSet<LookupCCOperationalHours>();
        public DbSet<LookupCCPhaseType> LookupCCPhaseTypes { get; set; } = new InMemoryDbSet<LookupCCPhaseType>();
        public DbSet<LookupChildcareFacilities> LookupChildcareFacilities { get; set; } = new InMemoryDbSet<LookupChildcareFacilities>();
        public DbSet<LookupDiocese> LookupDioceses { get; set; } = new InMemoryDbSet<LookupDiocese>();
        public DbSet<LookupDirectProvisionOfEarlyYears> LookupDirectProvisionOfEarlyYears { get; set; } = new InMemoryDbSet<LookupDirectProvisionOfEarlyYears>();
        public DbSet<LookupEducationPhase> LookupEducationPhases { get; set; } = new InMemoryDbSet<LookupEducationPhase>();
        public DbSet<LookupEstablishmentLinkType> LookupEstablishmentLinkTypes { get; set; } = new InMemoryDbSet<LookupEstablishmentLinkType>();
        public DbSet<LookupEstablishmentStatus> LookupEstablishmentStatuses { get; set; } = new InMemoryDbSet<LookupEstablishmentStatus>();
        public DbSet<LookupEstablishmentTypeGroup> LookupEstablishmentTypeGroups { get; set; } = new InMemoryDbSet<LookupEstablishmentTypeGroup>();
        public DbSet<LookupEstablishmentType> LookupEstablishmentTypes { get; set; } = new InMemoryDbSet<LookupEstablishmentType>();
        public DbSet<LookupFurtherEducationType> LookupFurtherEducationTypes { get; set; } = new InMemoryDbSet<LookupFurtherEducationType>();
        public DbSet<LookupGender> LookupGenders { get; set; } = new InMemoryDbSet<LookupGender>();
        public DbSet<LookupGovernmentOfficeRegion> LookupGovernmentOfficeRegions { get; set; } = new InMemoryDbSet<LookupGovernmentOfficeRegion>();
        public DbSet<LookupGovernorAppointingBody> LookupGovernorAppointingBodies { get; set; } = new InMemoryDbSet<LookupGovernorAppointingBody>();
        public DbSet<LookupGovernorRole> LookupGovernorRoles { get; set; } = new InMemoryDbSet<LookupGovernorRole>();
        public DbSet<LookupGroupStatus> LookupGroupStatuses { get; set; } = new InMemoryDbSet<LookupGroupStatus>();
        public DbSet<LookupGroupType> LookupGroupTypes { get; set; } = new InMemoryDbSet<LookupGroupType>();
        public DbSet<LookupGSSLA> LookupGSSLA { get; set; } = new InMemoryDbSet<LookupGSSLA>();
        public DbSet<LookupHeadTitle> LookupHeadTitles { get; set; } = new InMemoryDbSet<LookupHeadTitle>();
        public DbSet<LookupIndependentSchoolType> LookupIndependentSchoolTypes { get; set; } = new InMemoryDbSet<LookupIndependentSchoolType>();
        public DbSet<LookupInspectorateName> LookupInspectorateNames { get; set; } = new InMemoryDbSet<LookupInspectorateName>();
        public DbSet<LookupInspectorate> LookupInspectorates { get; set; } = new InMemoryDbSet<LookupInspectorate>();
        public DbSet<LookupLocalGovernors> LookupLocalGovernors { get; set; } = new InMemoryDbSet<LookupLocalGovernors>();
        public DbSet<LookupLSOA> LookupLSOAs { get; set; } = new InMemoryDbSet<LookupLSOA>();
        public DbSet<LookupMSOA> LookupMSOAs { get; set; } = new InMemoryDbSet<LookupMSOA>();
        public DbSet<LookupNationality> LookupNationalities { get; set; } = new InMemoryDbSet<LookupNationality>();
        public DbSet<LookupParliamentaryConstituency> LookupParliamentaryConstituencies { get; set; } = new InMemoryDbSet<LookupParliamentaryConstituency>();
        public DbSet<LookupProvisionBoarding> LookupProvisionBoarding { get; set; } = new InMemoryDbSet<LookupProvisionBoarding>();
        public DbSet<LookupProvisionNursery> LookupProvisionNurseries { get; set; } = new InMemoryDbSet<LookupProvisionNursery>();
        public DbSet<LookupProvisionOfficialSixthForm> LookupProvisionOfficialSixthForms { get; set; } = new InMemoryDbSet<LookupProvisionOfficialSixthForm>();
        public DbSet<LookupProvisionSpecialClasses> LookupProvisionSpecialClasses { get; set; } = new InMemoryDbSet<LookupProvisionSpecialClasses>();
        public DbSet<LookupPRUEBD> LookupPRUEBDs { get; set; } = new InMemoryDbSet<LookupPRUEBD>();
        public DbSet<LookupPruEducatedByOthers> LookupPruEducatedByOthers { get; set; } = new InMemoryDbSet<LookupPruEducatedByOthers>();
        public DbSet<LookupPruFulltimeProvision> LookupPruFulltimeProvisions { get; set; } = new InMemoryDbSet<LookupPruFulltimeProvision>();
        public DbSet<LookupPRUSEN> LookupPRUSENs { get; set; } = new InMemoryDbSet<LookupPRUSEN>();
        public DbSet<LookupReasonEstablishmentClosed> LookupReasonEstablishmentClosed { get; set; } = new InMemoryDbSet<LookupReasonEstablishmentClosed>();
        public DbSet<LookupReasonEstablishmentOpened> LookupReasonEstablishmentOpened { get; set; } = new InMemoryDbSet<LookupReasonEstablishmentOpened>();
        public DbSet<LookupReligiousCharacter> LookupReligiousCharacters { get; set; } = new InMemoryDbSet<LookupReligiousCharacter>();
        public DbSet<LookupReligiousEthos> LookupReligiousEthos { get; set; } = new InMemoryDbSet<LookupReligiousEthos>();
        public DbSet<LookupResourcedProvision> LookupResourcedProvisions { get; set; } = new InMemoryDbSet<LookupResourcedProvision>();
        public DbSet<LookupSection41Approved> LookupSection41Approved { get; set; } = new InMemoryDbSet<LookupSection41Approved>();
        public DbSet<LookupSpecialEducationNeeds> LookupSpecialEducationNeeds { get; set; } = new InMemoryDbSet<LookupSpecialEducationNeeds>();
        public DbSet<LookupTeenageMothersProvision> LookupTeenageMothersProvisions { get; set; } = new InMemoryDbSet<LookupTeenageMothersProvision>();
        public DbSet<LookupTypeOfResourcedProvision> LookupTypeOfResourcedProvisions { get; set; } = new InMemoryDbSet<LookupTypeOfResourcedProvision>();
        public DbSet<LookupUrbanRural> LookupUrbanRural { get; set; } = new InMemoryDbSet<LookupUrbanRural>();
        public DbSet<EstablishmentPermission> Permissions { get; set; } = new InMemoryDbSet<EstablishmentPermission>();
        public IDbSet<IdentityRole> Roles { get; set; } = new InMemoryDbSet<IdentityRole>();
        public IDbSet<ApplicationUser> Users { get; set; } = new InMemoryDbSet<ApplicationUser>();

        public int SaveChangesCalledCount { get; set; }

        public int SaveChanges()
        {
            SaveChangesCalledCount++;
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalledCount++;
            return Task.FromResult(0);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalledCount++;
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            
        }
    }
}
