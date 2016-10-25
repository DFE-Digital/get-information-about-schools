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
        List<HeadTitle> HeadTitleGetAll();
        Task<List<HeadTitle>> HeadTitleGetAllAsync();
        
        List<LocalAuthority> LocalAuthorityGetAll();
        Task<List<LocalAuthority>> LocalAuthorityGetAllAsync();
        List<EstablishmentType> EstablishmentTypesGetAll();
        Task<List<EstablishmentType>> EstablishmentTypesGetAllAsync();

        Task<List<GroupType>> GroupeTypesGetAllAsync();
        List<GroupType> GroupeTypesGetAll();

    }
}