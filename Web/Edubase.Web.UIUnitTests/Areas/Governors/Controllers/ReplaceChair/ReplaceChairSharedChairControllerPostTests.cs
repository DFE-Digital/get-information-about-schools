using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Models;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.ReplaceChair
{
    public class ReplaceChairSharedChairControllerPostTests : GovernorControllerTestBase
    {
        [Fact]
        public async Task Gov_ReplaceChair_Post_Shared_AddsAppointment_AndClosesOld()
        {
            var estabUrn = 2002;
            var newGovId = 4002;
            var oldGovId = 4001;

            mockGovernorsWriteService.Setup(s =>
                s.AddSharedGovernorAppointmentAsync(newGovId, estabUrn,
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(newGovId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateSharedChair(newGovId, estabUrn));

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(oldGovId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateExistingChair(oldGovId));

            mockGovernorsWriteService.Setup(s =>
                s.UpdateDatesAsync(oldGovId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            mockGovernorsReadService.Setup(s =>
                s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            mockGovernorsReadService.Setup(s =>
                s.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsReadService.Setup(s =>
                s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    ApplicableRoles = new List<eLookupGovernorRole>
                    {
                        eLookupGovernorRole.ChairOfLocalGoverningBody,
                        eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
                    },
                    CurrentGovernors = new List<GovernorModel>(),
                    HistoricalGovernors = new List<GovernorModel>(),
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                        { eLookupGovernorRole.ChairOfLocalGoverningBody, new GovernorDisplayPolicy() },
                        { eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, new GovernorDisplayPolicy() }
                    }
                });

            SetupCommonLookupMocks();

            var controller = BuildController();

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = oldGovId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.SharedChair,
                SharedGovernors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = newGovId,
                        AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddYears(1))
                    }
                },
                DateTermEnds = new DateTimeViewModel(DateTime.Today)
            };

            model.SelectedGovernorId = newGovId;

            // Act
            var result = await controller.ReplaceChair(model);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Contains("#school-governance", redirect.Url);

            mockGovernorsWriteService.Verify(s =>
                s.AddSharedGovernorAppointmentAsync(newGovId, estabUrn,
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()),
                Times.Once);

            mockGovernorsWriteService.Verify(s =>
                s.UpdateDatesAsync(oldGovId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()),
                Times.Once);
        }

        [Fact()]
        public async Task Gov_Post_ReplaceChair_NewChairShared()
        {
            var controller = BuildController();
            var govId = 465134;
            var newGovId = 68543;
            var estabUrn = 16802;

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = govId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.SharedChair,
                SharedGovernors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = newGovId,
                        AppointmentEndDate = new DateTimeViewModel(DateTime.Now.AddYears(1))
                    }
                },
                SelectedGovernorId = newGovId,
                DateTermEnds = new DateTimeViewModel(DateTime.Today),
            };

            mockGovernorsWriteService
                .Setup(g => g.AddSharedGovernorAppointmentAsync(newGovId, estabUrn, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(true));

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(newGovId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateSharedChair(newGovId, estabUrn));

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(govId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateExistingChair(govId));

            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            var result = await controller.ReplaceChair(model);
            var redirectResult = result as RedirectResult;
            Assert.NotNull(redirectResult);
            Assert.Contains("#school-governance", redirectResult.Url);

            mockGovernorsWriteService
                .Verify(g => g.AddSharedGovernorAppointmentAsync(newGovId, estabUrn, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact()]
        public async Task Gov_Post_ReplaceChair_NewChairNotShared_NoValidationErrors()
        {
            var controller = BuildController();
            var govId = 465134;
            var estabUrn = 16802;

            var termEnds = DateTime.Today.AddDays(10);
            var expectedStartDate = termEnds.AddDays(1);

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = govId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                NewLocalGovernor = new CreateEditGovernorViewModel
                {
                    AppointmentEndDate = new DateTimeViewModel(termEnds.AddYears(1)),
                    DOB = new DateTimeViewModel(),
                },
                DateTermEnds = new DateTimeViewModel(termEnds),
            };

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ValidationEnvelopeDto { Errors = new List<ApiError>() });

            mockGovernorsWriteService.Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(),
                It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<int>(true));

            mockGovernorsWriteService
                .Setup(a => a.SaveAsync(It.Is<GovernorModel>(g => g.RoleId == (int) eLookupGovernorRole.LocalGovernor),
                        It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true));

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(govId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateExistingChair(govId));

            var result = await controller.ReplaceChair(model);
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Contains("#school-governance", redirectResult.Url);
        }

        [Fact()]
        public async Task Gov_Put_ReplaceChair_NewChairNotShared_ValidationErrors()
        {
            var controller = BuildController();
            var govId = 465134;
            var estabUrn = 16802;
            var errorKey = "Test";
            var errorText = "Test Message";

            var existingGov = new GovernorModel
            {
                RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody
            };

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = govId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                NewLocalGovernor = new CreateEditGovernorViewModel
                {
                    AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddYears(1)),
                    DOB = new DateTimeViewModel(),
                },
                DateTermEnds = new DateTimeViewModel(DateTime.Today),
            };

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ValidationEnvelopeDto
                {
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Fields = errorKey,
                            Message = errorText
                        }
                    }
                });

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(model.ExistingGovernorId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => existingGov);

            mockGovernorsReadService
                .Setup(g => g.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties
                    (It.IsAny<ReplaceChairViewModel>(),
                        estabUrn,
                        null,
                        It.IsAny<IPrincipal>(),
                        It.IsAny<Action<EstablishmentModel>>(),
                        It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            base.SetupCommonLookupMocks();

            var result = await controller.ReplaceChair(model);
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var modelResult = viewResult.Model as ReplaceChairViewModel;
            Assert.NotNull(modelResult);
            Assert.Equal(model, modelResult);
        }

        [Fact]
        public async Task ReplaceChair_Post_SharedChair_SelectedIdLessOrEqualZero_AddsModelErrorAndReturnsView()
        {
            // Arrange
            var controller = BuildController();
            var estabUrn = 5555;
            var existingChairId = 123;

            var vm = new ReplaceChairViewModel
            {
                ExistingGovernorId = existingChairId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.SharedChair,
                SharedGovernors = new List<SharedGovernorViewModel>(),
                SelectedGovernorId = 0,
                DateTermEnds = new DateTimeViewModel(DateTime.Now)
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateSharedChair(existingChairId, estabUrn));

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor },
                    CurrentGovernors = new List<GovernorModel>(),
                    HistoricalGovernors = new List<GovernorModel>(),
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                        { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            base.SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.ReplaceChair(vm);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var returnedVm = Assert.IsType<ReplaceChairViewModel>(view.Model);

            Assert.False(view.ViewData.ModelState.IsValid);
            Assert.True(view.ViewData.ModelState.ContainsKey("SharedGovernors"));

            var errors = view.ViewData.ModelState["SharedGovernors"].Errors;
            Assert.Single(errors);
            Assert.Equal("Please select a shared chair.", errors[0].ErrorMessage);

            // Ensure that no write operations occurred
            mockGovernorsWriteService.Verify(
                w => w.AddSharedGovernorAppointmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<IPrincipal>()),
                Times.Never);

            mockGovernorsWriteService.Verify(
                w => w.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()),
                Times.Never);
        }

        [Fact]
        public async Task ReplaceChair_Post_SharedChair_SelectedIdNotFound_AddsModelErrorAndReturnsView()
        {
            // Arrange
            var controller = BuildController();
            var estabUrn = 5001;
            var existingChairId = 999;

            var selectedId = 1234;

            var vm = new ReplaceChairViewModel
            {
                ExistingGovernorId = existingChairId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.SharedChair,
                SharedGovernors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel { Id = 9999 }
                },
                SelectedGovernorId = selectedId,
                DateTermEnds = new DateTimeViewModel(DateTime.Today)
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateSharedChair(existingChairId, estabUrn));

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(selectedId, It.IsAny<IPrincipal>()))
                .ReturnsAsync((GovernorModel) null);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor },
                    CurrentGovernors = new List<GovernorModel>(),
                    HistoricalGovernors = new List<GovernorModel>(),
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                        { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            base.SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.ReplaceChair(vm);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var returned = Assert.IsType<ReplaceChairViewModel>(view.Model);

            Assert.False(view.ViewData.ModelState.IsValid);
            Assert.True(view.ViewData.ModelState.ContainsKey("SharedGovernors"));

            var errors = view.ViewData.ModelState["SharedGovernors"].Errors;
            Assert.Single(errors);
            Assert.Equal("The selected chair could not be found.", errors[0].ErrorMessage);

            // Ensure no write operations were attempted
            mockGovernorsWriteService.Verify(
                w => w.AddSharedGovernorAppointmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<IPrincipal>()),
                Times.Never);

            mockGovernorsWriteService.Verify(
                w => w.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()),
                Times.Never);
        }

        [Fact]
        public async Task ReplaceChair_Post_SharedChair_SelectedGovernorHasInvalidRole_AddsModelErrorAndReturnsView()
        {
            // Arrange
            var controller = BuildController();
            var estabUrn = 6002;
            var existingChairId = 200;
            var selectedId = 999;

            var vm = new ReplaceChairViewModel
            {
                ExistingGovernorId = existingChairId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.SharedChair,
                SharedGovernors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = selectedId,
                        AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddYears(1))
                    }
                },
                DateTermEnds = new DateTimeViewModel(DateTime.Today)
            };

            vm.SelectedGovernorId = selectedId;

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateSharedChair(existingChairId, estabUrn));

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(selectedId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = selectedId,
                    RoleId = null
                });

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor },
                    CurrentGovernors = new List<GovernorModel>(),
                    HistoricalGovernors = new List<GovernorModel>(),
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                        { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            base.SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.ReplaceChair(vm);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var returned = Assert.IsType<ReplaceChairViewModel>(view.Model);

            Assert.False(view.ViewData.ModelState.IsValid);
            Assert.True(view.ViewData.ModelState.ContainsKey(""));

            var errors = view.ViewData.ModelState[""].Errors;
            Assert.Single(errors);
            Assert.Equal("The selected shared governor has an invalid or missing role.", errors[0].ErrorMessage);

            // Ensure no write operations occur
            mockGovernorsWriteService.Verify(
                w => w.AddSharedGovernorAppointmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<IPrincipal>()),
                Times.Never);

            mockGovernorsWriteService.Verify(
                w => w.UpdateDatesAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<IPrincipal>()),
                Times.Never);
        }
    }
}
