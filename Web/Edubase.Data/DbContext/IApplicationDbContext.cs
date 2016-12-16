using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Edubase.Data.Entity.Lookups;
using Microsoft.AspNet.Identity.EntityFramework;
using Edubase.Data.Entity;

namespace Edubase.Data.DbContext
{
    public interface IApplicationDbContext
    {
        DbSet<EstablishmentApprovalQueue> EstablishmentApprovalQueue { get; set; }
        DbSet<EstablishmentChangeHistory> EstablishmentChangeHistories { get; set; }
        DbSet<EstablishmentLink> EstablishmentLinks { get; set; }
        DbSet<Establishment> Establishments { get; set; }
        DbSet<EstablishmentGroup> EstablishmentGroups { get; set; }
        DbSet<Governor> Governors { get; set; }
        DbSet<GroupCollection> Groups { get; set; }
        DbSet<LocalAuthority> LocalAuthorities { get; set; }
        DbSet<LookupAccommodationChanged> LookupAccommodationChanged { get; set; }
        DbSet<LookupDistrictAdministrative> LookupAdministrativeDistricts { get; set; }
        DbSet<LookupAdministrativeWard> LookupAdministrativeWards { get; set; }
        DbSet<LookupAdmissionsPolicy> LookupAdmissionsPolicies { get; set; }
        DbSet<LookupBoardingEstablishment> LookupBoardingEstablishment { get; set; }
        DbSet<LookupCASWard> LookupCASWards { get; set; }
        DbSet<LookupCCGovernance> LookupCCGovernance { get; set; }
        DbSet<LookupCCOperationalHours> LookupCCOperationalHours { get; set; }
        DbSet<LookupCCPhaseType> LookupCCPhaseTypes { get; set; }
        DbSet<LookupChildcareFacilities> LookupChildcareFacilities { get; set; }
        DbSet<LookupDiocese> LookupDioceses { get; set; }
        DbSet<LookupDirectProvisionOfEarlyYears> LookupDirectProvisionOfEarlyYears { get; set; }
        DbSet<LookupEducationPhase> LookupEducationPhases { get; set; }
        DbSet<LookupEstablishmentLinkType> LookupEstablishmentLinkTypes { get; set; }
        DbSet<LookupEstablishmentStatus> LookupEstablishmentStatuses { get; set; }
        DbSet<LookupEstablishmentTypeGroup> LookupEstablishmentTypeGroups { get; set; }
        DbSet<LookupEstablishmentType> LookupEstablishmentTypes { get; set; }
        DbSet<LookupFurtherEducationType> LookupFurtherEducationTypes { get; set; }
        DbSet<LookupGender> LookupGenders { get; set; }
        DbSet<LookupGovernmentOfficeRegion> LookupGovernmentOfficeRegions { get; set; }
        DbSet<LookupGovernorAppointingBody> LookupGovernorAppointingBodies { get; set; }
        DbSet<LookupGovernorRole> LookupGovernorRoles { get; set; }
        DbSet<LookupGroupStatus> LookupGroupStatuses { get; set; }
        DbSet<LookupGroupType> LookupGroupTypes { get; set; }
        DbSet<LookupGSSLA> LookupGSSLA { get; set; }
        DbSet<LookupHeadTitle> LookupHeadTitles { get; set; }
        DbSet<LookupIndependentSchoolType> LookupIndependentSchoolTypes { get; set; }
        DbSet<LookupInspectorateName> LookupInspectorateNames { get; set; }
        DbSet<LookupInspectorate> LookupInspectorates { get; set; }
        DbSet<LookupLocalGovernors> LookupLocalGovernors { get; set; }
        DbSet<LookupLSOA> LookupLSOAs { get; set; }
        DbSet<LookupMSOA> LookupMSOAs { get; set; }
        DbSet<LookupNationality> LookupNationalities { get; set; }
        DbSet<LookupParliamentaryConstituency> LookupParliamentaryConstituencies { get; set; }
        DbSet<LookupProvisionBoarding> LookupProvisionBoarding { get; set; }
        DbSet<LookupProvisionNursery> LookupProvisionNurseries { get; set; }
        DbSet<LookupProvisionOfficialSixthForm> LookupProvisionOfficialSixthForms { get; set; }
        DbSet<LookupProvisionSpecialClasses> LookupProvisionSpecialClasses { get; set; }
        DbSet<LookupPRUEBD> LookupPRUEBDs { get; set; }
        DbSet<LookupPruEducatedByOthers> LookupPruEducatedByOthers { get; set; }
        DbSet<LookupPruFulltimeProvision> LookupPruFulltimeProvisions { get; set; }
        DbSet<LookupPRUSEN> LookupPRUSENs { get; set; }
        DbSet<LookupReasonEstablishmentClosed> LookupReasonEstablishmentClosed { get; set; }
        DbSet<LookupReasonEstablishmentOpened> LookupReasonEstablishmentOpened { get; set; }
        DbSet<LookupReligiousCharacter> LookupReligiousCharacters { get; set; }
        DbSet<LookupReligiousEthos> LookupReligiousEthos { get; set; }
        DbSet<LookupResourcedProvision> LookupResourcedProvisions { get; set; }
        DbSet<LookupSection41Approved> LookupSection41Approved { get; set; }
        DbSet<LookupSpecialEducationNeeds> LookupSpecialEducationNeeds { get; set; }
        DbSet<LookupTeenageMothersProvision> LookupTeenageMothersProvisions { get; set; }
        DbSet<LookupTypeOfResourcedProvision> LookupTypeOfResourcedProvisions { get; set; }
        DbSet<LookupUrbanRural> LookupUrbanRural { get; set; }
        DbSet<EstablishmentPermission> Permissions { get; set; }
        IDbSet<IdentityRole> Roles { get; set; }
        IDbSet<ApplicationUser> Users { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}