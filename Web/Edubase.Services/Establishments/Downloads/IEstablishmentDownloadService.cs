using Edubase.Services.Domain;
using Edubase.Services.Establishments.Search;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Downloads
{
    public interface IEstablishmentDownloadService
    {
        Task<Guid> SearchWithDownloadGenerationAsync(EstablishmentSearchDownloadPayload payload, IPrincipal principal);
        Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal);
    }
}