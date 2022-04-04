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
using Edubase.Services.Core;
using Newtonsoft.Json;
using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services.Lookup;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupReadApiService : IGroupReadService
    {
        private const string ApiSuggestPath = "suggest/group";
        private readonly IHttpClientWrapper _httpClient;
        private readonly IApiClientWrapper _apiClient;
        private readonly ICachedLookupService _cachedLookupService;
        
        public GroupReadApiService(IHttpClientWrapper httpClient, IApiClientWrapper apiClient, ICachedLookupService cachedLookupService)
        {
            _httpClient = httpClient;
            _apiClient = apiClient;
            _cachedLookupService = cachedLookupService;
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

        public async Task<ServiceResultDto<GroupModel>> GetAsync(int uid, IPrincipal principal) => new ServiceResultDto<GroupModel>((await _apiClient.GetAsync<GroupModel>($"group/{uid}", principal, false)).Response);

        public async Task<bool> CanEditAsync(int uid, IPrincipal principal) => (await _httpClient.GetAsync<BoolResult>($"group/{uid}/canedit", principal)).GetResponse().Value;

        public async Task<bool> CanEditGovernanceAsync(int uid, IPrincipal principal) => (await _httpClient.GetAsync<BoolResult>($"group/{uid}/governance/canedit", principal)).GetResponse().Value;

        public async Task<PaginatedResult<GroupChangeDto>> GetChangeHistoryAsync(int uid, int skip, int take, string sortBy, IPrincipal principal)
        {
            var changes = (await _httpClient.GetAsync<ApiPagedResult<GroupChangeDto>>($"group/{uid}/changes?skip={skip}&take={take}&sortby={sortBy}", principal)).GetResponse(); 
            return new PaginatedResult<GroupChangeDto>(skip, take, changes.Count, changes.Items);
        }

        public async Task<PaginatedResult<GroupChangeDto>> GetChangeHistoryAsync(int uid, int skip, int take, string sortBy, DateTime? dateFrom, DateTime? dateTo, string suggestedBy, IPrincipal principal)
        {
            var changes = (await _httpClient.GetAsync<ApiPagedResult<GroupChangeDto>>($"group/{uid}/changes?skip={skip}&take={take}&sortby={sortBy}&dateFrom={(dateFrom != null ? JsonConvert.SerializeObject(dateFrom) : "")}&dateTo={(dateTo != null ? JsonConvert.SerializeObject(dateTo) : "")}&suggestedBy={suggestedBy}", principal)).GetResponse();
            return new PaginatedResult<GroupChangeDto>(skip, take, changes.Count, changes.Items);
        }

        public async Task<PaginatedResult<GroupChangeDto>> GetGovernanceChangeHistoryAsync(int uid, int skip, int take, string sortBy, IPrincipal principal)
        {
            var changes = (await _httpClient.GetAsync<ApiPagedResult<GroupChangeDto>>($"group/{uid}/governance/changes?skip={skip}&take={take}&sortby={sortBy}", principal)).GetResponse(); 
            return new PaginatedResult<GroupChangeDto>(skip, take, changes.Count, changes.Items);
        }

        public async Task<List<EstablishmentGroupModel>> GetEstablishmentGroupsAsync(int groupUid, IPrincipal principal, bool includeFutureDated = false) 
            => (await _httpClient.GetAsync<List<EstablishmentGroupModel>>($"group/{groupUid}/establishments?editMode={includeFutureDated}", principal)).GetResponse();

        public async Task<IEnumerable<LinkedGroupModel>> GetLinksAsync(int uid, IPrincipal principal)
            => (await _httpClient.GetAsync<List<LinkedGroupModel>>($"group/{uid}/links", principal)).GetResponse();

        public async Task<ApiPagedResult<SearchGroupDocument>> SearchAsync(GroupSearchPayload payload, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ApiPagedResult<SearchGroupDocument>>("group/search", payload, principal)).GetResponse();
        }

        public async Task<ApiPagedResult<SearchGroupDocument>> SearchByIdsAsync(string groupId, int? groupUId, string companiesHouseNumber, int? ukprn, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<ApiPagedResult<SearchGroupDocument>>(string.Concat("group/searchbyids?",
                groupId.UrlTokenize("groupId"), 
                groupUId.UrlTokenize("groupUId"),
                ukprn.UrlTokenize("ukprn"),
                companiesHouseNumber.UrlTokenize("companiesHouseNumber")), principal)).GetResponse();
        }

        public async Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            if (text.Clean() == null) return Enumerable.Empty<GroupSuggestionItem>();

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

        public async Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel model, IPrincipal principal)
        {
            var originalModel = (await GetAsync(model.GroupUId.Value, principal)).GetResult();
            return await GetModelChangesAsync(originalModel, model);
        }

        public async Task<List<ChangeDescriptorDto>> GetModelChangesAsync(GroupModel original, GroupModel model)
        {
            var changes = ReflectionHelper.DetectGroupChanges(model, original);
            var retVal = new List<ChangeDescriptorDto>();

            foreach (var change in changes)
            {
                if (_cachedLookupService.IsLookupField(change.Name))
                {
                    change.OldValue = await _cachedLookupService.GetNameAsync(change.Name, change.OldValue.ToInteger());
                    change.NewValue = await _cachedLookupService.GetNameAsync(change.Name, change.NewValue.ToInteger());
                }

                if (change.DisplayName == null)
                {
                    change.DisplayName = PropertyName2Label(change.Name);
                }

                retVal.Add(new ChangeDescriptorDto
                {
                    Id = change.Name,
                    Name = change.DisplayName ?? change.Name,
                    NewValue = change.NewValue.Clean(),
                    OldValue = change.OldValue.Clean(),
                    Tag = change.Tag,
                    RequiresApproval = false
                });
            }

            return retVal;
        }

        private string PropertyName2Label(string name)
        {
            if (name.EndsWith("Id", StringComparison.Ordinal))
            {
                name = name.Substring(0, name.Length - 2);
            }

            name = name.Replace("_", "").Replace("Group.", string.Empty);
            name = name.ToProperCase(true);
            return name;
        }
    }
}
