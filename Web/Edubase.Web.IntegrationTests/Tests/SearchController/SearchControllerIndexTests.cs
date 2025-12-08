using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Edubase.Services.Domain;
using Edubase.Services.IntegrationEndPoints.AzureMaps.Models;
using Edubase.Services.Lookup;
using Edubase.Web.IntegrationTests.Helpers;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Result = Edubase.Services.IntegrationEndPoints.AzureMaps.Models.Result;

namespace Edubase.Web.IntegrationTests.Tests.SearchController;

public sealed class SearchControllerIndexTests
{
    private static LookupDto[] LocalAuthorities => [
        new()
        {
            Id = 1,
            Name = "Bristol",
            DisplayOrder = 0,
            Code = "NA"
        },
        new ()
        {
            Id = 2,
            Name = "City of London",
            DisplayOrder = 10,
            Code = "01"
        },
        new ()
        {
            Id = 3,
            Name = "Barnsley",
            DisplayOrder = 20,
            Code = "02"
        }
    ];

    private static LookupDto[] GovernorRoles => [
        new()
        {
            Id = 1,
            Name = "Mr Doe",
            DisplayOrder = 0,
            Code = "NA"
        },
        new()
        {
            Id = 2,
            Name = "Mr John Doe",
            DisplayOrder = 10,
            Code = "01"
        },
        new()
        {
            Id = 3,
            Name = "Mr Doe John",
            DisplayOrder = 20,
            Code = "02"
        }
    ];

    private static WebApplicationFactory<Program> CreateWebApplicationFactory()
    {
        Mock<ICachedLookupService> lookupServiceMock = new();
        lookupServiceMock
            .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
            .ReturnsAsync(LocalAuthorities);

        lookupServiceMock
            .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
            .ReturnsAsync(GovernorRoles);

        WebApplicationFactory<Program> webAppFactory =
            new GiasWebApplicationFactory()
                .WithWebHostBuilder(
                (builder) =>
                    builder.ConfigureServices(
                        (services) =>
                        {
                            services.RemoveAll<ICachedLookupService>();
                            services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);
                        }));

        return webAppFactory;
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

    // TODO test redirects return page and contents of the redirected page

    [Fact]
    public async Task Search_FindAnEstablishment_Tab_IsSelected()
    {
        // Arrange

        WebApplicationFactory<Program> webApplicationFactory = CreateWebApplicationFactory();

        // Act
        HttpClient client = webApplicationFactory.CreateClient();
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
    public async Task Search_FindAnEstablishment_Remove_A_LocalAuthority_Redirects_BackToSearch_With_EmptyEstablishments()
    {
        // Arrange
        WebApplicationFactory<Program> webApplicationFactory = CreateWebApplicationFactory();

        // Act
        HttpClient client = webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?LocalAuthorityToRemove=100");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Found, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);

        httpResponse.Headers.TryGetValues("Location", out IEnumerable<string>? locations);
        var redirectPath = Assert.Single(locations);
        // TODO why does it append an empty query string?
        Assert.Equal("/?#la", redirectPath);
    }

    [Fact]
    public async Task Search_FindAnEstablishment_Remove_A_LocalAuthority_Redirects_BackToSearch_With_RemainingSelectedEstablishments()
    {
        // Arrange
        WebApplicationFactory<Program> webApplicationFactory = CreateWebApplicationFactory();
        // Act
        HttpClient client = webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?LocalAuthorityToRemove=1&d=1&d=2");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Found, httpResponse.StatusCode);
        Assert.NotNull(httpResponse);

        httpResponse.Headers.TryGetValues("Location", out IEnumerable<string>? locations);
        var redirectPath = Assert.Single(locations);
        Assert.Equal("/?d=2#la", redirectPath);
    }

    /*
    // TODO EXTEND TO DEDUPLICATE-ADD e.g d=1 and add 1 wait for ModelBinding fix on d=
    [Theory]
    [InlineData("Bristol")]
    [InlineData("bristol")]
    public async Task Search_FindAnEstablishmentPage_LocalAuthorityDisambiguation_Finds_Matching_Identifier(string searchKeyword)
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities-bristol.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);

        HttpMappedResponses response = await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=LocalAuthorityDisambiguation&LocalAuthorityToAdd={searchKeyword}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(302, (int) httpResponse.StatusCode);
        httpResponse.Headers.TryGetValues("Location", out IEnumerable<string>? locations);
        var redirectPath = Assert.Single(locations);
        Assert.Equal("?SearchType=ByLocalAuthority&d=1#la", redirectPath);
    }

    [Theory]
    [InlineData("bris")]
    [InlineData("BrI")]
    public async Task Search_FindAnEstablishmentPage_LocalAuthorityDisambiguation_No_ExactMatchFound_Returns_DisambiguateView(string keyWord)
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile(id: "1", "edubase/lookup/get-local-authorities-bristol-brisbane-barnsley.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);

        HttpMappedResponses response = await _edubaseApiFixture.RegisterHttpMapping(request);

        List<LookupDto> stubbedLocalAuthorities =
            response.GetResponseById("1")
                .GetResponseBody<List<LookupDto>>()
                .Where(t => t.Name.Contains(keyWord, StringComparison.OrdinalIgnoreCase))
                .ToList();

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=LocalAuthorityDisambiguation&LocalAuthorityToAdd={keyWord}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(200, (int) httpResponse.StatusCode);

        List<IElement> links = document.QuerySelectorAll("#search-localauthority-disambiguation-list a").ToList();

        Assert.Equal(stubbedLocalAuthorities.Count, links.Count);

        for (var index = 0; index < links.Count; index++)
        {
            IElement currentLink = links[index];
            LookupDto currentStubbedLocalAuthority = stubbedLocalAuthorities[index];

            Assert.Equal($"/Search/search?SearchType=ByLocalAuthority&OpenOnly=False&d={currentStubbedLocalAuthority.Id}#la", currentLink.GetAttribute("href"));
            Assert.Equal(currentStubbedLocalAuthority.Name, currentLink.TextContent.Trim());
        }
    }
    */
    /*
    // TODO: test
    *//*
     *     [InlineData("abc")]
    [InlineData("briz")]
    [InlineData("Briz")]
     *//*

    // TODO what could make the ModelState invalid?

    // TODO extend to links?
    [Fact]
    public async Task Search_LocationDisambiguation_Returns_View_When_MultipleMatches()
    {
        // Arrange
        const string searchLocationKeyword = "SEARCH_TEXT";

        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
            new HttpMappingFile("maps", "azuremaps/get-azuremaps-addresses.json"),
        ]);

        HttpMappedResponses response = await _edubaseApiFixture.RegisterHttpMapping(request);

        AzureMapsSearchResponseDto azureMapsAddressesResponse =
            response.GetResponseById("maps")
                .GetResponseBody<AzureMapsSearchResponseDto>();

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=Location&LocationSearchModel.Text={searchLocationKeyword}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal($"Search results for establishments", document.QuerySelector("h1")?.TextContent);

        Assert.Contains($"3 locations matching '{searchLocationKeyword}'", document.QuerySelector("h1 + p")?.TextContent);
        Assert.Equal(3, document.QuerySelectorAll(".govuk-list li").Count);

        List<IElement> disambiguateLinks =
            document.QuerySelectorAll("#search-location-matching-locations a").ToList();

        for (var index = 0; index < azureMapsAddressesResponse.results.Count(); index++)
        {
            IElement currentLink = disambiguateLinks[index];

            Result currentMapsResult = azureMapsAddressesResponse.results[index];

            Assert.Equal(
                $"?SearchType=Location&LocationSearchModel.Text={searchLocationKeyword}&LocationSearchModel.AutoSuggestValue={currentMapsResult.position.lat},{currentMapsResult.position.lon}",
                currentLink.GetAttribute("href"));

            Assert.Equal(
                $"{currentMapsResult.address.freeformAddress}, {currentMapsResult.address.countrySecondarySubdivision}",
                currentLink.TextContent.Trim());
        }

    }

    // TODO fix placesService needs to be stubbed.
    // TODO this seems like a bug, if there's no fuzzy match, shouldn't we display an error to the effect of "No suggestions available"
    [Fact]
    public async Task Search_LocationDisambiguation_NoMatches_Renders_Index_Without_ModelErrors()
    {
        // Arrange
        var unknownLocation = "UnknownPlace";

        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-empty-places.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=Location&LocationSearchModel.Text={unknownLocation}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        // Check the index is returned. Check no error is displayed at this point. Check the search term is preserved in the input field.
        Assert.Equal("Get Information about Schools", document.QuerySelector("#proposition-name")?.TextContent.Trim());
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Null(document.QuerySelector(".govuk-error-summary"));
        Assert.Equal(unknownLocation, document.QuerySelector("#LocationSearchModel_Text").GetAttribute("value"));
    }

    [Fact]
    public async Task Search_LocationSearch_With_Valid_AutoSuggestValue_Skips_Disambiguation()
    {
        // Arrange
        var autoSuggestValue = "51.5074,-0.1278"; // Valid LatLon coordinates

        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=Location&LocationSearchModel.AutoSuggestValue={autoSuggestValue}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        // Check the index is returned. Check no error is displayed at this point. Check the AutoSuggestValue is preserved.
        Assert.Equal("Get Information about Schools", document.QuerySelector("#proposition-name")?.TextContent.Trim());
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Equal(autoSuggestValue, document.QuerySelector("#LocationSearchModel_AutoSuggestValue")!.GetAttribute("value"));
        Assert.Null(document.QuerySelector(".govuk-error-summary"));
    }

    [Fact]
    public async Task Search_GovernorReference_NoGid_Shows_ErrorPanel()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SearchType=GovernorReference");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Get Information about Schools", document.QuerySelector("#proposition-name")!.TextContent.Trim());
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Equal(string.Empty, document.QuerySelector("#GovernorSearchModel_Gid")!.GetAttribute("value"));
        // Check viewModel.ErrorPanel is rendered
        Assert.NotNull(document.QuerySelector("#searchtype-gov-namerole-ref"));
    }

    [Fact]
    public async Task Search_GovernorReference_InvalidGid_Shows_NotFoundError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("1", "edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("2", "edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        // Act
        HttpClient client = _webApplicationFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SearchType=GovernorReference&GovernorSearchModel.Gid=999999");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Get Information about Schools", document.QuerySelector("#proposition-name")!.TextContent.Trim());
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.NotNull(document.QuerySelector("#searchtype-gov-namerole-ref")); // Error panel indicator
        // Optional: Check the Governor ID field retains the entered value
        Assert.Equal("999999", document.QuerySelector("#GovernorSearchModel_Gid")?.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_TextSearchType_NoResults_EmptyText_ShowsRequiredError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SearchType=Text&NoResults=True");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Contains("Please enter an establishment name, URN, LAESTAB or UKPRN to start a search",
            document.QuerySelector(".govuk-error-summary")?.TextContent);
        Assert.Equal(string.Empty, document.QuerySelector("#TextSearchModel_Text")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_TextSearchType_NoResults_WithText_ShowsNotFoundError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();
        var searchText = "Some School";

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=Text&NoResults=True&TextSearchModel.Text={searchText}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Contains("We could not find any establishments matching your search criteria",
            document.QuerySelector(".govuk-error-summary")?.TextContent);
        Assert.Equal(searchText, document.QuerySelector("#TextSearchModel_Text")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_LocationSearchType_NoResults_EmptyText_ShowsRequiredError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SearchType=Location&NoResults=True");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Contains("Please enter a postcode, town or city to start a search",
            document.QuerySelector(".govuk-error-summary")?.TextContent);
        Assert.Equal(string.Empty, document.QuerySelector("#LocationSearchModel_Text")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_LocationSearchType_NoResults_WithText_ShowsNotFoundError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);
        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();
        var locationText = "London";

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SearchType=Location&NoResults=True&LocationSearchModel.Text={locationText}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Contains("We couldn't find any establishments at that location",
            document.QuerySelector(".govuk-error-summary")?.TextContent);
        Assert.Equal(locationText, document.QuerySelector("#LocationSearchModel_Text")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_ByLocalAuthority_NoResults_NoSelectedIds_ShowsRequiredError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);
        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SearchType=ByLocalAuthority&NoResults=True");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());

        // Check error summary contains correct message
        Assert.Contains("Please enter a local authority to start a search",
            document.QuerySelector(".govuk-error-summary")?.TextContent);

        // Check input field is empty
        Assert.Equal(string.Empty, document.QuerySelector("#LocalAuthorityToAdd")!.GetAttribute("value"));
    }

    // TODO: the 'this la has no open schools' message is not displaying. Is there a binding issue?
    [Fact]
    public async Task Search_ByLocalAuthority_NoResults_WithSelectedIds_ShowsNoOpenSchoolsError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);
        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SearchType=ByLocalAuthority&NoResults=True&d=1");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Contains("This local authority has no open schools",
            document.QuerySelector(".govuk-error-summary")?.TextContent);
        Assert.Equal("1", document.QuerySelector("input[name='d']")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_GroupSearchType_NoResults_EmptyText_ShowsRequiredError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SelectedTab=Groups&SearchType=Group&NoResults=True");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment group", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Contains("Please enter an establishment group to start a search",
            document.QuerySelector(".govuk-error-summary")?.TextContent);
        Assert.Equal(string.Empty, document.QuerySelector("#GroupSearchModel_Text")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_GroupSearchType_NoResults_WithText_ShowsNotFoundError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);
        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();
        var groupText = "Academy Trust";

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SelectedTab=Groups&SearchType=Group&NoResults=True&GroupSearchModel.Text={groupText}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find an establishment group", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());
        Assert.Contains("We could not find any establishment groups matching your search criteria",
            document.QuerySelector(".govuk-error-summary")?.TextContent);
        Assert.Equal(groupText, document.QuerySelector("#GroupSearchModel_Text")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_GovernorSearchType_NoResults_EmptyDetails_ShowsRequiredError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);
        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync("/Search/search?SelectedTab=Governors&SearchType=Governor&NoResults=True");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find a governor", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());

        Assert.Contains("Please enter a governor to start a search",
            document.QuerySelector(".govuk-error-summary")?.TextContent);

        Assert.Equal(string.Empty, document.QuerySelector("#GovernorSearchModel_Forename")!.GetAttribute("value"));
        Assert.Equal(string.Empty, document.QuerySelector("#surname")!.GetAttribute("value"));
    }

    [Fact]
    public async Task Search_GovernorSearchType_NoResults_WithDetails_ShowsNotFoundError()
    {
        // Arrange
        HttpMappingRequest request = new(
        [
            new HttpMappingFile("edubase/lookup/get-local-authorities.json"),
            new HttpMappingFile("edubase/lookup/get-governor-roles.json"),
        ]);

        await _edubaseApiFixture.RegisterHttpMapping(request);

        HttpClient client = _webApplicationFactory.CreateClient();
        var forename = "John";
        var surname = "Smith";

        // Act
        HttpResponseMessage httpResponse = await client.GetAsync($"/Search/search?SelectedTab=Governors&SearchType=Governor&NoResults=True&GovernorSearchModel.Forename={forename}&GovernorSearchModel.Surname={surname}");
        IHtmlDocument document = await httpResponse.GetDocumentAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Find a governor", document.QuerySelector(".gias-tabs__list-item--selected")!.Text().Trim());

        Assert.Contains("We could not find any governors matching your search criteria",
            document.QuerySelector(".govuk-error-summary")?.TextContent);

        Assert.Equal(forename, document.QuerySelector("#GovernorSearchModel_Forename")!.GetAttribute("value"));
        Assert.Equal(surname, document.QuerySelector("#surname")!.GetAttribute("value"));
    }




    // I assume we don't want this test but leaving it here for the time being?
    [Theory]
    [InlineData("Text", "checked", null, null, null)]
    [InlineData("Location", null, "checked", null, null)]
    [InlineData("ByLocalAuthority", null, null, "checked", null)]
    [InlineData("EstablishmentAll", null, null, null, "checked")]
    public async Task Search_SearchType_Checkbox_IsChecked(
        string searchType,
        string? textChecked,
        string? locationChecked,
        string? laChecked,
        string? allChecked
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
*/
}
