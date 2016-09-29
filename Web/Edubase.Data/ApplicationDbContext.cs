using Edubase.Data.Entity.Lookups;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Edubase.Data.Entity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        [Obsolete]
        public IDbSet<SchoolMAT> SchoolMATs { get; set; }

        [Obsolete]
        public IDbSet<MAT> MATs { get; set; }

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

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}