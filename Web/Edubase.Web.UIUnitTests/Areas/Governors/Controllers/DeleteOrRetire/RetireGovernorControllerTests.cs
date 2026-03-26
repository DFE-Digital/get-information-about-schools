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
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Models;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.DeleteOrRetire
{
    public class RetireGovernorControllerTests : GovernorControllerTestBase
    {
        [Fact]
        public async Task DeleteOrRetireGovernor_NoGovernorSelected_ThrowsInvalidParameterException()
        {
            var controller = BuildController();
            await Assert.ThrowsAsync<InvalidParameterException>(() => controller.DeleteOrRetireGovernor(new GovernorsGridViewModel()));
        }

        [Fact]
        public async Task Gov_DeleteOrRetireGovernor_Save_ApiError()
        {
            // Arrange
            var groupId = 2436;
            var governorId = 6224;
            var errorKey = "DummyError";
            var errorMessage = "Test Error";

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

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);

            mockGovernorsReadService
                .Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });

            mockCachedLookupService
                .Setup(c => c.GovernorRolesGetAllAsync())
                .ReturnsAsync(() => new List<LookupDto>
                    {
                        new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                        new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
                    });

            mockLayoutHelper
                .Setup
                    (l => l.PopulateLayoutProperties
                        (It.IsAny<GovernorsGridViewModel>(),
                            null,
                            groupId,
                            It.IsAny<IPrincipal>(),
                            It.IsAny<Action<EstablishmentModel>>(),
                            It.IsAny<Action<GroupModel>>()))
                    .Returns(Task.CompletedTask);

            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(false)
                {
                    Errors = new[]
                    {
                        new ApiError {Code = "Test", Fields = errorKey, Message = errorMessage}
                    }
                });

            SetupCommonLookupMocks();

            // Act
            var controller = BuildController();
            var result = await controller
                .DeleteOrRetireGovernor(new GovernorsGridViewModel
                    {
                        GroupUId = groupId,
                        Action = "Save",
                        RemovalAppointmentEndDate = new DateTimeViewModel(new DateTime(2026, 01, 01)),
                        RemovalGid = governorId
                    });

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Single(viewResult.ViewData.ModelState);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(errorKey));
            Assert.Single(viewResult.ViewData.ModelState[errorKey].Errors);
            Assert.Equal(errorMessage, viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage);
        }
    }
}
