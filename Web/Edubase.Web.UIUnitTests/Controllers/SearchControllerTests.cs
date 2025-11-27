//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using Edubase.Services.Domain;
//using Edubase.Services.Establishments;
//using Edubase.Services.Geo;
//using Edubase.Services.Groups;
//using Edubase.Services.Lookup;
//using Edubase.Web.UI.Models.Search;
//using Moq;
//using Xunit;
//using Microsoft.AspNetCore.Mvc;

//namespace Edubase.Web.UI.Controllers.UnitTests
//{
//    public class SearchControllerTests
//    {
//        [Fact]
//        public async Task SearchController_Index_RemovesLocalAuthorityId()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>().Object;
//            var gps = new Mock<IPlacesLookupService>().Object;
//            var subject = new SearchController(ers, cls, grs, gps);
//            var result = await subject.Index(new SearchViewModel
//            {
//                LocalAuthorityToRemove = 1,
//                SelectedLocalAuthorityIds = new LocalAuthorityIdList(new[] { 1, 2, 3 })
//            }) as RedirectResult;

//            Assert.NotNull(result);
//            Assert.Equal($"/?{SearchViewModel.BIND_ALIAS_LAIDS}=2&{SearchViewModel.BIND_ALIAS_LAIDS}=3#la", result.Url);
//        }

//        [Fact]
//        public async Task SearchController_Index_RedirectsBackOnSelfForLADisambiguationExactMatch()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>();
//            var gps = new Mock<IPlacesLookupService>().Object;

//            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" } });

//            var subject = new SearchController(ers, cls.Object, grs, gps);
//            var result = await subject.Index(new SearchViewModel
//            {
//                SearchType = eSearchType.LocalAuthorityDisambiguation,
//                LocalAuthorityToAdd = "TESTLA",
//                SelectedLocalAuthorityIds = new LocalAuthorityIdList(new[] { 1 })
//            }) as RedirectResult;

//            Assert.NotNull(result);
//            Assert.Equal($"?SearchType=ByLocalAuthority&{SearchViewModel.BIND_ALIAS_LAIDS}=1&{SearchViewModel.BIND_ALIAS_LAIDS}=2#la",
//                result.Url);
//        }

//        [Fact]
//        public async Task SearchController_Index_ShowLADisambiguationPage()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>();
//            var gps = new Mock<IPlacesLookupService>().Object;

//            var testLa1 = new LookupDto { Id = 2, Name = "TESTLA" };
//            var testLa2 = new LookupDto { Id = 3, Name = "BOB" };

//            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] {testLa1, testLa2 });

//            var subject = new SearchController(ers, cls.Object, grs, gps);
//            var result = await subject.Index(new SearchViewModel
//            {
//                SearchType = eSearchType.LocalAuthorityDisambiguation,
//                LocalAuthorityToAdd = "TEST",
//                SelectedLocalAuthorityIds = new LocalAuthorityIdList(new[] { 1 })
//            }) as ViewResult;
//            var disambiguationResults = (result?.Model as LocalAuthorityDisambiguationViewModel)?.MatchingLocalAuthorities;
//            var firstDisambiguationResult = disambiguationResults?.First();

//            Assert.NotNull(result);
//            Assert.NotNull(disambiguationResults);
//            Assert.NotNull(firstDisambiguationResult);
//            Assert.Equal("LocalAuthorityDisambiguation", result.ViewName);
//            Assert.Single(disambiguationResults);
//            Assert.Equal(testLa1, firstDisambiguationResult);
//        }

//        [Fact]
//        public async Task SearchController_Index_ShowLocationDisambiguationPage()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>();
//            var gps = new Mock<IPlacesLookupService>();
//            var coords = new Common.Spatial.LatLon(1, 2);
//            const string placeName = "BobVille";

//            var placeDto = new PlaceDto(placeName, coords);

//            gps.Setup(x => x.SearchAsync("Bob", false)).ReturnsAsync(new[] { placeDto });

//            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
//            var viewModel = new SearchViewModel { SearchType = eSearchType.Location };
//            viewModel.LocationSearchModel.Text = "Bob";
//            var result = await subject.Index(viewModel) as ViewResult;
//            var disambiguationResults = (result?.Model as LocationDisambiguationViewModel)?.MatchingLocations;
//            var firstDisambiguationResult = disambiguationResults?.First();

//            Assert.NotNull(result);
//            Assert.Equal("LocationDisambiguation", result.ViewName);
//            Assert.NotNull(disambiguationResults);
//            Assert.Single(disambiguationResults);
//            Assert.NotNull(firstDisambiguationResult);
//            Assert.Equal(placeDto, firstDisambiguationResult);
//        }

//        [Fact]
//        public async Task SearchController_Index_ShowsSearchPage()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>();
//            var gps = new Mock<IPlacesLookupService>();

//            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[]
//            {
//                new LookupDto { Id = 2, Name = "TESTLA" },
//                new LookupDto { Id = 3, Name = "BOB" }
//            });
//            cls.Setup(x => x.GovernorRolesGetAllAsync()).ReturnsAsync(new[]
//            {
//                new LookupDto { Id = 2, Name = "role" },
//                new LookupDto { Id = 3, Name = "role" }
//            });

//            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
//            var result = await subject.Index(new SearchViewModel()) as ViewResult;

//            var viewModel = result?.Model as SearchViewModel;

//            Assert.NotNull(result);
//            Assert.NotNull(viewModel);
//            Assert.Equal("Index", result.ViewName);
//            Assert.NotEmpty(viewModel.GovernorRoles);
//            Assert.NotEmpty(viewModel.LocalAuthorities);
//        }

//        [Fact]
//        public async Task SearchController_IndexResults_RedirectsToEstabSearch()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>();
//            var gps = new Mock<IPlacesLookupService>();

//            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
//            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString("a=b&c=d&e=f"));

//            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
//            context.SetupGet(x => x.Request).Returns(request.Object);

//            var mockUrlHelper = new Mock<IUrlHelper>();
//            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Establishments/Search");

//            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
//            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
//            subject.Url = mockUrlHelper.Object;

//            const string resultantUrl = "/Establishments/Search?a=b&c=d&e=f&OpenOnly=false";
//            var searchTasks = new List<Task<ActionResult>>
//            {
//                subject.IndexResults(new SearchViewModel { SearchType = eSearchType.ByLocalAuthority }),
//                subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Location }),
//                subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Text }),
//                subject.IndexResults(new SearchViewModel { SearchType = eSearchType.EstablishmentAll })
//            };
            
//            while(searchTasks.Any())
//            {
//                var completeTask = await Task.WhenAny(searchTasks);
//                searchTasks.Remove(completeTask);
//                var result = completeTask.Result as RedirectResult;

//                Assert.NotNull(result);
//                Assert.Equal(resultantUrl, result.Url);
//            }

//            Assert.Empty(searchTasks);
//        }

//        [Fact]
//        public async Task SearchController_IndexResults_RedirectsToGroupSearch()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>();
//            var gps = new Mock<IPlacesLookupService>();

//            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
//            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString("a=b&c=d&e=f"));

//            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
//            context.SetupGet(x => x.Request).Returns(request.Object);

//            var mockUrlHelper = new Mock<IUrlHelper>();
//            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Group/Search");

//            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
//            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
//            subject.Url = mockUrlHelper.Object;

//            const string resultantUrl = "/Group/Search?a=b&c=d&e=f";
//            var result = await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Group }) as RedirectResult;

//            Assert.NotNull(result);
//            Assert.Equal(resultantUrl, result.Url);
//        }

//        [Fact]
//        public async Task SearchController_IndexResults_RedirectsToGovernorSearch()
//        {
//            var ers = new Mock<IEstablishmentReadService>().Object;
//            var grs = new Mock<IGroupReadService>().Object;
//            var cls = new Mock<ICachedLookupService>();
//            var gps = new Mock<IPlacesLookupService>();

//            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
//            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString("a=b&c=d&e=f"));

//            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
//            context.SetupGet(x => x.Request).Returns(request.Object);

//            var mockUrlHelper = new Mock<IUrlHelper>();
//            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Governor/Search");

//            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
//            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
//            subject.Url = mockUrlHelper.Object;

//            const string resultantUrl = "/Governor/Search?a=b&c=d&e=f&";
//            var result = await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Governor }) as RedirectResult;

//            Assert.NotNull(result);
//            Assert.Equal(resultantUrl, result.Url);
//        }
//    }
//}
