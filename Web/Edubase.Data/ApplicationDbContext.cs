using Edubase.Data.Entity.Lookup;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
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
        

        public ApplicationDbContext()
            : base("EdubaseSqlDb", throwIfV1Schema: false)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}