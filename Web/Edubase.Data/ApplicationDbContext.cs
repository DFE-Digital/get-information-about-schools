using Edubase.Data.Entity.Lookups;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;

namespace Edubase.Data.Entity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public IDbSet<AdmissionsPolicy> AdmissionsPolicies { get; set; }
        public IDbSet<Diocese> Dioceses { get; set; }
        public IDbSet<EducationPhase> EducationPhases { get; set; }
        public IDbSet<EstablishmentStatus> EstablishmentStatuses { get; set; }
        public IDbSet<EstablishmentType> EstablishmentTypes { get; set; }
        public IDbSet<Gender> Genders { get; set; }
        public IDbSet<GroupType> GroupTypes { get; set; }
        public IDbSet<HeadTitle> HeadTitles { get; set; }
        public IDbSet<ProvisionBoarding> BoardingProvisions { get; set; }
        public IDbSet<ProvisionNursery> NurseryProvisions { get; set; }
        public IDbSet<ProvisionOfficialSixthForm> OfficialSixthFormProvisions { get; set; }
        public IDbSet<ProvisionSpecialClasses> SpecialClassesProvisions { get; set; }
        public IDbSet<ReasonEstablishmentClosed> EstablishmentClosedReasons { get; set; }
        public IDbSet<ReasonEstablishmentOpened> EstablishmentOpenedReasons { get; set; }
        public IDbSet<ReligiousCharacter> ReligiousCharacters { get; set; }
        public IDbSet<ReligiousEthos> ReligiousEthos { get; set; }
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<Establishment> Establishments { get; set; }
        public IDbSet<LocalAuthority> LocalAuthorities { get; set; }
        public IDbSet<Estab2Estab> Estab2EstabLinks { get; set; }
        public IDbSet<Establishment2Company> Establishment2CompanyLinks { get; set; }
        public IDbSet<EstablishmentPermission> Permissions { get; set; }
        public IDbSet<EstablishmentApprovalQueue> EstablishmentApprovalQueue { get; set; }
        public IDbSet<Governor> Governors { get; set; }
        public IDbSet<GovernorRole> GovernorRoles { get; set; }
        public IDbSet<GovernorAppointingBody> GovernorAppointingBodies { get; set; }
        public IDbSet<EstablishmentChangeHistory> EstablishmentChangeHistories { get; set; }

        private bool _enableIdentityInsert;

        public ApplicationDbContext()
            : base("EdubaseSqlDb", throwIfV1Schema: false)
        {
            
        }

        public ApplicationDbContext(bool enableIdentityInsert) 
            : this()
        {
            _enableIdentityInsert = enableIdentityInsert;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            //modelBuilder.Entity<Establishment>().Property(a => a.Latitude).HasPrecision(18, 9);
            //modelBuilder.Entity<Establishment>().Property(a => a.Longitude).HasPrecision(18, 9);


            if (_enableIdentityInsert)
            {
                modelBuilder.Entity<AdmissionsPolicy>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ProvisionBoarding>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<EducationPhase>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<EstablishmentType>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<EstablishmentStatus>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<Gender>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<GroupType>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<HeadTitle>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ProvisionNursery>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ProvisionOfficialSixthForm>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ProvisionSpecialClasses>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ReasonEstablishmentClosed>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ReasonEstablishmentOpened>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ReligiousCharacter>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<ReligiousEthos>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<Company>()
                    .Property(x => x.GroupUID)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<LocalAuthority>()
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                modelBuilder.Entity<Establishment>()
                    .Property(x => x.Urn)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            }


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
    }
}