using Edubase.Common.Cache;
using Edubase.Data.Repositories.Establishments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories
{
    public abstract class CachedRepositoryBase
    {
        private ICacheAccessor _cache;

        protected ICacheAccessor CacheAccessor => _cache;

        public CachedRepositoryBase(ICacheAccessor cache)
        {
            _cache = cache;
        }

        protected async Task<T> AutoAsync<T>(Func<Task<T>> factory, string cacheKey, [CallerMemberName] string callerFuncName = null)
        {
            return await _cache.AutoAsync(factory, cacheKey, GetType().Name, callerFuncName);
        }

        protected string Keyify(params object[] items) => _cache.Keyify(items);
        
    }
}
