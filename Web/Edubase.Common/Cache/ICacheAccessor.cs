namespace Edubase.Common.Cache
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public interface ICacheAccessor : IDisposable
    {
        /// <summary>
        /// Returns the connected-status of the cache
        /// </summary>
        CacheAccessor.State Status { get; }

        /// <summary>
        /// Validates the data going into the Cache and then sets the item into the cache.
        /// The method will return very quickly, because it'll set things to the cache asynchronously (the client code can be agnostic about this fact).
        /// This method is purposely not with Async in the name because it should be treated synchronously. 
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="value">The object to put in</param>
        /// <param name="cacheExpiry">The absolute expiration. Pass in null for no cache expiry.</param>
        void Set(string key, object value, TimeSpan? cacheExpiry);

        /// <summary>
        /// Validates the data going into the Cache and then sets the item into the cache.
        /// The method will return very quickly, because it'll set things to the cache asynchronously (the client code can be agnostic about this fact).
        /// This method is purposely not with Async in the name because it should be treated synchronously. 
        /// </summary>
        /// <param name="domain">A prefix for the cache key, to help separate different cache areas; e.g., 'Users' &amp; 'Shoutouts' are examples of domains.</param>
        /// <param name="key">The cache key</param>
        /// <param name="value">The object to put in</param>
        /// <param name="cacheExpiry">The absolute expiration. Pass in null for no cache expiry.</param>
        void Set(string domain, string key, object value, TimeSpan? cacheExpiry);

        /// <summary>
        /// Retrieves an item from the cache
        /// </summary>
        /// <typeparam name="T">The type to return / deserialize to</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>An awaitable task that returns the type or default(type)</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Retrieves an item from the cache
        /// </summary>
        /// <typeparam name="T">The type to return / deserialize to</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>Returns the type or default(type)</returns>
        T Get<T>(string key);


        /// <summary>
        /// Retrieves an item from the cache
        /// </summary>
        /// <typeparam name="T">The type to return / deserialize to</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>An awaitable task that returns the type or default(type)</returns>
        /// <param name="domain"></param>
        Task<T> GetAsync<T>(string domain, string key);

        /// <summary>
        /// Retrieves an item from the cache
        /// </summary>
        /// <typeparam name="T">The type to return / deserialize to</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>An awaitable task that returns the type or default(type)</returns>
        /// <param name="domain"></param>
        T Get<T>(string domain, string key);

        /// <summary>
        /// Deletes an item from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string key);

        /// <summary>
        /// Deletes an item from the cache
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string domain, string key);

        /// <summary>
        /// Returns how many times the instance tried to connect to the cache
        /// </summary>
        int ConnectionAttemptsCount { get; }

        /// <summary>
        /// Transiently initialises the cache.  This will be done lazily if necessary, however, feel free to use it for eager init.
        /// </summary>
        /// <returns></returns>
        Task InitialiseIfNecessaryAsync();

        /// <summary>
        /// How long it took to connect to the cache
        /// </summary>
        long LastConnectionElapsedMilliseconds { get; }

        string CreateKey(string domain, string key);

        string CreateKey(string key);

        Task SetAsync(string key, object value, TimeSpan? cacheExpiry = null);

        string Keyify(params object[] items);

        Task<T> AutoAsync<T>(Func<Task<T>> factory, string cacheKey, string callerTypeName, [CallerMemberName] string callerFuncName = null);
    }
}
