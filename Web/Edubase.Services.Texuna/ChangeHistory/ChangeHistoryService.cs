using Edubase.Services.Domain;
using Edubase.Services.Texuna.ChangeHistory.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.ChangeHistory
{
    public class ChangeHistoryService : IChangeHistoryService
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ApiClientWrapper _apiClient;
        
        public ChangeHistoryService(HttpClientWrapper httpClient, ApiClientWrapper apiClient)
        {
            _httpClient = httpClient;
            _apiClient = apiClient;
        }

        public async Task<List<EstablishmentField>> GetEstablishmentFieldsAsync(IPrincipal principal)
            => (await _httpClient.GetAsync<List<EstablishmentField>>("establishment/change-history/fields", principal)).GetResponse();

        public async Task<ApiPagedResult<ChangeHistorySearchItem>> SearchAsync(SearchChangeHistoryBrowsePayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ApiPagedResult<ChangeHistorySearchItem>>("change-history", payload, principal)).GetResponse();

        public async Task<List<UserGroupModel>> GetSuggesterGroupsAsync(IPrincipal principal)
            => (await _httpClient.GetAsync<List<UserGroupModel>>("groups/suggesters", principal)).GetResponse();

        [Obsolete("Please use GetSuggesterGroupsAsync", true)]
        public async Task<List<UserGroupModel>> GetApproversGroupsAsync(IPrincipal principal)
        {
            throw new NotImplementedException("GetApproversGroupsAsync is not implemented because it returns the exact same information as GetSuggesterGroupsAsync");
        }

        public async Task<ProgressDto> SearchWithDownloadGenerationAsync(SearchChangeHistoryDownloadPayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ProgressDto>("change-history/download", payload, principal)).GetResponse();

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal) 
            => (await _httpClient.GetAsync<ProgressDto>($"change-history/download/progress/{taskId}", principal)).GetResponse();
    }
}
