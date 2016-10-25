using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System.Runtime.Caching;
using Edubase.Common;

namespace Edubase.Services
{
    public class CachedLookupService : ILookupService
    {
        private Dictionary<string, Func<ApplicationDbContext, object, Task<string>>> _mapping = null;

        private LookupService _svc = new LookupService();

        public CachedLookupService()
        {
            _mapping = new Dictionary<string, Func<ApplicationDbContext, object, Task<string>>>()
            {
                { "LocalAuthorityId", async (dc, id) => GetName((await LocalAuthorityGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "HeadTitleId", async (dc, id) => GetName((await HeadTitleGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "GenderId", async (dc, id) => GetName((await GendersGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "EducationPhaseId", async (dc, id) => GetName((await EducationPhasesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "AdmissionsPolicyId", async (dc, id) => GetName((await AdmissionsPoliciesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "StatusId", async (dc, id) => GetName((await EstablishmentStatusesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReasonEstablishmentOpenedId", async (dc, id) => GetName((await ReasonEstablishmentOpenedGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReasonEstablishmentClosedId", async (dc, id) => GetName((await ReasonEstablishmentClosedGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionBoardingId", async (dc, id) => GetName((await BoardingProvisionsGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionNurseryId", async (dc, id) => GetName((await NurseryProvisionsGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionOfficialSixthFormId", async (dc, id) => GetName((await OfficialSixthFormProvisionsGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReligiousCharacterId", async (dc, id) => GetName((await ReligiousCharactersGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ReligiousEthosId", async (dc, id) => GetName((await ReligiousEthosGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "DioceseId", async (dc, id) => GetName((await DiocesesGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "ProvisionSpecialClassesId", async (dc, id) => GetName((await SpecialClassesProvisionsGetAllAsync()).Cast<LookupBase>(), (int) id) },
                { "TypeId", async (dc, id) => GetName((await EstablishmentTypesGetAllAsync()).Cast<LookupBase>(), (int) id) }
            };
        }

        public List<AdmissionsPolicy> AdmissionsPoliciesGetAll() => Cacher.Auto(_svc.AdmissionsPoliciesGetAll);

        public async Task<List<AdmissionsPolicy>> AdmissionsPoliciesGetAllAsync() => await Cacher.AutoAsync(_svc.AdmissionsPoliciesGetAllAsync);

        public List<EducationPhase> EducationPhasesGetAll() => Cacher.Auto(_svc.EducationPhasesGetAll);

        public async Task<List<EducationPhase>> EducationPhasesGetAllAsync() => await Cacher.AutoAsync(_svc.EducationPhasesGetAllAsync);

        public List<EstablishmentStatus> EstablishmentStatusesGetAll() => Cacher.Auto(_svc.EstablishmentStatusesGetAll);

        public async Task<List<EstablishmentStatus>> EstablishmentStatusesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentStatusesGetAllAsync);

        public List<ReasonEstablishmentOpened> ReasonEstablishmentOpenedGetAll() => Cacher.Auto(_svc.EstablishmentOpenedReasonsGetAll);

        public async Task<List<ReasonEstablishmentOpened>> ReasonEstablishmentOpenedGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentOpenedReasonsGetAllAsync);

        public List<ReasonEstablishmentClosed> ReasonEstablishmentClosedGetAll() => Cacher.Auto(_svc.EstablishmentClosedReasonsGetAll);

        public async Task<List<ReasonEstablishmentClosed>> ReasonEstablishmentClosedGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentClosedReasonsGetAllAsync);

        public List<Gender> GendersGetAll() => Cacher.Auto(_svc.GendersGetAll);

        public async Task<List<Gender>> GendersGetAllAsync() => await Cacher.AutoAsync(_svc.GendersGetAllAsync);
        
        public List<HeadTitle> HeadTitleGetAll() => Cacher.Auto(_svc.HeadTitleGetAll);

        public async Task<List<HeadTitle>> HeadTitleGetAllAsync() => await Cacher.AutoAsync(_svc.HeadTitleGetAllAsync);
        
        public List<LocalAuthority> LocalAuthorityGetAll() => Cacher.Auto(_svc.LocalAuthorityGetAll);

        public async Task<List<LocalAuthority>> LocalAuthorityGetAllAsync() => await Cacher.AutoAsync(_svc.LocalAuthorityGetAllAsync);

        public List<EstablishmentType> EstablishmentTypesGetAll() => Cacher.Auto(_svc.EstablishmentTypesGetAll);

        public async Task<List<EstablishmentType>> EstablishmentTypesGetAllAsync() => await Cacher.AutoAsync(_svc.EstablishmentTypesGetAllAsync);

        public List<GroupType> GroupeTypesGetAll() => Cacher.Auto(_svc.GroupeTypesGetAll);

        public async Task<List<GroupType>> GroupeTypesGetAllAsync() => await Cacher.AutoAsync(_svc.GroupeTypesGetAllAsync);


        public List<ProvisionBoarding> BoardingProvisionsGetAll() => Cacher.Auto(_svc.BoardingProvisionsGetAll);
        public async Task<List<ProvisionBoarding>> BoardingProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.BoardingProvisionsGetAllAsync);


        public List<ProvisionNursery> NurseryProvisionsGetAll() => Cacher.Auto(_svc.NurseryProvisionsGetAll);
        public async Task<List<ProvisionNursery>> NurseryProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.NurseryProvisionsGetAllAsync);


        public List<ProvisionOfficialSixthForm> OfficialSixthFormProvisionsGetAll() => Cacher.Auto(_svc.OfficialSixthFormProvisionsGetAll);
        public async Task<List<ProvisionOfficialSixthForm>> OfficialSixthFormProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.OfficialSixthFormProvisionsGetAllAsync);


        public List<ReligiousCharacter> ReligiousCharactersGetAll() => Cacher.Auto(_svc.ReligiousCharactersGetAll);
        public async Task<List<ReligiousCharacter>> ReligiousCharactersGetAllAsync() => await Cacher.AutoAsync(_svc.ReligiousCharactersGetAllAsync);


        public List<ReligiousEthos> ReligiousEthosGetAll() => Cacher.Auto(_svc.ReligiousEthosGetAll);
        public async Task<List<ReligiousEthos>> ReligiousEthosGetAllAsync() => await Cacher.AutoAsync(_svc.ReligiousEthosGetAllAsync);


        public List<Diocese> DiocesesGetAll() => Cacher.Auto(_svc.DiocesesGetAll);
        public async Task<List<Diocese>> DiocesesGetAllAsync() => await Cacher.AutoAsync(_svc.DiocesesGetAllAsync);


        public List<ProvisionSpecialClasses> SpecialClassesProvisionsGetAll() => Cacher.Auto(_svc.SpecialClassesProvisionsGetAll);
        public async Task<List<ProvisionSpecialClasses>> SpecialClassesProvisionsGetAllAsync() => await Cacher.AutoAsync(_svc.SpecialClassesProvisionsGetAllAsync);

        public async Task<string> GetNameAsync(string lookupName, int id) => 
            await ApplicationDbContext.OperationAsync(async dc => await _mapping.Get(lookupName)?.Invoke(dc, id));

        public bool IsLookupField(string name) => _mapping.ContainsKey(name);

        private string GetName(IEnumerable<LookupBase> items, int id) => items.FirstOrDefault(x => x.Id == id)?.Name;

        public void Dispose() => _svc.Dispose();
    }
}
