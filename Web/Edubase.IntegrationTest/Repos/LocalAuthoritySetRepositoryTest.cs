using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Faker;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.IntegrationTest.Repos
{
    [TestFixture]
    public class LocalAuthoritySetRepositoryTest
    {
        [Test]
        public async Task InsertQueryDeleteLASets()
        {
            var subject = new LocalAuthoritySetRepository();
            for (int i = 0; i < 100; i++)
            {
                await subject.CreateAsync(new LocalAuthoritySet
                {
                    Ids = new [] { RandomNumber.Next(1,100), RandomNumber.Next(1, 100), RandomNumber.Next(1, 100) },
                    Title = Lorem.Sentence(2)
                });
            }
            
            var result = await subject.GetAllAsync(100);
            Assert.AreEqual(100, result.Items.Count());
            foreach (var item in result.Items)
            {
                Assert.IsNotNull(item.Ids);
                Assert.That(item.Ids.Count, Is.EqualTo(3));
            }


            result = await subject.GetAllAsync(1000);
            foreach (var item in result.Items)
            {
                await subject.DeleteAsync(item.RowKey);
            }
        }
    }
}
