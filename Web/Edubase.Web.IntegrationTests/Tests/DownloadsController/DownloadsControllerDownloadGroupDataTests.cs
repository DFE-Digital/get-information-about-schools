using System.Security.Principal;
using System.Text;
using System.Web;
using AngleSharp.Html.Dom;
using Edubase.Services.Domain;
using Edubase.Services.Groups.Downloads;
using Edubase.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerDownloadGroupDataTests
    {

        [Fact]
        public async Task DownloadGroupData_ReturnsBadRequest_WhenStateIsEmpty()
        {
            // Arrange
            int uid = 123;
            Mock<IGroupDownloadService> mockService = new();
            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Group/{uid}?state=");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DownloadGroupData_ReturnsBadRequest_WhenStateIsInvalid()
        {
            // Arrange
            int uid = 123;
            string invalidToken = "invalidToken";
            Mock<IGroupDownloadService> mockService = new();
            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Group/{uid}?state={invalidToken}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DownloadGroupData_ReturnsSelectFormatView_WhenDownloadTypeIsNull()
        {
            // Arrange
            int uid = 123;
            string token = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes("[]"));
            Mock<IGroupDownloadService> mockService = new();
            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Group/{uid}?state={token}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Select your preferred format", document.Body.TextContent);
        }

        [Fact]
        public async Task DownloadGroupData_ReturnsDownloadView_WhenDownloadTypeProvidedAndStartIsFalse()
        {
            // Arrange
            int uid = 123;
            string token = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes("[]"));
            DownloadType downloadType = DownloadType.csv;
            Mock<IGroupDownloadService> mockService = new();
            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Group/{uid}?state={token}&downloadType={downloadType}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Download", document.Body.TextContent);
        }

        [Fact]
        public async Task DownloadGroupData_RedirectsToDownloadUrl_WhenDownloadTypeProvidedAndStartIsTrue()
        {
            // Arrange
            int uid = 123;
            string token = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes("[]"));
            DownloadType downloadType = DownloadType.xlsx;
            Mock<IGroupDownloadService> mockService = new();
            mockService.Setup(s => s.DownloadGroupData(uid, downloadType, It.IsAny<IPrincipal>()))
                          .Returns(Task.FromResult(new DownloadDto { Url = "https://example.com/groupfile.csv" }));

            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Group/{uid}?state={token}&downloadType={downloadType}&start=true");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://example.com/groupfile.csv", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task DownloadGroupData_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            int uid = 123;
            string token = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes("[]"));
            DownloadType downloadType = DownloadType.csv;
            Mock<IGroupDownloadService> mockService = new();
            mockService.Setup(s => s.DownloadGroupData(uid, downloadType, It.IsAny<IPrincipal>()))
                       .ThrowsAsync(new Exception("Something went wrong"));

            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Group/{uid}?state={token}&downloadType={downloadType}&start=true");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        private WebApplicationFactory<Program> CreateFactory(Mock<IGroupDownloadService> mockService)
        {
            return new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IGroupDownloadService>();
                        services.AddSingleton(mockService.Object);
                    });
                });
        }
    }
}
