using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Models;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.DeleteOrRetire
{
    public class DeleteGovernorControllerTests : GovernorControllerTestBase
    {
        [Fact]
        public async Task Gov_DeleteOrRetireGovernor_Save_UnknownApiError()
        {
            var controller = BuildController();
            var groupId = 2436;
            var governorId = 6224;

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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties
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
                    Errors = new ApiError[0]
                });

            await Assert.ThrowsAsync<TexunaApiSystemException>(() =>
            controller.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                GroupUId = groupId,
                Action = "Save",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId
            }));
        }

        [Theory()]
        [InlineData(16513, null)]
        [InlineData(null, 81681)]
        public async Task Gov_DeleteOrRetireGovernor_Group_Save_OK(int? estabId, int? groupId)
        {
            var controller = BuildController();
            var governorId = 6224;

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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabId, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(true));

            var result = await controller.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                GroupUId = groupId,
                EstablishmentUrn = estabId,
                Action = "Save",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId
            });

            var viewResult = result as RedirectResult;
            Assert.NotNull(viewResult);

            Assert.Contains(estabId.HasValue ? "#school-governance" : "#governance", viewResult.Url);
            mockGovernorsWriteService.Verify(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task Gov_DeleteOrRetireGovernor_SharedGov_Save_ApiError()
        {
            var controller = BuildController();
            var governorId = 2436;
            var estabId = 6151;
            var errorKey = "Test";
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

            var governor = new GovernorModel
            {
                Id = governorId,
                Appointments = new[]
                {
                    new GovernorAppointment
                    {
                        EstablishmentUrn = estabId,
                        AppointmentStartDate = DateTime.Now.AddDays(-30)
                    }
                }
            };

            var error = new ApiResponse(false)
            {
                Errors = new[]
                {
                    new ApiError {Code = "Test", Fields = errorKey, Message = errorMessage}
                }
            };

            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            mockGovernorsWriteService.Setup(g => g.UpdateSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => error);
            mockCachedLookupService
                .Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService
                .Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService
                .Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService
                .Setup(s => s.GovernorRolesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            var result = await controller.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Save",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId,
                GovernorShared = true
            });

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Single(viewResult.ViewData.ModelState);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(errorKey));
            Assert.Single(viewResult.ViewData.ModelState[errorKey].Errors);
            Assert.Equal(errorMessage, viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Gov_DeleteOrRetireGovernor_SharedGov_Save_Estab_OK()
        {
            var controller = BuildController();
            var governorId = 2436;
            var estabId = 6151;

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

            var governor = new GovernorModel
            {
                Id = governorId,
                Appointments = new[]
                {
                    new GovernorAppointment
                    {
                        EstablishmentUrn = estabId,
                        AppointmentStartDate = DateTime.Now.AddDays(-30)
                    }
                }
            };

            var error = new ApiResponse(true);

            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            mockGovernorsWriteService.Setup(g => g.UpdateSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => error);


            var result = await controller.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Save",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId,
                GovernorShared = true
            });

            var viewResult = result as RedirectResult;
            Assert.NotNull(viewResult);

            Assert.Contains("#school-governance", viewResult.Url);
            mockGovernorsWriteService.Verify(g => g.UpdateSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task Gov_DeleteOrRetireGovernor_Estab_Remove_Shared_OK()
        {
            var controller = BuildController();
            var estabId = 7845;
            var governorId = 6224;

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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            mockGovernorsWriteService.Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse(true));
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsWriteService.Setup(g => g.DeleteSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<IPrincipal>())).Returns(Task.CompletedTask);


            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await controller.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Remove",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId,
                GovernorShared = true
            });

            var viewResult = result as RedirectResult;
            Assert.NotNull(viewResult);

            Assert.Contains("#school-governance", viewResult.Url);
            mockGovernorsWriteService.Verify(g => g.DeleteSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task Gov_DeleteOrRetireGovernor_Estab_Remove_NonShared_OK()
        {
            var controller = BuildController();
            var estabId = 7845;
            var governorId = 6224;

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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            mockGovernorsWriteService.Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse(true));
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsWriteService.Setup(g => g.DeleteAsync(governorId, It.IsAny<IPrincipal>())).Returns(Task.CompletedTask);


            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await controller.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Remove",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId
            });

            var viewResult = result as RedirectResult;
            Assert.NotNull(viewResult);

            Assert.Contains("#school-governance", viewResult.Url);
            mockGovernorsWriteService.Verify(g => g.DeleteAsync(governorId, It.IsAny<IPrincipal>()), Times.Once);
        }

        [Theory]
        [InlineData(null, 999)]   // group context
        [InlineData(888, null)]   // establishment context
        public async Task Gov_DeleteOrRetireGovernor_Save_Success(int? estabId, int? groupId)
        {
            // Arrange
            var governorId = 123;

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.Governor },
                CurrentGovernors = new List<GovernorModel>
        {
            new GovernorModel
            {
                Id = governorId,
                RoleId = (int)eLookupGovernorRole.Governor,
                Person_FirstName = "Test",
                Person_TitleId = 1,
                Appointments = new[]
                {
                    new GovernorAppointment
                    {
                        EstablishmentUrn = estabId ?? 12345,
                        AppointmentStartDate = DateTime.Today.AddYears(-1),
                        AppointmentEndDate = null
                    }
                }
            }
        },
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
        {
            { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
        }
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabId, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorPermissions(estabId, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorPermissions());

            mockGovernorsWriteService
                .Setup(s => s.DeleteAsync(governorId, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse(true)));

            var controller = BuildController();

            var postedModel = new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                GroupUId = groupId,
                Action = "Remove",
                GovernorShared = false,
                RemovalGid = governorId,
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Today)
            };

            // Act
            var result = await controller.DeleteOrRetireGovernor(postedModel);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);

            mockGovernorsWriteService.Verify(
                s => s.DeleteAsync(governorId, It.IsAny<IPrincipal>()),
                Times.Once);
        }
    }
}
