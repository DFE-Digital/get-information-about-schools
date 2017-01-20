using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.Groups.Abstract
{
    public interface ICachedGroupReadRepository : ICachedRepositoryBase<GroupCollection>
    {
        Task ClearRelationshipCacheAsync(int? urn);
    }
}