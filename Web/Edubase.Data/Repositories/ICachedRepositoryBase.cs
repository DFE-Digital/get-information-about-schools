using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories
{
    public interface ICachedRepositoryBase<TEntity> where TEntity : EdubaseEntity
    {
        Task<TEntity> GetAsync(int id);
        string GetWarmUpProgressCacheKey();
        Task<string> WarmAsync(int maxBatchSize = 500, int maxConcurrency = 40, int? maxTotalRecords = default(int?));
    }
}