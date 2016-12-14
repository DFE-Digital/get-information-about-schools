using Edubase.Common.Cache;
using Edubase.Data.Entity;
using System.Threading.Tasks;
using System;
using System.Linq;
using MoreLinq;
using Edubase.Common;
using System.Text;

namespace Edubase.Data.Repositories.Establishments
{
    public class CachedEstablishmentReadRepository : RepositoryBase, ICachedEstablishmentReadRepository
    {
        private IEstablishmentReadRepository _repo;

        public CachedEstablishmentReadRepository(IEstablishmentReadRepository repo, ICacheAccessor cache) 
            : base(cache)
        {
            _repo = repo;
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
            long totalMillisecondsCaching = 0;
            long totalMillisecondsGettingData = 0;
            var skip = 0;
            var repo = (EstablishmentReadRepository)_repo;

            Func<Task<int[]>> getBatch = async () =>
            {
                using (Disposer.Timed(after: ms => totalMillisecondsGettingData += ms))
                    return await repo.PreloadBatchAsync(skip, maxBatchSize);
            };

            var batch = await getBatch();
            while (batch.Any())
            {
                foreach (var set in batch.Batch(maxConcurrency))
                {
                    using (Disposer.Timed(after: ms => totalMillisecondsCaching += ms))
                    {
                        var tasks = set.Select(x => GetAsync(x));
                        await Task.WhenAll(tasks.ToArray());
                    }
                }
                
                skip += maxBatchSize;
                batch = await getBatch();

                if (maxTotalRecords.HasValue && skip > maxTotalRecords.Value) break;
            }

            var report = new StringBuilder();
            report.AppendLine($"totalMillisecondsCaching: {totalMillisecondsCaching}");
            report.AppendLine($"totalMillisecondsGettingData: {totalMillisecondsGettingData}");
            return report.ToString();
        }
    }
}
