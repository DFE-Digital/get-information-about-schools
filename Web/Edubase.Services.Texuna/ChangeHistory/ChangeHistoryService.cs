using Edubase.Services.Domain;
using Edubase.Services.Texuna.ChangeHistory.Models;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.ChangeHistory
{
    public class ChangeHistoryService
    {
        private readonly HttpClientWrapper _httpClient;

        public ChangeHistoryService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string[]> GetEstablishmentFieldsAsync(IPrincipal principal)
            => (await _httpClient.GetAsync<string[]>("establishment/change-history/fields", principal)).GetResponse();

        public async Task<ApiSearchResult<ChangeHistorySearchItem>> SearchAsync(SearchChangeHistoryBrowsePayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ApiSearchResult<ChangeHistorySearchItem>>("change-history", payload, principal)).GetResponse();

        public async Task<ProgressDto> SearchWithDownloadGenerationAsync(SearchChangeHistoryDownloadPayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ProgressDto>("change-history/download", payload, principal)).GetResponse();

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal) 
            => (await _httpClient.GetAsync<ProgressDto>($"change-history/download/progress/{taskId}", principal)).GetResponse();
    }
}
