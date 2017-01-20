using Edubase.Common.Cache;
using Edubase.Data.Entity;
using System.Threading.Tasks;
using Edubase.Data.DbContext;
using System;
using System.Data.Entity;

namespace Edubase.Data.Repositories.LocalAuthorities
{
    public class CachedLAReadRepository : CachedRepositoryBase<LocalAuthority>, ICachedLAReadRepository
    {
        private ILAReadRepository _repo;

        public CachedLAReadRepository(ILAReadRepository repo, ICacheAccessor cache) 
            : base(cache)
        {
            _repo = repo;
        }

        public override async Task<LocalAuthority> GetAsync(int id) => await AutoAsync(async () => await _repo.GetAsync(id), Keyify(id));
    }
}
