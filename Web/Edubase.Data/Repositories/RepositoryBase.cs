using Edubase.Common;
using Edubase.Data.DbContext;

namespace Edubase.Data.Repositories
{
    public class RepositoryBase
    {
        public IApplicationDbContextFactory DbContextFactory { get; set; }

        public RepositoryBase(IApplicationDbContextFactory dbContextFactory)
        {
            DbContextFactory = dbContextFactory;
        }

        protected IApplicationDbContext ObtainDbContext() => DbContextFactory.Obtain();
    }
}
