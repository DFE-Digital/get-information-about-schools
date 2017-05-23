using Edubase.Services.Domain;
using Edubase.Services.Establishments.Downloads;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Establishments
{
    public class EstablishmentDownloadApiService : IEstablishmentDownloadService
    {
        private readonly HttpClientWrapper _httpClient;
        
        public EstablishmentDownloadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return await _httpClient.GetAsync<ProgressDto>("establishment/search/download/progress?id=" + taskId, principal);
        }

        public async Task<Guid> SearchWithDownloadGenerationAsync(EstablishmentSearchDownloadPayload payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiResultDto<Guid>>("establishment/search/download/generate", payload, principal)).Response.Value;
        }
        
    }
}
