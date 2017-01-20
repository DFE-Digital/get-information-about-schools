using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.Groups.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.Groups
{
    public class GroupReadRepository : RepositoryBase<GroupCollection>, IGroupReadRepository
    {
        public GroupReadRepository(IApplicationDbContextFactory dbContextFactory) 
            : base(dbContextFactory) { }

        public async Task<GroupCollection> GetAsync(int id)
            => await ObtainDbContext().Groups
            .FirstOrDefaultAsync(x => x.GroupUID == id && x.IsDeleted == false);
        
        public override async Task<int> GetCountAsync()
            => await ObtainDbContext().Groups.Where(x => x.IsDeleted == false).CountAsync();

        public override IOrderedQueryable<GroupCollection> GetBatchQuery(IApplicationDbContext dbContext) =>
            dbContext.Groups.AsNoTracking().Where(x => x.IsDeleted == false).OrderBy(x => x.GroupUID);

    }
}
