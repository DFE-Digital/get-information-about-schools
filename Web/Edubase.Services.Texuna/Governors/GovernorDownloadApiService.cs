using Edubase.Services.Domain;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Governors.Search;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Governors
{
    public class GovernorDownloadApiService : IGovernorDownloadService
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ApiClientWrapper _apiClient;
        
        public GovernorDownloadApiService(HttpClientWrapper httpClient, ApiClientWrapper apiClient)
        {
            _httpClient = httpClient;
            _apiClient = apiClient;
        }

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<ProgressDto>("governor/search/download/progress?id=" + taskId, principal)).Response;
        }

        public async Task<Guid> SearchWithDownloadGenerationAsync(GovernorSearchDownloadPayload payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiResultDto<Guid>>("governor/search/download/generate", payload, principal)).Response.Value;
        }
    }
}
