using Edubase.Data.Entity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.IntegrationTest.Data
{
    [TestFixture]
    public class OptimisticLockTest
    {
        [Test]
        public async Task ConcurrentUpdatesArePreventedAsync()
        {
            using (var dc1 = new ApplicationDbContext())
            {
                using (var dc2 = new ApplicationDbContext())
                {
                    var e1 = dc1.Establishments.First();
                    var e2 = dc2.Establishments.First();
                    Assert.AreEqual(e1.Urn, e2.Urn);
                    e1.Name += "1";
                    e2.Name += "2";

                    await dc1.SaveChangesAsync();
                    Assert.ThrowsAsync<DbUpdateConcurrencyException>(async ()=> await dc2.SaveChangesAsync());
                }
            }
        }
    }
}
