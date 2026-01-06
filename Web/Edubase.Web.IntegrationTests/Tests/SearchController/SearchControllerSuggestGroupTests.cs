using System.Security.Principal;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Search;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.SearchController
{
    public sealed class SearchControllerSuggestGroupTests
    {
        [Fact]
        public async Task Search_SuggestGroup_ReturnsExpectedJson()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupReadService>();
            var expectedSuggestions = new[]
            {
                new GroupSuggestionItem { GroupUId = 1, Name = "Group A", GroupType = "Type1" },
                new GroupSuggestionItem { GroupUId = 2, Name = "Group B", GroupType = "Type2" }
            };

            mockGroupService
                .Setup(s => s.SuggestAsync(It.IsAny<string>(), It.IsAny<IPrincipal>(), It.IsAny<int>()))
                .ReturnsAsync(expectedSuggestions);

            using WebApplicationFactory<Program> webAppFactory =
                new GiasWebApplicationFactory()
                    .WithWebHostBuilder(
                    (builder) =>
                        builder.ConfigureServices(
                            (services) =>
                            {
                                services.RemoveAll<IGroupReadService>();
                                services.AddSingleton(mockGroupService.Object);
                            }));

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/SuggestGroup?text=test");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Assert

            var suggestions = System.Text.Json.JsonSerializer.Deserialize<GroupSuggestionItem[]>(
                json,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Assert.NotNull(suggestions);
            Assert.Equal(2, suggestions.Length);
            Assert.Equal("Group A", suggestions[0].Name);
            Assert.Equal(1, suggestions[0].GroupUId);
            Assert.Equal("Group B", suggestions[1].Name);
            Assert.Equal(2, suggestions[1].GroupUId);
        }


        [Fact]
        public async Task Search_SuggestGroup_EmptyText_ReturnsEmptyArray()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupReadService>();
            mockGroupService
                .Setup(s => s.SuggestAsync(It.IsAny<string>(), It.IsAny<IPrincipal>(), It.IsAny<int>()))
                .ReturnsAsync(Array.Empty<GroupSuggestionItem>());

            using var webAppFactory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IGroupReadService>();
                        services.AddSingleton(mockGroupService.Object);
                    });
                });

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/SuggestGroup?text=");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var suggestions = System.Text.Json.JsonSerializer.Deserialize<GroupSuggestionItem[]>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.NotNull(suggestions);
            Assert.Empty(suggestions);
        }


        [Fact]
        public async Task Search_SuggestGroup_NoResults_ReturnsEmptyArray()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupReadService>();
            mockGroupService
                .Setup(s => s.SuggestAsync("nomatch", It.IsAny<IPrincipal>(), It.IsAny<int>()))
                .ReturnsAsync(Array.Empty<GroupSuggestionItem>());

            using var webAppFactory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IGroupReadService>();
                        services.AddSingleton(mockGroupService.Object);
                    });
                });

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/SuggestGroup?text=nomatch");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var suggestions = System.Text.Json.JsonSerializer.Deserialize<GroupSuggestionItem[]>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.NotNull(suggestions);
            Assert.Empty(suggestions);
        }
    }
}
