using Edubase.Services.Governors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Governors
{
    public class GovernorsReadApiService : IGovernorsReadService
    {
        private readonly HttpClientWrapper _httpClient;

        public GovernorsReadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public GovernorDisplayPolicy GetEditorDisplayPolicy(eLookupGovernorRole role, bool isGroup, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public async Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null) 
            => await _httpClient.GetAsync<GovernorsDetailsTexunaDto>($"governors?{(groupUId.HasValue ? "uid" : "urn")}={(urn.HasValue ? urn : groupUId)}", principal);

        public async Task<ApiSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal)
        {
            return await _httpClient.PostAsync<ApiSearchResult<SearchGovernorDocument>>("governor/search", payload, principal);
        }

        public Task<IEnumerable<GovernorModel>> GetSharedGovernorsAsync(int establishmentUrn, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<GovernorModel> GetSharedGovernorAsync(int governorId, int establishmentUrn, IPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
