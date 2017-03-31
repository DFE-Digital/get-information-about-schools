using Edubase.Services.Groups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupReadApiService : IGroupReadService
    {
        private const string ApiSuggestPath = "/suggest/group";
        private readonly HttpClientWrapper _httpClient;

        public GroupReadApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<bool> ExistsAsync(CompaniesHouseNumber number)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string name, int? localAuthorityId = default(int?), int? existingGroupUId = default(int?))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GroupModel>> GetAllByEstablishmentUrnAsync(int urn)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResultDto<GroupModel>> GetAsync(int uid, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<GroupModel> GetByEstablishmentUrnAsync(int urn)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GroupChangeDto>> GetChangeHistoryAsync(int uid, int take, IPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<List<EstablishmentGroupModel>> GetEstablishmentGroupsAsync(int groupUid)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel model)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel original, GroupModel model)
        {
            throw new NotImplementedException();
        }

        public Task<int[]> GetParentGroupIdsAsync(int establishmentUrn)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSearchResult<SearchGroupDocument>> SearchAsync(GroupSearchPayload payload, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSearchResult<SearchGroupDocument>> SearchByIdsAsync(string groupId, int? groupUId, string companiesHouseNumber, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            return await _httpClient.GetAsync<List<GroupSuggestionItem>>($"{ApiSuggestPath}?q={text}&take={take}");
        }
    }
}
