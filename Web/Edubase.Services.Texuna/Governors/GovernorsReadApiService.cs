using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Governors;
using Edubase.Services.Establishments;

namespace Edubase.Services.Texuna.Governors
{
    public class GovernorsReadApiService : IGovernorsReadService
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly IEstablishmentReadService _establishmentReadService;

        public GovernorsReadApiService(HttpClientWrapper httpClient, IEstablishmentReadService establishmentReadService)
        {
            _httpClient = httpClient;
            _establishmentReadService = establishmentReadService;
        }

        public async Task<GovernorDisplayPolicy> GetDisplayPolicyAsync(eLookupGovernorRole role, int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null)
        {
            // The API returns the governance display policies within the API endpoint to fetch governor details,
            // therefore we must fetch the governors and inspect the display policies within this response.
            var governorsDetailsDto = await GetGovernorListAsync(urn, groupUId, principal);
            var governorDisplayPolicy = governorsDetailsDto.RoleDisplayPolicies[role];

            return governorDisplayPolicy;
        }

        public async Task<GovernorEditPolicy> GetEditPolicyAsync(eLookupGovernorRole role, bool isGroup, IPrincipal principal)
            => (await _httpClient.GetAsync<GovernorEditPolicy>($"governor/{(int) role}/edit-policy?isForGroup={isGroup.ToString().ToLower()}", principal)).GetResponse();

        public async Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal)
        {
            var retVal = (await _httpClient.GetAsync<GovernorModel>($"governor/{gid}", principal)).GetResponse();
            return retVal;
        }

        public async Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null)
        {
            var retVal = (await _httpClient.GetAsync<GovernorsDetailsTexunaDto>($"governors?{(groupUId.HasValue ? "uid" : "urn")}={(urn.HasValue ? urn : groupUId)}", principal)).GetResponse();
            return retVal;
        }

        public async Task<GovernorPermissions> GetGovernorPermissions(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null)
            => (await _httpClient.GetAsync<GovernorPermissions>($"governors/permissions?{(groupUId.HasValue ? "uid" : "urn")}={(urn.HasValue ? urn : groupUId)}", principal)).GetResponse();

        public async Task<ApiPagedResult<SearchGovernorModel>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal)
            => (await _httpClient.PostAsync<ApiPagedResult<SearchGovernorModel>>("governor/search", payload, principal)).GetResponse();

        public async Task<IEnumerable<GovernorModel>> GetSharedGovernorsAsync(int establishmentUrn, IPrincipal principal)
        {
            var retVal = (await _httpClient.GetAsync<GovernorModel[]>($"governors/shared/{establishmentUrn}", principal)).GetResponse();
            return retVal;
        }
        public async Task<string> GetGovernorBulkUpdateTemplateUri(IPrincipal principal) => (await _httpClient.GetAsync<FileDownloadDto>($"governor/bulk-update/template", principal)).GetResponse().Url;
    }
}
