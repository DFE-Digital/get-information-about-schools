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
    public class LookupService : IDisposable, ILookupService
    {
        private Lazy<ApplicationDbContext> _dc = new Lazy<ApplicationDbContext>(ApplicationDbContext.Create);
        private ApplicationDbContext DataContext => _dc.Value;
        
        public LookupService()
        {

        }
        
        public bool IsLookupField(string name) => true; // _mapping.ContainsKey(name);

        public List<LocalAuthority> LocalAuthorityGetAll() => DataContext.LocalAuthorities.ToList();
        public async Task<List<LocalAuthority>> LocalAuthorityGetAllAsync() => await DataContext.LocalAuthorities.ToListAsync();

        public List<HeadTitle> HeadTitleGetAll() => DataContext.HeadTitles.ToList();
        public async Task<List<HeadTitle>> HeadTitleGetAllAsync() => await DataContext.HeadTitles.ToListAsync();

        public List<Gender> GendersGetAll() => DataContext.Genders.ToList();
        public async Task<List<Gender>> GendersGetAllAsync() => await DataContext.Genders.ToListAsync();

        public List<EducationPhase> EducationPhasesGetAll() => DataContext.EducationPhases.ToList();
        public async Task<List<EducationPhase>> EducationPhasesGetAllAsync() => await DataContext.EducationPhases.ToListAsync();

        public List<AdmissionsPolicy> AdmissionsPoliciesGetAll() => DataContext.AdmissionsPolicies.ToList();
        public async Task<List<AdmissionsPolicy>> AdmissionsPoliciesGetAllAsync() => await DataContext.AdmissionsPolicies.ToListAsync();

        public List<EstablishmentStatus> EstablishmentStatusesGetAll() => DataContext.EstablishmentStatuses.ToList();
        public async Task<List<EstablishmentStatus>> EstablishmentStatusesGetAllAsync() => await DataContext.EstablishmentStatuses.ToListAsync();

        public List<EstablishmentType> EstablishmentTypesGetAll() => DataContext.EstablishmentTypes.ToList();
        public async Task<List<EstablishmentType>> EstablishmentTypesGetAllAsync() => await DataContext.EstablishmentTypes.ToListAsync();

        public List<ReasonEstablishmentClosed> EstablishmentClosedReasonsGetAll() => DataContext.EstablishmentClosedReasons.ToList();
        public async Task<List<ReasonEstablishmentClosed>> EstablishmentClosedReasonsGetAllAsync() => await DataContext.EstablishmentClosedReasons.ToListAsync();

        public List<ReasonEstablishmentOpened> EstablishmentOpenedReasonsGetAll() => DataContext.EstablishmentOpenedReasons.ToList();
        public async Task<List<ReasonEstablishmentOpened>> EstablishmentOpenedReasonsGetAllAsync() => await DataContext.EstablishmentOpenedReasons.ToListAsync();


        public List<ProvisionBoarding> BoardingProvisionsGetAll() => DataContext.BoardingProvisions.ToList();
        public async Task<List<ProvisionBoarding>> BoardingProvisionsGetAllAsync() => await DataContext.BoardingProvisions.ToListAsync();



        public List<ProvisionNursery> NurseryProvisionsGetAll() => DataContext.NurseryProvisions.ToList();
        public async Task<List<ProvisionNursery>> NurseryProvisionsGetAllAsync() => await DataContext.NurseryProvisions.ToListAsync();


        public List<ProvisionOfficialSixthForm> OfficialSixthFormProvisionsGetAll() => DataContext.OfficialSixthFormProvisions.ToList();
        public async Task<List<ProvisionOfficialSixthForm>> OfficialSixthFormProvisionsGetAllAsync() => await DataContext.OfficialSixthFormProvisions.ToListAsync();


        public List<ReligiousCharacter> ReligiousCharactersGetAll() => DataContext.ReligiousCharacters.ToList();
        public async Task<List<ReligiousCharacter>> ReligiousCharactersGetAllAsync() => await DataContext.ReligiousCharacters.ToListAsync();


        public List<ReligiousEthos> ReligiousEthosGetAll() => DataContext.ReligiousEthos.ToList();
        public async Task<List<ReligiousEthos>> ReligiousEthosGetAllAsync() => await DataContext.ReligiousEthos.ToListAsync();


        public List<Diocese> DiocesesGetAll() => DataContext.Dioceses.ToList();
        public async Task<List<Diocese>> DiocesesGetAllAsync() => await DataContext.Dioceses.ToListAsync();


        public List<ProvisionSpecialClasses> SpecialClassesProvisionsGetAll() => DataContext.SpecialClassesProvisions.ToList();
        public async Task<List<ProvisionSpecialClasses>> SpecialClassesProvisionsGetAllAsync() => await DataContext.SpecialClassesProvisions.ToListAsync();


        public async Task<List<GroupType>> GroupeTypesGetAllAsync() => await DataContext.GroupTypes.ToListAsync();
        public List<GroupType> GroupeTypesGetAll() => DataContext.GroupTypes.ToList();
        

        public void Dispose()
        {
            if (_dc.IsValueCreated)
                DataContext.Dispose();
        }

        
    }
}
