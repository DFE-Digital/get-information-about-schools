using Edubase.Services.Groups.Downloads;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using System.Security.Principal;
using System;
using Edubase.Services.Groups.Search;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupDownloadApiService : IGroupDownloadService
    {
        private readonly HttpClientWrapper _httpClient;

        public GroupDownloadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DownloadDto> DownloadGroupHistory(int groupUId, DownloadType downloadType, IPrincipal principal) => (await _httpClient.GetAsync<DownloadDto>($"group/{groupUId}/changes/download?format={downloadType}", principal)).GetResponse();

        public async Task<DownloadDto> DownloadGroupData(int groupUId, IPrincipal principal) => (await _httpClient.GetAsync<DownloadDto>($"group/{groupUId}/download", principal)).GetResponse();

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<ProgressDto>("group/search/download/progress?id=" + taskId, principal)).GetResponse();
        }

        public async Task<Guid> SearchWithDownloadGenerationAsync(SearchDownloadDto<GroupSearchPayload> payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiResultDto<Guid>>("group/search/download/generate", payload, principal)).GetResponse().Value;
        }
    }
}
