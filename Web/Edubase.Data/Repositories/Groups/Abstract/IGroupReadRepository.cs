using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using System.Collections.Generic;

namespace Edubase.Data.Repositories.Groups.Abstract
{
    public interface IGroupReadRepository
    {
        Task<GroupCollection> GetAsync(int id);
    }
}