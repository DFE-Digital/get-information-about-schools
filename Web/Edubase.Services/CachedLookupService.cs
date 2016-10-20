using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System.Runtime.Caching;
using Edubase.Common;

namespace Edubase.Services
{
    public class CachedLookupService : ILookupService
    {
        private LookupService _svc = new LookupService();

        public List<AdmissionsPolicy> AdmissionsPoliciesGetAll() => Cacher.Auto(_svc.AdmissionsPoliciesGetAll);

        public Task<List<AdmissionsPolicy>> AdmissionsPoliciesGetAllAsync() => Cacher.AutoAsync(_svc.AdmissionsPoliciesGetAllAsync);

        public List<EducationPhase> EducationPhasesGetAll() => Cacher.Auto(_svc.EducationPhasesGetAll);

        public Task<List<EducationPhase>> EducationPhasesGetAllAsync() => Cacher.AutoAsync(_svc.EducationPhasesGetAllAsync);

        public List<EstablishmentStatus> EstablishmentStatusesGetAll() => Cacher.Auto(_svc.EstablishmentStatusesGetAll);

        public Task<List<EstablishmentStatus>> EstablishmentStatusesGetAllAsync() => Cacher.AutoAsync(_svc.EstablishmentStatusesGetAllAsync);

        public List<Gender> GendersGetAll() => Cacher.Auto(_svc.GendersGetAll);

        public Task<List<Gender>> GendersGetAllAsync() => Cacher.AutoAsync(_svc.GendersGetAllAsync);
        
        public List<HeadTitle> HeadTitleGetAll() => Cacher.Auto(_svc.HeadTitleGetAll);

        public Task<List<HeadTitle>> HeadTitleGetAllAsync() => Cacher.AutoAsync(_svc.HeadTitleGetAllAsync);
        
        public List<LocalAuthority> LocalAuthorityGetAll() => Cacher.Auto(_svc.LocalAuthorityGetAll);

        public Task<List<LocalAuthority>> LocalAuthorityGetAllAsync() => Cacher.AutoAsync(_svc.LocalAuthorityGetAllAsync);

        public List<EstablishmentType> EstablishmentTypesGetAll() => Cacher.Auto(_svc.EstablishmentTypesGetAll);

        public Task<List<EstablishmentType>> EstablishmentTypesGetAllAsync() => Cacher.AutoAsync(_svc.EstablishmentTypesGetAllAsync);


        public string GetName(string lookupName, int id)
        {
            throw new NotImplementedException();
        }

        public bool IsLookupField(string name)
        {
            throw new NotImplementedException();
        }

        public void Dispose() => _svc.Dispose();
    }
}
