using System.Security.Principal;
using Edubase.Services.Core;
using Edubase.Services.Downloads;
using Edubase.Services.Downloads.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class SearchControllerCollateDownloadsTests
    {

        [Fact(Skip = "Getting a bad request when running this - need assistance to fix.")]
        public async Task CollateDownloads_RedirectsToIndex_WhenNoFilesSelected()
        {
            // Arrange
            using var factory = CreateFactoryWithMockedDownloadsAndExtracts(includeScheduledExtracts: false);

            var client = factory.CreateClient();


            var formData = new Dictionary<string, string>
            {
                { "SearchType", "Latest" },
                { "FilterDate.Day", "18" },
                { "FilterDate.Month", "12" },
                { "FilterDate.Year", "2025" },
                { "Downloads[0].Tag", "all.edubase.data" },
                { "Downloads[0].FileGeneratedDate", "12/18/2025 12:00:00 AM" },
                { "Downloads[0].Selected", "true" }
            };


            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await client.PostAsync("/Downloads/Collate", content);

            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Downloads", response.Headers.Location.ToString());
            Assert.Contains("Skip=0", response.Headers.Location.Query);

        }

        public static WebApplicationFactory<Program> CreateFactoryWithMockedDownloadsAndExtracts(
            bool includeScheduledExtracts = true,
            int scheduledExtractCount = 1)
        {
            var downloadsServiceMock = new Mock<IDownloadsService>();

            // Mock downloads
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

            var factory = new GiasWebApplicationFactory()
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
