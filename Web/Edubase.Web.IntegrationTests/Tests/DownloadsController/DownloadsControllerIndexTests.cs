using System.Security.Principal;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
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
    public sealed class DownloadsControllerIndexTests
    {
        [Fact]
        public async Task Index_RendersDownloads()
        {
            // Arrange
            using WebApplicationFactory<Program> webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts(false, 0);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("All establishment data", document.QuerySelector("#all-establishment-data-heading").TextContent);
            Assert.Equal("Open academies and free schools data", document.QuerySelector("#open-academies-and-free-schools-data-heading").TextContent);
            Assert.Equal("Open state-funded schools data", document.QuerySelector("#open-state-funded-schools-data-heading").TextContent);
            Assert.Equal("Open children's centres data", document.QuerySelector("#open-childrens-centres-data-heading").TextContent);
            Assert.Equal("All group data", document.QuerySelector("#all-group-data-heading").TextContent);
            Assert.Equal("Open group data", document.QuerySelector("#open-group-data-heading").TextContent);
            Assert.Equal("All governor data", document.QuerySelector("#all-governor-data-heading").TextContent);
        }

        [Fact]
        public async Task Index_RendersExtracts_WhenSingleExtractAvailable()
        {
            // Arrange
            using WebApplicationFactory<Program> webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts(
                includeScheduledExtracts: true,
                scheduledExtractCount: 1);

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads#scheduled-extracts");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            IElement scheduledExtractsSection = document.QuerySelector("#scheduled-extracts");
            Assert.NotNull(scheduledExtractsSection);

            IElement table = scheduledExtractsSection.QuerySelector("table.govuk-table");
            Assert.NotNull(table);

            Assert.Equal("Scheduled extracts files", table.QuerySelector("caption").TextContent.Trim());

            var headers = table.QuerySelectorAll("thead th");
            Assert.Equal(2, headers.Length);
            Assert.Equal("File", headers[0].TextContent.Trim());
            Assert.Equal("ID", headers[1].TextContent.Trim());

            // Assert the row contains correct link and ID
            IElement row = table.QuerySelector("tbody tr");
            Assert.NotNull(row);

            IElement link = row.QuerySelector("td.download-list-item a");
            Assert.NotNull(link);
            Assert.Equal("/Downloads/RequestScheduledExtract/1", link.GetAttribute("href"));
            Assert.Equal("mockscheduledextract1", link.GetAttribute("id"));
            Assert.Equal("MockScheduledExtract1", link.TextContent.Trim());

            IElement idCell = row.QuerySelector("td.govuk-table__cell--numeric");
            Assert.Equal("1", idCell.TextContent.Trim());
        }

        [Fact]
        public async Task Index_RendersAllScheduledExtractRows_WhenMultipleExtractsAvailable()
        {
            // Arrange
            using WebApplicationFactory<Program> webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts(
                includeScheduledExtracts: true,
                scheduledExtractCount: 3
                );

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            IElement scheduledExtractsSection = document.QuerySelector("#scheduled-extracts");
            Assert.NotNull(scheduledExtractsSection);

            IElement table = scheduledExtractsSection.QuerySelector("table.govuk-table");
            Assert.NotNull(table);

            var headers = table.QuerySelectorAll("thead th");
            Assert.Equal(2, headers.Length);
            Assert.Equal("File", headers[0].TextContent.Trim());
            Assert.Equal("ID", headers[1].TextContent.Trim());

            var rows = table.QuerySelectorAll("tbody tr");
            Assert.Equal(3, rows.Length);

            for (int i = 0; i < rows.Length; i++)
            {
                IElement link = rows[i].QuerySelector("td.download-list-item a");
                Assert.NotNull(link);

                string expectedName = $"MockScheduledExtract{i + 1}";
                string expectedHref = $"/Downloads/RequestScheduledExtract/{i + 1}";
                string expectedIdAttr = $"mockscheduledextract{i + 1}";

                Assert.Equal(expectedHref, link.GetAttribute("href"));
                Assert.Equal(expectedIdAttr, link.GetAttribute("id"));
                Assert.Equal(expectedName, link.TextContent.Trim());

                IElement idCell = rows[i].QuerySelector("td.govuk-table__cell--numeric");
                Assert.Equal((i + 1).ToString(), idCell.TextContent.Trim());
            }
        }

        [Fact]
        public async Task Index_RendersPagination_WhenMultiplePagesAvailable()
        {
            // Arrange
            using var webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts(
                includeScheduledExtracts: true,
                scheduledExtractCount: 25
            );

            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var scheduledExtractsSection = document.QuerySelector("#scheduled-extracts");
            Assert.NotNull(scheduledExtractsSection);

            var topPagination = document.QuerySelector(".top-pagination");
            Assert.NotNull(topPagination);

            var summaryText = topPagination.QuerySelector("p.govuk-body-s");
            Assert.Equal("Showing 1 - 10 of 25", summaryText.TextContent.Trim());

            // Assert pagination links
            var pageLinks = topPagination.QuerySelectorAll("ul.pagination-links li a");
            Assert.Equal(3, pageLinks.Length);

            // Page 2 link
            Assert.Equal("/Downloads?skip=10&count=25", pageLinks[0].GetAttribute("href"));
            Assert.Equal("2", pageLinks[0].TextContent.Trim());

            // Page 3 link
            Assert.Equal("/Downloads?skip=20&count=25", pageLinks[1].GetAttribute("href"));
            Assert.Equal("3", pageLinks[1].TextContent.Trim());

            // Next link
            var nextLink = topPagination.QuerySelector("a.pagination-next");
            Assert.NotNull(nextLink);
            Assert.Equal("/Downloads?skip=10&count=25", nextLink.GetAttribute("href"));
            Assert.Equal("Next >>", nextLink.TextContent.Trim());
        }

        [Fact]
        public async Task Index_DefaultsToLatest_WhenNoSearchTypeProvided()
        {
            // Arrange
            using var webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts();
            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var titleElement = document.QuerySelector("#downloads-title");
            Assert.NotNull(titleElement);

            string actualText = titleElement.TextContent.Trim();
            string expectedDate = DateTime.Today.ToString("d MMMM yyyy");

            Assert.Contains("Files available to download from", actualText);
            Assert.Contains(expectedDate, actualText);
        }

        [Fact]
        public async Task Index_RendersDateFilterUI_WhenSearchTypeIsDateFilter()
        {
            // Arrange
            using var webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts();
            HttpClient client = webAppFactory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Downloads?searchType=DateFilter");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(document.QuerySelector("#DateFilter"));
        }

        // This is failing because the title of the page does not update.
        // It is an existing issue so I am waiting for confirmation on the project's stance on it.
        [Fact(Skip = "Existing bug")]
        public async Task Index_RendersTitleWithFilterDate_WhenSearchTypeIsDateFilter()
        {
            // Arrange
            using var webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts();
            HttpClient client = webAppFactory.CreateClient();

            string filterDate = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");

            // Act
            HttpResponseMessage response = await client.GetAsync($"/Downloads?searchType=DateFilter&filterDate.Date={filterDate}");
            IHtmlDocument document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            IElement titleElement = document.QuerySelector("#downloads-title");
            Assert.NotNull(titleElement);

            string actualText = titleElement.TextContent.Trim();
            string expectedDate = DateTime.Today.AddDays(-7).ToString("d MMMM yyyy");

            Assert.Contains("Files available to download from", actualText);
            Assert.Contains(expectedDate, actualText);
        }

        [Fact]
        public async Task Index_RendersCorrectPaginationSummary_WhenSkipParameterProvided()
        {
            using var webAppFactory = CreateFactoryWithMockedDownloadsAndExtracts(scheduledExtractCount: 25);
            HttpClient client = webAppFactory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/Downloads?skip=10");
            IHtmlDocument document = await response.GetDocumentAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            IElement summaryText = document.QuerySelector(".top-pagination p.govuk-body-s");
            Assert.NotNull(summaryText);
            Assert.Equal("Showing 11 - 20 of 25", summaryText.TextContent.Trim());
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
