//using System.Collections.Generic;
//using System.Security.Principal;
//using System.Threading.Tasks;
//using System.Web;
//using Edubase.Services.Domain;
//using Edubase.Services.Groups;
//using Edubase.Services.Groups.Downloads;
//using Edubase.Services.Groups.Models;
//using Edubase.Services.Groups.Search;
//using Edubase.Services.Lookup;
//using Edubase.Web.UI.Areas.Groups.Models;
//using Moq;
//using Xunit;
//using Microsoft.AspNetCore.Mvc;

//namespace Edubase.Web.UI.Areas.Groups.Controllers.UnitTests
//{
//    public class GroupSearchControllerTests
//    {
//        [Fact]
//        public async Task GroupSearch_Index_ReturnsAllGroupsByDefault()
//        {
//            var grs = new Mock<IGroupReadService>();
//            var gds = new Mock<IGroupDownloadService>();
//            var cls = new Mock<ICachedLookupService>();
//            var request = new Mock<HttpRequestBase>();
//            var context = new Mock<HttpContextBase>();

//            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
//            context.SetupGet(x => x.Request).Returns(request.Object);
//            context.SetupGet(x => x.User).Returns(new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" }));

//            grs.Setup(x => x.SearchAsync(It.IsAny<GroupSearchPayload>(), It.IsAny<IPrincipal>()))
//                .ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(2, new List<SearchGroupDocument>()
//                {
//                    new SearchGroupDocument{ Name="Group 1" },
//                    new SearchGroupDocument{ Name="Group 2" }
//                }));

//            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);
//            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

//            var vm = new GroupSearchViewModel();
//            var result = await subject.Index(vm) as ViewResult;

//            Assert.NotNull(result);
//            Assert.Equal("GroupResults", result.ViewName);
//        }

//        [Fact()]
//        public async Task GroupSearch_Index_OneResultRedirectsToDetailPage()
//        {
//            var grs = new Mock<IGroupReadService>();
//            var gds = new Mock<IGroupDownloadService>();
//            var cls = new Mock<ICachedLookupService>();
//            var request = new Mock<HttpRequestBase>();
//            var context = new Mock<HttpContextBase>();

//            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
//            context.SetupGet(x => x.Request).Returns(request.Object);
//            context.SetupGet(x => x.User).Returns(new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" }));

//            grs.Setup(x => x.SearchAsync(It.IsAny<GroupSearchPayload>(),  It.IsAny<IPrincipal>()))
//                .ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>()
//                {
//                    new SearchGroupDocument{ Name="Group 1", GroupUId = 123 }
//                }));

//            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);
//            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

//            var vm = new GroupSearchViewModel();
//            var result = await subject.Index(vm) as RedirectToRouteResult;

//            Assert.NotNull(result);
//            Assert.Equal("Details", result.RouteValues["action"]);
//            Assert.Equal("Group", result.RouteValues["controller"]);
//            Assert.Equal(123, result.RouteValues["id"]);
//            Assert.Equal("Groups", result.RouteValues["area"]);
//        }

//        [Fact()]
//        public async Task GroupSearch_Index_SearchByIdRedirectsToDetailPage()
//        {
//            var grs = new Mock<IGroupReadService>(MockBehavior.Strict);
//            var gds = new Mock<IGroupDownloadService>();
//            var cls = new Mock<ICachedLookupService>();
//            var request = new Mock<HttpRequestBase>();
//            var context = new Mock<HttpContextBase>();

//            var principal = new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" });
//            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
//            context.SetupGet(x => x.Request).Returns(request.Object);
//            context.SetupGet(x => x.User).Returns(principal);

//            grs.Setup(x => x.SearchByIdsAsync(It.IsAny<string>(),
//                It.Is<int?>(i => i == 1000),
//                It.IsAny<string>(),
//                It.IsAny<int?>(),
//                It.Is<IPrincipal>(p => p == principal)))
//                .ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>()
//                {
//                    new SearchGroupDocument{ Name="Group 1000", GroupUId = 1000 }
//                }));

//            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);
//            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

//            var vm = new GroupSearchViewModel();
//            vm.GroupSearchModel.Text = "1000";
//            var result = await subject.Index(vm) as RedirectToRouteResult;

//            Assert.NotNull(result);
//            Assert.Equal("Details", result.RouteValues["action"]);
//            Assert.Equal("Group", result.RouteValues["controller"]);
//            Assert.Equal(1000, result.RouteValues["id"]);
//            Assert.Equal("Groups", result.RouteValues["area"]);
//        }

//        [Fact()]
//        public async Task GroupSearch_Index_AutosuggestRedirectsToDetailPage()
//        {
//            var grs = new Mock<IGroupReadService>(MockBehavior.Strict);
//            var gds = new Mock<IGroupDownloadService>(MockBehavior.Strict);
//            var cls = new Mock<ICachedLookupService>(MockBehavior.Strict);

//            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);

//            var vm = new GroupSearchViewModel();
//            vm.GroupSearchModel.AutoSuggestValue = "1000";
//            var result = await subject.Index(vm) as RedirectToRouteResult;

//            Assert.NotNull(result);
//            Assert.Equal("Details", result.RouteValues["action"]);
//            Assert.Equal("Group", result.RouteValues["controller"]);
//            Assert.Equal(1000, result.RouteValues["id"]);
//            Assert.Equal("Groups", result.RouteValues["area"]);
//        }
//    }
//}
