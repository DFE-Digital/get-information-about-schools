using System.Net;
using System.Security.Principal;
using Edubase.Services.Downloads;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerDownloadMATClosureReportAsyncTests
    {
        [Fact(Skip = "Need assistance to become authorised")]
        public async Task DownloadMATClosureReportAsync_ReturnsFileStream_WhenReportExists()
        {
            // Arrange
            MemoryStream fileContent = new MemoryStream(new byte[] { 1, 2, 3 });
            HttpResponseMessage responseMessage = new(HttpStatusCode.OK)
            {
                Content = new StreamContent(fileContent)
            };
            responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            Mock<IDownloadsService> downloadsServiceMock = new();
            downloadsServiceMock
                .Setup(s => s.DownloadMATClosureReport(It.IsAny<IPrincipal>()))
                .ReturnsAsync(responseMessage);

            using WebApplicationFactory<Program> factory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();
                        services.AddSingleton(downloadsServiceMock.Object);
                    });
                });

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/MATClosureReport?filename=closure-report.csv");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.ToString());
            Assert.Equal("closure-report.csv", response.Content.Headers.ContentDisposition?.FileName?.Trim('"'));
        }

        [Fact(Skip = "Need assistance to become authorised")]
        public async Task DownloadMATClosureReportAsync_ReturnsNotFound_WhenReportDoesNotExist()
        {
            // Arrange
            HttpResponseMessage responseMessage = new(HttpStatusCode.NotFound);

            Mock<IDownloadsService> downloadsServiceMock = new();
            downloadsServiceMock
                .Setup(s => s.DownloadMATClosureReport(It.IsAny<IPrincipal>()))
                .ReturnsAsync(responseMessage);

            using WebApplicationFactory<Program> factory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();
                        services.AddSingleton(downloadsServiceMock.Object);
                    });
                });

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/MATClosureReport?filename=closure-report.csv");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
