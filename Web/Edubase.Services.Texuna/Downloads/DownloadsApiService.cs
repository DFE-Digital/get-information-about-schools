using Edubase.Services.Downloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Downloads.Models;

namespace Edubase.Services.Texuna.Downloads
{
    public class DownloadsApiService : IDownloadsService
    {
        private readonly HttpClientWrapper _httpClient;

        public DownloadsApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FileDownload[]> GetListAsync()=> await _httpClient.GetAsync<FileDownload[]>($"downloads");
    }
}
