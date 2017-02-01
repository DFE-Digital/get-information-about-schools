using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using System.Collections.Generic;

namespace Edubase.Data.Repositories.Groups.Abstract
{
    public interface IEstablishmentGroupReadRepository
    {
        Task<EstablishmentGroup> GetAsync(int id);
        Task<List<EstablishmentGroup>> GetForUrnAsync(int urn);
        Task<List<EstablishmentGroup>> GetForGroupAsync(int groupUId);
    }
}