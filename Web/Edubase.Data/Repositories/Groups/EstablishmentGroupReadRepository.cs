using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Data.Repositories.Groups.Abstract;

namespace Edubase.Data.Repositories.Groups
{
    public class EstablishmentGroupReadRepository : RepositoryBase<EstablishmentGroup>, IEstablishmentGroupReadRepository
    {
        public EstablishmentGroupReadRepository(IApplicationDbContextFactory dbContextFactory) 
            : base(dbContextFactory) { }

        public async Task<EstablishmentGroup> GetAsync(int id)
            => await ObtainDbContext().EstablishmentGroups
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

        public async Task<List<EstablishmentGroup>> GetForUrnAsync(int urn)
            => await ObtainDbContext().EstablishmentGroups
                .Where(x => x.EstablishmentUrn == urn && x.IsDeleted == false).ToListAsync();

        public override async Task<int> GetCountAsync()
            => await ObtainDbContext().Groups.Where(x => x.IsDeleted == false).CountAsync();

        public override IOrderedQueryable<EstablishmentGroup> GetBatchQuery(IApplicationDbContext dbContext) =>
            dbContext.EstablishmentGroups.AsNoTracking().Where(x => x.IsDeleted == false).OrderBy(x => x.GroupUID);

        public async Task<List<EstablishmentGroup>> GetForGroupAsync(int groupUId)
            => await ObtainDbContext().EstablishmentGroups
                .Where(x => x.GroupUID == groupUId && x.IsDeleted == false).ToListAsync();

    }
}
