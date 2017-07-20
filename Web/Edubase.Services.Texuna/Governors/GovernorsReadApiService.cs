using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Governors;
using System;

namespace Edubase.Services.Texuna.Governors
{
    public class GovernorsReadApiService : IGovernorsReadService
    {
        private readonly HttpClientWrapper _httpClient;

        public GovernorsReadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GovernorDisplayPolicy> GetEditorDisplayPolicyAsync(eLookupGovernorRole role, bool isGroup, IPrincipal principal)
            => (await _httpClient.GetAsync<GovernorDisplayPolicy>($"governor/{(int)role}/edit-policy?isForGroup={isGroup.ToString().ToLower()}", principal)).GetResponse();

        public async Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal) => (await _httpClient.GetAsync<GovernorModel>($"governor/{gid}", principal)).GetResponse();

        public async Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null) 
            => (await _httpClient.GetAsync<GovernorsDetailsTexunaDto>($"governors?{(groupUId.HasValue ? "uid" : "urn")}={(urn.HasValue ? urn : groupUId)}", principal)).GetResponse();

        public async Task<ApiSearchResult<SearchGovernorModel>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal) 
            => (await _httpClient.PostAsync<ApiSearchResult<SearchGovernorModel>>("governor/search", payload, principal)).GetResponse();
        
        public async Task<IEnumerable<GovernorModel>> GetSharedGovernorsAsync(int establishmentUrn, IPrincipal principal)
            => (await _httpClient.GetAsync<GovernorModel[]>($"governors/shared/{establishmentUrn}", principal)).GetResponse();

        public async Task<string> GetGovernorBulkUpdateTemplateUri(IPrincipal principal) => (await _httpClient.GetAsync<FileDownloadDto>($"governor/bulk-update/template", principal)).GetResponse().Url;
    }
}
