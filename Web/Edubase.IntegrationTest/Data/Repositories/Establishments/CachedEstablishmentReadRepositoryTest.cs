using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.Establishments;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.IntegrationTest.Data.Repositories.Establishments
{
    [TestFixture]
    public class CachedEstablishmentReadRepositoryTest
    {
        [Test]
        public async Task GetAsync_Test()
        {
            var dbf = new ApplicationDbContextFactory();
            var repo = new EstablishmentReadRepository(dbf);
            var cache = new CacheAccessor(new CacheConfig { IsExceptionPropagationEnabled = true, IsPayloadCompressionEnabled = true });
            await cache.InitialiseIfNecessaryAsync();
            var subject = new CachedEstablishmentReadRepository(repo, cache);

            var report = await subject.WarmAsync(10, 1, 10);

        }
    }
}
