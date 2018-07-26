using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Common.Reflection;
using Edubase.Data;
using Edubase.Services;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Web.UI;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

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
