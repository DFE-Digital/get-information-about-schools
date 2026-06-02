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

        private static ToolsController CreateController(
            IEstablishmentReadService readService = null)
        {
            return new ToolsController(
                Mock.Of<ISecurityService>(),
                readService ?? Mock.Of<IEstablishmentReadService>(),
                Mock.Of<IEstablishmentWriteService>(),
                Mock.Of<ICachedLookupService>(),
                Mock.Of<IClientStorage>(),
                Mock.Of<ILocalAuthoritySetRepository>(),
                Mock.Of<IEstablishmentDownloadService>());
        }

        private static IEstablishmentReadService CreateReadServiceReturning(
            EstablishmentModel establishment)
        {
            var mock = new Mock<IEstablishmentReadService>();
            mock.Setup(x => x.GetAsync(123, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ServiceResultDto<EstablishmentModel>(establishment));
            return mock.Object;
        }

        [Fact]
        public async Task BulkAcademies_Search_NullUrn_AddsValidationError()
        {
            var controller = CreateController();
            var model = new BulkAcademiesViewModel { SearchUrn = null };

            await controller.BulkAcademies(model, null, null, "search");

            var errors = controller.ModelState[nameof(model.SearchUrn)].Errors;
            Assert.Single(errors);
            Assert.Equal("Please enter a valid URN", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task BulkAcademies_Search_DuplicateUrn_AddsDuplicateError()
        {
            var controller = CreateController();
            var model = new BulkAcademiesViewModel
            {
                SearchUrn = 123,
                ItemsToAdd = new List<BulkAcademyViewModel>
                {
                    new BulkAcademyViewModel { Urn = 123 }
                }
            };

            await controller.BulkAcademies(model, null, null, "search");

            var errors = controller.ModelState[nameof(model.SearchUrn)].Errors;
            Assert.Single(errors);
            Assert.Equal("URN is a duplicate", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task BulkAcademies_Search_EstablishmentNotFound_AddsValidationError()
        {
            var readService = CreateReadServiceReturning(null);
            var controller = CreateController(readService);

            var model = new BulkAcademiesViewModel { SearchUrn = 123 };

            await controller.BulkAcademies(model, null, null, "search");

            var errors = controller.ModelState[nameof(model.SearchUrn)].Errors;
            Assert.Single(errors);
            Assert.Equal("Please enter a valid URN", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task BulkAcademies_Search_NotAllowedEstablishmentType_AddsValidationError()
        {
            var notAllowedType = ES.LAMaintainedEstablishments
                .Except(ES.AllowedEstablishmentTypeForBulkCreateAcademies)
                .First();

            var establishment = new EstablishmentModel
            {
                Urn = 123,
                Name = "Test School",
                TypeId = notAllowedType
            };

            var controller = CreateController(
                CreateReadServiceReturning(establishment));

            var model = new BulkAcademiesViewModel { SearchUrn = 123 };

            await controller.BulkAcademies(model, null, null, "search");

            var errors = controller.ModelState[nameof(model.SearchUrn)].Errors;
            Assert.Single(errors);
            Assert.Equal("Please enter a valid URN", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task BulkAcademies_Cancel_ClearsModelAndModelState()
        {
            var controller = CreateController();

            var model = new BulkAcademiesViewModel
            {
                SearchUrn = 123,
                FoundItem = new BulkAcademyViewModel { Urn = 123 }
            };

            controller.ModelState.AddModelError("x", "error");

            var result = await controller.BulkAcademies(model, null, null, "cancel") as ViewResult;

            Assert.NotNull(result);
            Assert.Null(model.SearchUrn);
            Assert.Null(model.FoundItem);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task BulkAcademies_RemoveUrn_RemovesItemFromList()
        {
            var controller = CreateController();

            var model = new BulkAcademiesViewModel
            {
                ItemsToAdd = new List<BulkAcademyViewModel>
                {
                    new BulkAcademyViewModel { Urn = 111 },
                    new BulkAcademyViewModel { Urn = 222 }
                }
            };

            await controller.BulkAcademies(model, 111, null, null);

            Assert.Single(model.ItemsToAdd);
            Assert.Equal(222, model.ItemsToAdd[0].Urn);
        }

        [Fact]
        public async Task BulkAcademies_EditUrn_PopulatesFoundItem_AndSetsButtonText()
        {
            var controller = CreateController();

            var model = new BulkAcademiesViewModel
            {
                ItemsToAdd = new List<BulkAcademyViewModel>
                {
                    new BulkAcademyViewModel
                    {
                        Urn = 123,
                        Name = "Test School",
                        EstablishmentTypeId = 1
                    }
                }
            };

            await controller.BulkAcademies(model, null, 123, null);

            Assert.NotNull(model.FoundItem);
            Assert.Equal(123, model.FoundItem.Urn);
            Assert.Equal("Update establishment", controller.ViewBag.ButtonText);
        }

        [Fact]
        public async Task BulkAcademies_AddAction_NoEstablishmentType_AddsValidationError()
        {
            var controller = CreateController();

            var model = new BulkAcademiesViewModel
            {
                FoundItem = new BulkAcademyViewModel
                {
                    Urn = 123,
                    EstablishmentTypeId = null
                }
            };

            await controller.BulkAcademies(model, null, null, "add");

            var errors = controller.ModelState[nameof(model.FilteredItemTypes)].Errors;
            Assert.Single(errors);
            Assert.Equal("Please select an establishment type", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task BulkAcademies_FoundItem_AddsItemToItemsToAdd_AndClearsFoundItem()
        {
            var controller = CreateController();

            var model = new BulkAcademiesViewModel
            {
                FoundItem = new BulkAcademyViewModel
                {
                    Urn = 123,
                    Name = "Test School",
                    EstablishmentTypeId = 1
                },
                ItemsToAdd = new List<BulkAcademyViewModel>
                {
                    new BulkAcademyViewModel { Urn = 123, Name = "Old name" }
                }
            };

            await controller.BulkAcademies(model, null, null, null);

            Assert.Null(model.FoundItem);
            Assert.Single(model.ItemsToAdd);
            Assert.Equal("Test School", model.ItemsToAdd[0].Name);
        }
    }
}
