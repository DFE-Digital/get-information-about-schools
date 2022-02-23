using Edubase.Services.Domain;
using Edubase.Services.Establishments.Downloads;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Establishments
{
    public class EstablishmentDownloadApiService : IEstablishmentDownloadService
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ApiClientWrapper _apiClient;
        
        public EstablishmentDownloadApiService(HttpClientWrapper httpClient, ApiClientWrapper apiClient)
        {
            _httpClient = httpClient;
            _apiClient = apiClient;
        }

        public async Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<ProgressDto>("establishment/search/download/progress?id=" + taskId, principal)).GetResponse();
        }

        public async Task<Guid> SearchWithDownloadGenerationAsync(EstablishmentSearchDownloadPayload payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiResultDto<Guid>>("establishment/search/download/generate", payload, principal)).GetResponse().Value;
        }

        public async Task<IEnumerable<EstablishmentSearchDownloadCustomField>> GetSearchDownloadCustomFields(IPrincipal principal)
        {
            return (await _httpClient.GetAsync<IEnumerable<EstablishmentSearchDownloadCustomField>>("establishment/search/download/custom-fields", principal)).GetResponse();
        }

    }
}
