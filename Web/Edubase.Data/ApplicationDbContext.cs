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
        public IDbSet<Trust> Trusts { get; set; }
        public IDbSet<Establishment> Establishments { get; set; }
        public IDbSet<LocalAuthority> LocalAuthorities { get; set; }
        public IDbSet<EstablishmentLink> EstablishmentLinks { get; set; }
        public IDbSet<EstablishmentTrust> EstablishmentTrusts { get; set; }
        public IDbSet<EstablishmentPermission> Permissions { get; set; }
        public IDbSet<EstablishmentApprovalQueue> EstablishmentApprovalQueue { get; set; }
        public IDbSet<Governor> Governors { get; set; }
        public IDbSet<EstablishmentChangeHistory> EstablishmentChangeHistories { get; set; }
        

        #region Lookups
        public IDbSet<LookupAdmissionsPolicy> LookupAdmissionsPolicies { get; set; }
        public IDbSet<LookupEducationPhase> LookupEducationPhases { get; set; }
        public IDbSet<LookupEstablishmentStatus> LookupEstablishmentStatuses { get; set; }
        public IDbSet<LookupEstablishmentType> LookupEstablishmentTypes { get; set; }
        public IDbSet<LookupGender> LookupGenders { get; set; }
        public IDbSet<LookupGroupType> LookupGroupTypes { get; set; }
        public IDbSet<LookupHeadTitle> LookupHeadTitles { get; set; }
        public IDbSet<LookupProvisionBoarding> LookupProvisionBoarding { get; set; }
        public IDbSet<LookupProvisionNursery> LookupProvisionNurseries { get; set; }
        public IDbSet<LookupProvisionOfficialSixthForm> LookupProvisionOfficialSixthForms { get; set; }
        public IDbSet<LookupProvisionSpecialClasses> LookupProvisionSpecialClasses { get; set; }
        public IDbSet<LookupReasonEstablishmentClosed> LookupReasonEstablishmentClosed { get; set; }
        public IDbSet<LookupReasonEstablishmentOpened> LookupReasonEstablishmentOpened { get; set; }
        public IDbSet<LookupReligiousCharacter> LookupReligiousCharacters { get; set; }
        public IDbSet<LookupReligiousEthos> LookupReligiousEthos { get; set; }
        public IDbSet<LookupGovernorRole> LookupGovernorRoles { get; set; }
        public IDbSet<LookupGovernorAppointingBody> LookupGovernorAppointingBodies { get; set; }
        public IDbSet<LookupAccommodationChanged> LookupAccommodationChanged { get; set; }
        public IDbSet<LookupBoardingEstablishment> LookupBoardingEstablishment { get; set; }
        public IDbSet<LookupCCGovernance> LookupCCGovernance { get; set; }
        public IDbSet<LookupCCOperationalHours> LookupCCOperationalHours { get; set; }
        public IDbSet<LookupCCPhaseType> LookupCCPhaseTypes { get; set; }
        public IDbSet<LookupDiocese> LookupDioceses { get; set; }
        public IDbSet<LookupChildcareFacilities> LookupChildcareFacilities { get; set; }
        public IDbSet<LookupDirectProvisionOfEarlyYears> LookupDirectProvisionOfEarlyYears { get; set; }
        public IDbSet<LookupFurtherEducationType> LookupFurtherEducationTypes { get; set; }
        public IDbSet<LookupIndependentSchoolType> LookupIndependentSchoolTypes { get; set; }
        public IDbSet<LookupInspectorate> LookupInspectorates { get; set; }
        public IDbSet<LookupInspectorateName> LookupInspectorateNames { get; set; }
        public IDbSet<LookupLocalGovernors> LookupLocalGovernors { get; set; }
        public IDbSet<LookupNationality> LookupNationalities { get; set; }
        public IDbSet<LookupPRUEBD> LookupPRUEBDs { get; set; }
        public IDbSet<LookupPruEducatedByOthers> LookupPruEducatedByOthers { get; set; }
        public IDbSet<LookupPruFulltimeProvision> LookupPruFulltimeProvisions { get; set; }
        public IDbSet<LookupPRUSEN> LookupPRUSENs { get; set; }
        public IDbSet<LookupResourcedProvision> LookupResourcedProvisions { get; set; }
        public IDbSet<LookupSection41Approved> LookupSection41Approved { get; set; }
        public IDbSet<LookupSpecialEducationNeeds> LookupSpecialEducationNeeds { get; set; }
        public IDbSet<LookupTeenageMothersProvision> LookupTeenageMothersProvisions { get; set; }
        public IDbSet<LookupTypeOfResourcedProvision> LookupTypeOfResourcedProvisions { get; set; }
        public IDbSet<LookupEstablishmentLinkType> LookupEstablishmentLinkTypes { get; set; }
        public IDbSet<LookupEstablishmentTypeGroup> LookupEstablishmentTypeGroups { get; set; }
        public IDbSet<LookupGovernmentOfficeRegion> LookupGovernmentOfficeRegions { get; set; }
        public IDbSet<LookupDistrictAdministrative> LookupAdministrativeDistricts { get; set; }
        public IDbSet<LookupAdministrativeWard> LookupAdministrativeWards { get; set; }
        public IDbSet<LookupParliamentaryConstituency> LookupParliamentaryConstituencies { get; set; }
        public IDbSet<LookupUrbanRural> LookupUrbanRural { get; set; }
        public IDbSet<LookupGSSLA> LookupGSSLA { get; set; }
        public IDbSet<LookupCASWard> LookupCASWards { get; set; }

        public IDbSet<LookupMSOA> LookupMSOAs { get; set; }
        public IDbSet<LookupLSOA> LookupLSOAs { get; set; }

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
    }
}