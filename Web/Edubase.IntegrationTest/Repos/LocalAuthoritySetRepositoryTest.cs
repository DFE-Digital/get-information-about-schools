using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Web.UI;
using Faker;
using NUnit.Framework;

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
            for (var i = 0; i < 1000; i++)
            {
                var laIds = Enumerable.Range(0, RandomNumber.Next(0, 15)).Select(x => las.ElementAt(RandomNumber.Next(las.Length)).Id).ToArray();
                await subject.CreateAsync(new LocalAuthoritySet
                {
                    Ids = laIds,
                    Title = Lorem.Sentence(2)
                });
            }
        }
    }
}
