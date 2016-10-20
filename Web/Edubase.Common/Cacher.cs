using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common
{
    public static class Cacher
    {
        public static T Get<T>(string name, Func<T> factory, DateTimeOffset absExp)
        {
            if (MemoryCache.Default.Contains(name)) return (T) MemoryCache.Default.Get(name);
            else
            {
                var value = factory();
                MemoryCache.Default.Set(name, value, absExp);
                return value;
            }
        }

        public static async Task<T> GetAsync<T>(string name, Func<Task<T>> factory, DateTimeOffset absExp)
        {
            if (MemoryCache.Default.Contains(name)) return await Task.FromResult((T) MemoryCache.Default.Get(name));
            else
            {
                var value = await factory();
                MemoryCache.Default.Set(name, value, absExp);
                return value;
            }
        }

        public static T Get<T>(string name, Func<T> factory) => Get(name, factory, DateTimeOffset.MaxValue);

        public static async Task<T> GetAsync<T>(string name, Func<Task<T>> factory) => await GetAsync(name, factory, DateTimeOffset.MaxValue);

        public static T Get<T>(string domain, string name, Func<T> factory) => Get(domain + "_" + name, factory, DateTimeOffset.MaxValue);

        public static async Task<T> GetAsync<T>(string domain, string name, Func<Task<T>> factory) => await GetAsync(domain + "_" + name, factory, DateTimeOffset.MaxValue);

        public static void Remove(string name) => MemoryCache.Default.Remove(name);

        public static T Auto<T>(Func<T> factory, [CallerMemberName] string name = null) => Get(name, factory);

        public static Task<T> AutoAsync<T>(Func<Task<T>> factory, [CallerMemberName] string name = null) => GetAsync(name, factory);
    }
}
