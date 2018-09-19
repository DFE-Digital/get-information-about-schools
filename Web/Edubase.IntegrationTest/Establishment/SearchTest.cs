using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Services;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Web.UI;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Edubase.IntegrationTest.Establishment
{
    [TestFixture]
    public class SearchTest
    {
        [Test]
        public async Task Search()
        {
            var client = IocConfig.CreateHttpClient();
            var clientWrapper = new HttpClientWrapper(client, IocConfig.CreateJsonMediaTypeFormatter());

            var cls = new CachedLookupService(new LookupApiService(clientWrapper, new SecurityApiService(clientWrapper)), new CacheAccessor(new JsonConverterCollection() { new DbGeographyConverter() }));
            var las = await cls.LocalAuthorityGetAllAsync();
            var pcs = await cls.ParliamentaryConstituenciesGetAllAsync();
            var ets = await cls.EstablishmentTypesGetAllAsync();
            var gors = await cls.GovernmentOfficeRegionsGetAllAsync();
            var GSSLAs = await cls.GSSLAGetAllAsync();
            var dios = await cls.DiocesesGetAllAsync();
            var dis = await cls.AdministrativeDistrictsGetAllAsync();

            var subject = new EstablishmentReadApiService(clientWrapper, null);
            var result = await subject.SearchAsync(new EstablishmentSearchPayload
            {
                Skip = 0,
                Take = 100,
                Filters = new EstablishmentSearchFilters
                {
                    LocalAuthorityIds = las.Select(x => x.Id).Skip(1).ToArray(),
                    ParliamentaryConstituencyIds = pcs.Select(x => x.Id).Skip(1).ToArray(),
                    TypeIds = ets.Select(x => x.Id).Skip(1).ToArray(),
                    GovernmentOfficeRegionIds = gors.Select(x => x.Id).Skip(1).ToArray(),
                    GSSLAIds = GSSLAs.Select(x => x.Id).Skip(1).ToArray(),
                    DioceseIds = dios.Select(x => x.Id).Skip(1).ToArray(),
                    AdministrativeDistrictIds = dis.Select(x => x.Id).Skip(1).ToArray(),
                }
            }, new GenericPrincipal(new GenericIdentity(""), new string[0]));

            Assert.IsTrue(result.Items.Count == 100);
        }
    }
}
