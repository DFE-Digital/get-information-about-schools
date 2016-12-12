using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;

namespace Edubase.Services.Cache
{
    public abstract class CachedServiceBase
    {
        private ICacheAccessor _cache;
        
        public CachedServiceBase(ICacheAccessor cache)
        {
            _cache = cache;
        }

        protected string Keyify(params object[] items) => string.Join("-", items.Select(x => x?.ToString().Clean() ?? "null"));
            
        protected async Task<T> AutoAsync<T>(Func<Task<T>> factory, string cacheKey, [CallerMemberName] string callerFuncName = null)
        {
            var key = $"{GetType().Name}.{callerFuncName}({cacheKey})";
            var retVal = await _cache.GetAsync<T>(key);

            if (retVal == null)
            {
                retVal = await factory();
                if (retVal != null)
                    await _cache.SetAsync(key, retVal); // todo, don't await.
            }

            return retVal;
        }

    }
}
