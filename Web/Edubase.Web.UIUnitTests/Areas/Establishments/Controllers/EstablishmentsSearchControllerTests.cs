using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Establishments.Models.Search;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Models.Grid;
using Moq;
using Xunit;
using static Edubase.Web.UI.Areas.Establishments.Models.Search.EstablishmentSearchViewModel;
using GR = Edubase.Services.Enums.eLookupGovernorRole;

namespace Edubase.Web.UI.Areas.Establishments.Controllers.UnitTests
{
    public class EstablishmentsSearchControllerTests
    {
        [Fact]
        public async Task EstabSearch_Index_DisplaysResults()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            context.SetupGet(x => x.Request.IsAuthenticated).Returns(false);
            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new[]
            {
                new LookupDto { Id = 2, Name = "TESTLA" },
                new LookupDto { Id = 3, Name = "BOB" }
            });
            cls.Setup(x => x.EstablishmentStatusesGetAllAsync()).ReturnsAsync(new[]
            {
                new LookupDto { Id = 1, Name = "status1" },
                new LookupDto { Id = 2, Name = "status2" }
            });
            cls.Setup(x => x.GetNameAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync("bob");
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(false));
            ers.Setup(x => x.GetPermittedStatusIdsAsync(It.IsAny<IPrincipal>())).ReturnsAsync(new[] { 1 });
            ers.Setup(x => x.SearchAsync(It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiPagedResult<EstablishmentSearchResultModel>(2, new List<EstablishmentSearchResultModel>
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

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";

            var result = await subject.Index(vm) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(2, vm.Count);
            Assert.Equal(2, vm.Results.Count);
            Assert.Equal("School 1", vm.Results[0].Name);
            Assert.Equal("School 2", vm.Results[1].Name);
        }

        [Fact]
        public async Task EstabSearch_Index_GoToDetailPageOnOneResult_MultipleResults()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            context.SetupGet(x => x.Request.IsAuthenticated).Returns(false);
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

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = true;
            var result = await subject.Index(vm) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(2, vm.Count);
            Assert.Equal(2, vm.Results.Count);
            Assert.Equal("School 1", vm.Results[0].Name);
            Assert.Equal("School 2", vm.Results[1].Name);

            vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = false;
            result = await subject.Index(vm) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(2, vm.Count);
            Assert.Equal(2, vm.Results.Count);
            Assert.Equal("School 1", vm.Results[0].Name);
            Assert.Equal("School 2", vm.Results[1].Name);
        }

        [Fact]
        public async Task EstabSearch_Index_GoToDetailPageOnOneResult_OneResult()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            context.SetupGet(x => x.Request.IsAuthenticated).Returns(false);
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

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = true;
            var result = await subject.Index(vm) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Establishment", result.RouteValues["controller"]);
            Assert.Equal(672393, result.RouteValues["id"]);
            Assert.Equal("Details", result.RouteValues["action"]);
            Assert.Equal("Establishments", result.RouteValues["area"]);

            vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = "school";
            vm.GoToDetailPageOnOneResult = false;
            var result2 = await subject.Index(vm) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(1, vm.Count);
            Assert.Equal(1, vm.Results.Count);
            Assert.Equal("School 1", vm.Results[0].Name);
        }

        [Theory]
        [InlineData(@"432/5437")]
        [InlineData("4325437")]
        public async Task EstabSearch_Index_SearchWithLAESTAB(string laestab)
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            context.SetupGet(x => x.Request.IsAuthenticated).Returns(false);
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

            upr.Setup(x => x.Get(It.IsAny<string>())).Returns(new UserPreference(string.Empty));
            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.Text = laestab;
            var result = await subject.Index(vm) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(eTextSearchType.LAESTAB, vm.TextSearchType);
            Assert.Equal(1, vm.Count);
            Assert.Equal(1, vm.Results.Count);
            Assert.Equal("School 1", vm.Results[0].Name);
            Assert.Equal(432, vm.Results[0].LocalAuthorityId);
            Assert.Equal(5437, vm.Results[0].EstablishmentNumber);
        }

        [Fact]
        public async Task EstabSearch_Index_WithInvalidURN_RedirectsBackOnSelf()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Strict);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();

            mockUrlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns("/Establishments/Search");
            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            context.SetupGet(x => x.Request.IsAuthenticated).Returns(false);
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(false));
            ers.Setup(x => x.SearchAsync(It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<EstablishmentSearchResultModel>(0, new List<EstablishmentSearchResultModel>()));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.AutoSuggestValue = "1256";

            var result = await subject.Index(vm) as RedirectResult;
            Assert.NotNull(result);
            Assert.Equal("action=Index|controller=Search|area=|SearchType=Text|TextSearchModel.Text=|NoResults=True", result.Url);
        }

        [Fact]
        public async Task EstabSearch_Index_WithValidURN_RedirectsToEstabDetail()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Strict);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(null as IPrincipal);
            ers.Setup(x => x.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<bool>(true));

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new EstablishmentSearchViewModel();
            vm.TextSearchModel.AutoSuggestValue = "123456";

            var result = await subject.Index(vm) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Details", result.RouteValues["action"]);
            Assert.Equal("Establishment", result.RouteValues["controller"]);
            Assert.Equal(123456, result.RouteValues["id"]);
            Assert.Equal("Establishments", result.RouteValues["area"]);
        }

        [Fact]
        public async Task EstabSearch_PrepareDownload_Step1_BackOfficeUser()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel();
            var result = await subject.PrepareDownload(viewModel) as ViewResult;

            Assert.NotNull(result);
            Assert.True(viewModel.AllowAnyExtraFields);
            Assert.True(viewModel.AllowIncludeBringUpFields);
            Assert.True(viewModel.AllowIncludeChildrensCentreFields);
            Assert.True(viewModel.AllowIncludeEmailAddresses);
            Assert.True(viewModel.AllowIncludeIEBTFields);
            Assert.Equal("Downloads/SelectDataset", result.ViewName);
        }

        [Fact]
        public async Task EstabSearch_PrepareDownload_Step1_PublicUser()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel();
            var result = await subject.PrepareDownload(viewModel) as ViewResult;

            Assert.NotNull(result);
            Assert.False(viewModel.AllowAnyExtraFields);
            Assert.False(viewModel.AllowIncludeBringUpFields);
            Assert.False(viewModel.AllowIncludeChildrensCentreFields);
            Assert.False(viewModel.AllowIncludeEmailAddresses);
            Assert.False(viewModel.AllowIncludeIEBTFields);
            Assert.Equal("Downloads/SelectDataset", result.ViewName);
        }

        [Fact]
        public async Task EstabSearch_PrepareDownload_Step2()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            eds.Setup(x => x.GetSearchDownloadCustomFields(It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult<IEnumerable<EstablishmentSearchDownloadCustomField>>(new List<EstablishmentSearchDownloadCustomField>()));

            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel() { Dataset = eDataSet.Full };
            var result = await subject.PrepareDownload(viewModel) as ViewResult;

            Assert.NotNull(result);
            Assert.True(viewModel.AllowAnyExtraFields);
            Assert.True(viewModel.AllowIncludeBringUpFields);
            Assert.True(viewModel.AllowIncludeChildrensCentreFields);
            Assert.True(viewModel.AllowIncludeEmailAddresses);
            Assert.True(viewModel.AllowIncludeIEBTFields);
            Assert.Equal("Downloads/SelectFormat", result.ViewName);
        }

        [Fact]
        public async Task EstabSearch_PrepareDownload_Step3()
        {
            var ers = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
            var eds = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Loose);
            var upr = new Mock<IUserPreferenceRepository>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>();
            var principal = new Mock<IPrincipal>();

            eds.Setup(x => x.GetSearchDownloadCustomFields(It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult<IEnumerable<EstablishmentSearchDownloadCustomField>>(new List<EstablishmentSearchDownloadCustomField>()));

            var guid = Guid.NewGuid();
            mockUrlHelper.Setup(x => x.RouteUrl(It.IsAny<RouteValueDictionary>())).Returns<RouteValueDictionary>(n => n.Select(s => string.Format("{0}={1}", s.Key, s.Value)).Aggregate((c, nx) => string.Format("{0}|{1}", c, nx)));
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal.Object);
            principal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            eds.Setup(x => x.SearchWithDownloadGenerationAsync(It.IsAny<EstablishmentSearchDownloadPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(guid);

            var subject = new EstablishmentsSearchController(ers.Object, eds.Object, cls.Object, upr.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);
            subject.Url = mockUrlHelper.Object;
            var viewModel = new EstablishmentSearchDownloadViewModel() { Dataset = eDataSet.Full, FileFormat = Edubase.Services.Enums.eFileFormat.XLSX };
            var result = await subject.PrepareDownload(viewModel) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Download", result.RouteValues["action"]);
            Assert.Equal(guid, result.RouteValues["id"]);
        }

        [Fact]
        public void ExceptionShouldBeThrown_WhenMultipleAppointmentsMatchEstabUrn()
        {
            var dto = new Edubase.Services.Governors.Models.GovernorsDetailsDto
            {
                ApplicableRoles = new List<GR> { GR.Member },
                RoleDisplayPolicies =
                    new Dictionary<GR, GovernorDisplayPolicy>
                    {
                        { GR.Member, new GovernorDisplayPolicy { FullName = true, AppointmentEndDate = true } }
                    },
                CurrentGovernors = new List<GovernorModel>
                {
                    new GovernorModel
                    {
                        Id = 1,
                        RoleId = (int) GR.Member,
                        Appointments = new List<GovernorAppointment>
                        {
                            new GovernorAppointment
                            {
                                EstablishmentUrn = 123,
                                AppointmentStartDate = DateTime.Now.AddYears(-2),
                                AppointmentEndDate = DateTime.Now
                            },
                            new GovernorAppointment
                            {
                                EstablishmentUrn = 123,
                                AppointmentStartDate = DateTime.Now.AddYears(-1),
                                AppointmentEndDate = DateTime.Now.AddMonths(-6)
                            },
                        }
                    }
                }
            };

            var titles = new List<LookupDto>
            {
                new LookupDto { Id = 1, Name = "Mr" }, new LookupDto { Id = 2, Name = "Mrs" }
            };

            var nationalities = new List<LookupDto>
            {
                new LookupDto { Id = 1, Name = "British" }, new LookupDto { Id = 2, Name = "American" }
            };

            var appointingBodies = new List<LookupDto>
            {
                new LookupDto { Id = 1, Name = "Local Authority" }
            };

            var governorPermissions = new GovernorPermissions { Add = true };

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                var viewModel = new GovernorsGridViewModel(
                    dto, true, null, 123,
                    nationalities, appointingBodies,
                    titles, governorPermissions
                );
            });
            Assert.Equal("Multiple appointments found for governor with ID 1 and EstablishmentUrn 123 (governorsGridViewModel)", exception.Message);
        }
    }
}


// giovernorsgridviewmodel
