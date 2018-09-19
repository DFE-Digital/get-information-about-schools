using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Texuna.Establishments;
using Edubase.Web.UI;
using NUnit.Framework;

namespace Edubase.IntegrationTest.Establishment
{
    [TestFixture]
    public class IEBTSearchTest
    {
        [Test]
        public async Task Search()
        {
            var client = IocConfig.CreateHttpClient();
            var clientWrapper = new HttpClientWrapper(client, IocConfig.CreateJsonMediaTypeFormatter());
            var subject = new EstablishmentReadApiService(clientWrapper, null);
            var result = await subject.SearchAsync(new EstablishmentSearchPayload
            {
                Skip = 0,
                Take = 10,
                Filters = new EstablishmentSearchFilters
                {
                    NextActionRequiredByWELMin = new DateTime(2010, 1, 1),
                    NextActionRequiredByWELMax = new DateTime(2011, 1, 1)
                },
                Select = new List<string>
                {
                    nameof(EstablishmentSearchResultModel.Name),
                    nameof(EstablishmentSearchResultModel.LocalAuthorityId),
                    nameof(EstablishmentSearchResultModel.Address_CityOrTown),
                    nameof(EstablishmentSearchResultModel.StatusId),
                    nameof(EstablishmentSearchResultModel.TypeId),
                    nameof(EstablishmentSearchResultModel.NextGeneralActionRequired),
                    nameof(EstablishmentSearchResultModel.NextActionRequiredByWEL)
                }
            }, new GenericPrincipal(new GenericIdentity(""), new string[0]));

            result = await subject.SearchAsync(new EstablishmentSearchPayload
            {
                Skip = 0,
                Take = 10,
                Filters = new EstablishmentSearchFilters
                {
                    NextGeneralActionRequiredMin = new DateTime(2010, 1, 1),
                    NextGeneralActionRequiredMax = new DateTime(2011, 1, 1)
                },
                Select = new List<string>
                {
                    nameof(EstablishmentSearchResultModel.Name),
                    nameof(EstablishmentSearchResultModel.LocalAuthorityId),
                    nameof(EstablishmentSearchResultModel.Address_CityOrTown),
                    nameof(EstablishmentSearchResultModel.StatusId),
                    nameof(EstablishmentSearchResultModel.TypeId),
                    nameof(EstablishmentSearchResultModel.NextGeneralActionRequired),
                    nameof(EstablishmentSearchResultModel.NextActionRequiredByWEL)
                }
            }, new GenericPrincipal(new GenericIdentity(""), new string[0]));
        }
    }
}
