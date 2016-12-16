using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Edubase.UnitTest.Mocks.IntegrationEndPoints
{
    [Obsolete("use InMemoryApplicationDbContext")]
    public class MockApplicationDbContext : IApplicationDbContext
    {
        public DbSet<EstablishmentApprovalQueue> EstablishmentApprovalQueue { get; set; } = new MockDbSet<EstablishmentApprovalQueue>();
        public DbSet<EstablishmentChangeHistory> EstablishmentChangeHistories { get; set; } = new MockDbSet<EstablishmentChangeHistory>();
        public DbSet<EstablishmentLink> EstablishmentLinks { get; set; } = new MockDbSet<EstablishmentLink>();
        public DbSet<Establishment> Establishments { get; set; } = new MockDbSet<Establishment>();
        public DbSet<EstablishmentGroup> EstablishmentGroups { get; set; } = new MockDbSet<EstablishmentGroup>();
        public DbSet<Governor> Governors { get; set; } = new MockDbSet<Governor>();
        public DbSet<GroupCollection> Groups { get; set; } = new MockDbSet<GroupCollection>();
        public DbSet<LocalAuthority> LocalAuthorities { get; set; } = new MockDbSet<LocalAuthority>();
        public DbSet<LookupAccommodationChanged> LookupAccommodationChanged { get; set; } = new MockDbSet<LookupAccommodationChanged>();
        public DbSet<LookupDistrictAdministrative> LookupAdministrativeDistricts { get; set; } = new MockDbSet<LookupDistrictAdministrative>();
        public DbSet<LookupAdministrativeWard> LookupAdministrativeWards { get; set; } = new MockDbSet<LookupAdministrativeWard>();
        public DbSet<LookupAdmissionsPolicy> LookupAdmissionsPolicies { get; set; } = new MockDbSet<LookupAdmissionsPolicy>();
        public DbSet<LookupBoardingEstablishment> LookupBoardingEstablishment { get; set; } = new MockDbSet<LookupBoardingEstablishment>();
        public DbSet<LookupCASWard> LookupCASWards { get; set; } = new MockDbSet<LookupCASWard>();
        public DbSet<LookupCCGovernance> LookupCCGovernance { get; set; } = new MockDbSet<LookupCCGovernance>();
        public DbSet<LookupCCOperationalHours> LookupCCOperationalHours { get; set; } = new MockDbSet<LookupCCOperationalHours>();
        public DbSet<LookupCCPhaseType> LookupCCPhaseTypes { get; set; } = new MockDbSet<LookupCCPhaseType>();
        public DbSet<LookupChildcareFacilities> LookupChildcareFacilities { get; set; } = new MockDbSet<LookupChildcareFacilities>();
        public DbSet<LookupDiocese> LookupDioceses { get; set; } = new MockDbSet<LookupDiocese>();
        public DbSet<LookupDirectProvisionOfEarlyYears> LookupDirectProvisionOfEarlyYears { get; set; } = new MockDbSet<LookupDirectProvisionOfEarlyYears>();
        public DbSet<LookupEducationPhase> LookupEducationPhases { get; set; } = new MockDbSet<LookupEducationPhase>();
        public DbSet<LookupEstablishmentLinkType> LookupEstablishmentLinkTypes { get; set; } = new MockDbSet<LookupEstablishmentLinkType>();
        public DbSet<LookupEstablishmentStatus> LookupEstablishmentStatuses { get; set; } = new MockDbSet<LookupEstablishmentStatus>();
        public DbSet<LookupEstablishmentTypeGroup> LookupEstablishmentTypeGroups { get; set; } = new MockDbSet<LookupEstablishmentTypeGroup>();
        public DbSet<LookupEstablishmentType> LookupEstablishmentTypes { get; set; } = new MockDbSet<LookupEstablishmentType>();
        public DbSet<LookupFurtherEducationType> LookupFurtherEducationTypes { get; set; } = new MockDbSet<LookupFurtherEducationType>();
        public DbSet<LookupGender> LookupGenders { get; set; } = new MockDbSet<LookupGender>();
        public DbSet<LookupGovernmentOfficeRegion> LookupGovernmentOfficeRegions { get; set; } = new MockDbSet<LookupGovernmentOfficeRegion>();
        public DbSet<LookupGovernorAppointingBody> LookupGovernorAppointingBodies { get; set; } = new MockDbSet<LookupGovernorAppointingBody>();
        public DbSet<LookupGovernorRole> LookupGovernorRoles { get; set; } = new MockDbSet<LookupGovernorRole>();
        public DbSet<LookupGroupStatus> LookupGroupStatuses { get; set; } = new MockDbSet<LookupGroupStatus>();
        public DbSet<LookupGroupType> LookupGroupTypes { get; set; } = new MockDbSet<LookupGroupType>();
        public DbSet<LookupGSSLA> LookupGSSLA { get; set; } = new MockDbSet<LookupGSSLA>();
        public DbSet<LookupHeadTitle> LookupHeadTitles { get; set; } = new MockDbSet<LookupHeadTitle>();
        public DbSet<LookupIndependentSchoolType> LookupIndependentSchoolTypes { get; set; } = new MockDbSet<LookupIndependentSchoolType>();
        public DbSet<LookupInspectorateName> LookupInspectorateNames { get; set; } = new MockDbSet<LookupInspectorateName>();
        public DbSet<LookupInspectorate> LookupInspectorates { get; set; } = new MockDbSet<LookupInspectorate>();
        public DbSet<LookupLocalGovernors> LookupLocalGovernors { get; set; } = new MockDbSet<LookupLocalGovernors>();
        public DbSet<LookupLSOA> LookupLSOAs { get; set; } = new MockDbSet<LookupLSOA>();
        public DbSet<LookupMSOA> LookupMSOAs { get; set; } = new MockDbSet<LookupMSOA>();
        public DbSet<LookupNationality> LookupNationalities { get; set; } = new MockDbSet<LookupNationality>();
        public DbSet<LookupParliamentaryConstituency> LookupParliamentaryConstituencies { get; set; } = new MockDbSet<LookupParliamentaryConstituency>();
        public DbSet<LookupProvisionBoarding> LookupProvisionBoarding { get; set; } = new MockDbSet<LookupProvisionBoarding>();
        public DbSet<LookupProvisionNursery> LookupProvisionNurseries { get; set; } = new MockDbSet<LookupProvisionNursery>();
        public DbSet<LookupProvisionOfficialSixthForm> LookupProvisionOfficialSixthForms { get; set; } = new MockDbSet<LookupProvisionOfficialSixthForm>();
        public DbSet<LookupProvisionSpecialClasses> LookupProvisionSpecialClasses { get; set; } = new MockDbSet<LookupProvisionSpecialClasses>();
        public DbSet<LookupPRUEBD> LookupPRUEBDs { get; set; } = new MockDbSet<LookupPRUEBD>();
        public DbSet<LookupPruEducatedByOthers> LookupPruEducatedByOthers { get; set; } = new MockDbSet<LookupPruEducatedByOthers>();
        public DbSet<LookupPruFulltimeProvision> LookupPruFulltimeProvisions { get; set; } = new MockDbSet<LookupPruFulltimeProvision>();
        public DbSet<LookupPRUSEN> LookupPRUSENs { get; set; } = new MockDbSet<LookupPRUSEN>();
        public DbSet<LookupReasonEstablishmentClosed> LookupReasonEstablishmentClosed { get; set; } = new MockDbSet<LookupReasonEstablishmentClosed>();
        public DbSet<LookupReasonEstablishmentOpened> LookupReasonEstablishmentOpened { get; set; } = new MockDbSet<LookupReasonEstablishmentOpened>();
        public DbSet<LookupReligiousCharacter> LookupReligiousCharacters { get; set; } = new MockDbSet<LookupReligiousCharacter>();
        public DbSet<LookupReligiousEthos> LookupReligiousEthos { get; set; } = new MockDbSet<LookupReligiousEthos>();
        public DbSet<LookupResourcedProvision> LookupResourcedProvisions { get; set; } = new MockDbSet<LookupResourcedProvision>();
        public DbSet<LookupSection41Approved> LookupSection41Approved { get; set; } = new MockDbSet<LookupSection41Approved>();
        public DbSet<LookupSpecialEducationNeeds> LookupSpecialEducationNeeds { get; set; } = new MockDbSet<LookupSpecialEducationNeeds>();
        public DbSet<LookupTeenageMothersProvision> LookupTeenageMothersProvisions { get; set; } = new MockDbSet<LookupTeenageMothersProvision>();
        public DbSet<LookupTypeOfResourcedProvision> LookupTypeOfResourcedProvisions { get; set; } = new MockDbSet<LookupTypeOfResourcedProvision>();
        public DbSet<LookupUrbanRural> LookupUrbanRural { get; set; } = new MockDbSet<LookupUrbanRural>();
        public DbSet<EstablishmentPermission> Permissions { get; set; } = new MockDbSet<EstablishmentPermission>();
        public IDbSet<IdentityRole> Roles { get; set; } = new MockDbSet<IdentityRole>();
        public IDbSet<ApplicationUser> Users { get; set; } = new MockDbSet<ApplicationUser>();

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
    }
}
