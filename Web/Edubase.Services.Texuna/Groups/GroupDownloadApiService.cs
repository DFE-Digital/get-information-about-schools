using Edubase.Services.Groups.Downloads;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using System.Security.Principal;

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
    }
}
