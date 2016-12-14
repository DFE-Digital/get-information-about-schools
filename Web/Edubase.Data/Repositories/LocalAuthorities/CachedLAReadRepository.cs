using Edubase.Common.Cache;
using Edubase.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories.LocalAuthorities
{
    public class CachedLAReadRepository : RepositoryBase, ICachedLAReadRepository
    {
        private ILAReadRepository _repo;

        public CachedLAReadRepository(ILAReadRepository repo, ICacheAccessor cache) 
            : base(cache)
        {
            _repo = repo;
        }

        public async Task<LocalAuthority> GetAsync(int id)
        {
            return await AutoAsync(async () => await _repo.GetAsync(id), Keyify(id));
        }
    }
}
