using AutoMapper;
using Edubase.Data.Entity;
using Edubase.Services.Groups.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints;

namespace Edubase.Services.Groups
{
    public class GroupReadService : IGroupReadService
    {
        private IApplicationDbContext _dbContext;
        private IMapper _mapper;
        private IAzureSearchEndPoint _azureSearchService;

        public GroupReadService(IApplicationDbContext dc, IMapper mapper, IAzureSearchEndPoint azureSearchService)
        {
            _dbContext = dc;
            _mapper = mapper;
            _azureSearchService = azureSearchService;
        }

        public async Task<GroupModel> GetByEstablishmentUrnAsync(int urn)
        {
            var g = (await _dbContext.EstablishmentGroups.Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.EstablishmentUrn == urn))?.Group;
            if (g != null) return _mapper.Map<GroupCollection, GroupModel>(g);
            else return null;
        }

        public async Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, int take = 10)
            => await _azureSearchService.SuggestAsync<GroupSuggestionItem>(GroupsSearchIndex.INDEX_NAME, GroupsSearchIndex.SUGGESTER_NAME, text, take);

    }
}
