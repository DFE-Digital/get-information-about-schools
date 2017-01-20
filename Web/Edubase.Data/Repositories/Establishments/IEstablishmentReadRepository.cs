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

    public interface ICachedEstablishmentReadRepository : IEstablishmentReadRepository, ICachedRepositoryBase<Establishment>
    {
        Task ClearRelationshipCacheAsync(int? urn);
    }
}