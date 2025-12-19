using System.Security.Principal;
using Edubase.Services.Core;
using Edubase.Services.Downloads;
using Edubase.Services.Downloads.Models;
using Edubase.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerResultsPartialTests
    {

        [Fact]
        public async Task ResultsPartial_RendersExpectedHeadingsAndLinks()
        {
            // Arrange
            using var factory = CreateFactoryWithMockedDownloadsAndExtracts(includeScheduledExtracts: true, scheduledExtractCount: 3);
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Downloads/results-js");
            var document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);


            Assert.Contains("Select the files you want to download", document.Body.TextContent);

            Assert.Contains("Establishment downloads", document.QuerySelector("h2").TextContent);

            var headings = document.QuerySelectorAll("h3");
            var headingTexts = headings.Select(h => h.TextContent.Trim()).ToList();

            Assert.Contains("All establishment data", headingTexts);
            Assert.Contains("Open academies and free schools data", headingTexts);
            Assert.Contains("Open state-funded schools data", headingTexts);
            Assert.Contains("Open children's centres data", headingTexts);
            Assert.Contains("All group data", headingTexts);
            Assert.Contains("Open group data", headingTexts);
            Assert.Contains("All governor data", headingTexts);

            var checkboxes = document.QuerySelectorAll("input[type='checkbox']");
            Assert.True(checkboxes.Length > 0);

            var selectAllLink = document.QuerySelector("#select-all");
            var clearAllLink = document.QuerySelector("#clear-all");
            Assert.NotNull(selectAllLink);
            Assert.NotNull(clearAllLink);
            Assert.Equal("Select all", selectAllLink.TextContent.Trim());
            Assert.Equal("Clear all", clearAllLink.TextContent.Trim());

            var submitButton = document.QuerySelector("#download-selected-files-button");
            Assert.NotNull(submitButton);
            Assert.Equal("Download selected files", submitButton.GetAttribute("value"));
        }


        [Fact]
        public async Task ResultsPartial_RendersNoDownloadsMessage_WhenNoDownloadsExist()
        {
            // Arrange
            var factory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IDownloadsService>();

                        var mockService = new Mock<IDownloadsService>();

                        mockService.Setup(s => s.GetListAsync(It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                                   .ReturnsAsync(Array.Empty<FileDownload>());

                        mockService.Setup(s => s.GetScheduledExtractsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IPrincipal>()))
                                   .ReturnsAsync(new PaginatedResult<ScheduledExtract>(0, 10, 0, new List<ScheduledExtract>()));

                        services.AddSingleton(mockService.Object);
                    });
                });

            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Downloads/results-js");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var document = await response.GetDocumentAsync();

            var heading = document.QuerySelector("#no-downloads-available");
            Assert.NotNull(heading);
            Assert.Contains("No files available to download", heading.TextContent);

            Assert.Contains("Please choose a different date to find the files you need", document.Body.TextContent);
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
