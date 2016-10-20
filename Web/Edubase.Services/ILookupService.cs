using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;

namespace Edubase.Services
{
    public interface ILookupService
    {
        List<AdmissionsPolicy> AdmissionsPoliciesGetAll();
        Task<List<AdmissionsPolicy>> AdmissionsPoliciesGetAllAsync();
        void Dispose();
        List<EducationPhase> EducationPhasesGetAll();
        Task<List<EducationPhase>> EducationPhasesGetAllAsync();
        List<EstablishmentStatus> EstablishmentStatusesGetAll();
        Task<List<EstablishmentStatus>> EstablishmentStatusesGetAllAsync();
        List<Gender> GendersGetAll();
        Task<List<Gender>> GendersGetAllAsync();
        string GetName(string lookupName, int id);
        List<HeadTitle> HeadTitleGetAll();
        Task<List<HeadTitle>> HeadTitleGetAllAsync();
        bool IsLookupField(string name);
        List<LocalAuthority> LocalAuthorityGetAll();
        Task<List<LocalAuthority>> LocalAuthorityGetAllAsync();
        List<EstablishmentType> EstablishmentTypesGetAll();
        Task<List<EstablishmentType>> EstablishmentTypesGetAllAsync();
    }
}