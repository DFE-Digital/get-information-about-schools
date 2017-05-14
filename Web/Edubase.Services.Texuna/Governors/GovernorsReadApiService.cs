using Edubase.Services.Governors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using System.Security.Principal;
using Edubase.Services.Domain;

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
            => await _httpClient.GetAsync<GovernorDisplayPolicy>($"/governor/{(int)role}/edit-policy?isForGroup={isGroup.ToString().ToLower()}", principal);

        public async Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal) => await _httpClient.GetAsync<GovernorModel>($"governor/{gid}", principal);

        public async Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null) 
            => await _httpClient.GetAsync<GovernorsDetailsTexunaDto>($"governors?{(groupUId.HasValue ? "uid" : "urn")}={(urn.HasValue ? urn : groupUId)}", principal);

        public async Task<ApiSearchResult<GovernorModel>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal) 
            => await _httpClient.PostAsync<ApiSearchResult<GovernorModel>>("governor/search", payload, principal);
        
        public async Task<IEnumerable<GovernorModel>> GetSharedGovernorsAsync(int establishmentUrn, IPrincipal principal)
            => await _httpClient.GetAsync<GovernorModel[]>($"/governors/shared/{establishmentUrn}", principal);
    }
}
