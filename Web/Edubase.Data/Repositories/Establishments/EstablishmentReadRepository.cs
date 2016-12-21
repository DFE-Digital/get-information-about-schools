using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.Establishments
{
    public class EstablishmentReadRepository : RepositoryBase, IEstablishmentReadRepository
    {
        public EstablishmentReadRepository(IApplicationDbContextFactory dbContextFactory) 
            : base(dbContextFactory) { }

        public async Task<Establishment> GetAsync(int urn) 
            => await ObtainDbContext().Establishments
            .FirstOrDefaultAsync(x => x.Urn == urn && x.IsDeleted == false);

        public async Task<int?> GetStatusAsync(int urn)
            => await ObtainDbContext().Establishments
            .Where(x => x.Urn == urn && x.IsDeleted == false)
            .Select(x => x.StatusId).FirstOrDefaultAsync();


        public async Task<int[]> GetUrns(int skip, int take)
        {
            return await ObtainDbContext().Establishments.Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Urn).Skip(skip).Take(take).Select(x => x.Urn).ToArrayAsync();
        }

    }
}
