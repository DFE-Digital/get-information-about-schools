using System.Security.Principal;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Search;
using Moq;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;

namespace Edubase.Web.IntegrationTests.Tests.SearchController
{
    public sealed class SearchControllerSuggestTests
    {

        [Fact]
        public async Task Suggest_RemovesCityAndPostcode_ForUniqueNames()
        {
            // Arrange
            var mockService = new Mock<IEstablishmentReadService>();
            var suggestions = new[]
            {
                new EstablishmentSuggestionItem { Name = "School A", Address_CityOrTown = "London", Address_PostCode = "SW1A" },
                new EstablishmentSuggestionItem { Name = "School B", Address_CityOrTown = "Manchester", Address_PostCode = "M1" },
                new EstablishmentSuggestionItem { Name = "School A", Address_CityOrTown = "London", Address_PostCode = "SW1A" } // Duplicate
            };

            mockService.Setup(s => s.SuggestAsync(It.IsAny<string>(), It.IsAny<IPrincipal>(), It.IsAny<int>()))
                       .ReturnsAsync(suggestions);

            var controller = new Edubase.Web.UI.Controllers.SearchController(mockService.Object, null, null, null);

            // Act
            var result = await controller.Suggest("School");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedSuggestions = Assert.IsType<IEnumerable<EstablishmentSuggestionItem>>(jsonResult.Value, exactMatch: false);

            var uniqueItem = returnedSuggestions.First(x => x.Name == "School B");
            Assert.Equal(string.Empty, uniqueItem.Address_CityOrTown);
            Assert.Equal(string.Empty, uniqueItem.Address_PostCode);

            var duplicateItems = returnedSuggestions.Where(x => x.Name == "School A").ToList();
            Assert.All(duplicateItems, item =>
            {
                Assert.NotEqual(string.Empty, item.Address_CityOrTown);
                Assert.NotEqual(string.Empty, item.Address_PostCode);
            });
        }

        [Fact]
        public async Task Suggest_CallsServiceWithCorrectArguments()
        {
            // Arrange
            var mockService = new Mock<IEstablishmentReadService>();
            mockService.Setup(s => s.SuggestAsync(It.IsAny<string>(), It.IsAny<IPrincipal>(), It.IsAny<int>()))
                       .ReturnsAsync(Array.Empty<EstablishmentSuggestionItem>());

            var controller = new Edubase.Web.UI.Controllers.SearchController(mockService.Object, null, null, null);

            // Act
            await controller.Suggest("School");

            // Assert
            mockService.Verify(s => s.SuggestAsync("School", It.IsAny<IPrincipal>(), 10), Times.Once);
        }

        [Fact]
        public async Task Suggest_ReturnsEmptyJson_WhenNoSuggestions()
        {
            // Arrange
            var mockService = new Mock<IEstablishmentReadService>();
            mockService.Setup(s => s.SuggestAsync(It.IsAny<string>(), It.IsAny<IPrincipal>(), It.IsAny<int>()))
                       .ReturnsAsync(Array.Empty<EstablishmentSuggestionItem>());

            var controller = new Edubase.Web.UI.Controllers.SearchController(mockService.Object, null, null, null);

            // Act
            var result = await controller.Suggest("School");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedSuggestions = Assert.IsAssignableFrom<IEnumerable<EstablishmentSuggestionItem>>(jsonResult.Value);
            Assert.Empty(returnedSuggestions);
        }

        [Fact]
        public async Task Suggest_AllowsNullOrEmptyText()
        {
            // Arrange
            var mockService = new Mock<IEstablishmentReadService>();
            var suggestions = new[]
            {
                new EstablishmentSuggestionItem { Name = "School A", Address_CityOrTown = "London", Address_PostCode = "SW1A" }
            };

            mockService.Setup(s => s.SuggestAsync(It.IsAny<string>(), It.IsAny<IPrincipal>(), It.IsAny<int>()))
                       .ReturnsAsync(suggestions);

            var controller = new Edubase.Web.UI.Controllers.SearchController(mockService.Object, null, null, null);

            // Act
            var result = await controller.Suggest(string.Empty);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedSuggestions = Assert.IsAssignableFrom<IEnumerable<EstablishmentSuggestionItem>>(jsonResult.Value);
            Assert.Single(returnedSuggestions);
            Assert.Equal("School A", returnedSuggestions.First().Name);
        }
    }
}
