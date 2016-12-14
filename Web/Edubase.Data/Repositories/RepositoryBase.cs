using Edubase.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories
{
    public class RepositoryBase
    {
        private ICacheAccessor _cache;

        public RepositoryBase(ICacheAccessor cache)
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
