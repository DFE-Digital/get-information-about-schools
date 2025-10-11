using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.Search;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Governors.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers.UnitTests
{
    public class GovernorSearchControllerTests
    {
        [Fact()]
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

            grs.Setup(x => x.SearchAsync(It.IsAny<GovernorSearchPayload>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiPagedResult<SearchGovernorModel>(2, new List<SearchGovernorModel>
                    {
                        new SearchGovernorModel{ Person_FirstName="bob" },
                        new SearchGovernorModel{ Person_FirstName="jim" }
                    }));

            var subject = new GovernorSearchController(gds.Object, grs.Object, cls.Object, gprs.Object, ers.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new GovernorSearchViewModel();
            var result = await subject.Index(vm) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(2, vm.Count);
            Assert.Equal("bob", vm.Results[0].Person_FirstName);
            Assert.Equal("jim", vm.Results[1].Person_FirstName);
        }

        [Fact()]
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

            grs.Setup(x => x.SearchAsync(It.IsAny<GovernorSearchPayload>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiPagedResult<SearchGovernorModel>(2, new List<SearchGovernorModel>
                {
                    new SearchGovernorModel{ Person_FirstName="bob" }
                }));

            var subject = new GovernorSearchController(gds.Object, grs.Object, cls.Object, gprs.Object, ers.Object);
            subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), subject);

            var vm = new GovernorSearchViewModel();
            vm.GovernorSearchModel.Forename = "bob";
            var result = (ViewResult) await subject.Index(vm);
            grs.Verify(x => x.SearchAsync(
                It.Is<GovernorSearchPayload>(p => p.FirstName == "bob" && p.IncludeHistoric == false), It.IsAny<IPrincipal>()));

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
