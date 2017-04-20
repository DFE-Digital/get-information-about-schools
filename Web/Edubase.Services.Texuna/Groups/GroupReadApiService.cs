using Edubase.Services.Groups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System.Security.Principal;
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

        public Task<bool> ExistsAsync(CompaniesHouseNumber number)
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(ExistsAsync)}");
        }

        public Task<bool> ExistsAsync(string groupId, int? existingGroupUId = default(int?))
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(ExistsAsync)}");
        }

        public Task<bool> ExistsAsync(string name, int? localAuthorityId = default(int?), int? existingGroupUId = default(int?))
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(ExistsAsync)}");
        }

        public async Task<IEnumerable<GroupModel>> GetAllByEstablishmentUrnAsync(int urn) => await _httpClient.GetAsync<List<GroupModel>>($"establishment/{urn}/groups");

        public async Task<ServiceResultDto<GroupModel>> GetAsync(int uid, IPrincipal principal) => new ServiceResultDto<GroupModel>(await _httpClient.GetAsync<GroupModel>($"group/{uid}"));

        public Task<GroupModel> GetByEstablishmentUrnAsync(int urn)
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(GetByEstablishmentUrnAsync)}");
        }

        public async Task<IEnumerable<GroupChangeDto>> GetChangeHistoryAsync(int uid, int take, IPrincipal user)
        {
            // TODO TEXCHANGE: need to link this to the API when they've done it!
            return Enumerable.Empty<GroupChangeDto>();
        }

        public async Task<List<EstablishmentGroupModel>> GetEstablishmentGroupsAsync(int groupUid) => await _httpClient.GetAsync<List<EstablishmentGroupModel>>($"group/{groupUid}/establishments");

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel model)
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(GetModelChangesAsync)}");
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel original, GroupModel model)
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(GetModelChangesAsync)}");
        }

        public Task<int[]> GetParentGroupIdsAsync(int establishmentUrn)
        {
            throw new NotImplementedException($"{nameof(GroupReadApiService)}::{nameof(GetParentGroupIdsAsync)}");
        }

        public async Task<ApiSearchResult<SearchGroupDocument>> SearchAsync(GroupSearchPayload payload, IPrincipal principal)
        {
            return await _httpClient.PostAsync<ApiSearchResult<SearchGroupDocument>>("group/search", payload);
        }

        public async Task<ApiSearchResult<SearchGroupDocument>> SearchByIdsAsync(string groupId, int? groupUId, string companiesHouseNumber, IPrincipal principal)
        {
            return await _httpClient.GetAsync<ApiSearchResult<SearchGroupDocument>>($"group/searchbyids?groupId={groupId}&groupUId={groupUId}&companiesHouseNumber={companiesHouseNumber}");
        }

        public async Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            return await _httpClient.GetAsync<List<GroupSuggestionItem>>($"{ApiSuggestPath}?text={text}&take={take}");
        }
    }
}
