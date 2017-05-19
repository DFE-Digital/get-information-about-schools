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

        public async Task<DownloadDto> DownloadGroupHistory(int groupUid, DownloadType downloadType, IPrincipal principal) => await _httpClient.GetAsync<DownloadDto>($"group/{groupUid}/changes/download?format={downloadType}", principal);

        public async Task<SearchDownloadGenerationProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return await _httpClient.GetAsync<SearchDownloadGenerationProgressDto>("group/search/download/progress?id=" + taskId, principal);
        }

        public async Task<Guid> SearchWithDownloadGenerationAsync(SearchDownloadDto<GroupSearchPayload> payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiResultDto<Guid>>("group/search/download/generate", payload, principal)).Value;
        }
    }
}
