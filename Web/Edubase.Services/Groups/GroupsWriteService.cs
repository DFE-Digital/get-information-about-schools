using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Edubase.Data.DbContext;

namespace Edubase.Services.Groups
{
    /// </summary>
    public class GroupsWriteService : IGroupsWriteService
    {
        private IApplicationDbContext _dbContext;

        public GroupsWriteService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateEstablishmentCount(int groupUId, int? count = null)
        {
            var entity = await _dbContext.Groups.FirstOrDefaultAsync(x => x.GroupUID == groupUId && x.IsDeleted == false);
            if (entity != null)
            {
                if(count == null) // no count supplied, so calculate here
                    count = await _dbContext.EstablishmentGroups.CountAsync(x => x.IsDeleted == false && x.GroupUID == groupUId);

                entity.EstablishmentCount = count.Value;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
