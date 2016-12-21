using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.DbContext;

namespace Edubase.Data.Repositories.Establishments
{
    public interface IEstablishmentReadRepository
    {
        Task<Establishment> GetAsync(int urn);
        Task<int?> GetStatusAsync(int urn);
        Task<int[]> GetUrns(int skip, int take);
    }
    

    public interface IInMemoryEstablishmentReadRepository : IEstablishmentReadRepository
    {
    }

    public interface ICachedEstablishmentReadRepository : IEstablishmentReadRepository
    {
        Task<string> WarmAsync(int maxBatchSize = 1000, int maxConcurrency = 40, int? maxTotalRecords = null);
        string GetWarmUpProgressCacheKey();
        Task ClearRelationshipCacheAsync(int? urn);
    }
}