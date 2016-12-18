using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Edubase.Data.DbContext
{
    public interface IInMemoryApplicationDbContext : IApplicationDbContext
    {
        
    }
}