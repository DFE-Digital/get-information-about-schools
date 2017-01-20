using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Data.DbContext;
using Edubase.Data.Repositories.Establishments;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories
{
    public abstract class CachedRepositoryBase<TEntity> : ICachedRepositoryBase<TEntity> where TEntity : EdubaseEntity
    {
        private ICacheAccessor _cache;
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly IApplicationDbContextFactory _inMemoryDbContextFactory;
        private readonly RepositoryBase<TEntity> _repository;

        protected ICacheAccessor CacheAccessor => _cache;

        public CachedRepositoryBase(ICacheAccessor cache, 
            IInMemoryApplicationDbContextFactory inMemoryDbContextFactory,
            IApplicationDbContextFactory dbContextFactory,
            RepositoryBase<TEntity> repository)
        {
            _cache = cache;
            _inMemoryDbContextFactory = inMemoryDbContextFactory;
            _dbContextFactory = dbContextFactory;
            _repository = repository;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cache"></param>
        public CachedRepositoryBase(ICacheAccessor cache) : this(cache, null, null, null)
        {
            
        }

        protected async Task<T> AutoAsync<T>(Func<Task<T>> factory, string cacheKey, string relationshipKey = null, [CallerMemberName] string callerFuncName = null)
        {
            return await _cache.AutoAsync(factory, cacheKey, GetType().Name, relationshipKey, callerFuncName);
        }

        protected string Keyify(params object[] items) => _cache.Keyify(items);

        public string GetWarmUpProgressCacheKey() => string.Concat(GetType().Name, ".", nameof(WarmAsync), "-progress");

        public async Task<string> WarmAsync(int maxBatchSize = 500, int maxConcurrency = 40, int? maxTotalRecords = null)
        {
            try
            {
                Guard.IsNotNull(_repository, () => new Exception("Repository is null; is this cached repo not configured for warm-abiity?"));

                await CacheAccessor.SetAsync(GetWarmUpProgressCacheKey(), $"Starting...");

                var inMemContext = _inMemoryDbContextFactory.WhilstRetaining().Obtain();
                var realDbContext = _dbContextFactory.Obtain();

                var totalRecordsCount = await _repository.GetCountAsync(); 
                var processedCount = 0;
                var currentSkip = 0;

                _repository.SwapDbContextFactory(_inMemoryDbContextFactory);

                Func<string> progress = () => (int)Math.Round((double)(100 * processedCount) / totalRecordsCount)
                    + $"% done.  {processedCount} of {totalRecordsCount} items.";

                Func<Task<IEnumerable<TEntity>>> getBatch = async ()
                    => await _repository.GetBatchQuery(realDbContext).Skip(currentSkip).Take(maxBatchSize).ToArrayAsync();

                var batch = await getBatch();
                do
                {
                    GetEntityDbSet(inMemContext).AddRange(batch);

                    foreach (var set in batch.Batch(maxConcurrency))
                    {
                        var tasks = set.Select(x => GetAsync(x.GetId().Value));
                        await Task.WhenAll(tasks);
                        processedCount += maxConcurrency;
                    }

                    var inMemoryDbSet = GetEntityDbSet(inMemContext) as InMemoryDbSet<TEntity>; inMemoryDbSet.Clear(); // todo: make succinct

                    inMemContext = _inMemoryDbContextFactory.ObtainNew();
                    realDbContext = _dbContextFactory.ObtainNew();

                    currentSkip += maxBatchSize;
                    if (maxTotalRecords.HasValue && currentSkip >= maxTotalRecords.Value) break;

                    await CacheAccessor.SetAsync(GetWarmUpProgressCacheKey(), progress());
                }
                while ((batch = await getBatch()).Any());

                await CacheAccessor.SetAsync(GetWarmUpProgressCacheKey(), progress());

                return $"Cached {processedCount} entities";
            }
            catch (Exception ex)
            {
                await CacheAccessor.SetAsync(GetWarmUpProgressCacheKey(), $"Error: {ex}");
                throw;
            }
        }

        protected virtual DbSet<TEntity> GetEntityDbSet(IApplicationDbContext dbContext)
        {
            throw new NotImplementedException("This Cached Repository does not implement this method; maybe it's not configured for Warm-ability");
        }

        public abstract Task<TEntity> GetAsync(int id);
        
    }
}
