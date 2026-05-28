using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Data.Repositories;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Models.Tools;
using Moq;
using Xunit;
using ES = Edubase.Services.Enums.EnumSets;

namespace Edubase.Web.UIUnitTests.Controllers
{
    public class ToolsControllerTest
    {
        private readonly ToolsController controller;
        private readonly Mock<IEstablishmentReadService> mockEstablishmentReadService = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
        private readonly Mock<IEstablishmentWriteService> mockEstablishmentWriteService = new Mock<IEstablishmentWriteService>(MockBehavior.Strict);
        private readonly Mock<IEstablishmentDownloadService> mockEstablishmentDownloadService = new Mock<IEstablishmentDownloadService>(MockBehavior.Strict);

        public ToolsControllerTest()
        {
            controller = new ToolsController(
                Mock.Of<ISecurityService>(),
                mockEstablishmentReadService.Object,
                mockEstablishmentWriteService.Object,
                Mock.Of<ICachedLookupService>(),
                Mock.Of<IClientStorage>(),
                Mock.Of<ILocalAuthoritySetRepository>(),
                mockEstablishmentDownloadService.Object);
        }

        [Fact]
        public async Task BulkAcademies_Convert_AllowedType_Success()
        {
            var allowedType = ES.AllowedEstablishmentTypeForBulkCreateAcademies.First();

            var est = new EstablishmentModel
            {
                Urn = 999,
                Name = "Valid School",
                TypeId = allowedType
            };

            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
               .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(est));

            var model = CreateBulkAcademiesViewModel();

            var result = await controller.BulkAcademies(model, null, null, "search") as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(nameof(model.SearchUrn)));
            Assert.True(controller.ModelState.Values.First().Errors.Count > 0);
        }

        [Fact]
        public async Task BulkAcademies_Convert_Not_AllowedType_Failure()
        {
            var notAllowedType = ES.LAMaintainedEstablishments.Except(ES.AllowedEstablishmentTypeForBulkCreateAcademies).First();

            var est = new EstablishmentModel
            {
                Urn = 999,
                Name = "Valid School",
                TypeId = notAllowedType
            };

            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
               .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(est));

            var model = CreateBulkAcademiesViewModel();

            var result = await controller.BulkAcademies(model, null, null, "search") as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(nameof(model.SearchUrn)));
            Assert.True(controller.ModelState.Values.First().Errors.Count > 0);
            Assert.Equal("Please enter a valid URN", controller.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Fact]
        public async Task BulkAcademies_Convert_Duplicate_Urn_Failure()
        {
            var allowedType = ES.AllowedEstablishmentTypeForBulkCreateAcademies.First();

            var est = new EstablishmentModel
            {
                Urn = 999,
                Name = "Valid School",
                TypeId = allowedType
            };

            mockEstablishmentReadService.Setup(e => e.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
               .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(est));

            var model = CreateBulkAcademiesViewModelWithItems();

            var result = await controller.BulkAcademies(model, null, null, "search") as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(nameof(model.SearchUrn)));
            Assert.True(controller.ModelState.Values.First().Errors.Count > 0);
            Assert.Equal("URN is a duplicate", controller.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        private BulkAcademiesViewModel CreateBulkAcademiesViewModel()
        {
            var model = new BulkAcademiesViewModel
            {
                SearchUrn = 123,
            };

            return model;
        }

        private BulkAcademiesViewModel CreateBulkAcademiesViewModelWithItems()
        {
            var model = new BulkAcademiesViewModel
            {
                SearchUrn = 123,
                ItemsToAdd = new List<BulkAcademyViewModel>
                {
                    new BulkAcademyViewModel { Urn = 123 }
                }
            };

            return model;
        }
    }

}
