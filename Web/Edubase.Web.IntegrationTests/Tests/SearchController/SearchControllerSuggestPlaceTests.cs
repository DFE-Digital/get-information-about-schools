using Edubase.Common.Spatial;
using Edubase.Services.Geo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.SearchController
{
    public sealed class SearchControllerSuggestPlaceTests
    {

        [Fact]
        public async Task Search_SuggestPlace_ValidQuery_ReturnsExpectedJson()
        {
            // Arrange
            var mockPlacesService = new Mock<IPlacesLookupService>();
            var expectedPlaces = new[]
            {
                new PlaceDto { Name = "London", Coords = new LatLon(51.5074, -0.1278) },
                new PlaceDto { Name = "Manchester", Coords = new LatLon(53.4808, -2.2426) }
            };

            mockPlacesService
                .Setup(s => s.SearchAsync(It.IsAny<string>(), true))
                .ReturnsAsync(expectedPlaces);

            using var webAppFactory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IPlacesLookupService>();
                        services.AddSingleton(mockPlacesService.Object);
                    });
                });

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/SuggestPlace?text=London");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var places = Newtonsoft.Json.JsonConvert.DeserializeObject<PlaceDto[]>(json);

            // Assert
            Assert.NotNull(places);
            Assert.Equal(2, places.Length);
            Assert.Equal("London", places[0].Name);
            Assert.Equal(51.5074, places[0].Coords.Latitude);
            Assert.Equal(-0.1278, places[0].Coords.Longitude);
            Assert.Equal("Manchester", places[1].Name);
            Assert.Equal(53.4808, places[1].Coords.Latitude);
            Assert.Equal(-2.2426, places[1].Coords.Longitude);
        }

        [Fact]
        public async Task Search_SuggestPlace_InvalidQuery_ReturnsBadRequest()
        {
            // Arrange
            using var webAppFactory = new GiasWebApplicationFactory();
            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/SuggestPlace?text="); // Empty text is invalid

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_SuggestPlace_NoResults_ReturnsEmptyArray()
        {
            // Arrange
            var mockPlacesService = new Mock<IPlacesLookupService>();
            mockPlacesService
                .Setup(s => s.SearchAsync(It.IsAny<string>(), true))
                .ReturnsAsync(Array.Empty<PlaceDto>());

            using var webAppFactory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IPlacesLookupService>();
                        services.AddSingleton(mockPlacesService.Object);
                    });
                });

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/SuggestPlace?text=UnknownPlace");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var places = Newtonsoft.Json.JsonConvert.DeserializeObject<PlaceDto[]>(json);

            // Assert
            Assert.NotNull(places);
            Assert.Empty(places);
        }
    }
}
