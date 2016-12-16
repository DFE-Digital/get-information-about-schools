using Edubase.Common.Cache;
using Edubase.Data.Entity;
using System.Threading.Tasks;
using System;
using System.Linq;
using MoreLinq;
using Edubase.Common;
using System.Text;
using Edubase.Data.DbContext;
using System.Collections.Generic;

namespace Edubase.Data.Repositories.Establishments
{
    public class CachedEstablishmentReadRepository : CachedRepositoryBase, ICachedEstablishmentReadRepository
    {
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly IApplicationDbContextFactory _inMemoryDbContextFactory;
        private readonly IEstablishmentReadRepository _repo;

        public CachedEstablishmentReadRepository(IEstablishmentReadRepository repo, 
            ICacheAccessor cache,
            IInMemoryApplicationDbContextFactory inMemoryDbContextFactory,
            IApplicationDbContextFactory dbContextFactory) 
            : base(cache)
        {
            _repo = repo;
            _inMemoryDbContextFactory = inMemoryDbContextFactory;
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Establishment> GetAsync(int urn) 
            => await AutoAsync(async () => await _repo.GetAsync(urn), Keyify(urn));
        
        public async Task<int?> GetStatusAsync(int urn)
            => await AutoAsync(async () => await _repo.GetStatusAsync(urn), Keyify(urn));

        public async Task<int[]> GetUrns(int skip, int take)
        {
            return await _repo.GetUrns(skip, take);
        }

        public async Task<string> WarmAsync(int maxBatchSize = 1000, int maxConcurrency = 40, int? maxTotalRecords = null)
        {
            var inMemContext = _inMemoryDbContextFactory.WhilstRetaining().Obtain();
            var realDbContext = _dbContextFactory.Obtain();

            if (_repo == null) throw new Exception("Repository is null");
            var repo = _repo as RepositoryBase;
            if (repo == null) throw new Exception("Repository is not castable to RepositoryBase");
            repo.DbContextFactory = _inMemoryDbContextFactory;

            var processedCount = 0;
            var currentSkip = 0;
            Func<IEnumerable<Establishment>> getBatch = ()
                => realDbContext.Establishments.AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Urn).Skip(currentSkip).Take(maxBatchSize).ToArray();
            
            var batch = getBatch();
            do
            {
                inMemContext.Establishments.AddRange(batch);

                foreach (var set in batch.Batch(maxConcurrency))
                {
                    var tasks = set.Select(x => GetAsync(x.Urn));
                    await Task.WhenAll(tasks);
                    processedCount += maxConcurrency;
                }
                
                ((InMemoryDbSet<Establishment>)inMemContext.Establishments).Clear();
                inMemContext = _inMemoryDbContextFactory.ObtainNew();
                realDbContext = _dbContextFactory.Obtain();

                currentSkip += maxBatchSize;
                if (maxTotalRecords.HasValue && currentSkip >= maxTotalRecords.Value) break;
            }
            while ((batch = getBatch()).Any());
            
            return $"Cached {processedCount} entities";
        }
    }
}
