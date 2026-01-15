using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.TexunaUnitTests.Core;
using Edubase.Web.UI;
using Moq;
using Xunit;

namespace Edubase.Services.TexunaUnitTests.Establishments
{
    public class EstablishmentWriteApiServiceTests
    {
        [Fact]
        public async Task ValidateAsync_EmptySenIds_IsSetToNull_AndPreviousLocalAuthorityIdIsNotDefaulted()
        {
            var uri = new Uri("https://test.com/establishment/validate");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Add(uri, OkJson("{}"));

            var http = CreateWrapper(mockHandler);
            var readService = new Mock<IEstablishmentReadService>(MockBehavior.Loose);
            var sut = new EstablishmentWriteApiService(http, readService.Object);

            var model = new EstablishmentModel
            {
                SENIds = Array.Empty<int>(),
                HelpdeskPreviousLocalAuthorityId = null
            };

            var previousLaId = model.HelpdeskPreviousLocalAuthorityId;

            await sut.ValidateAsync(model, _principal);

            Assert.Equal(previousLaId, model.HelpdeskPreviousLocalAuthorityId);
            Assert.Null(model.SENIds);
        }


        private readonly IPrincipal _principal =
            new GenericPrincipal(new GenericIdentity(""), null);

        private static HttpClientWrapper CreateWrapper(HttpMessageHandler handler)
        {
            var clientStorage = new Mock<IClientStorage>(MockBehavior.Loose);
            clientStorage.SetupGet(x => x.IPAddress).Returns("127.0.0.1");

            return new HttpClientWrapper(
                new HttpClient(handler) { BaseAddress = new Uri("https://test.com/") },
                IocConfig.CreateJsonMediaTypeFormatter(),
                clientStorage.Object);
        }

        private static HttpResponseMessage OkJson(string json)
        {
            var msg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            msg.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return msg;
        }
    }
}
