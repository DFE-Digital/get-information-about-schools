using System.Net;
using System.Security.Principal;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Downloads;
using Edubase.Services.Downloads.Models;
using Edubase.Services.Establishments;
using Edubase.Services.Groups.Downloads;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerDownloadChangeHistoryTests
    {

        [Fact]
        public async Task DownloadChangeHistory_RedirectsToGroupHistory_WhenGroupIdProvided()
        {
            // Arrange
            Mock<IGroupDownloadService> groupDownloadServiceMock = new();
            groupDownloadServiceMock
                .Setup(s => s.DownloadGroupHistory(It.IsAny<int>(), It.IsAny<DownloadType>(), null, null, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new DownloadDto { Url = "/mock-group-history-url" });

            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownloadsAndExtracts(
                configureAdditionalServices: services =>
                {
                    services.AddSingleton(groupDownloadServiceMock.Object);
                });

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/ChangeHistory/1?groupId=123");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/mock-group-history-url", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task DownloadChangeHistory_RedirectsToEstablishmentHistory_WhenEstablishmentUrnProvided()
        {
            // Arrange
            Mock<IEstablishmentReadService> establishmentReadServiceMock = new();
            establishmentReadServiceMock
                .Setup(s => s.GetChangeHistoryDownloadAsync(It.IsAny<int>(), It.IsAny<DownloadType>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new FileDownloadDto { Url = "/mock-establishment-history-url" });

            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownloadsAndExtracts(
                configureAdditionalServices: services =>
                {
                    services.AddSingleton(establishmentReadServiceMock.Object);
                });

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/ChangeHistory/1?establishmentUrn=456");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/mock-establishment-history-url", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task DownloadChangeHistory_ReturnsNull_WhenNoParametersProvided()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownloadsAndExtracts();

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/ChangeHistory/1");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        public static WebApplicationFactory<Program> CreateFactoryWithMockedDownloadsAndExtracts(
            bool includeScheduledExtracts = true,
            int scheduledExtractCount = 1,
            Action<Mock<IDownloadsService>> configureDownloadsMock = null,
            Action<IServiceCollection> configureAdditionalServices = null)
        {
            var downloadsServiceMock = new Mock<IDownloadsService>();

            // Default mock for GetListAsync
            downloadsServiceMock
                .Setup(s => s.GetListAsync(It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new[]
                {
                    new FileDownload { Tag = "all.edubase.data", Name = "Mock", Url = "/mock-url" },
                    new FileDownload { Tag = "all.open.academies.and.free.schools", Name = "Mock", Url = "/mock-url" },
                    new FileDownload { Tag = "all.open.state-funded.schools", Name = "Mock", Url = "/mock-url" },
                    new FileDownload { Tag = "all.open.childrens.centres", Name = "Mock", Url = "/mock-url" },
                    new FileDownload { Tag = "all.group.records", Name = "Mock", Url = "/mock-url" },
                    new FileDownload { Tag = "academy.sponsor.and.trust.links", Name = "Mock", Url = "/mock-url" },
                    new FileDownload { Tag = "all.governance.records", Name = "Mock", Url = "/mock-url" }
                });

            // Optional scheduled extracts
            if (includeScheduledExtracts)
            {
                downloadsServiceMock
                    .Setup(s => s.GetScheduledExtractsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IPrincipal>()))
                    .ReturnsAsync((int skip, int take, IPrincipal principal) =>
                    {
                        var extracts = Enumerable.Range(1, scheduledExtractCount)
                            .Select(i => new ScheduledExtract
                            {
                                Id = i,
                                Name = $"MockScheduledExtract{i}",
                                Description = "Mock Desc",
                                Date = DateTime.Today
                            })
                            .ToList();

                        return new PaginatedResult<ScheduledExtract>(skip, 10, extracts.Count, extracts);
                    });
            }

            // Allow custom configuration for DownloadsService
            configureDownloadsMock?.Invoke(downloadsServiceMock);

            var factory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();
                        services.AddSingleton(downloadsServiceMock.Object);
                        services.RemoveAll<IGroupDownloadService>();
                        services.RemoveAll<IEstablishmentReadService>();
                        services.AddSingleton(new Mock<IGroupDownloadService>().Object);
                        services.AddSingleton(new Mock<IEstablishmentReadService>().Object);

                        // Allow additional service configuration (e.g., other mocks)
                        configureAdditionalServices?.Invoke(services);
                    });
                });

            return factory;
        }
    }
}
