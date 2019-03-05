using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Establishments.Controllers;
using Edubase.Web.UI.Areas.Establishments.Models.Search;
using Moq;
using NUnit.Framework;
using static Edubase.Web.UI.Areas.Establishments.Models.Search.EstablishmentSearchViewModel;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class EstablishmentsSearchControllerTest
    {
        [Test]
        public async Task EstabSearch_Index_DisplaysResults()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" }, new LookupDto { Id = 3, Name = "BOB" } });
            cls.Setup(x => x.EstablishmentStatusesGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 1, Name = "status1" }, new LookupDto { Id = 2, Name = "status2" } });
            cls.Setup(x => x.GetNameAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync("bob");
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(false));
            ers.Setup(x => x.GetPermittedStatusIdsAsync(It.IsAny<IPrincipal>())).ReturnsAsync(new[] { 1 });
            ers.Setup(x => x.SearchAsync(It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<EstablishmentSearchResultModel>(2, new List<EstablishmentSearchResultModel>
            {
                new EstablishmentSearchResultModel
                {
                    Name = "School 1"
                },
                new EstablishmentSearchResultModel
                {
                    Name = "School 2"
                }
            }));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";

            var result = (ViewResult) await subject.Index(vm);

            Assert.That(vm.Count, Is.EqualTo(2));
            Assert.That(vm.Results.Count, Is.EqualTo(2));
            Assert.That(vm.Results[0].Name, Is.EqualTo("School 1"));
            Assert.That(vm.Results[1].Name, Is.EqualTo("School 2"));
        }

        [Test]
        public async Task EstabSearch_Index_GoToDetailPageOnOneResult_MultipleResults()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" }, new LookupDto { Id = 3, Name = "BOB" } });
            cls.Setup(x => x.EstablishmentStatusesGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 1, Name = "status1" }, new LookupDto { Id = 2, Name = "status2" } });
            cls.Setup(x => x.GetNameAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync("bob");
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(false));
            ers.Setup(x => x.GetPermittedStatusIdsAsync(It.IsAny<IPrincipal>())).ReturnsAsync(new[] { 1 });
            ers.Setup(x => x.SearchAsync(It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<EstablishmentSearchResultModel>(2, new List<EstablishmentSearchResultModel>
            {
                new EstablishmentSearchResultModel
                {
                    Name = "School 1"
                },
                new EstablishmentSearchResultModel
                {
                    Name = "School 2"
                }
            }));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = true;
            var result = (ViewResult) await subject.Index(vm);
            Assert.That(vm.Count, Is.EqualTo(2));
            Assert.That(vm.Results.Count, Is.EqualTo(2));
            Assert.That(vm.Results[0].Name, Is.EqualTo("School 1"));
            Assert.That(vm.Results[1].Name, Is.EqualTo("School 2"));

            vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = false;
            result = (ViewResult) await subject.Index(vm);
            Assert.That(vm.Count, Is.EqualTo(2));
            Assert.That(vm.Results.Count, Is.EqualTo(2));
            Assert.That(vm.Results[0].Name, Is.EqualTo("School 1"));
            Assert.That(vm.Results[1].Name, Is.EqualTo("School 2"));
        }

        [Test]
        public async Task EstabSearch_Index_GoToDetailPageOnOneResult_OneResult()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" }, new LookupDto { Id = 3, Name = "BOB" } });
            cls.Setup(x => x.EstablishmentStatusesGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 1, Name = "status1" }, new LookupDto { Id = 2, Name = "status2" } });
            cls.Setup(x => x.GetNameAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync("bob");
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(false));
            ers.Setup(x => x.GetPermittedStatusIdsAsync(It.IsAny<IPrincipal>())).ReturnsAsync(new[] { 1 });
            ers.Setup(x => x.SearchAsync(It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<EstablishmentSearchResultModel>(1, new List<EstablishmentSearchResultModel>
            {
                new EstablishmentSearchResultModel
                {
                    Name = "School 1",
                    Urn = 672393
                }
            }));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = true;
            var result = (RedirectToRouteResult) await subject.Index(vm);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Establishment"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(672393));
            Assert.That(result.RouteValues["area"], Is.EqualTo("Establishments"));

            vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = false;
            var result2 = (ViewResult) await subject.Index(vm);
            Assert.That(vm.Count, Is.EqualTo(1));
            Assert.That(vm.Results.Count, Is.EqualTo(1));
            Assert.That(vm.Results[0].Name, Is.EqualTo("School 1"));
        }

        [Test, TestCase("432/5437"), TestCase("4325437")]
        public async Task EstabSearch_Index_SearchWithLAESTAB(string laestab)
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 2, Name = "TESTLA" }, new LookupDto { Id = 3, Name = "BOB" } });
            cls.Setup(x => x.EstablishmentStatusesGetAllAsync()).ReturnsAsync(new[] { new LookupDto { Id = 1, Name = "status1" }, new LookupDto { Id = 2, Name = "status2" } });
            cls.Setup(x => x.GetNameAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync("bob");
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(false));
            ers.Setup(x => x.GetPermittedStatusIdsAsync(It.IsAny<IPrincipal>())).ReturnsAsync(new[] { 1 });
            ers.Setup(x => x.SearchAsync(It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<EstablishmentSearchResultModel>(1, new List<EstablishmentSearchResultModel>
            {
                new EstablishmentSearchResultModel
                {
                    Name = "School 1",
                    Urn = 672393,
                    EstablishmentNumber = 5437,
                    LocalAuthorityId = 432
                }
            }));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = laestab;
            var result = (ViewResult) await subject.Index(vm);
            Assert.That(vm.TextSearchType, Is.EqualTo(eTextSearchType.LAESTAB));
            Assert.That(vm.Count, Is.EqualTo(1));
            Assert.That(vm.Results.Count, Is.EqualTo(1));
            Assert.That(vm.Results[0].Name, Is.EqualTo("School 1"));
            Assert.That(vm.Results[0].LocalAuthorityId, Is.EqualTo(432));
            Assert.That(vm.Results[0].EstablishmentNumber, Is.EqualTo(5437));
        }

        [Test]
        public async Task EstabSearch_Index_WithInvalidURN_RedirectsBackOnSelf()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Strict);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Establishments/Search");
            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(false));
            ers.Setup(x => x.SearchAsync(It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<EstablishmentSearchResultModel>(0, new List<EstablishmentSearchResultModel>()));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.AutoSuggestValue = "1256";

            var result = (RedirectResult) await subject.Index(vm);

            Assert.That(result.Url, Is.EqualTo("action=Index|controller=Search|area=|SearchType=Text|TextSearchModel.Text=|NoResults=True"));
        }

        [Test]
        public async Task EstabSearch_Index_WithValidURN_RedirectsToEstabDetail()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Strict);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(true));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.AutoSuggestValue = "123456";

            var result = (RedirectToRouteResult) await subject.Index(vm);

            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Establishment"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123456));
            Assert.That(result.RouteValues["area"], Is.EqualTo("Establishments"));
        }

        [Test]
        public async Task EstabSearch_PrepareDownload_Step1_BackOfficeUser()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel();
            var result = (ViewResult) await subject.PrepareDownload(viewModel);

            Assert.That(viewModel.AllowAnyExtraFields, Is.True);
            Assert.That(viewModel.AllowIncludeBringUpFields, Is.True);
            Assert.That(viewModel.AllowIncludeChildrensCentreFields, Is.True);
            Assert.That(viewModel.AllowIncludeEmailAddresses, Is.True);
            Assert.That(viewModel.AllowIncludeIEBTFields, Is.True);
            Assert.That(result.ViewName, Is.EqualTo("Downloads/SelectDataset"));
        }

        [Test]
        public async Task EstabSearch_PrepareDownload_Step1_PublicUser()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel();
            var result = (ViewResult) await subject.PrepareDownload(viewModel);

            Assert.That(viewModel.AllowAnyExtraFields, Is.False);
            Assert.That(viewModel.AllowIncludeBringUpFields, Is.False);
            Assert.That(viewModel.AllowIncludeChildrensCentreFields, Is.False);
            Assert.That(viewModel.AllowIncludeEmailAddresses, Is.False);
            Assert.That(viewModel.AllowIncludeIEBTFields, Is.False);
            Assert.That(result.ViewName, Is.EqualTo("Downloads/SelectDataset"));
        }

        [Test]
        public async Task EstabSearch_PrepareDownload_Step2()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel() { Dataset = eDataSet.Full };
            var result = (ViewResult) await subject.PrepareDownload(viewModel);

            Assert.That(viewModel.AllowAnyExtraFields, Is.True);
            Assert.That(viewModel.AllowIncludeBringUpFields, Is.True);
            Assert.That(viewModel.AllowIncludeChildrensCentreFields, Is.True);
            Assert.That(viewModel.AllowIncludeEmailAddresses, Is.True);
            Assert.That(viewModel.AllowIncludeIEBTFields, Is.True);
            Assert.That(result.ViewName, Is.EqualTo("Downloads/SelectFormat"));
        }

        [Test]
        public async Task EstabSearch_PrepareDownload_Step3()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            var guid = Guid.NewGuid();
            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            eds.Setup(x => x.SearchWithDownloadGenerationAsync(It.IsAny<EstablishmentSearchDownloadPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(guid);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel() { Dataset = eDataSet.Full, FileFormat = Edubase.Services.Enums.eFileFormat.XLSX };
            var result = (RedirectToRouteResult) await subject.PrepareDownload(viewModel);

            Assert.That(result.RouteValues["action"], Is.EqualTo("Download"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(guid));
        }
    }
}
