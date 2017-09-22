using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Web.UI;
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
            var client = IocConfig.CreateHttpClient();
            var clientWrapper = new HttpClientWrapper(client, IocConfig.CreateJsonMediaTypeFormatter());
            var l = new LookupApiService(clientWrapper, new SecurityApiService(clientWrapper));
            var las = (await l.LocalAuthorityGetAllAsync()).ToArray();
            
            var subject = new LocalAuthoritySetRepository();
            for (int i = 0; i < 1000; i++)
            {
                var laIds = Enumerable.Range(0, RandomNumber.Next(0, 15)).Select(x => las.ElementAt(RandomNumber.Next(las.Length)).Id).ToArray();
                await subject.CreateAsync(new LocalAuthoritySet
                {
                    Ids = laIds,
                    Title = Lorem.Sentence(2)
                });
            }
            
            //var result = await subject.GetAllAsync(100);
            //Assert.AreEqual(100, result.Items.Count());
            //foreach (var item in result.Items)
            //{
            //    Assert.IsNotNull(item.Ids);
            //    Assert.That(item.Ids.Count, Is.EqualTo(3));
            //}


            //result = await subject.GetAllAsync(1000);
            //foreach (var item in result.Items)
            //{
            //    await subject.DeleteAsync(item.RowKey);
            //}
        }
    }
}
