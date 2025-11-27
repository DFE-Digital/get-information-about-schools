using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Edubase.Services.Domain;
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

    /*
     * Tackle when we do the form posting
     * 
     *         var nameOrRefNumRadio = document.QuerySelector("#searchtype-name");
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
     */

    // &LocalAuthorityToRemove=72 - JsDisabled remove button after finding a local authorityId on the pills

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
    }

    [Fact]
    public async Task Search_FindAnEstablishmentPage_Redirects_Back_With_EmptyEstablishments()
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
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?LocalAuthorityToRemove=100");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Found, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);

        httpResponse.Headers.TryGetValues("Location", out IEnumerable<string>? locations);
        string redirectPath = Assert.Single(locations);
        // TODO why does it append an empty query string?
        Assert.Equal("/?#la", redirectPath);
    }


    // FAILING MODEL BINDING: https://github.com/DFE-Digital/get-information-about-schools/pull/787#issuecomment-3584901280
    [Fact]
    public async Task Search_FindAnEstablishmentPage_Redirects_Back_With_RemainingSelectedEstablishments()
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
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?LocalAuthorityToRemove=1&d=1&d=2");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Found, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);

        httpResponse.Headers.TryGetValues("Location", out IEnumerable<string>? locations);
        string redirectPath = Assert.Single(locations);
        Assert.Equal("/?d=2#la", redirectPath);
    }

    // TODO EXTEND TO DEDUPLICATE-ADD e.g d=1 and add 1 wait for ModelBinding fix on d=
    [Theory]
    [InlineData("Bristol")]
    [InlineData("bristol")]
    public async Task Search_FindAnEstablishmentPage_LocalAuthorityDisambiguation_Finds_Matching_Identifier(string searchKeyword)
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-local-authorities-bristol.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        HttpMappedResponses response = await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=LocalAuthorityDisambiguation&LocalAuthorityToAdd={searchKeyword}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(302, (int) httpResponse.StatusCode);
        httpResponse.Headers.TryGetValues("Location", out IEnumerable<string>? locations);
        string redirectPath = Assert.Single(locations);
        Assert.Equal("?SearchType=ByLocalAuthority&d=1#la", redirectPath);
    }

    [Theory]
    [InlineData("bris")]
    [InlineData("BrI")]
    public async Task Search_FindAnEstablishmentPage_LocalAuthorityDisambiguation_No_Match_Found_Disambiguate(string keyWord)
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-local-authorities-bristol-brisbane-barnsley.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        HttpMappedResponses response = await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=LocalAuthorityDisambiguation&LocalAuthorityToAdd={keyWord}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(200, (int) httpResponse.StatusCode);

        List<LookupDto> stubbedLocalAuthorities =
            response.GetResponseById("1")
            .GetResponseBody<List<LookupDto>>()
            .Where(t => t.Name.Contains(keyWord, StringComparison.OrdinalIgnoreCase))
            .ToList();

        List<IElement> links = document.QuerySelectorAll("#search-localauthority-disambiguation-list a").ToList();

        Assert.Equal(stubbedLocalAuthorities.Count, links.Count);

        for (int index = 0; index < links.Count; index++)
        {
            IElement currentLink = links[index];
            LookupDto currentStubbedLocalAuthority = stubbedLocalAuthorities[index];

            string expectedLink = $"/Search/search?SearchType=ByLocalAuthority&OpenOnly=False&d={currentStubbedLocalAuthority.Id}#la";
            Assert.Equal(expectedLink, currentLink.GetAttribute("href"));
            Assert.Equal(currentStubbedLocalAuthority.Name, links[index].TextContent.Trim());
        }
    }
    /*
     *     [InlineData("abc")]
    [InlineData("briz")]
    [InlineData("Briz")]
     */


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
