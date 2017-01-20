using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.Groups.Abstract
{
    public interface ICachedEstablishmentGroupReadRepository : IEstablishmentGroupReadRepository, ICachedRepositoryBase<EstablishmentGroup>
    {
        Task ClearRelationshipCacheAsync(int? urn);
    }
}