using System.Security.Principal;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Edubase.Services.Domain;
using Edubase.Services.Downloads;
using Edubase.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerRequestScheduleExtractTests
    {


        [Fact]
        public async Task RequestScheduledExtract_RedirectsToDownloadGenerated_WhenResponseContainsFileLocationUri()
        {
            // Arrange
            int eid = 123;
            Guid guid = Guid.NewGuid();
            string responseJson = "{\"fileLocationUri\":\"https://example.com/download?id=" + guid + "\"}";

            Mock<IDownloadsService> mockService = new();
            mockService.Setup(s => s.GenerateScheduledExtractAsync(eid, It.IsAny<IPrincipal>()))
                       .ReturnsAsync(responseJson);

            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/RequestScheduledExtract/{eid}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith($"/Downloads/Generated/", response.Headers.Location.ToString());
            Assert.Contains("?isExtract=True", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task RequestScheduledExtract_RedirectsToDownloadGenerated_WhenResponseContainsApiResultDto()
        {
            // Arrange
            int eid = 123;
            ApiResultDto<Guid> apiResult = new ApiResultDto<Guid> { Value = Guid.NewGuid() };
            string responseJson = JsonConvert.SerializeObject(apiResult);

            Mock<IDownloadsService> mockService = new();
            mockService.Setup(s => s.GenerateScheduledExtractAsync(eid, It.IsAny<IPrincipal>()))
                       .ReturnsAsync(responseJson);

            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/RequestScheduledExtract/{eid}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Downloads/Generated", response.Headers.Location.ToString());
            Assert.DoesNotContain("isExtract=True", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task RequestScheduledExtract_ReturnsErrorView_WhenApiWarningReturned()
        {
            // Arrange
            int eid = 123;
            ApiWarning apiWarning = new ApiWarning { Message = "Already generating", Code = "ERR001" };
            string responseJson = JsonConvert.SerializeObject(apiWarning);

            Mock<IDownloadsService> mockService = new();
            mockService.Setup(s => s.GenerateScheduledExtractAsync(eid, It.IsAny<IPrincipal>()))
                       .ReturnsAsync("{\"code\":\"ERR001\",\"message\":\"Already generating\"}");

            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/RequestScheduledExtract/{eid}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Download Error", document.QuerySelector("h1").Text());
            Assert.Equal("Request Error", document.QuerySelector("h3").Text());
            Assert.Contains("An extract is already being generated", document.Body.TextContent);
            Assert.Contains("Please wait a few minutes for the extract to complete. " +
                                "If nothing happens after that time, please try again.", document.Body.TextContent);
        }

        [Fact]
        public async Task RequestScheduledExtract_ReturnsErrorView_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            int eid = 123;
            Mock<IDownloadsService> mockService = new();
            mockService.Setup(s => s.GenerateScheduledExtractAsync(eid, It.IsAny<IPrincipal>()))
                       .ThrowsAsync(new Exception("Something went wrong"));

            using WebApplicationFactory<Program> factory = CreateFactory(mockService);
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/RequestScheduledExtract/{eid}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Download Error", document.QuerySelector("h1").Text());
            Assert.Equal("Unknown Error", document.QuerySelector("h3").Text());
            Assert.Contains("We could not generate your download due to a system issue", document.Body.TextContent);
            Assert.Contains("Please try again shortly or contact support.", document.Body.TextContent);
        }

        private WebApplicationFactory<Program> CreateFactory(Mock<IDownloadsService> mockService)
        {
            return new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();
                        services.AddSingleton(mockService.Object);
                    });
                });
        }
    }
}
