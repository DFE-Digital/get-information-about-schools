using Edubase.Services.Governors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public GovernorDisplayPolicy GetEditorDisplayPolicy(eLookupGovernorRole role)
        {
            throw new NotImplementedException();
        }

        public Task<GovernorModel> GetGovernorAsync(int gid)
        {
            throw new NotImplementedException();
        }

        public async Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null)
        {
            return await _httpClient.GetAsync<GovernorsDetailsTexunaDto>($"governors?uId={groupUId}&urn={urn}");
        }

        public async Task<ApiSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload)
        {
            return await _httpClient.PostAsync<ApiSearchResult<SearchGovernorDocument>>("governor/search", payload);
        }
    }
}
