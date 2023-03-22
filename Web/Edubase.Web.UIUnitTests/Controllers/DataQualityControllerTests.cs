using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Data.Entity;
using Edubase.Services.DataQuality;
using Edubase.Web.UI.Models.DataQuality;
using Moq;
using Xunit;

namespace Edubase.Web.UI.Controllers.UnitTests
{
    public class DataQualityControllerTests
    {
        [Fact]
        public async Task Test()
        {
            var mockDataQualityReadService = new Mock<IDataQualityReadService>();
            var mockDataQualityWriteService = new Mock<IDataQualityWriteService>();
            var mockUser = new Mock<IPrincipal>();

            mockDataQualityWriteService.Setup(x => x.GetDataQualityStatus())
                .ReturnsAsync(new List<DataQualityStatus>() { });

            mockUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var dataQualityController = new DataQualityController(mockDataQualityWriteService.Object);


            var request = new SimpleWorkerRequest("/dummy", @"c:\dummy", "dummy.html", null, new StringWriter());
            var context = new HttpContext(request);

            dataQualityController.ControllerContext = new ControllerContext
            {
                HttpContext = new HttpContextWrapper(context),
            };

            dataQualityController.HttpContext.User = mockUser.Object;

            var status = await dataQualityController.Status();
            Assert.NotNull(status);
        }


        [Fact]
        public async Task Test2()
        {
            var mockDataQualityReadService = new Mock<IDataQualityReadService>();
            var mockDataQualityWriteService = new Mock<IDataQualityWriteService>();

            mockDataQualityWriteService.Setup(x => x.GetDataQualityStatus())
                .ReturnsAsync(new List<DataQualityStatus>() { });

            var mockUser = new Mock<IPrincipal>();
            mockUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockUser.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            var dataQualityController = new DataQualityController(mockDataQualityWriteService.Object);


            var request = new SimpleWorkerRequest("", "", "", null, new StringWriter());
            var context = new HttpContext(request);

            dataQualityController.ControllerContext = new ControllerContext
            {
                HttpContext = new HttpContextWrapper(context),
            };

            dataQualityController.HttpContext.User = mockUser.Object;

            var status = await dataQualityController.Status();
            Assert.NotNull(status);
        }

        [Fact]
        public async Task Test3()
        {

            var mockDataQualityReadService = new Mock<IDataQualityReadService>(MockBehavior.Strict);
            var mockDataQualityWriteService = new Mock<IDataQualityWriteService>(MockBehavior.Strict);

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            var mockUrlHelper = new Mock<UrlHelper>(MockBehavior.Strict);

            var mockUser = new Mock<IPrincipal>(MockBehavior.Strict);
            mockUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockUser.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            request.SetupGet(x => x.QueryString).Returns(HttpUtility.ParseQueryString(string.Empty));
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.User).Returns(mockUser.Object);
            context.SetupGet(x => x.Request.IsAuthenticated).Returns(false);



            mockDataQualityWriteService.Setup(x => x.GetDataQualityStatus())
                .ReturnsAsync(new List<DataQualityStatus>() { });

            var dataQualityController = new DataQualityController(mockDataQualityWriteService.Object);
            dataQualityController.ControllerContext = new ControllerContext(context.Object, new RouteData(), dataQualityController);
            dataQualityController.Url = mockUrlHelper.Object;


            // Act
            var result = await dataQualityController.Status() as ViewResult;



            // Assert
            Assert.NotNull(result);
            var viewModel = Assert.IsType<DataQualityStatusViewModel>(result.Model); // FIXME: This assertion should fail, where the user is not logged in and does not have permissions -- cannot currently explain / make fail

            Assert.False(viewModel.Urgent);
            Assert.Empty(viewModel.Items);
        }
    }
}
