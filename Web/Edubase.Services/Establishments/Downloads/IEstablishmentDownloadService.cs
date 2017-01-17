using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Search;

namespace Edubase.Services.Establishments.Downloads
{
    public interface IEstablishmentDownloadService
    {
        Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId);
        Task SearchWithDownloadGenerationAsync(Guid taskId, EstablishmentSearchPayload payload, IPrincipal principal, 
            EstablishmentDownloadService.eDataSet dataSet);
        Task<SearchDownloadGenerationProgressDto> SearchWithDownloadGeneration_InitialiseAsync();
    }
}