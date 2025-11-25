using System.Net;
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
    public async Task Search_FindAnEstablishmentPage_IsDisplayed()
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

        Assert.Equal("Get Information about Schools", document.QuerySelector("#proposition-name").Text().Trim());
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected").Text().Trim());
        Assert.Equal("/Search/search?SelectedTab=Establishments", document.QuerySelector(".gias-tabs__list-item--selected a").GetAttribute("href"));

        var nameOrRefNumRadio = document.QuerySelector("#searchtype-name");
        Assert.Equal("radio", nameOrRefNumRadio.GetAttribute("type"));
        Assert.Equal("#searchby-name-ref", nameOrRefNumRadio.GetAttribute("data-toggle-panel"));
        Assert.Equal("Text", nameOrRefNumRadio.GetAttribute("value"));

        var locationRadio = document.QuerySelector("#searchtype-location");
        Assert.Equal("radio", locationRadio.GetAttribute("type"));
        Assert.Equal("#searchby-location-ref", locationRadio.GetAttribute("data-toggle-panel"));
        Assert.Equal("Location", locationRadio.GetAttribute("value"));

        var laRadio = document.QuerySelector("#searchtype-la");
        Assert.Equal("radio", laRadio.GetAttribute("type"));
        Assert.Equal("#searchby-la-ref", laRadio.GetAttribute("data-toggle-panel"));
        Assert.Equal("ByLocalAuthority", laRadio.GetAttribute("value"));
    }

    [Theory]
    [InlineData("Text", "checked", null, null, null)]
    [InlineData("Location", null, "checked", null, null)]
    [InlineData("ByLocalAuthority", null, null, "checked", null)]
    [InlineData("EstablishmentAll", null, null, null, "checked")]
    public async Task Search_SearchType_Checkbox_IsChecked(
        string searchType,
        string textChecked,
        string locationChecked,
        string laChecked,
        string allChecked
        )
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
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType={searchType}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected").Text().Trim());
        Assert.Equal(textChecked, document.QuerySelector("#searchtype-name").GetAttribute("checked"));
        Assert.Equal(locationChecked, document.QuerySelector("#searchtype-location").GetAttribute("checked"));
        Assert.Equal(laChecked, document.QuerySelector("#searchtype-la").GetAttribute("checked"));
        Assert.Equal(allChecked, document.QuerySelector("#searchtype-all").GetAttribute("checked"));
    }

    /// <summary>
    /// TODO: Using the SearchType=LocalAuthorityDisambiguation on a deployed environment displays the LA matching page.
    /// For some reason using it here I am getting a 500 error.
    /// Error appears to be:
    /// <pre style="word-wrap: break-word; white-space: pre-wrap;">System.InvalidOperationException: The partial view '_LocalAuthorityLink' was not found.
    /// The following locations were searched:
    /// /Views/Search/_LocalAuthorityLink.cshtml
    /// /Views/Shared/_LocalAuthorityLink.cshtml
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Search_SearchType_LocalAuthorityDisambiguation_LocalAuthoritiesMatchingPage_IsDisplayed()
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
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=LocalAuthorityDisambiguation");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);
    }

    [Fact]
    public async Task Search_SearchType_LocationSuggest_LatLong()
    {
        // Arrange
        double latitude = 51.500152587890625;
        double longitude = -0.1262362003326416;

        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        HttpMappedResponses response = await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=Location&LocationSearchModel.AutoSuggestValue={latitude}%2c+{longitude}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected").Text().Trim());
        Assert.Equal("checked", document.QuerySelector("#searchtype-location").GetAttribute("checked"));
        Assert.Equal($"{latitude}, {longitude}", document.QuerySelector("#LocationSearchModel_AutoSuggestValue").GetAttribute("value"));
    }

    /// <summary>
    /// Using SelectedTab=Groups and SearchType=Group to display the Find an establishment group page.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Search_SearchType_Group()
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
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SelectedTab=Groups&SearchType=Group");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);
        Assert.Equal("Find an establishment group", document.QuerySelector(".gias-tabs__list-item--selected").Text().Trim());
        Assert.Equal("checked", document.QuerySelector("#searchtype-name-group").GetAttribute("checked"));
    }

    // TODO: as per eSearchType.cs - Governor, GovernorReference, GovernorAll, GroupAll

}
