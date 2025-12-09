using System.Net;
using System.Net.NetworkInformation;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Enums;
using Edubase.Services.Geo;
using Edubase.Services.Governors.Factories;
using Edubase.Services.Lookup;
using Edubase.Web.IntegrationTests.Helpers;
using Edubase.Web.UI.Models.Search;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Edubase.Web.IntegrationTests.Tests.SearchController
{
    public sealed class SearchControllerIndexResultsTests
    {
        private static LookupDto[] DefaultLocalAuthorities => [
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

        private static LookupDto[] DefaultGovernorRoles => [
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

        [Fact]
        public async Task IndexResults_Removes_LocalAuthority_And_Redirects_With_RemainingIds()
        {
            // Arrange
            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
            .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
            .ReturnsAsync(DefaultLocalAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using WebApplicationFactory<Program> webAppFactory =
            new GiasWebApplicationFactory()
                .WithWebHostBuilder(
                (builder) =>
                    builder.ConfigureServices(
                        (services) =>
                        {
                            services.RemoveAll<ICachedLookupService>();
                            services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);
                        }));

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/Results?LocalAuthorityToRemove=1&d=1&d=2");

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            response.Headers.TryGetValues("Location", out var locations);
            var redirectPath = Assert.Single(locations);

            // Expected: removes ID 1, keeps ID 2
            Assert.Equal("?d=2#la", redirectPath);
        }


        [Theory]
        [InlineData("Bristol")]
        [InlineData("bristol")] // Case-insensitive
        public async Task IndexResults_LocalAuthorityDisambiguation_ExactMatch_Redirects(string searchKeyword)
        {
            // Arrange
            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
            .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
            .ReturnsAsync(DefaultLocalAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using WebApplicationFactory<Program> webAppFactory =
            new GiasWebApplicationFactory()
                .WithWebHostBuilder(
                (builder) =>
                    builder.ConfigureServices(
                        (services) =>
                        {
                            services.RemoveAll<ICachedLookupService>();
                            services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);
                        }));

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"/Search/Results?SearchType=LocalAuthorityDisambiguation&LocalAuthorityToAdd={searchKeyword}");

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            response.Headers.TryGetValues("Location", out var locations);
            var redirectPath = Assert.Single(locations);
            Assert.Equal("?SearchType=ByLocalAuthority&d=1#la", redirectPath);
        }


        [Theory]
        [InlineData("bris")]
        [InlineData("BrI")]
        public async Task IndexResults_LocalAuthorityDisambiguation_NoExactMatch_RendersDisambiguationView(string keyword)
        {
            // Arrange
            var stubbedAuthorities = new[]
            {
                new LookupDto { Id = 1, Name = "Bristol" },
                new LookupDto { Id = 2, Name = "Brisbane" },
                new LookupDto { Id = 3, Name = "Barnsley" }
            };

            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
            .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
            .ReturnsAsync(stubbedAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using WebApplicationFactory<Program> webAppFactory =
            new GiasWebApplicationFactory()
                .WithWebHostBuilder(
                (builder) =>
                    builder.ConfigureServices(
                        (services) =>
                        {
                            services.RemoveAll<ICachedLookupService>();
                            services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);
                        }));

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync($"/Search/Results?SearchType=LocalAuthorityDisambiguation&LocalAuthorityToAdd={keyword}");
            var document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var links = document.QuerySelectorAll("#search-localauthority-disambiguation-list a").ToList();
            var expectedMatches = stubbedAuthorities.Where(la => la.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            Assert.Equal(expectedMatches.Count, links.Count);
            for (int i = 0; i < links.Count; i++)
            {
                Assert.Equal(expectedMatches[i].Name, links[i].TextContent.Trim());
                Assert.Equal($"/Search/search?SearchType=ByLocalAuthority&OpenOnly=False&d={expectedMatches[i].Id}#la", links[i].GetAttribute("href"));
            }
        }

        [Fact]
        public async Task IndexResults_ByLocalAuthority_WithLocalAuthorityToAdd_ExactMatch_Redirects()
        {
            // Arrange
            Mock<ICachedLookupService> lookupServiceMock = new ();

            lookupServiceMock
            .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
            .ReturnsAsync(DefaultLocalAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using var webAppFactory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<ICachedLookupService>();
                        services.AddSingleton(lookupServiceMock.Object);
                    });
                });

            var client = webAppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Search/Results?SearchType=ByLocalAuthority&LocalAuthorityToAdd=Bristol");

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            response.Headers.TryGetValues("Location", out var locations);
            var redirectPath = Assert.Single(locations);
            Assert.Equal("?SearchType=ByLocalAuthority&d=1#la", redirectPath);
        }

        [Fact]
        public async Task IndexResults_ByLocalAuthority_WithLocalAuthorityToAdd_NoExactMatch_RendersDisambiguationView()
        {
            // Arrange
            var stubbedAuthorities = new[]
                {
                new LookupDto { Id = 1, Name = "Bristol" },
                new LookupDto { Id = 2, Name = "Brisbane" },
                new LookupDto { Id = 3, Name = "Barnsley" }
            };

            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
            .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
            .ReturnsAsync(stubbedAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using var webAppFactory = new GiasWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<ICachedLookupService>();
                        services.AddSingleton(lookupServiceMock.Object);
                    });
                });

            var client = webAppFactory.CreateClient();

            // Act

            var response = await client.GetAsync("/Search/Results?SearchType=ByLocalAuthority&LocalAuthorityToAdd=bris");
            var document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var links = document.QuerySelectorAll("#search-localauthority-disambiguation-list a").ToList();
            var expectedMatches = stubbedAuthorities.Where(la => la.Name.Contains("bris", StringComparison.OrdinalIgnoreCase)).ToList();

            Assert.Equal(expectedMatches.Count, links.Count);
            for (int i = 0; i < links.Count; i++)
            {
                Assert.Equal(expectedMatches[i].Name, links[i].TextContent.Trim());
                Assert.Equal($"/Search/search?SearchType=ByLocalAuthority&OpenOnly=False&d={expectedMatches[i].Id}#la", links[i].GetAttribute("href"));
            }
        }


        [Fact]
        public async Task IndexResults_LocationSearch_DisambiguationView_WhenMultipleMatches()
        {
            // Arrange
            PlaceDto[] placesDtos = [
            new()
            {
                Name = "High Street, Leicester, England",
                Coords = new(52.6369, -1.1398)
            },
            new()
            {
                Name = "Granby Street, Leicester, England",
                Coords = new(52.6375, -1.1332)
            },
            new()
            {
                Name = "London Road, Leicester, England",
                Coords = new(52.6290, -1.1200)
            }

        ];

            Mock<IPlacesLookupService> placesLookupServiceMock = new();

            placesLookupServiceMock
                .Setup(placesLookup => placesLookup.SearchAsync(It.IsAny<string>(), false))
                .ReturnsAsync(placesDtos);

            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
                .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
                .ReturnsAsync(DefaultLocalAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using WebApplicationFactory<Program> webAppFactory =
                new GiasWebApplicationFactory()
                    .WithWebHostBuilder(
                    (builder) =>
                        builder.ConfigureServices(
                            (services) =>
                            {
                                services.RemoveAll<ICachedLookupService>();
                                services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);

                                services.RemoveAll<IPlacesLookupService>();
                                services.AddSingleton<IPlacesLookupService>(sp => placesLookupServiceMock.Object);
                            }));

            // Act
            HttpClient client = webAppFactory.CreateClient();

            const string searchText = "Leicester";
            var response = await client.GetAsync($"/Search/Results?SearchType=Location&LocationSearchModel.Text={searchText}");
            var document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains($"3 locations matching '{searchText}'", document.QuerySelector("h1 + p")?.TextContent);
            var links = document.QuerySelectorAll("#search-location-matching-locations a").ToList();
            Assert.Equal(placesDtos.Length, links.Count);

            for (int i = 0; i < links.Count; i++)
            {
                Assert.Equal(placesDtos[i].Name, links[i].TextContent.Trim());
                Assert.Contains($"LocationSearchModel.AutoSuggestValue={placesDtos[i].Coords.Latitude},{placesDtos[i].Coords.Longitude}", links[i].GetAttribute("href"));
            }
        }

        [Fact]
        public async Task IndexResults_LocationSearch_NoMatches_RedirectsToEstablishmentsSearch()
        {
            // Arrange
            Mock<IPlacesLookupService> placesLookupServiceMock = new();

            placesLookupServiceMock
                .Setup(placesLookup => placesLookup.SearchAsync(It.IsAny<string>(), false))
                .ReturnsAsync([]);

            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
                .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
                .ReturnsAsync(DefaultLocalAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using WebApplicationFactory<Program> webAppFactory =
                new GiasWebApplicationFactory()
                    .WithWebHostBuilder(
                    (builder) =>
                        builder.ConfigureServices(
                            (services) =>
                            {
                                services.RemoveAll<ICachedLookupService>();
                                services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);

                                services.RemoveAll<IPlacesLookupService>();
                                services.AddSingleton<IPlacesLookupService>(sp => placesLookupServiceMock.Object);
                            }));

            // Act
            HttpClient client = webAppFactory.CreateClient();

            const string searchText = "UnknownPlace";
            var response = await client.GetAsync($"/Search/Results?SearchType=Location&LocationSearchModel.Text={searchText}");
            var document = await response.GetDocumentAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            response.Headers.TryGetValues("Location", out var locations);
            var redirectPath = Assert.Single(locations);
            Assert.StartsWith("/Establishments/Search/index?", redirectPath);
            Assert.Contains("SearchType=Location", redirectPath);
            Assert.Contains("LocationSearchModel.Text=UnknownPlace", redirectPath);
        }


        [Theory]
        [InlineData(eSearchType.Text)]
        [InlineData(eSearchType.Location)]
        [InlineData(eSearchType.ByLocalAuthority)]
        [InlineData(eSearchType.EstablishmentAll)]
        public async Task IndexResults_EstablishmentSearch_Redirects_WithOpenOnlyTrue(eSearchType searchType)
        {
            // Arrange
            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
                .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
                .ReturnsAsync(DefaultLocalAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using WebApplicationFactory<Program> webAppFactory =
                new GiasWebApplicationFactory()
                    .WithWebHostBuilder(
                    (builder) =>
                        builder.ConfigureServices(
                            (services) =>
                            {
                                services.RemoveAll<ICachedLookupService>();
                                services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);
                            }));

            // Act
            HttpClient client = webAppFactory.CreateClient();
            var response = await client.GetAsync($"/Search/Results?SearchType={searchType}&TextSearchModel.Text=Academy&OpenOnly=true");

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            response.Headers.TryGetValues("Location", out var locations);
            var redirectPath = Assert.Single(locations);

            Assert.StartsWith("/Establishments/Search/index?", redirectPath);
            Assert.Contains($"SearchType={searchType}", redirectPath);
            Assert.Contains("TextSearchModel.Text=Academy", redirectPath);
            Assert.Contains($"b={(int) eLookupEstablishmentStatus.Open},{(int) eLookupEstablishmentStatus.OpenButProposedToClose}", redirectPath);
        }

        [Theory]
        [InlineData(eSearchType.Text)]
        [InlineData(eSearchType.Location)]
        [InlineData(eSearchType.ByLocalAuthority)]
        [InlineData(eSearchType.EstablishmentAll)]
        public async Task IndexResults_EstablishmentSearch_Redirects_WithOpenOnlyFalse(eSearchType searchType)
        {
            // Arrange
            Mock<ICachedLookupService> lookupServiceMock = new();

            lookupServiceMock
                .Setup((lookupService) => lookupService.LocalAuthorityGetAllAsync())
                .ReturnsAsync(DefaultLocalAuthorities);

            lookupServiceMock
                .Setup((lookupService) => lookupService.GovernorRolesGetAllAsync())
                .ReturnsAsync(DefaultGovernorRoles);

            using WebApplicationFactory<Program> webAppFactory =
                new GiasWebApplicationFactory()
                    .WithWebHostBuilder(
                    (builder) =>
                        builder.ConfigureServices(
                            (services) =>
                            {
                                services.RemoveAll<ICachedLookupService>();
                                services.AddSingleton<ICachedLookupService>(sp => lookupServiceMock.Object);
                            }));

            // Act
            HttpClient client = webAppFactory.CreateClient();
            var response = await client.GetAsync($"/Search/Results?SearchType={searchType}&TextSearchModel.Text=Academy&OpenOnly=false");

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            response.Headers.TryGetValues("Location", out var locations);
            var redirectPath = Assert.Single(locations);

            Assert.StartsWith("/Establishments/Search/index?", redirectPath);
            Assert.Contains($"SearchType={searchType}", redirectPath);
            Assert.Contains("TextSearchModel.Text=Academy", redirectPath);
            Assert.Contains($"OpenOnly=false", redirectPath);
        }
    }
}
