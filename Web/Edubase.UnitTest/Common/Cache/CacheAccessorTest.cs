namespace Edubase.Integration.Tests.Cache
{
    using Edubase.Common.Cache;
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class CacheAccessorTest
    {
        private ICacheAccessor _cacheThatsConnected;

        [OneTimeSetUp]
        public void Setup()
        {
            // Create cache that is actually connected
            _cacheThatsConnected = CacheAccessor.Create();
            _cacheThatsConnected.InitialiseIfNecessaryAsync().Wait();
        }

        [Test]
        public async Task CacheExtension_SetAndGetMethods_ReturnActualValuesCorrectly()
        {
            // ARRANGE
            var key = "Employee25";
            await _cacheThatsConnected.SetAsync(key, new Employee(25, "Clayton Gragg"));

            // ACT
            var value = await _cacheThatsConnected.GetAsync<Employee>(key); // should return null, as the connection 
            bool deletionResult = await _cacheThatsConnected.DeleteAsync(key);

            Assert.That(value.Id, Is.EqualTo(25));
            Assert.IsTrue(deletionResult);
        }

        [Test]
        public void CacheData_TestThatMultipleInitialisationsResultsInOne()
        {
            var tempCache = CacheAccessor.Create();

            Parallel.For(0, 20, i =>
            {
                tempCache.InitialiseIfNecessaryAsync();
            });

            var connectingTask = tempCache.InitialiseIfNecessaryAsync();
            var status1 = tempCache.Status;
            connectingTask.Wait(); // wait for connection to complete
            var status2 = tempCache.Status;

            Assert.AreEqual(1, tempCache.ConnectionAttemptsCount, "Only 1 connection attempt should have been made in-spite of the bombardment of connection requests");
            Assert.IsTrue(status1 == CacheAccessor.State.NotConnected || status1 == CacheAccessor.State.Connecting);
            Assert.AreEqual(CacheAccessor.State.Connected, status2, "The cache should have connected by status2");
        }

        [Test]
        public async Task CacheData_WhenCacheIsUnavailable_ReturnDefaultValueAndCheckConnectionTimeout()
        {
            // Arrange
            var tempCache = new CacheAccessor(new CacheConfig
            {
                ConnectionString = "invalid_connection_string"
            }); // configured with an incorrect server and will return null on error

            // Act
            var testValue = await tempCache.GetAsync<string>("lovely_non_existent_item");
            Thread.Sleep(50);
            var status1 = tempCache.Status;

            await tempCache.InitialiseIfNecessaryAsync();
            var status2 = tempCache.Status;

            // Assert
            Assert.IsNull(testValue);
            Assert.AreEqual(CacheAccessor.State.Connecting, status1);
            Assert.AreEqual(CacheAccessor.State.NotConnected, status2);
            Assert.IsTrue(tempCache.LastConnectionElapsedMilliseconds > 0 && tempCache.LastConnectionElapsedMilliseconds < 12000,
                "The connection timeout should be more than 0 but less than 12 seconds. Actual timeouts are never inline with the stated max timeout, so for 10ms, the actual timeout is ~5s");
        }

        [Test]
        public async Task CacheData_WhenCacheIsUnavailable_ReturnNull()
        {
            // Arrange
            var tempCache = new CacheAccessor(new CacheConfig
            {
                ConnectionString = "invalid_connection_string"
            }); // configured with an incorrect server and will return null on error

            // Act
            var testValue = await tempCache.GetAsync<string>("lovely_non_existent_item");

            Assert.IsNull(testValue);
        }

        //[Test]
        //public async Task CacheData_WhenCacheIsUnavailable_InterceptErrorsViaLogger()
        //{
        //    // Arrange
        //    var logger = MockRepository.GenerateStub<ILogger>();
        //    logger.Stub(x => x.Error(Arg<string>.Is.Anything)).IgnoreArguments();

        //    var tempCache = new CacheAccessor(new CacheConfig
        //    {
        //        ConnectionString = "invalid_connection_string"
        //        Logger = logger
        //    });

        //    // Act
        //    var testValue = await tempCache.GetAsync<string>("lovely_non_existent_item");
        //    await tempCache.InitialiseIfNecessaryAsync();


        //    // Assert
        //    Assert.AreEqual(CacheAccessor.State.NotConnected, tempCache.Status);
        //    Assert.IsNull(testValue);
        //    logger.AssertWasCalled(x => x.Error(Arg<string>.Is.Anything));
        //}

        //[Test]
        //public async Task Cache_SetAndGet_CheckValues()
        //{
        //    // Arrange
        //    var key = Key();
        //    var value = CreateGuid();

        //    // Act
        //    await _cacheThatsConnectedOps.SetAsync(key, value);
        //    var value2 = await _cacheThatsConnected.GetAsync<string>(key);

        //    // Assert
        //    Assert.AreEqual(value, value2);

        //    // Clean up
        //    await _cacheThatsConnected.DeleteAsync(key);
        //}

        /// <summary>
        /// This test will test that when you attempt to get data from the cache where it's not been initialised previously,
        /// that it will immediately return null.
        /// </summary>
        /// <returns></returns>
        [Test, Ignore("Too time dependent")]
        public async Task Cache_TestThatConnectIsAsync()
        {
            const long MAX_CONNECTION_TIME_MS = 60000;

            // Arrange
            var key = Key();
            var value = CreateGuid();

            // Put something in the cache, the close the connection
            using (var cache = CacheAccessor.Create())
                await cache.SetAsync(key, value);

            // Re-create the cache
            using (var cache = CacheAccessor.Create())
            {
                // Act
                var cacheValue = await cache.GetAsync<string>(key);
                Assert.IsNull(cacheValue);

                var sw = Stopwatch.StartNew();
                while (cache.Status == CacheAccessor.State.NotConnected || cache.Status == CacheAccessor.State.Connecting)
                {
                    await Task.Delay(500);
                    Assert.IsFalse(sw.ElapsedMilliseconds > MAX_CONNECTION_TIME_MS, "Connecting took too long ( > {0})", sw.ElapsedMilliseconds);
                }
                sw.Stop();
                Assert.IsFalse(sw.ElapsedMilliseconds > MAX_CONNECTION_TIME_MS, "Connecting took too long ( > {0})", sw.ElapsedMilliseconds);

                cacheValue = await cache.GetAsync<string>(key);
                Assert.AreEqual(value, cacheValue);

                await cache.DeleteAsync(key);
            }
        }

        [Test]
        public void Cache_SetIsReallyAsync()
        {
            var cache = CacheAccessor.Create();
            var key = Key();
            var value = CreateGuid();

            Task settingTask = cache.SetAsync(key, value);

            List<bool> flags = new List<bool>();
            while (true)
            {
                flags.Add(settingTask.IsCompleted);
                if (settingTask.IsCompleted) break;
                Thread.Sleep(10);
            }

            Assert.IsTrue(flags.Where(x => x == false).Count() > 0, "There should be at least a few points where the setting task was running");
            Assert.IsTrue(flags.Where(x => x).Count() > 0, "Some flags should indicate the task completed");
        }

        [Test]
        public void Cache_TestSynchronously()
        {
            var key = Key();
            var value = CreateGuid();

            var settingTask = _cacheThatsConnected.SetAsync(key, value);
            settingTask.Wait();
            var value2 = _cacheThatsConnected.GetAsync<string>(key).Result;
            var deletionResult = _cacheThatsConnected.DeleteAsync(key).Result;

            Assert.AreEqual(value, value2);
            Assert.IsTrue(deletionResult);
        }

        [Test, Ignore("Too time dependent")]
        public async Task Cache_Dist_TestKeyValuePairPropagation()
        {
            // Arrange
            var KEY = Key();
            var VALUE = CreateGuid();
            var cacheAccessor1 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
            var cacheAccessor2 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
            await cacheAccessor1.InitialiseIfNecessaryAsync();
            await cacheAccessor2.InitialiseIfNecessaryAsync();

            cacheAccessor1.AuditLog.Clear();
            cacheAccessor2.AuditLog.Clear();

            // Act
            var v1 = await cacheAccessor1.GetWithMetaDataAsync<string>(KEY);
            await cacheAccessor1.SetAsync(KEY, VALUE, TimeSpan.FromMinutes(2));

            await Task.Delay(300);
            var logOfCA2 = cacheAccessor2.AuditLog.ToArray();

            var v2 = await cacheAccessor2.GetWithMetaDataAsync<string>(KEY);
            var v3 = await cacheAccessor2.GetWithMetaDataAsync<string>(KEY);

            var cacheAccessor3 = new CacheAccessor(new CacheConfig());
            await cacheAccessor3.InitialiseIfNecessaryAsync();
            var v6 = await cacheAccessor3.GetWithMetaDataAsync<string>(KEY);
            var v7 = await cacheAccessor3.GetWithMetaDataAsync<string>(KEY);

            await cacheAccessor2.DeleteAsync(KEY);
            var v4_1 = await cacheAccessor1.GetWithMetaDataAsync<string>(KEY);
            var v5_1 = await cacheAccessor2.GetWithMetaDataAsync<string>(KEY);
            await Task.Delay(300);
            var v4 = await cacheAccessor1.GetWithMetaDataAsync<string>(KEY);
            var v5 = await cacheAccessor2.GetWithMetaDataAsync<string>(KEY);


            Assert.IsTrue(logOfCA2.First().CachingEvent == eCacheEvent.KeySetInMemory, "The next audit event should have been to receive the key update message");
            Assert.IsTrue(v1.IsFromCentralCacheServer, "Trying to get something that isn't set, should _AT_LEAST_ try the central cache");
            Assert.IsTrue(v2.IsFromInMemoryCache, "Getting an item after setting it should mean that the in-memory cache has had time to catch up");
            Assert.IsTrue(v3.IsFromInMemoryCache, "The value should have propagated to cache accessor");
            Assert.IsNull(v4.Data, "The value should be null");
            Assert.IsNull(v5.Data, "The value should be null");
            Assert.IsTrue(v4.IsFromCentralCacheServer, "If a value is null, it should have tried the central cache server");
            Assert.IsTrue(v5.IsFromCentralCacheServer, "If a value is null, it should have tried the central cache server");

            Assert.IsNotNull(v6.Data, "The data shouldn't be null");
            Assert.IsTrue(v6.IsFromCentralCacheServer, "The value should have been retrieved from the central server");

            Assert.IsNotNull(v7.Data, "The data shouldn't be null");
            Assert.AreEqual(VALUE, v7.Data);
            Assert.IsTrue(v7.IsFromInMemoryCache, "The value should have been retrieved from the memory cache");
            
            // This test is too subjective for the build server
            // Assert.IsTrue(v7.ElapsedMilliseconds < v6.ElapsedMilliseconds, "It should have been quicker to get the value the second time");
        }

        [Test]
        public async Task Cache_Dist_Test_InMemoryPerformance()
        {
            // Arrange
            var cacheAccessor1 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
            var cacheAccessor2 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
            await cacheAccessor1.InitialiseIfNecessaryAsync();
            await cacheAccessor2.InitialiseIfNecessaryAsync();

            var bigList = new List<int>(Enumerable.Range(0, 100000));
            await cacheAccessor1.SetAsync("biglist", bigList);
            var v1 = await cacheAccessor1.GetWithMetaDataAsync<List<int>>("biglist"); // should return instantly from the originating cache accessor (it's in-memory)
            var v2 = await cacheAccessor2.GetWithMetaDataAsync<List<int>>("biglist"); // should return with some delay from the new cache accessor (it's on the central cache)
            var v3 = await cacheAccessor2.GetWithMetaDataAsync<List<int>>("biglist"); // should return instantly, having put the object into in-memory cache the last time it was called

            Assert.IsNotNull(v1.Data);
            Assert.IsTrue(v1.IsFromInMemoryCache, "Shoulda oughta a bin from the in-memory cache");
            Assert.IsNotNull(v2.Data);
            Assert.IsTrue(v2.IsFromCentralCacheServer || v2.IsFromInMemoryCache, "Shoulda oughta a bin from the central / memory cache");
            Assert.IsNotNull(v3.Data);
            Assert.IsTrue(v3.IsFromInMemoryCache, "Shoulda oughta a bin from the in-memory cache");
        }

        [Test, Ignore("Too time dependent")]
        public async Task Cache_Dist_Test_LargeVolumeOfCacheAccessors()
        {
            // Arrange
            var inits = new List<Task>();
            var caches = new List<CacheAccessor>();
            const int MAX_CACHES = 20;

            // Create MAX_CACHES cache accessors and therefore MAX_CACHES connections to redis
            for (int i = 0; i < MAX_CACHES; i++)
            {
                var c = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
                inits.Add(c.InitialiseIfNecessaryAsync());
                caches.Add(c);
            }
            
            await Task.WhenAll(inits.ToArray()); // wait for initialisation
            var cacheOne = caches.First();

            var KEY = Key();
            var VALUE = new List<int>(Enumerable.Range(0, 1000));

            // Clear audit logs of all caches
            caches.ForEach(x => x.AuditLog.Clear());

            // ACT
            await cacheOne.SetAsync(KEY, VALUE);
            var valueFromOriginator = await cacheOne.GetWithMetaDataAsync<List<int>>(KEY);

            // Wait until the value has propagated
            var propagationTasks = new List<Task>();
            foreach (var cacher in caches)
            {
                if (cacher == cacheOne) continue; // ignore the originating cacher
                propagationTasks.Add(cacher.WaitForEvent(eCacheEvent.KeySetInMemory, 60000));
            }
            await Task.WhenAll(propagationTasks.ToArray());


            var values = new List<CacheResponseDto<List<int>>>();
            for (int i = 0; i < MAX_CACHES; i++)
            {
                var currentCache = caches[i];
                values.Add(await currentCache.GetWithMetaDataAsync<List<int>>(KEY));
            }

            // ASSERT
            Assert.AreEqual(VALUE, valueFromOriginator.Data);
            Assert.IsTrue(valueFromOriginator.IsFromInMemoryCache);
            foreach (var item in values)
            {
                Assert.AreEqual(VALUE, item.Data);
                Assert.IsTrue(item.IsFromInMemoryCache);
            }

            for (int i = 1; i < MAX_CACHES; i++)
            {
                Assert.IsTrue(caches[i].AuditLog.First().CachingEvent == eCacheEvent.KeySetInMemory);
            }
        }

        [Test, Ignore("Based on timing info too much.  Works only intermittently.")]
        public async Task Cache_Dist_Test_That_Cache_Expiry_Works_OK()
        {
            // ARRANGE
            var ca1 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
            var ca2 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });

            await ca1.InitialiseIfNecessaryAsync();
            await ca2.InitialiseIfNecessaryAsync();

            var KEY = CreateGuid();
            var VALUE = CreateGuid();

            // ACT
            await ca1.SetAsync(KEY, VALUE, TimeSpan.FromSeconds(5));

            await ca2.WaitForEvent(eCacheEvent.KeySetInMemory);
            var v1 = await ca2.GetWithMetaDataAsync<string>(KEY);
            var v2 = await ca1.GetWithMetaDataAsync<string>(KEY);
            await Task.Delay(5000);
            var v3 = await ca2.GetWithMetaDataAsync<string>(KEY);
            var v4 = await ca1.GetWithMetaDataAsync<string>(KEY);

            Assert.IsNotNull(v1.Data);
            Assert.IsNotNull(v2.Data);
            Assert.IsTrue(v1.IsFromInMemoryCache);
            Assert.IsTrue(v2.IsFromInMemoryCache);

            Assert.IsNull(v3.Data);
            Assert.IsNull(v4.Data);
            Assert.IsTrue(v3.IsFromCentralCacheServer);
            Assert.IsTrue(v4.IsFromCentralCacheServer);
        }

        [Test, Ignore("Based on timing info too much.  Works only intermittently.")]
        public async Task Cache_Dist_Test_That_Cache_Delete_Key_Works_OK()
        {
            // ARRANGE
            var ca1 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
            var ca2 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });

            await ca1.InitialiseIfNecessaryAsync();
            await ca2.InitialiseIfNecessaryAsync();

            var KEY = CreateGuid();
            var VALUE = CreateGuid();

            // ACT
            await ca1.SetAsync(KEY, VALUE); // set the value

            // Wait for propagation to ca2
            await ca2.WaitForEvent(eCacheEvent.KeySetInMemory);
            var v1 = await ca2.GetWithMetaDataAsync<string>(KEY);
            await ca2.DeleteAsync(KEY);
            var propagationTimeInMS = await ca1.WaitForEvent(eCacheEvent.KeyDeletedInMemory);

            var v2 = await ca1.GetWithMetaDataAsync<string>(KEY);
            var v3 = await ca2.GetWithMetaDataAsync<string>(KEY);

            // ASSERT
            Assert.IsNotNull(v1.Data);
            Assert.IsTrue(v1.IsFromInMemoryCache);
            Assert.IsNull(v2.Data);
            Assert.IsTrue(v2.IsFromCentralCacheServer);
            Assert.IsNull(v3.Data);
            Assert.IsTrue(v3.IsFromCentralCacheServer);
        }

        [Test]
        public async Task Cache_NonDist_Test_That_Cache_Delete_Key_Works_OK()
        {
            // ARRANGE
            var ca1 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true, IsCentralCacheEnabled = false });
            
            await ca1.InitialiseIfNecessaryAsync();
            
            var KEY = CreateGuid();
            var VALUE = CreateGuid();

            // ACT
            await ca1.SetAsync(KEY, VALUE); // set the value
            await ca1.DeleteAsync(KEY);

            var evt1 = ca1.LastCachingEvent;
            var v2 = await ca1.GetWithMetaDataAsync<string>(KEY);

            // ASSERT
            Assert.IsNull(v2.Data);
            Assert.IsTrue(evt1 == eCacheEvent.KeyDeletedInMemory);
            Assert.IsTrue(ca1.LastCachingEvent == eCacheEvent.KeyValueGotFromMemoryAttempt);
        }


        /// <summary>
        /// This is an important test because the serialization format is binary, so it tests that byte arrays themselves are also fine
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Cache_Dist_Test_Caching_Byte_Arrays()
        {
            // ARRANGE
            var ca1 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });
            var ca2 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true });

            await ca1.InitialiseIfNecessaryAsync();
            await ca2.InitialiseIfNecessaryAsync();

            var KEY1 = CreateGuid();
            var VALUE1 = new byte[] { 32, 34, 55 };
            var KEY2 = CreateGuid();
            var VALUE2 = new byte[] { 1, 11, 33 };

            // ACT
            await ca1.SetAsync(KEY1, VALUE1); // set the value
            await ca2.WaitForEvent(eCacheEvent.KeySetInMemory);

            await ca2.SetAsync(KEY2, VALUE2); // set the value
            await ca1.WaitForEvent(eCacheEvent.KeySetInMemory);

            var data1 = await ca2.GetWithMetaDataAsync<byte[]>(KEY1);
            var data2 = await ca1.GetWithMetaDataAsync<byte[]>(KEY2);

            Assert.AreEqual(VALUE1.Length, data1.Data.Length);
            Assert.AreEqual(VALUE2.Length, data2.Data.Length);
            Assert.AreEqual(VALUE1[0], data1.Data[0]);
            Assert.AreEqual(VALUE1[1], data1.Data[1]);
            Assert.AreEqual(VALUE1[2], data1.Data[2]);

            Assert.AreEqual(VALUE2[0], data2.Data[0]);
            Assert.AreEqual(VALUE2[1], data2.Data[1]);
            Assert.AreEqual(VALUE2[2], data2.Data[2]);

        }



        [Test]
        public async Task Cache_TestImmutableRefTypeObjectDoesNOTCauseAnException()
        {
            var ca1 = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true, IsCentralCacheEnabled = false });
            var ops = ca1;
            await ca1.InitialiseIfNecessaryAsync();
            await ops.SetAsync(Guid.NewGuid().ToString(), "this object is immutable", null);
        }

        [Test]
        public async Task Cache_TestValueTypeObject_Roundtrips()
        {
            var cache = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true, IsCentralCacheEnabled = false });
            var ops = cache;
            var key = Guid.NewGuid().ToString("N");
            var value = 4658792;
            await cache.InitialiseIfNecessaryAsync();
            await ops.SetAsync(key, value, null);
            var value2 = await cache.GetAsync<int>(key);
            Assert.AreEqual(value, value2);
        }


        [Test]
        public async Task Cache_TestEnumerableObject_Roundtrips()
        {
            var cache = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true, IsCentralCacheEnabled = false });
            var ops = cache;
            var key = Guid.NewGuid().ToString("N");
            var value = (IEnumerable)new List<string> { "this is a thing in the list" };
            await cache.InitialiseIfNecessaryAsync();
            await ops.SetAsync(key, value, null);
            var value2 = await cache.GetAsync<IEnumerable>(key);
            Assert.AreEqual(value, value2);
        }

        [Test]
        public async Task Cache_TestDictionaryObject_Roundtrips()
        {
            var cache = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true, IsCentralCacheEnabled = false });
            var ops = cache;
            var key = Guid.NewGuid().ToString("N");
            var value = (IDictionary<string, string>)new Dictionary<string, string> { { "k", "v" } };
            await cache.InitialiseIfNecessaryAsync();
            await ops.SetAsync(key, value, null);
            var value2 = await cache.GetAsync<Dictionary<string, string>>(key);
            Assert.AreEqual(value, value2);
        }

        [Test]
        public async Task Cache_TestCustomObjects_AreNotClones()
        {
            var cache = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true, IsCentralCacheEnabled = false });
            var ops = cache;
            var key = Guid.NewGuid().ToString("N");
            var value = new Employee(3456, "test");
            await cache.InitialiseIfNecessaryAsync();
            await ops.SetAsync(key, value, null);
            var value2 = await cache.GetAsync<Employee>(key);

            Assert.AreEqual(value.Id, value2.Id);
            Assert.AreEqual(value.Name, value2.Name);
            Assert.AreNotEqual(value, value2);
        }

        [Test]
        public async Task Cache_TestWithoutRedis()
        {
            var cache = new CacheAccessor(new CacheConfig { IsAuditingEnabled = true, IsCentralCacheEnabled = false });
            var ops = cache;
            var key = Guid.NewGuid().ToString("N");
            var value = new Employee(3456, "test");
            await cache.InitialiseIfNecessaryAsync();
            await ops.SetAsync(key, value, null);
            var value2 = await cache.GetAsync<Employee>(key);

            Assert.AreEqual(cache.AuditLog[0].CachingEvent, eCacheEvent.KeySetInMemory);
            Assert.AreEqual(cache.AuditLog[1].CachingEvent, eCacheEvent.KeyValueGotFromMemoryAttempt);
            Assert.AreEqual(cache.AuditLog[2].CachingEvent, eCacheEvent.KeyValueGotFromMemory);
        }

        [OneTimeTearDown]
        public void ClearUp()
        {
            if (_cacheThatsConnected != null)
            {
                _cacheThatsConnected.Dispose();
                _cacheThatsConnected = null;
            }
        }


        private string Key()
        {
            var key = CreateGuid();
            return key;
        }

        private string CreateGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }


    [Serializable]
    class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Employee(int EmployeeId, string Name)
        {
            Id = EmployeeId;
            this.Name = Name;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
