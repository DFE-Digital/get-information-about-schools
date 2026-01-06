using System.Net;
using System.Security.Principal;
using AngleSharp.Html.Dom;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Downloads;
using Edubase.Services.Downloads.Models;
using Edubase.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json;

namespace Edubase.Web.IntegrationTests.Tests.DownloadsController
{
    public sealed class DownloadsControllerCollateDownloadsTests
    {
        [Fact]
        public async Task CollateDownloads_RedirectsToIndex_WhenNoFilesSelected()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownloadsAndExtracts(includeScheduledExtracts: false);
            HttpClient client = factory.CreateClient();

            // Get anti-forgery token
            HttpResponseMessage getResponse = await client.GetAsync("/Downloads");
            IHtmlDocument document= await getResponse.GetDocumentAsync();
            string token = document.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            var formData = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", token },
                { "Skip", "0" },
                { "SearchType", "Latest" },
                { "FilterDate.Day", "18" },
                { "FilterDate.Month", "12" },
                { "FilterDate.Year", "2025" },
                { "Downloads[0].Tag", "all.edubase.data" },
                { "Downloads[0].FileGeneratedDate", "12/18/2025 12:00:00 AM" },
                { "Downloads[0].Selected", "false" }
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(formData);

            // Act
            HttpResponseMessage response = await client.PostAsync("/Downloads/Collate", content);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Downloads", response.Headers.Location.ToString());
            Assert.Contains("Skip=0", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task CollateDownloads_RedirectsToDownloadGenerated_WhenFilesSelected_AndApiReturnsProgressDto()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownloadsAndExtracts(
                configureMock: mock =>
                {
                    mock.Setup(s => s.CollateDownloadsAsync(It.IsAny<List<FileDownloadRequest>>(), It.IsAny<IPrincipal>()))
                        .ReturnsAsync(JsonConvert.SerializeObject(new ProgressDto { FileLocationUri = "/mock-uri/123" }));
                });

            HttpClient client = factory.CreateClient();

            // Get anti-forgery token
            HttpResponseMessage getResponse = await client.GetAsync("/Downloads");
            IHtmlDocument document= await getResponse.GetDocumentAsync();
            string token = document.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            var formData = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", token },
                { "Skip", "0" },
                { "SearchType", "Latest" },
                { "FilterDate.Day", "18" },
                { "FilterDate.Month", "12" },
                { "FilterDate.Year", "2025" },
                { "Downloads[0].Tag", "all.edubase.data" },
                { "Downloads[0].FileGeneratedDate", "12/18/2025 12:00:00 AM" },
                { "Downloads[0].Selected", "true" }
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(formData);

            // Act
            HttpResponseMessage response = await client.PostAsync("/Downloads/Collate", content);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            string redirectUrl = response.Headers.Location.ToString();
            Assert.Contains("/Downloads/Generate", redirectUrl);
            Assert.Matches(@"[0-9a-fA-F\-]{36}$", redirectUrl);
        }

        [Fact]
        public async Task CollateDownloads_RedirectsToDownloadGenerated_WhenFilesSelected_AndApiReturnsApiResultDto()
        {
            // Arrange
            using WebApplicationFactory<Program> factory = CreateFactoryWithMockedDownloadsAndExtracts(
                configureMock: mock =>
                {
                    mock.Setup(s => s.CollateDownloadsAsync(It.IsAny<List<FileDownloadRequest>>(), It.IsAny<IPrincipal>()))
                        .ReturnsAsync(JsonConvert.SerializeObject(new ApiResultDto<Guid> { Value = Guid.NewGuid() }));
                });

            HttpClient client = factory.CreateClient();

            // Get anti-forgery token
            HttpResponseMessage getResponse = await client.GetAsync("/Downloads");
            IHtmlDocument document= await getResponse.GetDocumentAsync();
            string token = document.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            var formData = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", token },
                { "Skip", "0" },
                { "SearchType", "Latest" },
                { "FilterDate.Day", "18" },
                { "FilterDate.Month", "12" },
                { "FilterDate.Year", "2025" },
                { "Downloads[0].Tag", "all.edubase.data" },
                { "Downloads[0].FileGeneratedDate", "12/18/2025 12:00:00 AM" },
                { "Downloads[0].Selected", "true" }
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(formData);

            // Act
            HttpResponseMessage response = await client.PostAsync("/Downloads/Collate", content);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            string redirectUrl = response.Headers.Location.ToString();
            Assert.Contains("/Downloads/Generate", redirectUrl);
            Assert.Matches(@"[0-9a-fA-F\-]{36}$", redirectUrl);
        }

        public static WebApplicationFactory<Program> CreateFactoryWithMockedDownloadsAndExtracts(
            bool includeScheduledExtracts = true,
            int scheduledExtractCount = 1,
            Action<Mock<IDownloadsService>> configureMock = null)
        {
            Mock<IDownloadsService> downloadsServiceMock = new();

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

            // Allow custom configuration for CollateDownloadsAsync or other methods
            configureMock?.Invoke(downloadsServiceMock);

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
