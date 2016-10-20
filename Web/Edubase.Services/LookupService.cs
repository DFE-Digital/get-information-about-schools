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

        private Dictionary<string, Func<ApplicationDbContext, int, string>> _mapping = new Dictionary<string, Func<ApplicationDbContext, int, string>>()
        {
            { "LocalAuthorityId", (dc, id) => dc.LocalAuthorities.FirstOrDefault(x=>x.Id==id)?.Name },
            { "HeadTitleId", (dc, id) => dc.HeadTitles.FirstOrDefault(x=>x.Id==id)?.Name },
            { "GenderId", (dc, id) => dc.Genders.FirstOrDefault(x=>x.Id==id)?.Name },
            { "EducationPhaseId", (dc, id) => dc.EducationPhases.FirstOrDefault(x=>x.Id==id)?.Name },
            { "AdmissionsPolicyId", (dc, id) => dc.AdmissionsPolicies.FirstOrDefault(x=>x.Id==id)?.Name },
            { "StatusId", (dc, id) => dc.EstablishmentStatuses.FirstOrDefault(x=>x.Id==id)?.Name }
        };
        
        public LookupService()
        {

        }

        public string GetName(string lookupName, int id)
        {
            if (IsLookupField(lookupName))
            {
                using (var dc = new ApplicationDbContext())
                    return _mapping[lookupName]?.Invoke(dc, id);
            }
            else throw new Exception($"The lookup '{lookupName}' is not a valid lookup item");
        }

        public bool IsLookupField(string name) => _mapping.ContainsKey(name);

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

        public void Dispose()
        {
            if (_dc.IsValueCreated)
                DataContext.Dispose();
        }
    }
}
