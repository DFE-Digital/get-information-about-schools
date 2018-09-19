using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Governors.Controllers;
using Edubase.Web.UI.Areas.Governors.Models;
using Moq;
using NUnit.Framework;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class GovernorSearchControllerTest
    {
        [Test]
        public async Task GovernorSearch_Index_ReturnsAllByDefault()
        {
            var gds = new Mock<IGovernorDownloadService>();
            var grs = new Mock<IGovernorsReadService>();
            var cls = new Mock<ICachedLookupService>();
            var gprs = new Mock<IGroupReadService>();
            var ers = new Mock<IEstablishmentReadService>();
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" }));

            grs.Setup(x => x.SearchAsync(It.IsAny<GovernorSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGovernorModel>(2, new List<SearchGovernorModel>
            {
                new SearchGovernorModel{ Person_FirstName="bob" },
                new SearchGovernorModel{ Person_FirstName="jim" }
            }));

            var subject = new GovernorSearchController(gds.Object, grs.Object, cls.Object, gprs.Object, ers.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new GovernorSearchViewModel();
            var result = (ViewResult) await subject.Index(vm);

            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(vm.Count, Is.EqualTo(2));
            Assert.That(vm.Results[0].Person_FirstName, Is.EqualTo("bob"));
            Assert.That(vm.Results[1].Person_FirstName, Is.EqualTo("jim"));
        }

        [Test]
        public async Task GovernorSearch_Index_ProcessesFilters()
        {
            var gds = new Mock<IGovernorDownloadService>();
            var grs = new Mock<IGovernorsReadService>();
            var cls = new Mock<ICachedLookupService>();
            var gprs = new Mock<IGroupReadService>();
            var ers = new Mock<IEstablishmentReadService>();
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(new GenericPrincipal(new GenericIdentity("bob"), new[] { "superhuman" }));

            grs.Setup(x => x.SearchAsync(It.IsAny<GovernorSearchPayload>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGovernorModel>(2, new List<SearchGovernorModel>
            {
                new SearchGovernorModel{ Person_FirstName="bob" }
            }));

            var subject = new GovernorSearchController(gds.Object, grs.Object, cls.Object, gprs.Object, ers.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new GovernorSearchViewModel();
            vm.GovernorSearchModel.Forename = "bob";
            var result = (ViewResult) await subject.Index(vm);
            grs.Verify(x => x.SearchAsync(It.Is<GovernorSearchPayload>(p => p.FirstName == "bob" && p.IncludeHistoric == false), It.IsAny<IPrincipal>()));

            vm = new GovernorSearchViewModel();
            vm.GovernorSearchModel.Surname = "yup";
            result = (ViewResult) await subject.Index(vm);
            grs.Verify(x => x.SearchAsync(It.Is<GovernorSearchPayload>(p => p.LastName == "yup"), It.IsAny<IPrincipal>()));

            vm = new GovernorSearchViewModel();
            vm.GovernorSearchModel.Gid = 1000;
            result = (ViewResult) await subject.Index(vm);
            grs.Verify(x => x.SearchAsync(It.Is<GovernorSearchPayload>(p => p.Gid == "1000"), It.IsAny<IPrincipal>()));

            vm = new GovernorSearchViewModel
            {
                SelectedRoleIds = new List<int> { 1, 2, 3 }
            };
            result = (ViewResult) await subject.Index(vm);
            grs.Verify(x => x.SearchAsync(It.Is<GovernorSearchPayload>(p => p.RoleIds != null & p.RoleIds.Length == 3), It.IsAny<IPrincipal>()));

            vm = new GovernorSearchViewModel();
            vm.GovernorSearchModel.IncludeHistoric = true;
            result = (ViewResult) await subject.Index(vm);
            grs.Verify(x => x.SearchAsync(It.Is<GovernorSearchPayload>(p => p.IncludeHistoric == true), It.IsAny<IPrincipal>()));
        }
    }
}
