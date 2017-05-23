using Edubase.Services.Downloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Downloads.Models;
using System.Security.Principal;
using Edubase.Services.Domain;

namespace Edubase.Services.Texuna.Downloads
{
    public class DownloadsApiService : IDownloadsService
    {
        private readonly HttpClientWrapper _httpClient;

        public DownloadsApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FileDownload[]> GetListAsync(IPrincipal principal) => await _httpClient.GetAsync<FileDownload[]>($"downloads", principal);

        public async Task<ScheduledExtractsResult> GetScheduledExtractsAsync(int skip, int take, IPrincipal principal)
            => await _httpClient.GetAsync<ScheduledExtractsResult>($"scheduled-extracts?skip={skip}&take={take}", principal);

        public async Task<ApiResultDto<Guid>> GenerateScheduledExtractAsync(int id, IPrincipal principal) 
            => await _httpClient.GetAsync<ApiResultDto<Guid>>($"scheduled-extract/generate/{id}", principal);

        public async Task<ProgressDto> GetProgressOfScheduledExtractGenerationAsync(Guid id, IPrincipal principal)
            => await _httpClient.GetAsync<ProgressDto>($"scheduled-extract/progress/{id}", principal);


    }
}
