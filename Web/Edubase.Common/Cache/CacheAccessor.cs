namespace Edubase.Common.Cache
{
    using MoreLinq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using StackExchange.Redis;
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

    public class CacheAccessor : ICacheAccessor, IDisposable
    {
        private ConnectionMultiplexer _connection;
        private readonly object _mutex = new object();
        private readonly object _mutex2 = new object();
        private IDatabase _cacheDatabase;
        private CacheConfig _config;
        private Task _connectingTask = null;
        private const string CHANNEL_KEY_UPDATES = "key-updates";
        private const string CHANNEL_KEY_DELETES = "key-deletes";
        private const string CHANNEL_CLEAR_CACHE = "clear-cache";
        private ISubscriber _subscriber = null; // subscription to key-updates
        private MemoryCache _memoryCache;
        private MemoryCache _fastMemcache;
        private List<CacheAuditLogItem> _auditLog = new List<CacheAuditLogItem>();
        private volatile bool _isPendingSetOperation = false;

        /// <summary>
        /// Whether to have this instance record the keys that are set within this session.
        /// </summary>
        public bool ShouldRecordKeysInSession { get; set; }
        private List<string> _keysSetInSession = new List<string>(); // log of keys set in this instance
        
        public enum State
        {
            NotConnected,
            Connecting,
            Connected
        }

        public State Status { get; private set; } 

        public long LastConnectionElapsedMilliseconds { get; set; }

        public int ConnectionAttemptsCount { get; private set; }

        /// <summary>
        /// Returns the name of the current cache accessor (useful for debugging)
        /// </summary>
        public string Name { get { return _config.Name; } }

        public eCacheEvent LastCachingEvent { get; set; }

        /// <summary>
        /// Returns an audit log of activity on the cache accessor. Useful for debugging. Auditing is disabled by default.
        /// </summary>
        public List<CacheAuditLogItem> AuditLog { get { return _auditLog; } }

        public bool IsAuditingEnabled
        {
            get
            {
                return _config.IsAuditingEnabled;
            }
            set
            {
                _config.IsAuditingEnabled = value;
            }
        }

        private IExceptionLogger _exceptionLogger;
        private JsonConverterCollection _jsonConverterCollection;

        public CacheAccessor(JsonConverterCollection jsonConverterCollection) : this(new CacheConfig(), null)
        {
            _jsonConverterCollection = jsonConverterCollection;
        }

        public CacheAccessor(CacheConfig config) : this(config, null)
        {

        }

        public CacheAccessor(IExceptionLogger exceptionLogger) : this(new CacheConfig(), exceptionLogger)
        {

        }
        
        public CacheAccessor(CacheConfig config, IExceptionLogger exceptionLogger)
        {
            _config = config;
            _exceptionLogger = exceptionLogger;
            
            CreateFastUncloningMemoryCache();
            CreateCloningMemoryCache();
            
        }
        
        public static ICacheAccessor Create() => new CacheAccessor(new JsonConverterCollection()) as ICacheAccessor;

        public CacheAccessor SetJsonConverterCollection(JsonConverterCollection jsonConverterCollection)
        {
            _jsonConverterCollection = jsonConverterCollection;
            return this;
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

            var cacheSettings = new NameValueCollection(2);
            cacheSettings.Add("cacheMemoryLimitMegabytes", "2000");
            cacheSettings.Add("pollingInterval", "00:00:05");
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

            var cacheSettings = new NameValueCollection(2);
            cacheSettings.Add("cacheMemoryLimitMegabytes", "300");
            cacheSettings.Add("pollingInterval", "00:00:02");
            _fastMemcache = new MemoryCache(_config.Name + "-uncloned", cacheSettings);
        }

        #region Async methods

        /// <summary>
        /// Asynchronously invokes a cache server connection if necessary.  This is thread-safe.
        /// </summary>
        /// <returns>The task responsible for creating the connection</returns>
        public Task InitialiseIfNecessaryAsync()
        {
            // Do thread-sync to ensure multiple threads attempt to connect simultaneously
            if (_config.IsCentralCacheEnabled)
            {
                if (Status == State.NotConnected)
                {
                    lock (_mutex)
                    {
                        if (Status == State.NotConnected)
                        {
                            Status = State.Connecting; // tells other threads not to try to connect
                            _connectingTask = ConnectAsync();
                        }
                    }
                }
            }
            else _connectingTask = Task.FromResult(null as object);

            return _connectingTask;
        }

        /// <summary>
        /// Connects to the central cache server
        /// </summary>
        /// <returns></returns>
        private async Task ConnectAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_config.ConnectionString))
                    throw new Exception("Cache server connection string was not set");

                if (_connection != null) _connection.Dispose();
                
                var sw = Stopwatch.StartNew();
                try
                {
                    ConnectionAttemptsCount++;
                    _connection = await ConnectionMultiplexer.ConnectAsync(_config.ConnectionString);
                }
                finally
                {
                    sw.Stop();
                    LastConnectionElapsedMilliseconds = sw.ElapsedMilliseconds;
                }

                _cacheDatabase = _connection.GetDatabase();

                if (_config.IsDistributedCachingEnabled)
                {
                    _subscriber = _connection.GetSubscriber();
                    await _subscriber.SubscribeAsync(CHANNEL_KEY_UPDATES, OnKeyValueUpdated);
                    await _subscriber.SubscribeAsync(CHANNEL_KEY_DELETES, OnKeyDeleted);
                    await _subscriber.SubscribeAsync(CHANNEL_CLEAR_CACHE, OnClearCache);
                }

                Status = State.Connected;
                Log(eCacheEvent.ConnectedToServer, null);
            }
            catch (Exception ex)
            {
                Status = State.NotConnected;
                Log(ex);
            }
        }


        /// <summary>
        /// Invoked whenever a cache key-value-pair is updated on another node.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        private void OnKeyValueUpdated(RedisChannel channel, RedisValue value)
        {
            try
            {
                if (value.HasValue)
                {
                    var message = FromByteBuffer<DistributedCacheMessage>(value, false)?.Object;
                    if (message.SenderCacheName != Name) // don't process messages sent by the sender instance, as the job should have been synchronously
                    {
                        if (!_memoryCache.Contains(message.TransactionCacheKey)) // checks whether the message has already been processed (i.e., this node did the op directly)
                        {
                            SetInMemoryCache(message.Key, message.Buffer, message.ExpirationUtc);
                            if (_fastMemcache.Contains(message.Key)) _fastMemcache.Remove(message.Key);
                            Log(eCacheEvent.KeySetInMemory, message.Key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        /// <summary>
        /// Helper function that waits for a caching event on this instance
        /// </summary>
        /// <param name="cacheEvent"></param>
        /// <param name="maxWaitMilliseconds"></param>
        /// <returns></returns>
        public async Task<long> WaitForEvent(eCacheEvent cacheEvent, int maxWaitMilliseconds = 20000)
        {
            var sw = Stopwatch.StartNew();
            while (LastCachingEvent != cacheEvent)
            {
                if (sw.ElapsedMilliseconds > maxWaitMilliseconds) 
                    throw new Exception(string.Format("Max wait time elapsed waiting for status '{0}', but was '{1}'", cacheEvent, LastCachingEvent));
                await Task.Delay(20);
            }
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Waits for any pending [Set] operation to complete
        /// </summary>
        /// <param name="maxWaitMilliseconds"></param>
        /// <returns></returns>
        public async Task<long> WaitForPendingSetOperationsToComplete(int maxWaitMilliseconds = 20000)
        {
            await Task.Delay(100);
            var sw = Stopwatch.StartNew();
            while (_isPendingSetOperation)
            {
                if (sw.ElapsedMilliseconds > maxWaitMilliseconds)
                    throw new Exception("Max wait time elapsed waiting for set op to complete");
                await Task.Delay(50);
            }
            return sw.ElapsedMilliseconds;
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
            if (expirationUtc.HasValue) policy.AbsoluteExpiration = new DateTimeOffset(expirationUtc.Value);
            if (data == null) _memoryCache.Remove(key);
            else _memoryCache.Set(key, data, policy);
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
            DateTime? expires = (!expiration.HasValue ? null as DateTime? : (DateTime.UtcNow.Add(expiration.Value)));
            SetInMemoryCache(key, value, expires, recordAsSessionKey);
        }

        /// <summary>
        /// Removes an item from MemoryCache
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        private void OnKeyDeleted(RedisChannel channel, RedisValue value)
        {
            _memoryCache.Remove(value);
            Log(eCacheEvent.KeyDeletedInMemory, value);
        }

        /// <summary>
        /// Clears the local memory caches
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        private void OnClearCache(RedisChannel channel, RedisValue value)
        {
            CreateCloningMemoryCache();
            CreateFastUncloningMemoryCache();
            Log(eCacheEvent.ClearedCache, "");
        }


        private void Log(eCacheEvent cachingEvent, string key, [CallerMemberName] string callerName = null)
        {
            if (_config.IsAuditingEnabled)
            {
                _auditLog.Add(new CacheAuditLogItem { CachingEvent = cachingEvent, CallerFunctionName = callerName, Key = key });
            }
            LastCachingEvent = cachingEvent;
        }

        /// <summary>
        /// Helper function for retrieving the audit log text
        /// </summary>
        /// <returns></returns>
        public string GetAuditLogText()
        {
            return _config.Name + Environment.NewLine + string.Join(Environment.NewLine, _auditLog.Select(x => string.Format("{0}: event:{1}, func:{2}, key:{3}", 
                x.DateTime, x.CachingEvent, x.CallerFunctionName, x.Key)).ToArray());
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
        /// Deletes all data from the cache server
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync()
        {
            CreateCloningMemoryCache();
            CreateFastUncloningMemoryCache();
            await _subscriber.PublishAsync(CHANNEL_CLEAR_CACHE, RedisValue.EmptyString);

            var endPoints = _connection.GetEndPoints();
            foreach (var endPoint in endPoints)
            {
                await _connection.GetServer(endPoint).FlushAllDatabasesAsync();
            }
        }

        /// <summary>
        /// Deletes all data from the cache server
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            CreateCloningMemoryCache();
            CreateFastUncloningMemoryCache();
            _subscriber.Publish(CHANNEL_CLEAR_CACHE, RedisValue.EmptyString);

            var endPoints = _connection.GetEndPoints();
            foreach (var endPoint in endPoints)
            {
                _connection.GetServer(endPoint).FlushAllDatabases();
            }
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
                        if (_cacheDatabase != null && Status == State.Connected) _cacheDatabase.KeyDelete(key);
                    }
                    _keysSetInSession.Clear();
                    LastCachingEvent = eCacheEvent.None;
                }
                else throw new Exception("The flag 'ShouldRecordKeysInSession' was not on.");
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
            var t = SetAsync(key, value, cacheExpiry);
        }

        /// <summary>
        /// Validates the data going into the Cache and then sets the item into the cache.
        /// The method will return very quickly, because it'll set things to the cache asynchronously (the client code can be agnostic about this fact).
        /// This method is purposely not with Async in the name because it should be treated synchronously.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheExpiry">Pass in null for no cache expiry</param>
        /// <returns></returns>
        public void Set<T>(string domain, string key, T value, TimeSpan? cacheExpiry) => Set(CreateKey(domain, key), value, cacheExpiry);

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
            _isPendingSetOperation = true;
            key = CleanKey(key);
            try
            {
                await InitialiseIfNecessaryAsync(); // attempts to connect, or awaits on a pre-invoked connection. If connection is complete, this [SetAsync] task will continue immediately.

                PutInFastCache(key, value);

                DateTime? expirationUtc = null;
                if (cacheExpiry.HasValue) expirationUtc = DateTime.UtcNow.Add(cacheExpiry.Value);
                byte[] buffer = ToByteBuffer(value, _config.IsPayloadCompressionEnabled, new Dictionary<string, object>() { ["ExpirationUtc"] = expirationUtc });
                SetInMemoryCache(key, buffer, cacheExpiry);
                Log(eCacheEvent.KeySetInMemory, key);

                if (Status == State.Connected)
                {
                    if (_cacheDatabase == null) throw new Exception("The cache database is null, in-spite of it being connected");
                    await PutItemIntoCentralCache(key, buffer, cacheExpiry);
                    await DistributeCacheItem(key, buffer, expirationUtc);
                }

            }
            catch (Exception ex)
            {
                Log(ex);
                if (_config.IsExceptionPropagationEnabled) throw;
            }
            finally
            {
                _isPendingSetOperation = false;
            }
        }

        private void PutInFastCache<T>(string key, T value) 
            => _fastMemcache.Add(key, value, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) });

        /// <summary>
        /// Puts an item into central redis cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="buffer"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        private async Task PutItemIntoCentralCache(string key, byte[] buffer, TimeSpan? expiry)
        {
            if (_config.IsCentralCacheEnabled && Status == State.Connected)
            {
                // Set the item into the central cache
                await _cacheDatabase.StringSetAsync(key, buffer, expiry);
                Log(eCacheEvent.KeySetCentrally, key);
            }
        }

        /// <summary>
        /// Sends a cache item to other nodes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task DistributeCacheItem(string key, byte[] data, DateTime? expirationUtc)
        {
            if (_config.IsDistributedCachingEnabled && _subscriber != null)
            {
                var msg = new DistributedCacheMessage
                {
                    Key = key,
                    Buffer = data,
                    SenderCacheName = Name,
                    ExpirationUtc = expirationUtc
                };
                
                await _subscriber.PublishAsync(CHANNEL_KEY_UPDATES, ToByteBuffer(msg, false));
            }
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
        public async Task<CacheResponseDto<T>> GetWithMetaDataAsync<T>(string key)
        {
            key = CleanKey(key);
            var dto = new CacheResponseDto<T>().StartTiming();
            try
            {
                if (!TryGetFromMemoryCache(key, ref dto)) // try memory cache first
                {
                    if (IsRedisAccessible()) // Couldn't get it from memory, so try redis
                    {
                        var byteBuffer = await _cacheDatabase.StringGetAsync(key);
                        if (!byteBuffer.IsNullOrEmpty)
                        {
                            Log(eCacheEvent.KeyValueGotFromCentralAttempt, key);
                            PostProcessRedisBuffer(key, ref dto, byteBuffer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_config.IsExceptionPropagationEnabled) throw;
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
                if (!TryGetFromMemoryCache(key, ref dto))
                {
                    if (IsRedisAccessible())
                    {
                        var byteBuffer = _cacheDatabase.StringGet(key);
                        PostProcessRedisBuffer(key, ref dto, byteBuffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                dto.Exception = ex;
            }
            return dto.StopTiming();
        }

        /// <summary>
        /// Returns whether redis is accessible and useable. It will lazily initiate a connection if necessary
        /// </summary>
        /// <returns></returns>
        private bool IsRedisAccessible()
        {
            if (_config.IsCentralCacheEnabled)
            {
                if (Status == State.Connected) return true;
                else if (Status == State.NotConnected)
                {
#pragma warning disable 4014
                    InitialiseIfNecessaryAsync(); // do not await this.  We're happy for this to continue async to this task.
#pragma warning restore 4014
                }
                return false;
            }
            else return false;
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

            #region Fast uncloned cache
            if (_fastMemcache.Contains(key))
            {
                dto.Data = (T) _fastMemcache.Get(key);
                dto.IsFromInMemoryCache = true;
                Log(eCacheEvent.KeyValueGotFromMemory, key);
                return true;
            }
            #endregion


            byte[] buffer = null;
            if ((buffer = (byte[]) _memoryCache.Get(key)) != null)
            {
                dto.Data = FromByteBuffer<T>(buffer, true).Object;
                dto.IsFromInMemoryCache = true;
                Log(eCacheEvent.KeyValueGotFromMemory, key);
                return true;
            } 
            else return false;
        }

        /// <summary>
        /// Deserializes a redis buffer, puts it into the DTO, puts it into the memory cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dto"></param>
        /// <param name="byteBuffer"></param>
        private void PostProcessRedisBuffer<T>(string key, ref CacheResponseDto<T> dto, byte[] byteBuffer)
        {
            var item = FromByteBuffer<T>(byteBuffer, true);
            if (item != null) dto.Data = item.Object;
            dto.IsFromCentralCacheServer = true;
            if (item != null) Log(eCacheEvent.KeyValueGotFromCentral, key);
            // Put it into the in-memory cache
            if (item != null)
            {
                SetInMemoryCache(key, byteBuffer, item.Properties.Get("ExpirationUtc") as DateTime?);
                PutInFastCache(key, item.Object);
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
                _fastMemcache.Remove(key);
                _memoryCache.Remove(key);
                Log(eCacheEvent.KeyDeletedInMemory, key);

                await InitialiseIfNecessaryAsync(); // attempts to connect, or awaits on a pre-invoked connection. If connection is complete, this [DeleteAsync] task will continue immediately.

                // When this task resumes, the cache server MAY be connected, it may not (if there was an issue connecting)
                if (Status == State.Connected) // check the state, just to be sure.
                {
                    if (_cacheDatabase == null) throw new Exception("The cache database is null, in-spite of it being connected");
                     
                    retVal = await _cacheDatabase.KeyDeleteAsync(key);
                    Log(eCacheEvent.KeyDeletedCentrally, key);

                    if (_config.IsDistributedCachingEnabled)
                        await _subscriber.PublishAsync(CHANNEL_KEY_DELETES, key);
                }
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
            if (string.IsNullOrWhiteSpace(key)) throw new Exception("The key was invalid");
            return key.ToLower();
        }

        public string CreateKey(string domain, string key) => CreateKey(string.Concat(domain, "_", key));
        public string CreateKey(string key) => key.ToLower();
        public T Get<T>(string domain, string key) => Get<T>(CreateKey(domain, key));
        public async Task<T> GetAsync<T>(string domain, string key) => await GetAsync<T>(CreateKey(domain, key));
        public async Task<bool> DeleteAsync(string domain, string key) => await DeleteAsync(CreateKey(domain, key));

        #endregion
        
        
        private byte[] ToByteBuffer<T>(T o, bool compress, Dictionary<string, object> metadata = null)
        {
            byte[] retVal = null;

            if (o != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new BsonWriter(memoryStream))
                    {
                        CreateJsonSerializer().Serialize(writer, new ObjectEnvelope<T>(o, metadata));
                    }
                    retVal = memoryStream.ToArray();
                }

                if (compress) retVal = retVal.Compress();
            }
            return retVal;
        }

        private ObjectEnvelope<T> FromByteBuffer<T>(byte[] buffer, bool decompress)
        {
            if (buffer == null) return new ObjectEnvelope<T>(default(T), null);
            if (decompress) buffer = buffer.Decompress();
            
            using (var memoryStream = new MemoryStream(buffer))
            {
                using (var reader = new BsonReader(memoryStream))
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
            if (_connection != null)
            {
                ConnectionAttemptsCount = 0;
                _connection.Dispose();
                _connection = null;
                Status = State.NotConnected;
            }

            if (_memoryCache != null) _memoryCache.Dispose();
        }


        public string Keyify(params object[] items) => string.Join("-", items.Select(x => x?.ToString().Clean() ?? "null"));

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

            relationshipKey = _config.ProcessKey(relationshipKey);

            if (retVal == null)
            {
                retVal = await asyncFactory();
                if (retVal != null)
                {
                    await SetAsync(key, retVal);

                    if (relationshipKey.Clean() != null && _config.IsCentralCacheEnabled)
                        await _cacheDatabase.StringAppendAsync(relationshipKey.ToLower(), string.Concat(key, ";"));
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

            relationshipKey = _config.ProcessKey(relationshipKey);

            if (retVal == null)
            {
                retVal = factory();
                if (retVal != null)
                {
                    await SetAsync(key, retVal);
                    if (relationshipKey.Clean() != null && _config.IsCentralCacheEnabled)
                        await _cacheDatabase.StringAppendAsync(relationshipKey.ToLower(), string.Concat(key, ";"));
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
            relationshipKey = _config.ProcessKey(relationshipKey);

            if (retVal == null)
            {
                retVal = factory();
                if (retVal != null)
                {
                    Set(key, retVal);
                    if (relationshipKey.Clean() != null && _config.IsCentralCacheEnabled)
                        _cacheDatabase.StringAppend(relationshipKey.ToLower(), string.Concat(key, ";"));
                }
            }
            return retVal;
        }

        /// <summary>
        /// Using the rel. key passed in, it gets all cache keys that have been created in relation to the key
        /// and then clears all those cache items including the rel. key item itself.
        /// </summary>
        /// <param name="relationshipKey"></param>
        /// <returns></returns>
        public async Task ClearRelatedCacheKeysAsync(string relationshipKey)
        {
            if (!_config.IsCentralCacheEnabled) return;
            
            var data = (string) await _cacheDatabase.StringGetAsync(_config.ProcessKey(relationshipKey));
            if (data != null)
            {
                var cacheKeys = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Clean() != null).ToArray();
                foreach (var cacheKey in cacheKeys)
                {
                    await DeleteAsync(cacheKey);
                }
                await DeleteAsync(relationshipKey);
            }
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

        public IGrouping<string, KeyValuePair<string, string>>[] GetRedisMemoryUsage() => 
            _connection.GetServer(_connection.GetEndPoints()[0]).Info("Memory");
        
    }


}
