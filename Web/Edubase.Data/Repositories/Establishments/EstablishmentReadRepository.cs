using Edubase.Data.Entity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Edubase.Common;

namespace Edubase.Data.Repositories.Establishments
{
    public class EstablishmentReadRepository : IEstablishmentReadRepository
    {
        private IApplicationDbContextFactory _dbContextFactory;
        private Dictionary<int, Establishment> _cache;

        public EstablishmentReadRepository(IApplicationDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Establishment> GetAsync(int urn) 
            => _cache?.Get(urn) 
            ?? await _dbContextFactory.Create().Establishments
            .FirstOrDefaultAsync(x => x.Urn == urn && x.IsDeleted == false);

        public async Task<int?> GetStatusAsync(int urn)
            => await _dbContextFactory.Create().Establishments
            .Where(x => x.Urn == urn && x.IsDeleted == false)
            .Select(x => x.StatusId).FirstOrDefaultAsync();


        public async Task<int[]> GetUrns(int skip, int take)
        {
            return await _dbContextFactory.Create().Establishments.Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Urn).Skip(skip).Take(take).Select(x => x.Urn).ToArrayAsync();
        }

        internal async Task<int[]> PreloadBatchAsync(int skip, int take)
        {
            _cache = await _dbContextFactory.Create().Establishments.Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Urn).Skip(skip).Take(take).ToDictionaryAsync(x => x.Urn);
            return _cache.Keys.ToArray();
        }

    }
}
