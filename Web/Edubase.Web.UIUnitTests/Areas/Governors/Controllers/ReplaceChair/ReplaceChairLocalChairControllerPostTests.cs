using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ReplaceChairLocalChairControllerPostTests : GovernorControllerTestBase
    {
        [Fact]
        public async Task ReplaceChair_Post_LocalChair_Reinstate_InvalidPreviousRole_AddsModelError_ThenRedirects()
        {
            // Arrange
            var estabUrn = 7001;
            var existingChairId = 444;
            var today = DateTime.Today;

            var vm = new ReplaceChairViewModel
            {
                ExistingGovernorId = existingChairId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                Reinstate = true,

                DateTermEnds = new DateTimeViewModel
                {
                    Day = today.Day,
                    Month = today.Month,
                    Year = today.Year
                },

                NewLocalGovernor = new CreateEditGovernorViewModel(),
                SelectedPreviousExistingNonChairId = null
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateExistingChair(existingChairId));

            mockGovernorsReadService
                .SetupSequence(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = today
                })
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = null,
                    AppointmentEndDate = today
                });

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

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

            SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(), estabUrn, null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockGovernorsWriteService
                .Setup(s => s.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(s => s.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true));

            mockGovernorsWriteService
                .Setup(s => s.UpdateDatesAsync(existingChairId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            // Act
            var controller = BuildController();
            var result = await controller.ReplaceChair(vm);

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Contains("#school-governance", redirect.Url);

            Assert.True(controller.ViewData.ModelState.ContainsKey(""));
            var error = controller.ViewData.ModelState[""].Errors.Single().ErrorMessage;
            Assert.Equal("Could not determine a valid local role for reinstatement.", error);
        }

        [Fact]
        public async Task ReplaceChair_Post_LocalChair_Reinstate_ValidPreviousRole_CallsReInstateAsGovernor()
        {
            // Arrange
            var estabUrn = 7001;
            var existingChairId = 444;
            var today = DateTime.Today;

            var vm = new ReplaceChairViewModel
            {
                ExistingGovernorId = existingChairId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                Reinstate = true,

                DateTermEnds = new DateTimeViewModel
                {
                    Day = today.Day,
                    Month = today.Month,
                    Year = today.Year
                },

                NewLocalGovernor = new CreateEditGovernorViewModel(),

                SelectedPreviousExistingNonChairId = null
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateExistingChair(existingChairId));

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    ApplicableRoles = new List<eLookupGovernorRole>
                    {
                        eLookupGovernorRole.LocalGovernor
                    },
                    CurrentGovernors = new List<GovernorModel>(),
                    HistoricalGovernors = new List<GovernorModel>(),
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                        { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockGovernorsWriteService
                .Setup(s => s.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(s => s.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true));

            mockGovernorsWriteService
                .Setup(s => s.UpdateDatesAsync(existingChairId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            mockGovernorsWriteService
                .Setup(s => s.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true))
                .Verifiable();

            // Act
            var controller = BuildController();
            var result = await controller.ReplaceChair(vm);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);

            // Verify the reinstate method executed the “oldRole → newRole → Save” path
            mockGovernorsWriteService.Verify(
                s => s.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task ReplaceChair_Post_LocalChair_BuildsExistingNonChairsList()
        {
            // Arrange
            var estabUrn = 7001;
            var existingChairId = 444;
            var today = DateTime.Today;

            var vm = new ReplaceChairViewModel
            {
                ExistingGovernorId = existingChairId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                Reinstate = false,

                DateTermEnds = new DateTimeViewModel
                {
                    Day = today.Day,
                    Month = today.Month,
                    Year = today.Year
                },

                SelectedPreviousExistingNonChairId = 20,
                NewLocalGovernor = new CreateEditGovernorViewModel()
            };

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateExistingChair(existingChairId));

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel>
                    {
                        new GovernorModel { Id = 10, Person_FirstName = "Alice", Person_LastName = "Smith", RoleId = (int)eLookupGovernorRole.LocalGovernor },
                        new GovernorModel { Id = 20, Person_FirstName = "Bob",   Person_LastName = "Jones", RoleId = (int)eLookupGovernorRole.LocalGovernor },
                        new GovernorModel { Id = 30, Person_FirstName = "Carol", Person_LastName = "Brown", RoleId = (int)eLookupGovernorRole.LocalGovernor }
                    },
                    ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor },
                    HistoricalGovernors = new List<GovernorModel>(),
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                        { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockGovernorsWriteService
                .Setup(s => s.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto
                {
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Message = "force view",
                            Fields = "Dummy"
                        }
                    }
                });

            // Act
            var controller = BuildController();
            var result = await controller.ReplaceChair(vm);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var returned = Assert.IsType<ReplaceChairViewModel>(view.Model);

            var items = returned.ExistingNonChairs.ToList();

            Assert.Equal(3, items.Count);

            Assert.Equal("Carol Brown", items[0].Text);
            Assert.Equal("30", items[0].Value);
            Assert.False(items[0].Selected);

            Assert.Equal("Bob Jones", items[1].Text);
            Assert.Equal("20", items[1].Value);
            Assert.True(items[1].Selected);

            Assert.Equal("Alice Smith", items[2].Text);
            Assert.Equal("10", items[2].Value);
            Assert.False(items[2].Selected);
        }
    }
}
