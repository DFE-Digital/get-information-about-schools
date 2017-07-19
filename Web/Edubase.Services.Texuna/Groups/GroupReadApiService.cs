using Edubase.Services.Groups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using System.Security.Principal;
using Edubase.Services.Texuna.Models;
using System.Linq;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupReadApiService : IGroupReadService
    {
        private const string ApiSuggestPath = "suggest/group";
        private readonly HttpClientWrapper _httpClient;

        public GroupReadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ExistsAsync(IPrincipal principal, CompaniesHouseNumber? companiesHouseNumber = null, string groupId = null, int? existingGroupUId = null, string name = null, int? localAuthorityId = null)
        {
            var parameters = new Dictionary<string, string>
            {
                [nameof(groupId)] = groupId,
                [nameof(companiesHouseNumber)] = companiesHouseNumber?.ToString(),
                [nameof(existingGroupUId)] = existingGroupUId?.ToString(),
                [nameof(name)] = name,
                [nameof(localAuthorityId)] = localAuthorityId?.ToString()
            };
            var queryString = string.Join("&", parameters.Where(x => x.Value != null).Select(x => $"{x.Key}={x.Value}"));
            return (await _httpClient.GetAsync<BoolResult>($"group/exists?{queryString}", principal)).Response.Value;
        }

        public async Task<IEnumerable<GroupModel>> GetAllByEstablishmentUrnAsync(int urn, IPrincipal principal)
        {
            try
            {
                return (await _httpClient.GetAsync<List<GroupModel>>($"establishment/{urn}/groups", principal)).GetResponse();
            }
            catch (Exceptions.TexunaApiNotFoundException)
            {
                return Enumerable.Empty<GroupModel>();
            }
        }

        public async Task<ServiceResultDto<GroupModel>> GetAsync(int uid, IPrincipal principal) => new ServiceResultDto<GroupModel>((await _httpClient.GetAsync<GroupModel>($"group/{uid}", principal)).GetResponse());

        public async Task<bool> CanEditAsync(int uid, IPrincipal principal) => (await _httpClient.GetAsync<BoolResult>($"group/{uid}/canedit", principal)).GetResponse().Value;

        public Task<GroupModel> GetByEstablishmentUrnAsync(int urn, IPrincipal principal)
        {
            throw new NotImplementedException($"NOT REQUIRED; {nameof(GroupReadApiService)}::{nameof(GetByEstablishmentUrnAsync)}");
        }

        public async Task<IEnumerable<GroupChangeDto>> GetChangeHistoryAsync(int uid, int take, IPrincipal principal) =>
            (await _httpClient.GetAsync<List<GroupChangeDto>>($"group/{uid}/changes?take={take}", principal)).Response;

        public async Task<List<EstablishmentGroupModel>> GetEstablishmentGroupsAsync(int groupUid, IPrincipal principal, bool includeFutureDated = false) 
            => (await _httpClient.GetAsync<List<EstablishmentGroupModel>>($"group/{groupUid}/establishments?editMode={includeFutureDated}", principal)).GetResponse();

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel model)
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(GetModelChangesAsync)}");
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel original, GroupModel model)
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(GetModelChangesAsync)}");
        }

        public Task<int[]> GetParentGroupIdsAsync(int establishmentUrn, IPrincipal principal)
        {
            throw new NotImplementedException($"NOT REQUIRED POST INT: {nameof(GroupReadApiService)}::{nameof(GetParentGroupIdsAsync)}");
        }

        public async Task<ApiSearchResult<SearchGroupDocument>> SearchAsync(GroupSearchPayload payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiSearchResult<SearchGroupDocument>>("group/search", payload, principal)).Response;
        }

        public async Task<ApiSearchResult<SearchGroupDocument>> SearchByIdsAsync(string groupId, int? groupUId, string companiesHouseNumber, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<ApiSearchResult<SearchGroupDocument>>(string.Concat("group/searchbyids?",
                groupId.UrlTokenize("groupId"), 
                groupUId.UrlTokenize("groupUId"), 
                companiesHouseNumber.UrlTokenize("companiesHouseNumber")), principal)).Response;
        }

        public async Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            var suggestions = (await _httpClient.GetAsync<List<GroupSuggestionItem>>($"{ApiSuggestPath}?text={text}&take={take}",
                principal)).GetResponse();

            foreach (var suggestion in suggestions)
            {
                if (!suggestions.Any(s => string.Equals(s.Name, suggestion.Name) && s.GroupUId != suggestion.GroupUId))
                {
                    suggestion.GroupType = "";
                }
            }

            return suggestions;
        }
    }
}
