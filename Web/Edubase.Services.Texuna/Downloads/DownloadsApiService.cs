using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Downloads;
using Edubase.Services.Downloads.Models;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Texuna.Models;

namespace Edubase.Services.Texuna.Downloads
{
    public class DownloadsApiService : IDownloadsService
    {
        private readonly HttpClientWrapper _httpClient;

        public DownloadsApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FileDownload[]> GetListAsync(IPrincipal principal) => (await _httpClient.GetAsync<FileDownload[]>($"downloads", principal)).Response;
        
        public async Task<PaginatedResult<ScheduledExtract>> GetScheduledExtractsAsync(int skip, int take, IPrincipal principal)
        {
            try
            {
                var set = (await _httpClient.GetAsync<ApiPagedResult<ScheduledExtract>>($"scheduled-extracts?skip={skip}&take={take}", principal)).GetResponse();
                return new PaginatedResult<ScheduledExtract>(skip, take, set.Count, set.Items);
            }
            catch (Exception)
            {
                return new PaginatedResult<ScheduledExtract>(skip, take, 0, Enumerable.Empty<ScheduledExtract>().ToList());
            }
            
        }

        public async Task<string> GenerateExtractAsync(string id, IPrincipal principal)
            => (await _httpClient.PostAsync<string>($"download/generate/{id}", null, principal)).Response;

        public async Task<ProgressDto> GetProgressOfGeneratedExtractAsync(Guid id, IPrincipal principal)
            => (await _httpClient.GetAsync<ProgressDto>($"download/progress/{id}", principal)).Response;

        public async Task<string> GenerateScheduledExtractAsync(int id, IPrincipal principal) 
            => (await _httpClient.PostAsync<string>($"scheduled-extract/generate/{id}", null, principal)).Response;

        public async Task<ProgressDto> GetProgressOfScheduledExtractGenerationAsync(Guid id, IPrincipal principal)
            => (await _httpClient.GetAsync<ProgressDto>($"scheduled-extract/progress/{id}", principal)).Response;

        public async Task<bool> IsDownloadAvailable(string path, string id, IPrincipal principal)
            => (await _httpClient.GetAsync<BoolResult>($"download/available?resource={path}&id={id}", principal)).GetResponse().Value;
    }
}
