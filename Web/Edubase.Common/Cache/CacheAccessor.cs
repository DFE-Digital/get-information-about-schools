using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Edubase.Common.Cache
{
    public class CacheAccessor : ICacheAccessor, IDisposable
    {
        private readonly object _mutex2 = new object();
        private readonly CacheConfig _config;
        private MemoryCache _memoryCache;
        private MemoryCache _fastMemcache;

        /// <summary>
        /// Whether to have this instance record the keys that are set within this session.
        /// </summary>
        public bool ShouldRecordKeysInSession { get; set; }
        private readonly List<string> _keysSetInSession = new List<string>(); // log of keys set in this instance


        /// <summary>
        /// Returns the name of the current cache accessor (useful for debugging)
        /// </summary>
        public string Name => _config.Name;

        public eCacheEvent LastCachingEvent { get; set; }

        /// <summary>
        /// Returns an audit log of activity on the cache accessor. Useful for debugging. Auditing is disabled by default.
        /// </summary>
        public List<CacheAuditLogItem> AuditLog { get; } = new List<CacheAuditLogItem>();

        public bool IsAuditingEnabled
        {
            get => _config.IsAuditingEnabled;
            set => _config.IsAuditingEnabled = value;
        }

        private readonly IExceptionLogger _exceptionLogger;
        private readonly JsonConverterCollection _jsonConverterCollection;

        public CacheAccessor(JsonConverterCollection jsonConverterCollection) : this(new CacheConfig(), null)
        {
            _jsonConverterCollection = jsonConverterCollection;
        }

        public CacheAccessor(CacheConfig config, IExceptionLogger exceptionLogger)
        {
            _config = config;
            _exceptionLogger = exceptionLogger;
            
            CreateFastUncloningMemoryCache();
            CreateCloningMemoryCache();
            
        }
        
        /// <summary>
        /// Creates a memory caches that's used for serialised, compressed and cloned payloads.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private void CreateCloningMemoryCache()
        {
            if(_memoryCache != null)
            {
                _memoryCache.Dispose();
                _memoryCache = null;
            }

            var cacheSettings = new NameValueCollection(2)
            {
                { "cacheMemoryLimitMegabytes", "2000" },
                { "pollingInterval", "00:00:05" }
            };
            _memoryCache = new MemoryCache(_config.Name, cacheSettings);
        }

        /// <summary>
        /// Creates a memory cache that's used for frequently used immutable uncloned objects (v. fast, but immutable objects only)
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private void CreateFastUncloningMemoryCache()
        {
            if (_fastMemcache != null)
            {
                _fastMemcache.Dispose();
                _fastMemcache = null;
            }

            var cacheSettings = new NameValueCollection(2)
            {
                { "cacheMemoryLimitMegabytes", "300" },
                { "pollingInterval", "00:00:02" }
            };
            _fastMemcache = new MemoryCache(_config.Name + "-uncloned", cacheSettings);
        }

        /// <summary>
        /// Sets an item into Memory Cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationUtc"></param>
        /// <param name="recordAsSessionKey">
        ///     This parameter flags whether this is a key that should be recorded as part of the current session.
        ///     This was introduced because during testing, you want to clear down the cache, but there may still be pending cache messages
        ///     so when ShouldRecordKeysInSession is true and we're recording that a msg has been processed, then this ensures
        ///     that subsequent queued messages do not get processed.
        /// </param>
        private void SetInMemoryCache(string key, byte[] data, DateTime? expirationUtc = null, bool recordAsSessionKey = true)
        {
            key = CleanKey(key);
            var policy = new CacheItemPolicy();
            if (expirationUtc.HasValue)
            {
                policy.AbsoluteExpiration = new DateTimeOffset(expirationUtc.Value);
            }

            if (data == null)
            {
                _memoryCache.Remove(key);
            }
            else
            {
                _memoryCache.Set(key, data, policy);
            }

            if (ShouldRecordKeysInSession && recordAsSessionKey)
            {
                lock (_mutex2)
                {
                    _keysSetInSession.Add(key);
                }
            }
        }

        /// <summary>
        /// Sets an item into Memory Cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        private void SetInMemoryCache(string key, byte[] value, TimeSpan? expiration = null, bool recordAsSessionKey = true)
        {
            var expires = !expiration.HasValue ? null as DateTime? : DateTime.UtcNow.Add(expiration.Value);
            SetInMemoryCache(key, value, expires, recordAsSessionKey);
        }

        private void Log(eCacheEvent cachingEvent, string key, [CallerMemberName] string callerName = null)
        {
            if (_config.IsAuditingEnabled)
            {
                AuditLog.Add(new CacheAuditLogItem { CachingEvent = cachingEvent, CallerFunctionName = callerName, Key = key });
            }
            LastCachingEvent = cachingEvent;
        }

        /// <summary>
        /// Logs an exception to the exception logger
        /// </summary>
        /// <param name="ex"></param>
        private void Log(Exception ex, [CallerMemberName] string callerName = null)
        {
            _exceptionLogger?.Log(ex);
            Log(eCacheEvent.Exception, ex.ToString(), callerName);
        }

        /// <summary>
        /// Deletes all data from the cache
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync()
        {
            await Task.Run(() => Clear());
        }

        /// <summary>
        /// Deletes all data from the cache
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            CreateCloningMemoryCache();
            CreateFastUncloningMemoryCache();
        }

        /// <summary>
        /// Deletes the keys that have been cached during this instance. Useful for clearing the cache during a integration test session.
        /// Better than ClearAsync because we all share the dev cache and this will only remove items cached by the current session
        /// </summary>
        /// <returns></returns>
        public void DeleteKeysInThisSession()
        {
            lock (_mutex2) // make thread-safe; this method doesn't support re-entrancy
            {
                if (ShouldRecordKeysInSession)
                {
                    foreach (var key in _keysSetInSession)
                    {
                        _memoryCache.Remove(key);
                    }
                    _keysSetInSession.Clear();
                    LastCachingEvent = eCacheEvent.None;
                }
                else
                {
                    throw new Exception("The flag 'ShouldRecordKeysInSession' was not on.");
                }
            }
        }

        /// <summary>
        /// Validates the data going into the Cache and then sets the item into the cache.
        /// The method will return very quickly, because it'll set things to the cache asynchronously (the client code can be agnostic about this fact).
        /// This method is purposely not with Async in the name because it should be treated synchronously. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheExpiry"></param>
        /// <returns></returns>
        public void Set<T>(string key, T value, TimeSpan? cacheExpiry = null)
        {
            _ = SetAsync(key, value, cacheExpiry);
        }

        /// <summary>
        /// Use 'Set'; this way validation errors will propagate into client code.
        /// Sets an item to the cache asynchronously; can be awaited upon, or used in a fire-and-forget way.  
        /// The cache server connection will be initialised lazily if necessary.  
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheExpiry"></param>
        /// <returns></returns>
        public async Task SetAsync<T>(string key, T value, TimeSpan? cacheExpiry = null)
        {
            key = CleanKey(key);
            try
            {
                var tasks = new List<Task>
                {
                    Task.Run(() => PutInFastCache(key, value))
                };

                DateTime? expirationUtc = null;
                if (cacheExpiry.HasValue)
                {
                    expirationUtc = DateTime.UtcNow.Add(cacheExpiry.Value);
                }
                var buffer = ToByteBuffer(value, _config.IsPayloadCompressionEnabled, new Dictionary<string, object>() { ["ExpirationUtc"] = expirationUtc });
                tasks.Add(Task.Run(() => SetInMemoryCache(key, buffer, cacheExpiry)));

                await Task.WhenAll(tasks);
                Log(eCacheEvent.KeySetInMemory, key);
            }
            catch (Exception ex)
            {
                Log(ex);
                if (_config.IsExceptionPropagationEnabled)
                {
                    throw;
                }
            }
        }

        private void PutInFastCache<T>(string key, T value) 
            => _fastMemcache.Add(key, value, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) });

        /// <summary>
        /// Retrieves an item from the cache, if possible.
        /// It'll lazily init the cache asynchronously and return null if necessary.
        /// It will always return in a timely manner; e.g., without waiting for a cache connection
        /// It will never propagate an exception, unless the throwExceptions flag is set on the constructor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<CacheResponseDto<T>> GetWithMetaDataAsync<T>(string key)
        {
            key = CleanKey(key);
            var dto = new CacheResponseDto<T>().StartTiming();
            try
            {
                await Task.Run(() => TryGetFromMemoryCache(key, ref dto));
            }
            catch (Exception ex)
            {
                if (_config.IsExceptionPropagationEnabled)
                {
                    throw;
                }

                Log(ex);
                dto.Exception = ex;
            }
            return dto.StopTiming();
        }

        /// <summary>
        /// Retrieves an item from the cache, if possible.
        /// It'll lazily init the cache asynchronously and return null if necessary.
        /// It will always return in a timely manner; e.g., without waiting for a cache connection
        /// It will never propagate an exception, unless the throwExceptions flag is set on the constructor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheResponseDto<T> GetWithMetaData<T>(string key)
        {
            key = CleanKey(key);
            var dto = new CacheResponseDto<T>().StartTiming();
            try
            {
                TryGetFromMemoryCache(key, ref dto);
            }
            catch (Exception ex)
            {
                Log(ex);
                dto.Exception = ex;
            }
            return dto.StopTiming();
        }

        /// <summary>
        /// Attempts to retrieve an item from memory cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        private bool TryGetFromMemoryCache<T>(string key, ref CacheResponseDto<T> dto)
        {
            Log(eCacheEvent.KeyValueGotFromMemoryAttempt, key);

            if (_fastMemcache.Contains(key))
            {
                dto.Data = (T) _fastMemcache.Get(key);
                dto.IsFromInMemoryCache = true;
                Log(eCacheEvent.KeyValueGotFromMemory, key);
                return true;
            }

            byte[] buffer;

            if ((buffer = (byte[]) _memoryCache.Get(key)) != null)
            {
                dto.Data = FromByteBuffer<T>(buffer, true).Object;
                dto.IsFromInMemoryCache = true;
                Log(eCacheEvent.KeyValueGotFromMemory, key);
                return true;
            } 
            else
            {
                return false;
            }
        }

        public async Task<T> GetAsync<T>(string key) => (await GetWithMetaDataAsync<T>(key)).Data;

        /// <summary>
        /// Retrieves an item from the cache, if possible.
        /// It will always return in a timely manner; e.g., without waiting for a cache connection
        /// It will never propagate an exception, unless the throwExceptions flag is set on the constructor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return GetWithMetaData<T>(key).Data;
        }

        /// <summary>
        /// Deletes an item from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key)
        {
            key = CleanKey(key);
            var retVal = false;
            try
            {
                var tasks = new List<Task>()
                {
                    Task.Run(() => _fastMemcache.Remove(key)),
                    Task.Run(() => _memoryCache.Remove(key)),
                };

                await Task.WhenAll(tasks);
                Log(eCacheEvent.KeyDeletedInMemory, key);
                retVal = true;
            }
            catch (Exception ex)
            {
                Log(ex);
            }

            return retVal;
        }

        private string CleanKey(string key)
        {
            key = _config.ProcessKey(key);
            return string.IsNullOrWhiteSpace(key) ? throw new Exception("The key was invalid") : key.ToLower();
        }

        public string CreateKey(string domain, string key) => CreateKey(string.Concat(domain, "_", key));
        public string CreateKey(string key) => key.ToLower();
        public T Get<T>(string domain, string key) => Get<T>(CreateKey(domain, key));
        public async Task<T> GetAsync<T>(string domain, string key) => await GetAsync<T>(CreateKey(domain, key));
        public async Task<bool> DeleteAsync(string domain, string key) => await DeleteAsync(CreateKey(domain, key));
                
        
        private byte[] ToByteBuffer<T>(T o, bool compress, Dictionary<string, object> metadata = null)
        {
            byte[] retVal = null;

            if (o != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new BsonDataWriter(memoryStream))
                    {
                        CreateJsonSerializer().Serialize(writer, new ObjectEnvelope<T>(o, metadata));
                    }
                    retVal = memoryStream.ToArray();
                }

                if (compress)
                {
                    retVal = retVal.Compress();
                }
            }
            return retVal;
        }

        private ObjectEnvelope<T> FromByteBuffer<T>(byte[] buffer, bool decompress)
        {
            if (buffer == null)
            {
                return new ObjectEnvelope<T>(default, null);
            }

            if (decompress)
            {
                buffer = buffer.Decompress();
            }

            using (var memoryStream = new MemoryStream(buffer))
            {
                using (var reader = new BsonDataReader(memoryStream))
                {
                    return CreateJsonSerializer().Deserialize<ObjectEnvelope<T>>(reader);
                }
            }
        }

        private JsonSerializer CreateJsonSerializer()
        {
            var retVal = new JsonSerializer();
            _jsonConverterCollection.ForEach(x => retVal.Converters.Add(x));
            return retVal;
        }

        public void Dispose()
        {
            if (_memoryCache != null)
            {
                _memoryCache.Dispose();
            }

            if (_fastMemcache != null)
            {
                _fastMemcache.Dispose();
            }
        }


        public string Keyify(params object[] items)
        {
            return string.Join("-", items.Select(x => x?.ToString().Clean() ?? "null"));
        }

        /// <summary>
        /// Automatically caches an items and calls the factory if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncFactory">An async factory method</param>
        /// <param name="paramsCacheKey">The main cache key comes from the caller type name and caller func. name. This is purely the parameters, as a concatenated string</param>
        /// <param name="callerTypeName"></param>
        /// <param name="callerFuncName"></param>
        /// <param name="relationshipKey">Appends this cache item with this key value</param>
        /// <returns></returns>
        public async Task<T> AutoAsync<T>(Func<Task<T>> asyncFactory, string paramsCacheKey, string callerTypeName, string relationshipKey = null, [CallerMemberName] string callerFuncName = null)
        {
            var key = $"{callerTypeName}.{callerFuncName}({paramsCacheKey})".ToLower();
            var retVal = await GetAsync<T>(key);

            if (retVal == null)
            {
                retVal = await asyncFactory();
                if (retVal != null)
                {
                    await SetAsync(key, retVal);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Automatically sets the cache key based on the caller function name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="paramsCacheKey"></param>
        /// <param name="callerTypeName"></param>
        /// <param name="relationshipKey">
        ///     Rel. key is where a cache key is appended to a relationship key.  This allows you to clear down cache items that
        ///     relate to a particular entity.
        /// </param>
        /// <param name="callerFuncName"></param>
        /// <returns></returns>
        public async Task<T> AutoAsync<T>(Func<T> factory, string paramsCacheKey, string callerTypeName, string relationshipKey = null, [CallerMemberName] string callerFuncName = null)
        {
            var key = $"{callerTypeName}.{callerFuncName}({paramsCacheKey})".ToLower();
            var retVal = await GetAsync<T>(key);

            if (retVal == null)
            {
                retVal = factory();
                if (retVal != null)
                {
                    await SetAsync(key, retVal);
                }
            }
            return retVal;
        }
        
        /// <summary>
        /// Automatically sets the cache key based on the caller function name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="paramsCacheKey"></param>
        /// <param name="callerTypeName"></param>
        /// <param name="relationshipKey"></param>
        /// <param name="callerFuncName"></param>
        /// <returns></returns>
        public T Auto<T>(Func<T> factory, string paramsCacheKey, string callerTypeName, string relationshipKey = null, [CallerMemberName] string callerFuncName = null)
        {
            var key = $"{callerTypeName}.{callerFuncName}({paramsCacheKey})".ToLower();
            var retVal = Get<T>(key);

            if (retVal == null)
            {
                retVal = factory();
                if (retVal != null)
                {
                    Set(key, retVal);
                }
            }
            return retVal;
        }

        
        public long GetMemoryCacheApproximateSize()
        {
            var statsField = typeof(MemoryCache).GetField("_stats", BindingFlags.NonPublic | BindingFlags.Instance);
            var statsValue = statsField.GetValue(_memoryCache);
            var monitorField = statsValue.GetType().GetField("_cacheMemoryMonitor", BindingFlags.NonPublic | BindingFlags.Instance);
            var monitorValue = monitorField.GetValue(statsValue);
            var sizeField = monitorValue.GetType().GetField("_sizedRefMultiple", BindingFlags.NonPublic | BindingFlags.Instance);
            var sizeValue = sizeField.GetValue(monitorValue);
            var approxProp = sizeValue.GetType().GetProperty("ApproximateSize", BindingFlags.NonPublic | BindingFlags.Instance);
            return (long)approxProp.GetValue(sizeValue, null);
        }
    }


}
