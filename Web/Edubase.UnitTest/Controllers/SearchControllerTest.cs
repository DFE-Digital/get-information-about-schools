using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Geo;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Models.Search;
using Moq;
using NUnit.Framework;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class SearchControllerTest
    {
        [Test]
        public async Task SearchController_Index_RemovesLocalAuthorityId()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>().Object;
            var gps = new Mock<IPlacesLookupService>().Object;
            var subject = new SearchController(ers, cls, grs, gps);
            var result = (RedirectResult) await subject.Index(new SearchViewModel { LocalAuthorityToRemove = 1, SelectedLocalAuthorityIds = new LocalAuthorityIdList(new[] { 1, 2, 3 }) });
            Assert.That(result.Url, Is.EqualTo($"/?{SearchViewModel.BIND_ALIAS_LAIDS}=2&{SearchViewModel.BIND_ALIAS_LAIDS}=3#la"));
        }

        [Test]
        public async Task SearchController_Index_RedirectsBackOnSelfForLADisambiguationExactMatch()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>();
            var gps = new Mock<IPlacesLookupService>().Object;

            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" } });

            var subject = new SearchController(ers, cls.Object, grs, gps);
            var result = (RedirectResult) await subject.Index(new SearchViewModel { SearchType = eSearchType.LocalAuthorityDisambiguation, LocalAuthorityToAdd = "TESTLA", SelectedLocalAuthorityIds = new LocalAuthorityIdList(new[] { 1 }) });

            Assert.That(result.Url, Is.EqualTo($"/?{SearchViewModel.BIND_ALIAS_LAIDS}=1&{SearchViewModel.BIND_ALIAS_LAIDS}=2#la"));
        }

        [Test]
        public async Task SearchController_Index_ShowLADisambiguationPage()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>();
            var gps = new Mock<IPlacesLookupService>().Object;

            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" }, new LookupDto { Id = 3, Name = "BOB" } });

            var subject = new SearchController(ers, cls.Object, grs, gps);
            var result = (ViewResult) await subject.Index(new SearchViewModel { SearchType = eSearchType.LocalAuthorityDisambiguation, LocalAuthorityToAdd = "TEST", SelectedLocalAuthorityIds = new LocalAuthorityIdList(new[] { 1 }) });

            Assert.That(result.ViewName, Is.EqualTo("LocalAuthorityDisambiguation"));
            var disambiguationResults = ((LocalAuthorityDisambiguationViewModel) result.Model).MatchingLocalAuthorities;
            Assert.That(disambiguationResults[0].Id, Is.EqualTo(2));
            Assert.That(disambiguationResults[0].Name, Is.EqualTo("TESTLA"));
            Assert.That(disambiguationResults.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task SearchController_Index_ShowLocationDisambiguationPage()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>();
            var gps = new Mock<IPlacesLookupService>();
            var coords = new Edubase.Common.Spatial.LatLon(1, 2);
            const string placeName = "BobVille";

            gps.Setup(x => x.SearchAsync("Bob", false)).ReturnsAsync(new[] { new PlaceDto(placeName, coords) });

            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
            var viewModel = new SearchViewModel { SearchType = eSearchType.Location };
            viewModel.LocationSearchModel.Text = "Bob";
            var result = (ViewResult) await subject.Index(viewModel);

            Assert.That(result.ViewName, Is.EqualTo("LocationDisambiguation"));
            var disambiguationResults = ((LocationDisambiguationViewModel) result.Model).MatchingLocations;
            Assert.That(disambiguationResults[0].Coords, Is.EqualTo(coords));
            Assert.That(disambiguationResults[0].Name, Is.EqualTo(placeName));
            Assert.That(disambiguationResults.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task SearchController_Index_ShowsSearchPage()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>();
            var gps = new Mock<IPlacesLookupService>();

            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" }, new LookupDto { Id = 3, Name = "BOB" } });
            cls.Setup(x => x.GovernorRolesGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "role" }, new LookupDto { Id = 3, Name = "role" } });

            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
            var result = (ViewResult) await subject.Index(new SearchViewModel());

            var viewModel = (SearchViewModel) result.Model;
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(viewModel.GovernorRoles, Is.Not.Empty);
            Assert.That(viewModel.LocalAuthorities, Is.Not.Empty);
        }

        [Test]
        public async Task SearchController_IndexResults_RedirectsToEstabSearch()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>();
            var gps = new Mock<IPlacesLookupService>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString("a=b&c=d&e=f"));

            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);

            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Establishments/Search");

            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            const string resultantUrl = "/Establishments/Search?a=b&c=d&e=f&OpenOnly=false";
            var result = (RedirectResult) await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.ByLocalAuthority });
            Assert.That(result.Url, Is.EqualTo(resultantUrl));

            result = (RedirectResult) await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Location });
            Assert.That(result.Url, Is.EqualTo(resultantUrl));

            result = (RedirectResult) await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Text });
            Assert.That(result.Url, Is.EqualTo(resultantUrl));

            result = (RedirectResult) await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.EstablishmentAll });
            Assert.That(result.Url, Is.EqualTo(resultantUrl));
        }

        [Test]
        public async Task SearchController_IndexResults_RedirectsToGroupSearch()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>();
            var gps = new Mock<IPlacesLookupService>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString("a=b&c=d&e=f"));

            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);

            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Group/Search");

            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            const string resultantUrl = "/Group/Search?a=b&c=d&e=f";
            var result = (RedirectResult) await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Group });
            Assert.That(result.Url, Is.EqualTo(resultantUrl));
        }

        [Test]
        public async Task SearchController_IndexResults_RedirectsToGovernorSearch()
        {
            var ers = new Mock<IEstablishmentReadService>().Object;
            var grs = new Mock<IGroupReadService>().Object;
            var cls = new Mock<ICachedLookupService>();
            var gps = new Mock<IPlacesLookupService>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString("a=b&c=d&e=f"));

            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);

            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Governor/Search");

            var subject = new SearchController(ers, cls.Object, grs, gps.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            const string resultantUrl = "/Governor/Search?a=b&c=d&e=f&";
            var result = (RedirectResult) await subject.IndexResults(new SearchViewModel { SearchType = eSearchType.Governor });
            Assert.That(result.Url, Is.EqualTo(resultantUrl));
        }
    }
}
