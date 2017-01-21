using AutoMapper;
using Edubase.Data.Entity;
using Edubase.Services.Enums;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Linq;
using Edubase.Common;

namespace Edubase.Services.Groups
{
    using System;
    using Data;
    using Data.DbContext;
    using Data.Repositories.Groups;
    using eStatus = eLookupGroupStatus;
    using Data.Repositories.Groups.Abstract;

    public class GroupReadService : IGroupReadService
    {
        private IApplicationDbContext _dbContext;
        private IMapper _mapper;
        private IAzureSearchEndPoint _azureSearchService;
        private ICachedGroupReadRepository _groupRepository;
        private ICachedEstablishmentGroupReadRepository _cachedEstablishmentGroupReadRepository;

        /// <summary>
        /// Allow these roles to see establishments of all statuses
        /// </summary>
        private readonly string[] _nonStatusRestrictiveRoles = new[] { EdubaseRoles.EFA, EdubaseRoles.AOS, EdubaseRoles.FSG,
            EdubaseRoles.IEBT, EdubaseRoles.School, EdubaseRoles.PRU, EdubaseRoles.Admin };
        
        public GroupReadService(IApplicationDbContext dc, 
            IMapper mapper, 
            IAzureSearchEndPoint azureSearchService,
            ICachedGroupReadRepository groupRepository,
            ICachedEstablishmentGroupReadRepository cachedEstablishmentGroupReadRepository)
        {
            _dbContext = dc;
            _mapper = mapper;
            _azureSearchService = azureSearchService;
            _groupRepository = groupRepository;
            _cachedEstablishmentGroupReadRepository = cachedEstablishmentGroupReadRepository;
        }

        public async Task<GroupModel> GetByEstablishmentUrnAsync(int urn)
        {
            var g = (await _dbContext.EstablishmentGroups.Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.EstablishmentUrn == urn && x.IsDeleted == false))?.Group;
            if (g != null) return _mapper.Map<GroupCollection, GroupModel>(g);
            else return null;
        }

        public async Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            var oDataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);
            if (IsRoleRestrictedOnStatus(principal)) oDataFilters.Add(nameof(GroupModel.StatusId), (int)eStatus.Open);
            return await _azureSearchService.SuggestAsync<GroupSuggestionItem>(GroupsSearchIndex.INDEX_NAME, GroupsSearchIndex.SUGGESTER_NAME, text, oDataFilters.ToString(), take);
        }

        public async Task<AzureSearchResult<SearchGroupDocument>> SearchAsync(GroupSearchPayload payload, IPrincipal principal)
        {
            var oDataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);
            if (IsRoleRestrictedOnStatus(principal)) oDataFilters.Add(nameof(GroupModel.StatusId), (int)eStatus.Open);
            
            return await _azureSearchService.SearchAsync<SearchGroupDocument>(GroupsSearchIndex.INDEX_NAME,
                payload.Text,
                oDataFilters.ToString(),
                payload.Skip,
                payload.Take,
                new[] { nameof(GroupModel.Name) }.ToList(),
                payload.OrderBy);
        }

        public async Task<AzureSearchResult<SearchGroupDocument>> SearchByIdsAsync(string groupId, int? groupUId, string companiesHouseNumber, IPrincipal principal)
        {
            var outerODataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);
            if (IsRoleRestrictedOnStatus(principal)) outerODataFilters.Add(nameof(GroupModel.StatusId), (int)eStatus.Open);
            
            var innerODataFilters = new ODataFilterList(ODataFilterList.OR);
            if (groupId != null) innerODataFilters.Add(nameof(GroupModel.GroupId), $"'{groupId}'");
            if (groupUId.HasValue) innerODataFilters.Add(nameof(GroupModel.GroupUID), $"'{groupUId}'");
            if (companiesHouseNumber.Clean() != null) innerODataFilters.Add(nameof(GroupModel.CompaniesHouseNumber), $"'{companiesHouseNumber.Clean()}'");
            outerODataFilters.Add(innerODataFilters);

            return await _azureSearchService.SearchAsync<SearchGroupDocument>(GroupsSearchIndex.INDEX_NAME, filter: outerODataFilters.ToString());
        }

        private bool IsRoleRestrictedOnStatus(IPrincipal principal)
            => !_nonStatusRestrictiveRoles.Any(x => principal.IsInRole(x));

        public async Task<int[]> GetParentGroupIdsAsync(int establishmentUrn)
        {
            return await _dbContext.EstablishmentGroups.Where(x => x.EstablishmentUrn == establishmentUrn && x.IsDeleted == false).Select(x => x.GroupUID).ToArrayAsync();
        }

        public async Task<IEnumerable<GroupModel>> GetAllByEstablishmentUrnAsync(int urn)
        {
            var retVal = new List<GroupModel>();
            var links = await _cachedEstablishmentGroupReadRepository.GetForUrnAsync(urn);
            foreach (var link in links)
            {
                var dataModel = await _groupRepository.GetAsync(link.GroupUID);
                retVal.Add(_mapper.Map<GroupCollection, GroupModel>(dataModel));
            }
            return retVal;
        }

        public async Task<GroupModel> GetAsync(int uid)
        {
            var dataModel = await _groupRepository.GetAsync(uid);
            if (dataModel == null) return null;
            else return _mapper.Map<GroupCollection, GroupModel>(dataModel);
        }
    }
}
