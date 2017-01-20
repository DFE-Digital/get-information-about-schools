using Edubase.Common;
using Edubase.Data.DbContext;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories
{
    public abstract class RepositoryBase<T>
    {
        public IApplicationDbContextFactory DbContextFactory { get; set; }

        public RepositoryBase(IApplicationDbContextFactory dbContextFactory)
        {
            DbContextFactory = dbContextFactory;
        }

        protected IApplicationDbContext ObtainDbContext() => DbContextFactory.Obtain();

        /// <summary>
        /// Gets the total number of records in the database
        /// </summary>
        public abstract Task<int> GetCountAsync();

        public abstract IOrderedQueryable<T> GetBatchQuery(IApplicationDbContext dbContext);

        internal void SwapDbContextFactory(IApplicationDbContextFactory forThisOne) => DbContextFactory = forThisOne;
    }
}
