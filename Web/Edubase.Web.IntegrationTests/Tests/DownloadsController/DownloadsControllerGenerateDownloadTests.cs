using System.Net.Http;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Downloads;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerGenerateDownloadTests
    {
        [Fact]
        public async Task GenerateDownload_RedirectsToDownloadGenerated_WhenResponseContainsLowercaseFileLocationUri()
        {
            // Arrange
            Guid expectedId = Guid.NewGuid();
            string jsonResponse = $"{{\"fileLocationUri\":\"https://example.com/?id={expectedId}\"}}";

            using var webAppFactory = CreateFactoryWithDownloadResponse(jsonResponse);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            var redirectLocation = response.Headers.Location.ToString();
            Assert.Contains($"/Downloads/Generated/{expectedId}", redirectLocation);
            Assert.Contains("isExtract=True", redirectLocation);
        }

        [Fact]
        public async Task GenerateDownload_RedirectsToDownloadGenerated_WhenResponseContainsApiResultsDto()
        {
            // Arrange
            Guid expectedId = Guid.NewGuid();
            var apiResult = new ApiResultDto<Guid> { Value = expectedId };
            string jsonResponse = JsonConvert.SerializeObject(apiResult);

            using var webAppFactory = CreateFactoryWithDownloadResponse(jsonResponse);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            var redirectLocation = response.Headers.Location.ToString();
            Assert.Contains($"/Downloads/Generated/{expectedId}", redirectLocation);
            Assert.DoesNotContain("isExtract=True", redirectLocation);
        }

        // Bug https://github.com/DFE-Digital/get-information-about-schools/issues/801 raised against this test.
        // Waiting for feedback on whether to fix or leave test as-is.
        [Fact]
        public async Task GenerateDownload_Throws_WhenFileLocationUriMissingId()
        {
            // Arrange
            string jsonResponse = "{\"fileLocationUri\":\"https://example.com/\"}";

            using var webAppFactory = CreateFactoryWithDownloadResponse(jsonResponse);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GenerateDownload_Throws_WhenFileLocationUriContainsInvalidGuid()
        {
            // Arrange
            string jsonResponse = "{\"fileLocationUri\":\"https://example.com/?id=not-a-guid\"}";

            using var webAppFactory = CreateFactoryWithDownloadResponse(jsonResponse);

            HttpClient client = webAppFactory.CreateClient();

            // Act 
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GenerateDownload_Throws_WhenResponseIsEmpty()
        {
            // Arrange
            using var webAppFactory = CreateFactoryWithDownloadResponse(string.Empty);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GenerateDownload_Throws_WhenResponseIsMalformedJson()
        {
            // Arrange
            string jsonResponse = "{\"invalidJson\":}";

            using var webAppFactory = CreateFactoryWithDownloadResponse(jsonResponse);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GenerateDownload_Throws_WhenServiceThrowsException()
        {
            // Arrange
            var mockService = new Mock<IDownloadsService>();
            mockService.Setup(s => s.GenerateExtractAsync(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                       .ThrowsAsync(new InvalidOperationException("Service failure"));

            using var factory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();
                        services.AddSingleton(mockService.Object);
                    });
                });

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GenerateDownload_RedirectsWithEmptyGuid_WhenResponseHasNoFileLocationUriOrValue()
        {
            // Arrange
            string jsonResponse = "{}";

            using var webAppFactory = CreateFactoryWithDownloadResponse(jsonResponse);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Generate?id=test-id");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            var redirectLocation = response.Headers.Location.ToString();
            Assert.Contains("/Downloads/Generated/00000000-0000-0000-0000-000000000000", redirectLocation);
            Assert.DoesNotContain("isExtract", redirectLocation);
        }

        private WebApplicationFactory<Program> CreateFactoryWithDownloadResponse(string serviceResponse)
        {
            Mock<IDownloadsService> mockDownloadsService = new ();
            mockDownloadsService
                .Setup(s => s.GenerateExtractAsync(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(serviceResponse);

            WebApplicationFactory<Program> factory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();
                        services.AddSingleton(mockDownloadsService.Object);
                    });
                });

            return factory;
        }
    }
}
