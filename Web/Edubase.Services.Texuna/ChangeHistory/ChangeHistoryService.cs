using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Texuna.ChangeHistory.Models;

namespace Edubase.Services.Texuna.ChangeHistory
{
    public class ChangeHistoryService : IChangeHistoryService
    {
        private readonly HttpClientWrapper _httpClient;

        public ChangeHistoryService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EstablishmentField>> GetEstablishmentFieldsAsync(IPrincipal principal)
        {
            return (await _httpClient.GetAsync<List<EstablishmentField>>("establishment/change-history/fields",
                principal)).GetResponse();
        }

        public async Task<ApiPagedResult<ChangeHistorySearchItem>> SearchAsync(SearchChangeHistoryBrowsePayload payload,
            IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiPagedResult<ChangeHistorySearchItem>>("change-history", payload,
                principal)).GetResponse();
        }

        public async Task<List<UserGroupModel>> GetSuggesterGroupsAsync(IPrincipal principal)
        {
            return (await _httpClient.GetAsync<List<UserGroupModel>>("groups/suggesters", principal)).GetResponse();
        }

        public async Task<ProgressDto> SearchWithDownloadGenerationAsync(SearchChangeHistoryDownloadPayload payload,
            IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ProgressDto>("change-history/download", payload, principal))
                .GetResponse();
        }

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<ProgressDto>($"change-history/download/progress/{taskId}", principal))
                .GetResponse();
        }
    }
}
