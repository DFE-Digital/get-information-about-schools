using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.DbContext;
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
        public async Task WarmAsync_Test()
        {
            const int AMOUNT = 10;

            var dbf = (IApplicationDbContextFactory) new ApplicationDbContextFactory<ApplicationDbContext>();

            var imdbf = (IInMemoryApplicationDbContextFactory) new ApplicationDbContextFactory<InMemoryApplicationDbContext>();

            var repo = new EstablishmentReadRepository(dbf);
            var cache = new CacheAccessor(new CacheConfig { IsExceptionPropagationEnabled = true, IsPayloadCompressionEnabled = true });
            await cache.InitialiseIfNecessaryAsync();
            var subject = new CachedEstablishmentReadRepository(repo, cache, imdbf, dbf);

            var report = await subject.WarmAsync(AMOUNT, 1, AMOUNT);
            Assert.AreEqual($"Cached {AMOUNT} entities", report);
        }
    }
}
