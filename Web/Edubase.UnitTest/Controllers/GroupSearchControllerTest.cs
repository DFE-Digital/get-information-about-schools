using Edubase.Services.Domain;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Groups.Controllers;
using Edubase.Web.UI.Areas.Groups.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class GroupSearchControllerTest
    {
        [Test]
        public async Task GroupSearch_Index_ReturnsAllGroupsByDefault()
        {
            var grs = new Mock<IGroupReadService>();
            var gds = new Mock<IGroupDownloadService>();
            var cls = new Mock<ICachedLookupService>();
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" }));

            grs.Setup(x => x.SearchAsync(It.IsAny<GroupSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(2, new List<SearchGroupDocument>(){
                new SearchGroupDocument{ Name="Group 1" },
                new SearchGroupDocument{ Name="Group 2" }
            }));

            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new GroupSearchViewModel();
            var result = (ViewResult) await subject.Index(vm);

            Assert.That(result.ViewName, Is.EqualTo("GroupResults"));
        }

        [Test]
        public async Task GroupSearch_Index_OneResultRedirectsToDetailPage()
        {
            var grs = new Mock<IGroupReadService>();
            var gds = new Mock<IGroupDownloadService>();
            var cls = new Mock<ICachedLookupService>();
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" }));

            grs.Setup(x => x.SearchAsync(It.IsAny<GroupSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>(){
                new SearchGroupDocument{ Name="Group 1", GroupUId = 123 }
            }));

            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new GroupSearchViewModel();
            var result = (RedirectToRouteResult)await subject.Index(vm);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Group"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123));
            Assert.That(result.RouteValues["area"], Is.EqualTo("Groups"));
        }


        [Test]
        public async Task GroupSearch_Index_SearchByIdRedirectsToDetailPage()
        {
            var grs = new Mock<IGroupReadService>(MockBehavior.Strict);
            var gds = new Mock<IGroupDownloadService>();
            var cls = new Mock<ICachedLookupService>();
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();

            var principal = new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" });
            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(principal);

            grs.Setup(x => x.SearchByIdsAsync(It.IsAny<string>(), It.Is<int?>(i => i == 1000), It.IsAny<string>(), It.Is<IPrincipal>(p => p == principal))).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>(){
                new SearchGroupDocument{ Name="Group 1000", GroupUId = 1000 }
            }));

            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new GroupSearchViewModel();
            vm.GroupSearchModel.Text = "1000";
            var result = (RedirectToRouteResult)await subject.Index(vm);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Group"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(1000));
            Assert.That(result.RouteValues["area"], Is.EqualTo("Groups"));
        }

        [Test]
        public async Task GroupSearch_Index_AutosuggestRedirectsToDetailPage()
        {
            var grs = new Mock<IGroupReadService>(MockBehavior.Strict);
            var gds = new Mock<IGroupDownloadService>(MockBehavior.Strict);
            var cls = new Mock<ICachedLookupService>(MockBehavior.Strict);
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            
            var subject = new GroupSearchController(grs.Object, gds.Object, cls.Object);
            
            var vm = new GroupSearchViewModel();
            vm.GroupSearchModel.AutoSuggestValue = "1000";
            var result = (RedirectToRouteResult)await subject.Index(vm);

            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Group"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(1000));
            Assert.That(result.RouteValues["area"], Is.EqualTo("Groups"));
        }


    }

}
