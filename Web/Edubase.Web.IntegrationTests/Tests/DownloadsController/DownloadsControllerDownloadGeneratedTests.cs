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

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerDownloadGeneratedTests
    {

        [Fact]
        public async Task DownloadGenerated_ReturnsReadyToDownload_WhenIsComplete_AndIsExtractFalse()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var progressDto = new ProgressDto
            {
                FileLocationUri = "https://example.com/download?id=123",
                IsComplete = true,
                Error = string.Empty
            };

            var mockService = new Mock<IDownloadsService>();
            mockService.Setup(s => s.GetProgressOfGeneratedExtractAsync(It.IsAny<Guid>(), It.IsAny<IPrincipal>()))
                       .ReturnsAsync(progressDto);

            using var factory = CreateFactory(mockService);
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/Downloads/Generated/{guid}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal("Download generation completed", document.QuerySelector("#generation-completed-heading").Text());
            Assert.Equal("Your extract is ready", document.QuerySelector("#extract-ready-heading").Text());
            Assert.Equal("Your requested file is ready to download.", document.QuerySelector("#download-ready-body").Text());

            IElement returnSourceElement = document.QuerySelector("#returnSource");
            Assert.Contains("returnSource", returnSourceElement.GetAttribute("name"));
            Assert.Contains("hidden", returnSourceElement.GetAttribute("type"));
            Assert.Contains("Downloads", returnSourceElement.GetAttribute("value"));
        }

        [Fact]
        public async Task DownloadGenerated_ReturnsReadyToDownload_WhenIsComplete_AndIsExtractTrue()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var progressDto = new ProgressDto
            {
                FileLocationUri = "https://example.com/download?id=123",
                IsComplete = true,
                Error = string.Empty
            };

            var mockService = new Mock<IDownloadsService>();
            mockService.Setup(s => s.GetProgressOfScheduledExtractGenerationAsync(guid, It.IsAny<IPrincipal>()))
                       .ReturnsAsync(progressDto);

            using var factory = CreateFactory(mockService);
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/Downloads/Generated/{guid}?isExtract=true");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal("Extract generation completed", document.QuerySelector("#generation-completed-heading").Text());
            Assert.Equal("Your extract is ready", document.QuerySelector("#extract-ready-heading").Text());
            Assert.Equal("Your requested file is ready to download.", document.QuerySelector("#download-ready-body").Text());

            IElement returnSourceElement = document.QuerySelector("#returnSource");
            Assert.Contains("returnSource", returnSourceElement.GetAttribute("name"));
            Assert.Contains("hidden", returnSourceElement.GetAttribute("type"));
            Assert.Contains("Extracts", returnSourceElement.GetAttribute("value"));
        }


        [Fact]
        public async Task DownloadGenerated_RendersPreparingView_WhenNotComplete()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var progressDto = new ProgressDto
            {
                IsComplete = false,
                Error = "",
                FileLocationUri = "https://example.com/download?id=123"
            };

            var mockService = new Mock<IDownloadsService>();
            mockService.Setup(s => s.GetProgressOfGeneratedExtractAsync(It.IsAny<Guid>(), It.IsAny<IPrincipal>()))
                       .ReturnsAsync(progressDto);

            using var factory = CreateFactory(mockService);
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/Downloads/Generated/{guid}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Download generation", document.QuerySelector("#generation-heading").Text());
            Assert.Equal("Please wait while your file is being generated", document.QuerySelector("#file-being-generated-heading").Text());
            Assert.Contains("Generating your requested data. Large files may take some time to generate.", document.QuerySelector("#file-being-generated-heading + p").Text());
            Assert.Contains("Please wait", document.QuerySelector(".gias-wait-mask").Text());
        }


        [Fact]
        public async Task DownloadGenerated_RendersErrorView_WhenIdIsInvalid()
        {
            // Arrange
            var mockService = new Mock<IDownloadsService>();
            using var factory = CreateFactory(mockService);
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Downloads/Generated/not-a-guid");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal("Download error", document.QuerySelector("h1").Text());
            Assert.Equal("The file you have requested no longer exists.", document.QuerySelector("#main-content h2.govuk-heading-m").Text());
            Assert.Equal("This may be due to the following factors:", document.QuerySelector("#main-content h2.govuk-heading-m + p").Text());
            Assert.Contains("Your file successfully downloaded, but the request was repeated. Please check your browser window."
                , document.QuerySelector("#main-content ul.govuk-list--bullet li").Text().Trim());
            Assert.Contains("Your original download request was cancelled."
                , document.QuerySelector("#main-content ul.govuk-list--bullet li + li").Text().Trim());
            Assert.Contains("If so, please return to the downloads page and repeat your request."
                , document.QuerySelector("#main-content ul.govuk-list--bullet li + li").Text().Trim());
        }

        [Fact]
        public async Task DownloadGenerated_RendersErrorView_WhenHasErroredIsTrue()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var progressDto = new ProgressDto
            {
                IsComplete = false,
                Error = "Something went wrong",
                FileLocationUri = "https://example.com/download?id=123"
            };

            var mockService = new Mock<IDownloadsService>();
            mockService.Setup(s => s.GetProgressOfGeneratedExtractAsync(It.IsAny<Guid>(), It.IsAny<IPrincipal>()))
                       .ReturnsAsync(progressDto);

            using var factory = CreateFactory(mockService);
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/Downloads/Generated/{guid}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal("Something went wrong", document.QuerySelector("h1").Text());
            Assert.Equal("The requested file is no longer available for download.", document.QuerySelector("#main-content h2.govuk-heading-m").Text());
            Assert.Equal("Please return to the downloads page and repeat your request.", document.QuerySelector("#main-content h2.govuk-heading-m + p").Text());
        }

        [Fact]
        public async Task DownloadGenerated_RendersErrorView_WhenApiThrows404()
        {
            // Arrange
            var mockService = new Mock<IDownloadsService>();
            mockService.Setup(s => s.GetProgressOfGeneratedExtractAsync(It.IsAny<Guid>(), It.IsAny<IPrincipal>()))
                       .ThrowsAsync(new Exception("The API returned 404 Not Found"));

            using var factory = CreateFactory(mockService);
            var client = factory.CreateClient();

            var guid = Guid.NewGuid();

            // Act
            var response = await client.GetAsync($"/Downloads/Generated/{guid}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal("Something went wrong", document.QuerySelector("h1").Text());
            Assert.Equal("The requested file is no longer available for download.", document.QuerySelector("#main-content h2.govuk-heading-m").Text());
            Assert.Equal("Please return to the downloads page and repeat your request.", document.QuerySelector("#main-content h2.govuk-heading-m + p").Text());
        }


        [Fact]
        public async Task DownloadGenerated_ThrowsException_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var mockService = new Mock<IDownloadsService>();
            mockService.Setup(s => s.GetProgressOfGeneratedExtractAsync(It.IsAny<Guid>(), It.IsAny<IPrincipal>()))
                       .ThrowsAsync(new Exception("Some random error"));

            using var factory = CreateFactory(mockService);
            var client = factory.CreateClient();

            // Act
            var response  = await client.GetAsync($"/Downloads/Generated/{guid}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(response.StatusCode, System.Net.HttpStatusCode.InternalServerError);
            Assert.Contains("Download generation failed; Underlying error: '", document.Body.InnerHtml);
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
