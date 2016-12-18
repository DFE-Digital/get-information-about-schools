using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.LocalAuthorities
{
    public class LAReadRepository : ILAReadRepository
    {
        private IApplicationDbContext _dbContext;

        public LAReadRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LocalAuthority> GetAsync(int id) 
            => await _dbContext.LocalAuthorities.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);


        

    }
}
