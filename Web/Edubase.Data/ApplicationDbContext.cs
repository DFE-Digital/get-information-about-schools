using Edubase.Data.Entity.Lookups;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using System.Threading;

namespace Edubase.Data.Entity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public DbSet<GroupCollection> Groups { get; set; }
        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<LocalAuthority> LocalAuthorities { get; set; }
        public DbSet<EstablishmentLink> EstablishmentLinks { get; set; }
        public DbSet<EstablishmentGroup> EstablishmentGroups { get; set; }
        public DbSet<EstablishmentPermission> Permissions { get; set; }
        public DbSet<EstablishmentApprovalQueue> EstablishmentApprovalQueue { get; set; }
        public DbSet<Governor> Governors { get; set; }
        public DbSet<EstablishmentChangeHistory> EstablishmentChangeHistories { get; set; }

        public override IDbSet<ApplicationUser> Users
        {
            get
            {
                return base.Users;
            }

            set
            {
                base.Users = value;
            }
        }

        public override IDbSet<IdentityRole> Roles
        {
            get
            {
                return base.Roles;
            }

            set
            {
                base.Roles = value;
            }
        }

        #region Lookups
        public DbSet<LookupAdmissionsPolicy> LookupAdmissionsPolicies { get; set; }
        public DbSet<LookupEducationPhase> LookupEducationPhases { get; set; }
        public DbSet<LookupEstablishmentStatus> LookupEstablishmentStatuses { get; set; }
        public DbSet<LookupEstablishmentType> LookupEstablishmentTypes { get; set; }
        public DbSet<LookupGender> LookupGenders { get; set; }
        public DbSet<LookupGroupType> LookupGroupTypes { get; set; }
        public DbSet<LookupHeadTitle> LookupHeadTitles { get; set; }
        public DbSet<LookupProvisionBoarding> LookupProvisionBoarding { get; set; }
        public DbSet<LookupProvisionNursery> LookupProvisionNurseries { get; set; }
        public DbSet<LookupProvisionOfficialSixthForm> LookupProvisionOfficialSixthForms { get; set; }
        public DbSet<LookupProvisionSpecialClasses> LookupProvisionSpecialClasses { get; set; }
        public DbSet<LookupReasonEstablishmentClosed> LookupReasonEstablishmentClosed { get; set; }
        public DbSet<LookupReasonEstablishmentOpened> LookupReasonEstablishmentOpened { get; set; }
        public DbSet<LookupReligiousCharacter> LookupReligiousCharacters { get; set; }
        public DbSet<LookupReligiousEthos> LookupReligiousEthos { get; set; }
        public DbSet<LookupGovernorRole> LookupGovernorRoles { get; set; }
        public DbSet<LookupGovernorAppointingBody> LookupGovernorAppointingBodies { get; set; }
        public DbSet<LookupAccommodationChanged> LookupAccommodationChanged { get; set; }
        public DbSet<LookupBoardingEstablishment> LookupBoardingEstablishment { get; set; }
        public DbSet<LookupCCGovernance> LookupCCGovernance { get; set; }
        public DbSet<LookupCCOperationalHours> LookupCCOperationalHours { get; set; }
        public DbSet<LookupCCDisadvantagedArea> LookupCCDisadvantagedAreas { get; set; }
        public DbSet<LookupCCPhaseType> LookupCCPhaseTypes { get; set; }
        public DbSet<LookupCCGroupLead> LookupCCGroupLeads { get; set; }
        public DbSet<LookupDeliveryModel> LookupCCDeliveryModels { get; set; }
        public DbSet<LookupDiocese> LookupDioceses { get; set; }
        public DbSet<LookupChildcareFacilities> LookupChildcareFacilities { get; set; }
        public DbSet<LookupDirectProvisionOfEarlyYears> LookupDirectProvisionOfEarlyYears { get; set; }
        public DbSet<LookupFurtherEducationType> LookupFurtherEducationTypes { get; set; }
        public DbSet<LookupIndependentSchoolType> LookupIndependentSchoolTypes { get; set; }
        public DbSet<LookupInspectorate> LookupInspectorates { get; set; }
        public DbSet<LookupInspectorateName> LookupInspectorateNames { get; set; }
        public DbSet<LookupLocalGovernors> LookupLocalGovernors { get; set; }
        public DbSet<LookupNationality> LookupNationalities { get; set; }
        public DbSet<LookupPRUEBD> LookupPRUEBDs { get; set; }
        public DbSet<LookupPruEducatedByOthers> LookupPruEducatedByOthers { get; set; }
        public DbSet<LookupPruFulltimeProvision> LookupPruFulltimeProvisions { get; set; }
        public DbSet<LookupPRUSEN> LookupPRUSENs { get; set; }
        public DbSet<LookupResourcedProvision> LookupResourcedProvisions { get; set; }
        public DbSet<LookupSection41Approved> LookupSection41Approved { get; set; }
        public DbSet<LookupSpecialEducationNeeds> LookupSpecialEducationNeeds { get; set; }
        public DbSet<LookupTeenageMothersProvision> LookupTeenageMothersProvisions { get; set; }
        public DbSet<LookupTypeOfResourcedProvision> LookupTypeOfResourcedProvisions { get; set; }
        public DbSet<LookupEstablishmentLinkType> LookupEstablishmentLinkTypes { get; set; }
        public DbSet<LookupEstablishmentTypeGroup> LookupEstablishmentTypeGroups { get; set; }
        public DbSet<LookupGovernmentOfficeRegion> LookupGovernmentOfficeRegions { get; set; }
        public DbSet<LookupDistrictAdministrative> LookupAdministrativeDistricts { get; set; }
        public DbSet<LookupAdministrativeWard> LookupAdministrativeWards { get; set; }
        public DbSet<LookupParliamentaryConstituency> LookupParliamentaryConstituencies { get; set; }
        public DbSet<LookupUrbanRural> LookupUrbanRural { get; set; }
        public DbSet<LookupGSSLA> LookupGSSLA { get; set; }
        public DbSet<LookupCASWard> LookupCASWards { get; set; }

        public DbSet<LookupMSOA> LookupMSOAs { get; set; }
        public DbSet<LookupLSOA> LookupLSOAs { get; set; }
        public DbSet<LookupGroupStatus> LookupGroupStatuses { get; set; }

        #endregion


        public ApplicationDbContext()
            : base("EdubaseSqlDb", throwIfV1Schema: false)
        {
            
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create() => new ApplicationDbContext();

        public static async Task<T> OperationAsync<T>(Func<ApplicationDbContext, Task<T>> action, ApplicationDbContext dc = null)
        {
            if (dc == null)
            {
                using (var dataContext = new ApplicationDbContext())
                {
                    T retVal = await action(dataContext);
                    await dataContext.SaveChangesAsync();
                    return retVal;
                }
            }
            else return await action(dc);
        }
        public static T Operation<T>(Func<ApplicationDbContext, T> action, ApplicationDbContext dc = null)
        {
            if (dc == null)
            {
                using (var dataContext = new ApplicationDbContext())
                {
                    T retVal = action(dataContext);
                    dataContext.SaveChanges();
                    return retVal;
                }
            }
            else return action(dc);
        }

        public static async Task OperationAsync(Func<ApplicationDbContext, Task> action, ApplicationDbContext dc = null)
        {
            if (dc == null)
            {
                using (var dataContext = new ApplicationDbContext())
                {
                    await action(dataContext);
                    await dataContext.SaveChangesAsync();
                }
            }
            else await action(dc);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

    }
}