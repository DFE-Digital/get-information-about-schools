using System.Net;
using System.Security.Principal;
using Edubase.Services.Downloads;
using Edubase.Services.Downloads.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerDownloadFileAsyncTests
    {
        [Fact]
        public async Task DownloadFileAsync_ReturnsFileStreamResult_WhenFileExists()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedFileDownload();

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/File?id=test-file");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.ToString());
            Assert.Equal("mock-file.txt", response.Content.Headers.ContentDisposition.FileName.Trim('"'));
        }

        [Fact]
        public async Task DownloadFileAsync_UsesProvidedFilterDate_WhenSpecified()
        {
            // Arrange

            DateTime expectedDate = new DateTime(2024, 12, 25); // Christmas test date

            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedFileDownload();

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads/Download/File?id=test-file&filterDate={expectedDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DownloadFileAsync_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            Mock<IDownloadsService> downloadsServiceMock = new();
            downloadsServiceMock
                .Setup(s => s.GetListAsync(It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(Array.Empty<FileDownload>());

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
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/File?id=non-existent");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public static WebApplicationFactory<Program> CreateFactoryWithMockedFileDownload()
        {
            FileDownload fileDownload = new FileDownload
            {
                Tag = "test-file",
                Name = "Mock",
                Url = "/mock-url",
                Description = "Test file for download",
                FileSizeInBytes = 1024,
                RequiresGeneration = false,
                AuthenticationRequired = false,
                FileGeneratedDate = DateTime.Now
            };

            Mock<IDownloadsService> downloadsServiceMock = new();
            downloadsServiceMock
                .Setup(s => s.GetListAsync(It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new[] { fileDownload });

            MemoryStream fileContent = new MemoryStream(new byte[] { 1, 2, 3 });

            HttpResponseMessage responseMessage = new(HttpStatusCode.OK)
            {
                Content = new StreamContent(fileContent)
            };
            responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            responseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "\"mock-file.txt\""
            };

            downloadsServiceMock
                .Setup(s => s.DownloadFile(fileDownload, It.IsAny<IPrincipal>()))
                .ReturnsAsync(responseMessage);

            WebApplicationFactory<Program> factory = new GiasWebApplicationFactory()
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
