using System.Security.Principal;
using System.Text;
using System.Web;
using AngleSharp.Html.Dom;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerDownloadEstablishmentDataTests
    {

        [Fact]
        public async Task DownloadEstablishmentData_ThrowsException_WhenStateIsEmpty()
        {
            // Arrange
            int urn = 123;
            Mock<IEstablishmentReadService> mockService = new();
            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Establishment/{urn}?state=");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task DownloadEstablishmentData_ReturnsSelectFormatView_WhenDownloadTypeIsNull()
        {
            // Arrange
            int urn = 123;
            string json = "[]";
            string token = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(json));
            Mock<IEstablishmentReadService> mockService = new();
            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Establishment/{urn}?state={token}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Download data", document.QuerySelector("h1").TextContent);
            Assert.Contains("You can download your requested data in either CSV or XLSX format.", document.QuerySelector("#select-file-format-body").TextContent);
            Assert.Contains("The file will download as a ZIP file: open the ZIP to access its contents.", document.QuerySelector("#select-file-format-body").TextContent);
            Assert.Contains("Select your preferred format", document.QuerySelector("legend").TextContent);
            Assert.NotNull(document.QuerySelector("#downloadtype_csv"));
            Assert.NotNull(document.QuerySelector("#downloadtype_xlsx"));
            Assert.NotNull(document.QuerySelector("#select-and-continue-button"));
        }

        [Fact]
        public async Task DownloadEstablishmentData_ReturnsDownloadView_WhenDownloadTypeProvidedAndStartIsFalse()
        {
            // Arrange
            int urn = 123;
            string json = "[]";
            string token = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(json));
            DownloadType downloadType = DownloadType.csv;
            Mock<IEstablishmentReadService> mockService = new();
            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Establishment/{urn}?state={token}&downloadType={downloadType}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Download data", document.QuerySelector("h1").TextContent);
            Assert.Equal("Your requested file is ready to download.", document.QuerySelector("#download-file-ready-body").TextContent);
            Assert.Equal("Download", document.QuerySelector("#download-button").TextContent.Trim());
            Assert.Equal("The data is in CSV format and will download as a ZIP file. Open the ZIP file to access its content.", document.QuerySelector("#download-file-format-body").TextContent.Trim());
        }

        [Fact]
        public async Task DownloadEstablishmentData_RedirectsToDownloadUrl_WhenDownloadTypeProvidedAndStartIsTrue()
        {
            // Arrange
            int urn = 123;
            string json = "[]";
            string token = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(json));
            DownloadType downloadType = DownloadType.xlsx;
            Mock<IEstablishmentReadService> mockService = new();
            mockService.Setup(s => s.GetDownloadAsync(urn, downloadType, It.IsAny<IPrincipal>()))
                       .ReturnsAsync(new FileDownloadDto { Url = "https://example.com/file.csv" });

            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/Establishment/{urn}?state={token}&downloadType={downloadType}&start=true");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://example.com/file.csv", response.Headers.Location.ToString());
        }

        private WebApplicationFactory<Program> CreateFactory(Mock<IEstablishmentReadService> mockService)
        {
            return new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IEstablishmentReadService>();
                        services.AddSingleton(mockService.Object);
                    });
                });
        }
    }
}
