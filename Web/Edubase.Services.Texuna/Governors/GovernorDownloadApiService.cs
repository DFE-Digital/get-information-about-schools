using Edubase.Services.Domain;
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

        public GovernorDownloadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return await _httpClient.GetAsync<SearchDownloadGenerationProgressDto>("governor/search/download/progress?id=" + taskId, principal);
        }

        public async Task<Guid> SearchWithDownloadGenerationAsync(SearchDownloadDto<GovernorSearchPayload> payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiResultDto<Guid>>("governor/search/download/generate", payload, principal)).Value;
        }
    }
}
