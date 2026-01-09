using System.Net;
using System.Security.Principal;
using AngleSharp.Html.Dom;
using Edubase.Services.Downloads;
using Edubase.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{


    public sealed class DownloadsControllerDownloadExtractAsyncTests
    {
        [Fact]
        public async Task DownloadExtractAsync_Redirects_WhenDownloadAvailable()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownload(isDownloadAvailable: true);

            HttpClient client = factory.CreateClient();

            // Get anti-forgery token
            HttpResponseMessage getResponse = await client.GetAsync("/Downloads");
            IHtmlDocument document = await getResponse.GetDocumentAsync();
            string token = document.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            var formData = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", token },
                { "path", "https://example.com/mock-path" },
                { "id", "test-id" },
                { "searchQueryString", "query" },
                { "searchSource", "Establishment" },
                { "returnSource", "Extracts" }
            };

            FormUrlEncodedContent content = new(formData);

            // Act
            HttpResponseMessage response = await client.PostAsync("/Downloads/Download/Extract", content);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://example.com/mock-path?id=test-id", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task DownloadExtractAsync_ReturnsErrorView_WhenDownloadNotAvailable()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownload(isDownloadAvailable: false);

            HttpClient client = factory.CreateClient();

            // Get anti-forgery token
            HttpResponseMessage getResponse = await client.GetAsync("/Downloads");
            IHtmlDocument document = await getResponse.GetDocumentAsync();
            string token = document.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            var formData = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", token },
                { "path", "https://example.com/mock-path" },
                { "id", "test-id" },
                { "searchQueryString", "query" },
                { "searchSource", "Establishment" },
                { "returnSource", "Extracts" }
            };

            FormUrlEncodedContent content = new(formData);

            // Act
            HttpResponseMessage response = await client.PostAsync("/Downloads/Download/Extract", content);
            IHtmlDocument errorDocument = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Download error", errorDocument.QuerySelector("h1").TextContent);
            Assert.Equal("The file you have requested no longer exists.", errorDocument.QuerySelector("#main-content h2.govuk-heading-m").TextContent);
            Assert.Equal("This may be due to the following factors:", errorDocument.QuerySelector("#main-content p").TextContent);
            Assert.Equal("Your file successfully downloaded, but the request was repeated. Please check your browser window.", errorDocument.QuerySelector("#main-content li").TextContent);
            Assert.Equal("Your original download request was cancelled. If so, please return to the scheduled extract page and repeat your request.", errorDocument.QuerySelector("#main-content li:nth-of-type(2)").TextContent);
            Assert.Equal("/Downloads#scheduled-extracts", errorDocument.QuerySelector("#main-content li a").GetAttribute("href"));
        }

        public static WebApplicationFactory<Program> CreateFactoryWithMockedDownload(bool isDownloadAvailable = true)
        {
            Mock<IDownloadsService> downloadsServiceMock = new();
            downloadsServiceMock
                .Setup(s => s.IsDownloadAvailable("/mock-path", "test-id", It.IsAny<IPrincipal>()))
                .ReturnsAsync(isDownloadAvailable);

            using WebApplicationFactory<Program> factory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();
                        services.AddSingleton(downloadsServiceMock.Object);
                    });
                });

            return factory;
        }
    }
}
