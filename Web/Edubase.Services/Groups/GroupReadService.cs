using AutoMapper;
using Edubase.Data.Entity;
using Edubase.Services.Groups.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Edubase.Services.Groups
{
    public class GroupReadService : IGroupReadService
    {
        private IApplicationDbContext _dbContext;
        private IMapper _mapper;

        public GroupReadService(IApplicationDbContext dc, IMapper mapper)
        {
            _dbContext = dc;
            _mapper = mapper;
        }

        public async Task<GroupModel> GetByEstablishmentUrnAsync(int urn)
        {
            var g = (await _dbContext.EstablishmentGroups.Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.EstablishmentUrn == urn))?.Group;
            if (g != null) return _mapper.Map<GroupCollection, GroupModel>(g);
            else return null;
        }
    }
}
