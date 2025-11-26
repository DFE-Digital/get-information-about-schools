using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Edubase.Data.Entity;
using Edubase.Services.DataQuality;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Models.DataQuality;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using static Edubase.Data.Entity.DataQualityStatus;

namespace Edubase.Web.UIUnitTests.Controllers
{
    public class DataQualityControllerTests
    {
        private readonly Mock<IDataQualityWriteService> _writeServiceMock = new Mock<IDataQualityWriteService>();
        private readonly Mock<IPrincipal> mockPrincipal = new Mock<IPrincipal>();
        private readonly Mock<IIdentity> mockIdentity = new Mock<IIdentity>();
        private readonly Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
        private readonly Mock<HttpContext> mockHttpContextBase = new Mock<HttpContext>();
        private readonly DataQualityController _controllerUnderTest;

        public DataQualityControllerTests()
        {
            mockHttpContextBase.SetupGet(x => x.User)
                .Returns(mockPrincipal.Object);
            mockControllerContext.SetupGet(x => x.HttpContext)
                .Returns(mockHttpContextBase.Object);
            mockPrincipal.SetupGet(x => x.Identity)
                .Returns(mockIdentity.Object);
            _controllerUnderTest = new DataQualityController(_writeServiceMock.Object)
            {
                ControllerContext = mockControllerContext.Object
            };
        }

        [Theory]
        [InlineData((int) DataQualityEstablishmentType.AcademyOpeners)]
        [InlineData((int) DataQualityEstablishmentType.FreeSchoolOpeners)]
        [InlineData((int) DataQualityEstablishmentType.OpenAcademiesAndFreeSchools)]
        [InlineData((int) DataQualityEstablishmentType.LaMaintainedSchools)]
        [InlineData((int) DataQualityEstablishmentType.IndependentSchools)]
        [InlineData((int) DataQualityEstablishmentType.PupilReferralUnits)]
        [InlineData((int) DataQualityEstablishmentType.AcademySecure16to19Openers)]
        public async Task ViewStatus_ReturnsPopulatedViewItems(int establishmentType)
        {
            var dataQualityStatus = new DataQualityStatus()
            {
                EstablishmentType = (DataQualityEstablishmentType) establishmentType,
                DataOwner = "Test data owner",
                Email = "Test email"                
            };
            _writeServiceMock.Setup(x => x.GetDataQualityStatus())
                .ReturnsAsync(new List<DataQualityStatus>() { dataQualityStatus });

            var result = await _controllerUnderTest.ViewStatus(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewmodel = Assert.IsType<FullDataQualityStatusViewModel>(viewResult.Model);
            var dataQualityStatusItemReturned = Assert.Single(viewmodel.Items);
            Assert.Equal(dataQualityStatus.EstablishmentType, dataQualityStatusItemReturned.EstablishmentType);
            Assert.Equal(dataQualityStatus.DataOwner, dataQualityStatusItemReturned.DataOwner);
            Assert.Equal(dataQualityStatus.Email, dataQualityStatusItemReturned.Email);
        }
    }
}
