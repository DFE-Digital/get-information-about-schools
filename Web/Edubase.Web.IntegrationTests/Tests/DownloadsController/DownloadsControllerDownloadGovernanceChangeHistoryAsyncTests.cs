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
    public sealed class DownloadsControllerDownloadGovernanceChangeHistoryAsyncTests
    {
        [Fact]
        public async Task DownloadGovernanceChangeHistoryAsync_RedirectsToExpectedUrl()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownloadsAndExtracts(
                includeGovernanceMocks: true);

            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads/Download/Establishment/123/1"); // id=123, downloadType=1

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/mock-governance-history-url", response.Headers.Location.ToString());
        }

        public static WebApplicationFactory<Program> CreateFactoryWithMockedDownloadsAndExtracts(
            bool includeScheduledExtracts = true,
            int scheduledExtractCount = 1,
            Action<Mock<IDownloadsService>> configureDownloadsMock = null,
            bool includeGovernanceMocks = false)
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

                        // Add governance mocks if requested
                        if (includeGovernanceMocks)
                        {
                            var establishmentReadServiceMock = new Mock<IEstablishmentReadService>();
                            establishmentReadServiceMock
                                .Setup(s => s.GetGovernanceChangeHistoryDownloadAsync(It.IsAny<int>(), It.IsAny<DownloadType>(), It.IsAny<IPrincipal>()))
                                .ReturnsAsync(new FileDownloadDto { Url = "/mock-governance-history-url" });

                            services.RemoveAll<IEstablishmentReadService>();
                            services.AddSingleton(establishmentReadServiceMock.Object);

                            // Add other related mocks if needed
                            services.RemoveAll<IGroupDownloadService>();
                            services.AddSingleton(new Mock<IGroupDownloadService>().Object);
                        }
                    });
                });

            return factory;
        }

    }

}
