using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Edubase.Data.Entity;
using Edubase.Services.DataQuality;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.DataQuality;
using Moq;
using Xunit;
using static Edubase.Data.Entity.DataQualityStatus;

namespace Edubase.Web.UIUnitTests.Controllers
{
    public class DataQualityControllerTests
    {
        private readonly Mock<IDataQualityWriteService> _writeServiceMock = new Mock<IDataQualityWriteService>();
        private readonly DataQualityController _controllerUnderTest;

        public DataQualityControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["DataQualityUpdatePeriod"] = "7"
                })
                .Build();

            _controllerUnderTest = new DataQualityController(_writeServiceMock.Object, configuration)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
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
