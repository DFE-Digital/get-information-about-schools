using Edubase.IntegrationTest.Services.IntegrationEndPoints.AzureSearch;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.IntegrationTest.Services.Establishments
{
    [TestFixture]
    public class EstablishmentReadServiceTest
    {
        [Test]
        public async Task SearchAsyncTest()
        {
            var svc = new EstablishmentReadService(null, null, null, new AzureSearchEndPoint(AzureSearchEndPointTest.GetAZSConnStr()));
            var result = await svc.SearchAsync(new EstablishmentSearchPayload(nameof(SearchEstablishmentDocument.Name), 0, 200)
            {
                Text = "Academy",
                Filters = new EstablishmentSearchFilters { EstablishmentTypeGroupIds = new int[] { (int)eLookupEstablishmentTypeGroup.Academies } }
            }, new GenericPrincipal(new GenericIdentity(""), new string[0]));
            Assert.IsTrue(result.Count > 10);
            Assert.IsTrue(result.Items.All(x => x.Name.IndexOf("Academy", StringComparison.OrdinalIgnoreCase) > -1));
            Assert.IsTrue(result.Items.All(x => x.EstablishmentTypeGroupId == (int)eLookupEstablishmentTypeGroup.Academies));
        }
    }
}
