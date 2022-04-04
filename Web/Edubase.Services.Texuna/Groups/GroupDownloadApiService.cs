using Edubase.Services.Groups.Downloads;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using System.Security.Principal;
using System;
using Edubase.Services.Groups.Search;
using Newtonsoft.Json;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupDownloadApiService : IGroupDownloadService
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly IApiClientWrapper _apiClient;
        
        public GroupDownloadApiService(IHttpClientWrapper httpClient, IApiClientWrapper apiClient)
        {
            _httpClient = httpClient;
            _apiClient = apiClient;
        }

        public async Task<DownloadDto> DownloadGroupHistory(int groupUId, DownloadType downloadType, DateTime? dateFrom, DateTime? dateTo, string suggestedBy, IPrincipal principal) 
            => (await _httpClient.GetAsync<DownloadDto>($"group/{groupUId}/changes/download?format={downloadType}&dateFrom={(dateFrom.HasValue ? JsonConvert.SerializeObject(dateFrom.Value) : "")}&dateTo={(dateTo.HasValue ? JsonConvert.SerializeObject(dateTo.Value) : "")}&suggestedBy={suggestedBy}", principal)).GetResponse();

        public async Task<DownloadDto> DownloadGroupData(int groupUId, DownloadType downloadType, IPrincipal principal) => (await _httpClient.GetAsync<DownloadDto>($"group/{groupUId}/download?format={downloadType}", principal)).GetResponse();

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<ProgressDto>("group/search/download/progress?id=" + taskId, principal)).GetResponse();
        }

        public async Task<Guid> SearchWithDownloadGenerationAsync(SearchDownloadDto<GroupSearchPayload> payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiResultDto<Guid>>("group/search/download/generate", payload, principal)).GetResponse().Value;
        }

        public async Task<DownloadDto> GetGovernanceChangeHistoryDownloadAsync(int groupUId, DownloadType downloadType, IPrincipal principal) => (await _httpClient.GetAsync<DownloadDto>($"group/{groupUId}/governance/changes/download?format={downloadType}", principal)).GetResponse();
    }
}
