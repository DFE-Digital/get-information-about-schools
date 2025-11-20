using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Edubase.Web.IntegrationTests.Helpers;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;

namespace Edubase.Web.IntegrationTests.Tests.SearchController;

[Collection(IntegrationTestsCollectionMarker.Name)]
public sealed class SearchControllerIndexTests
{
    private readonly EdubaseApiServerFixture _edubaseApiFixture;
    private readonly GiasWebApplicationFactory _webApplicationFactory;

    public SearchControllerIndexTests(EdubaseApiServerFixture fixture, GiasWebApplicationFactory webApplicationFactory)
    {
        _edubaseApiFixture = fixture;
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task Test()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        HttpMappedResponses response = await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected").Text().Trim());
    }
}
