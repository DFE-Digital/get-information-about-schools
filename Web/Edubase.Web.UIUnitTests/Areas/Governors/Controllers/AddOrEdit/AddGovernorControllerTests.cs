using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.AddOrEdit
{
    public class AddGovernorControllerTests : GovernorControllerTestBase
    {
        [Fact]
        public async Task Gov_AddEditOrReplace_NullParams()
        {
            // Arrange
            var controller = BuildController();

            var route = new Route("", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            rd.Values["controller"] = "Governors";
            rd.Values["action"] = "AddEditOrReplace";

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            // Act & Assert
            await Assert.ThrowsAsync<EdubaseException>(() =>
                controller.AddEditOrReplace(null, null, null, null));
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Single_NotShared_AlreadyExists()
        {
            // Arrange
            var estabUrn = 4;
            var controller = BuildController();

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel>
                    {
                    new GovernorModel { RoleId = (int)eLookupGovernorRole.ChairOfGovernors }
                    }
                });

            var route = new Route("", new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["controller"] = "Governors";
            routeData.Values["action"] = "AddEditOrReplace";

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.ChairOfGovernors,
                gid: null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("EstabEditGovernance", redirectResult.RouteName);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Single_NotShared_DoesntExist()
        {
            // Arrange
            var estabUrn = 4;
            var controller = BuildController();

            var route = new Route("", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            SetupCommonLookupMocks();

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel>()
                });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfGovernors,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorDisplayPolicy());

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.ChairOfGovernors,
                gid: null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);
            Assert.Equal(eLookupGovernorRole.ChairOfGovernors, model.GovernorRole);
            Assert.Equal(estabUrn, model.EstablishmentUrn);
            Assert.Null(model.GroupUId);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_GIDSpecified()
        {
            // Arrange
            var estabUrn = 4;
            var governorId = 1032;
            var controller = BuildController();

            var governor = new GovernorModel
            {
                Id = governorId,
                RoleId = (int) eLookupGovernorRole.Governor
            };

            var route = new Route("", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            SetupCommonLookupMocks();

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governor);

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorDisplayPolicy());

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: null,
                gid: governorId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Edit, model.Mode);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Shared()
        {
            // Arrange
            var estabUrn = 4;
            var controller = BuildController();

            var route = new Route("", new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["controller"] = "Governors";
            routeData.Values["action"] = "AddEditOrReplace";

            controller.ControllerContext = new ControllerContext(
                    controller.ControllerContext.HttpContext,
                    routeData,
                    controller);

            SetupCommonLookupMocks();

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel>()
                });

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                gid: null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("SelectSharedGovernor", redirectResult.RouteName);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_ReplaceMode_NoRole_GidSpecified_RedirectsToEstabReplaceChair()
        {
            // Arrange
            var estabUrn = 123;
            var gid = 456;

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["d"] = "10";
            qs["m"] = "12";
            qs["y"] = "2024";

            var controller = BuildController(qs);

            var route = new Route(
                "establishment/edit/{establishmentUrn}/governance/Replace/{gid}",
                new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["establishmentUrn"] = estabUrn;
            routeData.Values["gid"] = gid;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            var previousGov = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.ChairOfGovernors
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(previousGov);

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: null,
                gid: gid);

            // Assert
            var redirect = Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("EstabReplaceChair", redirect.RouteName);
            Assert.Equal(estabUrn, redirect.RouteValues["establishmentUrn"]);
            Assert.Equal(gid, redirect.RouteValues["gid"]);
            Assert.Equal("10", redirect.RouteValues["d"]);
            Assert.Equal("12", redirect.RouteValues["m"]);
            Assert.Equal("2024", redirect.RouteValues["y"]);
            Assert.Equal("true", redirect.RouteValues["ri"]);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Single_NotShared_AlreadyExists_Group()
        {
            // Arrange
            var groupId = 42;
            var controller = BuildController();

            var dto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>
            {
                new GovernorModel { RoleId = (int)eLookupGovernorRole.ChairOfGovernors }
            },
                HistoricalGovernors = new List<GovernorModel>(),
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.ChairOfGovernors }
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            var route = new Route("", new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["controller"] = "Governors";
            routeData.Values["action"] = "AddEditOrReplace";

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<object>(),
                    null,
                    groupId,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: groupId,
                establishmentUrn: null,
                role: eLookupGovernorRole.ChairOfGovernors,
                gid: null);

            // Assert
            var redirect = Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("GroupEditGovernance", redirect.RouteName);
            Assert.Equal(groupId, redirect.RouteValues["groupUId"]);
            Assert.Null(redirect.RouteValues["establishmentUrn"]);
            Assert.True((bool) redirect.RouteValues["roleAlreadyExists"]);
            Assert.NotNull(redirect.RouteValues["selectedRole"]);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_ReplaceMode_ChairOfTrustees_PopulatesReplaceViewModel()
        {
            // Arrange
            var estabUrn = 4000;
            var gid = 200;
            var replacementGovernorId = 300;

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["gid2"] = replacementGovernorId.ToString();

            var controller = BuildController(qs);

            var route = new Route(
                "establishment/edit/{establishmentUrn}/governance/Replace/{gid}",
                new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["establishmentUrn"] = estabUrn;
            routeData.Values["gid"] = gid;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            var model = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.ChairOfTrustees,
                AppointmentEndDate = new DateTime(2025, 6, 30),
                Person_FirstName = "Alex",
                Person_LastName = "Chair",
                Person_TitleId = 1
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(model);

            var dto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>
            {
                new GovernorModel
                {
                    Id = replacementGovernorId,
                    RoleId = (int)eLookupGovernorRole.Governor,
                    Person_FirstName = "Beth",
                    Person_LastName = "Gov",
                    AppointmentStartDate = new DateTime(2020, 1, 1),
                    AppointmentEndDate = new DateTime(2024, 1, 1),
                    DOB = new DateTime(1980, 1, 1),
                    Person_TitleId = 1
                },
                new GovernorModel
                {
                    Id = 301,
                    RoleId = (int)eLookupGovernorRole.Trustee,
                    Person_FirstName = "Chris",
                    Person_LastName = "Trustee",
                    Person_TitleId = 1
                }
            },
                HistoricalGovernors = new List<GovernorModel>()
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfTrustees,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.ChairOfTrustees,
                gid: gid);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);

            Assert.Equal(CreateEditGovernorViewModel.EditMode.Replace, vm.Mode);
            Assert.Equal(gid, vm.ReplaceGovernorViewModel.GID);
            Assert.NotNull(vm.ReplaceGovernorViewModel.AppointmentEndDate);
            Assert.Equal(model.AppointmentEndDate, vm.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime());
            Assert.False(string.IsNullOrWhiteSpace(vm.ReplaceGovernorViewModel.Name));

            Assert.NotNull(vm.ExistingGovernors);
            var existingList = vm.ExistingGovernors.ToList();
            Assert.Equal(2, existingList.Count);

            var selectedItem = Assert.Single(existingList.Where(i => i.Selected));
            Assert.Equal(replacementGovernorId.ToString(), selectedItem.Value);
            Assert.NotNull(vm.SelectedGovernor);
            Assert.Equal(replacementGovernorId, vm.SelectedGovernor.Id);
            Assert.Equal(vm.SelectedGovernor.Person_FirstName, vm.FirstName);
            Assert.Equal(vm.SelectedGovernor.Person_LastName, vm.LastName);
            Assert.Equal(vm.SelectedGovernor.Person_TitleId, vm.GovernorTitleId);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Estab_ReplaceMode_ChairOfTrustees_HitsEligibleGovernorBlock()
        {
            // Arrange
            var estabUrn = 4001;
            var gid = 200;
            var replacementId = 300;

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["gid2"] = replacementId.ToString();

            var controller = BuildController(qs);

            var route = new Route(
                "establishment/{establishmentUrn}/governance/Replace/{gid}",
                new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["establishmentUrn"] = estabUrn;
            routeData.Values["gid"] = gid;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            var chair = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.ChairOfTrustees,
                Person_FirstName = "Alex",
                Person_LastName = "Chair",
                AppointmentEndDate = new DateTime(2025, 6, 30),
                Person_TitleId = 1
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(chair);

            var dto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>
            {
                new GovernorModel
                {
                    Id = replacementId,
                    RoleId = (int)eLookupGovernorRole.Governor,
                    Person_FirstName = "Beth",
                    Person_LastName = "Gov",
                    Person_TitleId = 1,
                    DOB = new DateTime(1980, 1, 1)
                },
                new GovernorModel
                {
                    Id = 301,
                    RoleId = (int)eLookupGovernorRole.Trustee,
                    Person_FirstName = "Chris",
                    Person_LastName = "Trustee",
                    Person_TitleId = 1
                }
            },
                HistoricalGovernors = new List<GovernorModel>()
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfTrustees,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.ChairOfTrustees,
                gid: gid);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(view.Model);
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Replace, vm.Mode);

            var existing = vm.ExistingGovernors.ToList();
            Assert.Equal(2, existing.Count);

            var selected = Assert.Single(existing.Where(x => x.Selected));
            Assert.Equal(replacementId.ToString(), selected.Value);
            Assert.NotNull(vm.SelectedGovernor);
            Assert.Equal("Beth", vm.FirstName);
            Assert.Equal("Gov", vm.LastName);
            Assert.Equal(1, vm.GovernorTitleId);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Estab_ReplaceMode_ChairOfTrustees_BuildsExistingGovernors_WhenNoReplacementSelected()
        {
            // Arrange
            var estabUrn = 4002;
            var gid = 201;

            var qs = HttpUtility.ParseQueryString(string.Empty);
            var controller = BuildController(qs);

            var route = new Route(
                "establishment/{establishmentUrn}/governance/Replace/{gid}",
                new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["establishmentUrn"] = estabUrn;
            routeData.Values["gid"] = gid;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            var chair = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.ChairOfTrustees,
                Person_FirstName = "Alex",
                Person_LastName = "Chair",
                AppointmentEndDate = new DateTime(2026, 1, 31),
                Person_TitleId = 1
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(chair);

            var dto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>
            {
                new GovernorModel
                {
                    Id = 1001,
                    RoleId = (int)eLookupGovernorRole.Governor,
                    Person_FirstName = "Donna",
                    Person_LastName = "Gov",
                    Person_TitleId = 1
                },
                new GovernorModel
                {
                    Id = 1002,
                    RoleId = (int)eLookupGovernorRole.Trustee,
                    Person_FirstName = "Elliot",
                    Person_LastName = "Trustee",
                    Person_TitleId = 1
                }
            },
                HistoricalGovernors = new List<GovernorModel>()
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfTrustees,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.ChairOfTrustees,
                gid: gid);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(view.Model);
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Replace, vm.Mode);

            var existing = vm.ExistingGovernors.ToList();
            Assert.Equal(2, existing.Count);
            Assert.DoesNotContain(existing, x => x.Selected);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Estab_EditMode_SetsIsHistoric_WhenEndDateInPast()
        {
            // Arrange
            var estabUrn = 7777;
            var gid = 123;
            var controller = BuildController();

            var route = new Route(
                "establishment/{urn}/governance",
                new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["urn"] = estabUrn;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            var historicEndDate = DateTime.Now.Date.AddDays(-10);
            var governor = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.Governor,
                AppointmentEndDate = historicEndDate,
                Person_FirstName = "Amy",
                Person_LastName = "Old",
                Person_TitleId = 1
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(governor);

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: null,
                gid: gid);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(view.Model);
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Edit, vm.Mode);
            Assert.True(vm.IsHistoric);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Estab_EditMode_SetsIsHistoricFalse_WhenEndDateTodayOrFuture()
        {
            // Arrange
            var estabUrn = 7778;
            var gid = 124;
            var controller = BuildController();

            var route = new Route(
                "establishment/{urn}/governance",
                new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["urn"] = estabUrn;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            var endDateToday = DateTime.Now.Date;
            var governor = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.Governor,
                AppointmentEndDate = endDateToday,
                Person_FirstName = "Sam",
                Person_LastName = "Current",
                Person_TitleId = 1
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(governor);

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: null,
                gid: gid);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(view.Model);
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Edit, vm.Mode);
            Assert.False(vm.IsHistoric);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Estab_ReplaceMode_ChairOfTrustees_SetsReplacementAppointmentEndDate_FromQueryString()
        {
            // Arrange
            var estabUrn = 6001;
            var gid = 200;
            var replacementId = 300;

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["gid2"] = replacementId.ToString();
            qs["d"] = "10";
            qs["m"] = "12";
            qs["y"] = "2024";
            qs["rag"] = "true";

            var controller = BuildController(qs);

            var route = new Route(
                "establishment/{establishmentUrn}/governance/Replace/{gid}",
                new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["establishmentUrn"] = estabUrn;
            routeData.Values["gid"] = gid;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            var chair = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.ChairOfTrustees,
                AppointmentEndDate = new DateTime(2023, 1, 1),
                Person_FirstName = "Alex",
                Person_LastName = "Chair",
                Person_TitleId = 1
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(chair);

            var dto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>
            {
                new GovernorModel
                {
                    Id = replacementId,
                    RoleId = (int)eLookupGovernorRole.Governor,
                    Person_FirstName = "Beth",
                    Person_LastName = "Gov",
                    Person_TitleId = 1
                }
            }
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfTrustees,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.ChairOfTrustees,
                gid: gid);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(view.Model);
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Replace, vm.Mode);
            Assert.True(vm.ReinstateAsGovernor);
            Assert.Equal(10, vm.ReplaceGovernorViewModel.AppointmentEndDate.Day);
            Assert.Equal(12, vm.ReplaceGovernorViewModel.AppointmentEndDate.Month);
            Assert.Equal(2024, vm.ReplaceGovernorViewModel.AppointmentEndDate.Year);
        }

        /// <summary>
        /// Every chair of local governing body role, combined with every chair of local governing body role.
        /// At the time of writing, equivalent to:
        /// [InlineData( eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody )]
        /// [InlineData( eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody )]
        /// [InlineData( eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody )]
        /// [InlineData( eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody )]
        /// </summary>
        public static IEnumerable<object[]> PairwiseChairOfLocalGoverningBodyRoles =>
            from preExistingRole in EnumSets.eChairOfLocalGoverningBodyRoles
            from newRole in EnumSets.eChairOfLocalGoverningBodyRoles
            select new object[] { preExistingRole, newRole };

        [Theory]
        [MemberData(nameof(PairwiseChairOfLocalGoverningBodyRoles))]
        public async Task Gov_AddEditOrReplace_RoleSpecified_ChairOfLocalGoverningBody_RoleAlreadyExists(
            eLookupGovernorRole preExistingGovernorRole,
            eLookupGovernorRole newGovernorRole)
        {
            // Arrange
            var estabUrn = 4;
            var controller = BuildController();

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel>
                    {
                    new GovernorModel { RoleId = (int)preExistingGovernorRole }
                    }
                });

            var route = new Route("", new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["controller"] = "Governors";
            routeData.Values["action"] = "AddEditOrReplace";

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                routeData,
                controller);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: newGovernorRole,
                gid: null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToRouteResult>(result);

            string expectedRoute;
            if (newGovernorRole == eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody)
            {
                expectedRoute = "SelectSharedGovernor";
            }
            else
            {
                expectedRoute = "EstabEditGovernance";
            }

            Assert.Equal(expectedRoute, redirectResult.RouteName);
        }

        // Delegate to EnumSets
        public static TheoryData<eLookupGovernorRole, eLookupGovernorRole> ForbiddenCombinationsOfGovernanceProfessionalRoles
        {
            get
            {
                var theoryData = new TheoryData<eLookupGovernorRole, eLookupGovernorRole>();
                foreach (var combination in EnumSets.ForbiddenCombinationsOfGovernanceProfessionalRoles)
                {
                    theoryData.Add((eLookupGovernorRole) combination[0], (eLookupGovernorRole) combination[1]);
                }

                return theoryData;
            }
        }

        [Theory]
        [MemberData(nameof(ForbiddenCombinationsOfGovernanceProfessionalRoles))]
        public async Task Gov_AddEditOrReplace_RoleSpecified_GovernanceProfessional_RoleAlreadyExists_DisallowedThereforeReject(
            eLookupGovernorRole preExistingGovernorRole,
            eLookupGovernorRole newGovernorRole)
        {
            // Arrange
            var currentGovernors = new List<GovernorModel>
        {
            new GovernorModel { RoleId = (int)preExistingGovernorRole }
        };

            var governorsDetails = new GovernorsDetailsDto
            {
                CurrentGovernors = currentGovernors,
                ApplicableRoles = new List<eLookupGovernorRole> { newGovernorRole },
                HistoricalGovernors = new List<GovernorModel>(),
                HasFullAccess = true
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(governorsDetails);

            var controller = BuildController();

            // Act
            var result = await controller.RoleAllowed(
                newGovernorRole,
                null,
                null,
                null,
                false);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Create_Defaults_OriginalChairOfTrustees_ToNo()
        {
            // Arrange
            var groupId = 42;
            var controller = BuildController();

            var governorDetailsDto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>(),
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.ChairOfTrustees },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
            {
                { eLookupGovernorRole.ChairOfTrustees, new GovernorDisplayPolicy() }
            }};

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);

            var route = new Route("", new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["controller"] = "Governors";
            routeData.Values["action"] = "AddEditOrReplace";

            controller.ControllerContext = new ControllerContext(
                    controller.ControllerContext.HttpContext,
                    routeData,
                    controller);

            SetupCommonLookupMocks();

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    null,
                    groupId,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfTrustees,
                    true,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorDisplayPolicy());

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: groupId,
                establishmentUrn: null,
                role: eLookupGovernorRole.ChairOfTrustees,
                gid: null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);
            Assert.Null(model.IsOriginalChairOfTrustees);

            var noItem = Assert.Single(model.YesNoSelect.Where(i => i.Value == "false"));
            Assert.True(noItem.Selected);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Edit_KeepsOriginalSignatoryFlag_AndDoesNotModifyAppointingBody()
        {
            // Arrange
            var estabUrn = 4000;
            var governorId = 200;
            var controller = BuildController();

            var route = new Route("", new PageRouteHandler("~/"));
            var routeData = new RouteData(route, new PageRouteHandler("~/"));
            routeData.Values["controller"] = "Governors";
            routeData.Values["action"] = "AddEditOrReplace";

            controller.ControllerContext = new ControllerContext(
                    controller.ControllerContext.HttpContext,
                    routeData,
                    controller);

            SetupCommonLookupMocks();

            var governor = new GovernorModel
            {
                Id = governorId,
                RoleId = (int) eLookupGovernorRole.ChairOfTrustees,
                IsOriginalSignatoryMember = true,
                IsOriginalChairOfTrustees = true
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(governor);

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfTrustees,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy
                {
                    AppointingBodyId = true
                });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: null,
                gid: governorId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);

            Assert.True(vm.IsOriginalSignatoryMember);
            Assert.True(vm.IsOriginalChairOfTrustees);
            // AppointingBodyId is intentionally not asserted (frontend-only rule via JS)
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_EditMode_MapsFieldsCorrectly()
        {
            // Arrange
            var estabUrn = 1010;
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                Mode = CreateEditGovernorViewModel.EditMode.Edit,
                GovernorRole = eLookupGovernorRole.Governor,
                AppointingBodyId = 5,
                AppointmentStartDate = new DateTimeViewModel(new DateTime(2020, 1, 1)),
                AppointmentEndDate = new DateTimeViewModel(new DateTime(2025, 1, 1)),
                DOB = new DateTimeViewModel(new DateTime(1990, 5, 10)),
                EmailAddress = "person@example.com",
                GroupUId = null,
                EstablishmentUrn = estabUrn,
                GID = 444,
                FirstName = "Jane",
                MiddleName = "Elizabeth",
                LastName = "Doe",
                IsOriginalChairOfTrustees = true,
                IsOriginalSignatoryMember = false,
                GovernorTitleId = 3,
                PreviousFirstName = "J.",
                PreviousMiddleName = "E.",
                PreviousLastName = "D.",
                PreviousTitleId = 7,
                PostCode = "XY1 2ZZ",
                TelephoneNumber = "0800123456"
            };

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Callback<GovernorModel, IPrincipal>((gm, _) =>
                {
                    Assert.Equal(5, gm.AppointingBodyId);
                    Assert.Equal(new DateTime(2025, 1, 1), gm.AppointmentEndDate);
                    Assert.Equal(new DateTime(2020, 1, 1), gm.AppointmentStartDate);
                    Assert.Equal(new DateTime(1990, 5, 10), gm.DOB);
                    Assert.Equal("person@example.com", gm.EmailAddress);
                    Assert.Equal(estabUrn, gm.EstablishmentUrn);
                    Assert.Equal(444, gm.Id);
                    Assert.Equal("Jane", gm.Person_FirstName);
                    Assert.Equal("Elizabeth", gm.Person_MiddleName);
                    Assert.Equal("Doe", gm.Person_LastName);
                    Assert.True(gm.IsOriginalChairOfTrustees);
                    Assert.False(gm.IsOriginalSignatoryMember);
                    Assert.Equal(3, gm.Person_TitleId);
                    Assert.Equal("J.", gm.PreviousPerson_FirstName);
                    Assert.Equal("E.", gm.PreviousPerson_MiddleName);
                    Assert.Equal("D.", gm.PreviousPerson_LastName);
                    Assert.Equal(7, gm.PreviousPerson_TitleId);
                    Assert.Equal("XY1 2ZZ", gm.PostCode);
                    Assert.Equal((int) eLookupGovernorRole.Governor, gm.RoleId);
                    Assert.Equal("0800123456", gm.TelephoneNumber);
                })
                .ReturnsAsync(new ApiResponse<int>(true));

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            await controller.AddEditOrReplace(vm);

            // Assert
            // Assertions are inside the SaveAsync callback
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_ReplaceMode_SetsAppointmentStartDateCorrectly()
        {
            // Arrange
            var estabUrn = 2020;
            var endDate = new DateTime(2024, 6, 15);
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                Mode = CreateEditGovernorViewModel.EditMode.Replace,
                GovernorRole = eLookupGovernorRole.Governor,
                ReplaceGovernorViewModel = new ReplaceGovernorViewModel
                {
                    AppointmentEndDate = new DateTimeViewModel(endDate)
                },
                FirstName = "Mark",
                MiddleName = "Alan",
                LastName = "River",
                GovernorTitleId = 2,
                PreviousFirstName = "M.",
                PreviousMiddleName = "A.",
                PreviousLastName = "R.",
                PreviousTitleId = 9,
                AppointingBodyId = 10,
                AppointmentEndDate = new DateTimeViewModel(new DateTime(2025, 1, 1)),
                DOB = new DateTimeViewModel(new DateTime(1985, 4, 4)),
                EmailAddress = "mark@example.com",
                EstablishmentUrn = estabUrn,
                PostCode = "ZZ9 3PL",
                TelephoneNumber = "07700123456"
            };

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Callback<GovernorModel, IPrincipal>((gm, _) =>
                {
                    Assert.Equal(endDate.AddDays(1), gm.AppointmentStartDate);
                    Assert.Equal("Mark", gm.Person_FirstName);
                    Assert.Equal("Alan", gm.Person_MiddleName);
                    Assert.Equal("River", gm.Person_LastName);
                    Assert.Equal(2, gm.Person_TitleId);
                    Assert.Equal("M.", gm.PreviousPerson_FirstName);
                    Assert.Equal("A.", gm.PreviousPerson_MiddleName);
                    Assert.Equal("R.", gm.PreviousPerson_LastName);
                    Assert.Equal(9, gm.PreviousPerson_TitleId);
                    Assert.Equal(10, gm.AppointingBodyId);
                    Assert.Equal(estabUrn, gm.EstablishmentUrn);
                    Assert.Equal("mark@example.com", gm.EmailAddress);
                    Assert.Equal("ZZ9 3PL", gm.PostCode);
                    Assert.Equal("07700123456", gm.TelephoneNumber);
                })
                .ReturnsAsync(new ApiResponse<int>(true));

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            await controller.AddEditOrReplace(vm);

            // Assert
            // Assertions are inside the SaveAsync callback
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_DuplicateSharedGovernor_RedirectsToGroupEdit()
        {
            // Arrange
            var groupId = 900;
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GroupUId = groupId,
                GovernorRole = eLookupGovernorRole.Establishment_SharedLocalGovernor,
                FirstName = "Alex",
                MiddleName = "J",
                LastName = "Taylor",
                GovernorTitleId = 1,
                GID = null,
                EstablishmentUrn = null
            };

            var existing = new GovernorModel
            {
                Id = 123,
                Person_FirstName = "Alex",
                Person_MiddleName = "J",
                Person_LastName = "Taylor",
                Person_TitleId = 1,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedLocalGovernor
            };

            var route = new Route("", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            SetupCommonLookupMocks();

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel> { existing },
                    HistoricalGovernors = new List<GovernorModel>(),
                    ApplicableRoles = new List<eLookupGovernorRole>
                    {
                    eLookupGovernorRole.Establishment_SharedLocalGovernor
                    },
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                    { eLookupGovernorRole.Establishment_SharedLocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Establishment_SharedLocalGovernor,
                    true,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    null,
                    groupId,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            var redirect = Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("GroupEditGovernance", redirect.RouteName);
            Assert.Equal(groupId, redirect.RouteValues["groupUId"]);
            Assert.Equal(existing.Id, redirect.RouteValues["duplicateGovernorId"]);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Never);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_SharedRole_DifferentName_DoesNotRedirect_Duplicate()
        {
            // Arrange
            var groupId = 901;
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GroupUId = groupId,
                GovernorRole = eLookupGovernorRole.Establishment_SharedLocalGovernor,
                FirstName = "Alex",
                MiddleName = "J",
                LastName = "Taylor",
                GovernorTitleId = 1,
                GID = null,
                EstablishmentUrn = null
            };

            var existing = new GovernorModel
            {
                Id = 456,
                Person_FirstName = "Alex",
                Person_MiddleName = "K",
                Person_LastName = "Taylor",
                Person_TitleId = 1,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedLocalGovernor
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel> { existing },
                    HistoricalGovernors = new List<GovernorModel>(),
                    ApplicableRoles = new List<eLookupGovernorRole>
                    {
                    eLookupGovernorRole.Establishment_SharedLocalGovernor
                    },
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                    { eLookupGovernorRole.Establishment_SharedLocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Establishment_SharedLocalGovernor,
                    true,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true) { Response = 999 });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    null,
                    groupId,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Contains("#governance", redirect.Url);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Once);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_SameName_DifferentRole_DoesNotTreatAsDuplicate()
        {
            // Arrange
            var groupId = 902;
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GroupUId = groupId,
                GovernorRole = eLookupGovernorRole.Establishment_SharedLocalGovernor,
                FirstName = "Alex",
                MiddleName = "J",
                LastName = "Taylor",
                GovernorTitleId = 1,
                GID = null,
                EstablishmentUrn = null
            };

            var existing = new GovernorModel
            {
                Id = 789,
                Person_FirstName = "Alex",
                Person_MiddleName = "J",
                Person_LastName = "Taylor",
                Person_TitleId = 1,
                RoleId = (int) eLookupGovernorRole.Governor
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel> { existing },
                    HistoricalGovernors = new List<GovernorModel>(),
                    ApplicableRoles = new List<eLookupGovernorRole>
                    {
                    eLookupGovernorRole.Establishment_SharedLocalGovernor
                    },
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                    {
                    { eLookupGovernorRole.Establishment_SharedLocalGovernor, new GovernorDisplayPolicy() }
                    }
                });

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Establishment_SharedLocalGovernor,
                    true,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true) { Response = 1000 });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    null,
                    groupId,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Contains("#governance", redirect.Url);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Once);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_SaveFails_AddsErrorsToModelState()
        {
            // Arrange
            var estabUrn = 1234;
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.Governor,
                EstablishmentUrn = estabUrn,
                AppointmentStartDate = new DateTimeViewModel(DateTime.Today),
                AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddMonths(1)),
                DOB = new DateTimeViewModel(DateTime.Today.AddYears(-30)),
                FirstName = "John",
                LastName = "Smith",
                GovernorTitleId = 1
            };

            var route = new Route("", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            SetupCommonLookupMocks();

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            var apiError = new ApiError
            {
                Fields = "Person_FirstName",
                Message = "First name is invalid"
            };

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(false)
                {
                    Errors = new[] { apiError }
                });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedVm = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);

            Assert.Equal(vm, returnedVm);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("Person_FirstName"));

            var errors = viewResult.ViewData.ModelState["Person_FirstName"].Errors;
            Assert.Single(errors);
            Assert.Equal("First name is invalid", errors[0].ErrorMessage);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Once);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_SaveSuccess_RedirectsToEstablishmentDetails()
        {
            // Arrange
            var estabUrn = 5678;
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.Governor,
                EstablishmentUrn = estabUrn,
                AppointmentStartDate = new DateTimeViewModel(DateTime.Today),
                AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddMonths(6)),
                DOB = new DateTimeViewModel(DateTime.Today.AddYears(-35)),
                FirstName = "Emma",
                LastName = "Jones",
                GovernorTitleId = 1
            };

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true) { Response = 888 });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.EndsWith("#school-governance", redirect.Url);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Once);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_ReinstateAsGovernor_FetchesOldGovernorModel()
        {
            // Arrange
            var estabUrn = 5000;
            var oldGovernorId = 321;
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.Governor,
                EstablishmentUrn = estabUrn,
                ReinstateAsGovernor = true,
                ReplaceGovernorViewModel = new ReplaceGovernorViewModel
                {
                    GID = oldGovernorId
                },
                AppointmentStartDate = new DateTimeViewModel(DateTime.Today),
                AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddMonths(1)),
                DOB = new DateTimeViewModel(DateTime.Today.AddYears(-30)),
                FirstName = "Test",
                LastName = "User",
                GovernorTitleId = 1
            };

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(oldGovernorId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel { Id = oldGovernorId });

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true) { Response = 999 });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);


            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            mockGovernorsReadService.Verify(
                g => g.GetGovernorAsync(oldGovernorId, It.IsAny<IPrincipal>()),
                Times.Once);

            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_SelectedPreviousGovernor_RetiresOnly()
        {
            // Arrange
            var estabUrn = 7000;
            var previousId = 111;
            var replacementEndDate = new DateTime(2025, 6, 1);
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.Governor,
                EstablishmentUrn = estabUrn,
                SelectedPreviousGovernorId = previousId,
                ReinstateAsGovernor = false,
                ReplaceGovernorViewModel = new ReplaceGovernorViewModel
                {
                    AppointmentEndDate = new DateTimeViewModel(replacementEndDate)
                },
                AppointmentStartDate = new DateTimeViewModel(DateTime.Today),
                AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddMonths(1)),
                DOB = new DateTimeViewModel(DateTime.Today.AddYears(-30)),
                FirstName = "Test",
                LastName = "User",
                GovernorTitleId = 1
            };

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true) { Response = 999 });

            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            mockGovernorsWriteService.Verify(
                g => g.UpdateDatesAsync(previousId, replacementEndDate, It.IsAny<IPrincipal>()),
                Times.Once);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Once);

            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_ReinstateAndRetire_CallsRetireAndReinstate()
        {
            // Arrange
            var estabUrn = 8000;
            var previousId = 150;
            var oldChairId = 250;
            var oldChairEndDate = new DateTime(2024, 3, 1);
            var replacementEndDate = new DateTime(2025, 7, 1);
            var controller = BuildController();

            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.ChairOfGovernors,
                EstablishmentUrn = estabUrn,
                SelectedPreviousGovernorId = previousId,
                ReinstateAsGovernor = true,
                ReplaceGovernorViewModel = new ReplaceGovernorViewModel
                {
                    AppointmentEndDate = new DateTimeViewModel(replacementEndDate),
                    GID = oldChairId
                },
                AppointmentStartDate = new DateTimeViewModel(new DateTime(2025, 7, 2)),
                AppointmentEndDate = new DateTimeViewModel(new DateTime(2026, 7, 2)),
                DOB = new DateTimeViewModel(DateTime.Today.AddYears(-40)),
                FirstName = "Maria",
                LastName = "Chair",
                GovernorTitleId = 2
            };

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfGovernors,
                    false,
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ValidationEnvelopeDto());

            mockGovernorsWriteService
                .SetupSequence(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true) { Response = 500 })
                .ReturnsAsync(new ApiResponse<int>(true) { Response = 501 });

            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(oldChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = oldChairId,
                    AppointmentEndDate = oldChairEndDate
                });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            mockGovernorsWriteService.Verify(
                g => g.UpdateDatesAsync(previousId, replacementEndDate, It.IsAny<IPrincipal>()),
                Times.Once);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.AtLeast(2));

            mockGovernorsReadService.Verify(
                g => g.GetGovernorAsync(oldChairId, It.IsAny<IPrincipal>()),
                Times.AtLeast(2));

            Assert.IsType<RedirectResult>(result);
        }

        [Theory]
        [InlineData(null, 153513)]
        [InlineData(16851, null)]
        public async Task Gov_Post_AddEditOrReplace_ApiError(int? estabId, int? groupId)
        {
            // Arrange
            var errorKey = "test";
            var errorMessage = "test message";
            var controller = BuildController();

            SetupCommonLookupMocks();

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<CreateEditGovernorViewModel>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ValidationEnvelopeDto
                {
                    Errors =
                    {
                    new ApiError { Code = "Test", Fields = errorKey, Message = errorMessage }
                    }
                });

            var vm = new CreateEditGovernorViewModel
            {
                EstablishmentUrn = estabId,
                GroupUId = groupId
            };

            // Act
            var result = await controller.AddEditOrReplace(vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);

            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(errorKey));
            Assert.Single(viewResult.ViewData.ModelState[errorKey].Errors);
            Assert.Equal(errorMessage, viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task CreateGovernorsViewModel_BuildsBaseViewModel()
        {
            // Arrange
            var estabUrn = 1000;

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    ApplicableRoles = new List<eLookupGovernorRole>(),
                    CurrentGovernors = new List<GovernorModel>(),
                    HistoricalGovernors = new List<GovernorModel>(),
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>()
                });

            mockGovernorsReadService
                .Setup(s => s.GetGovernorPermissions(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorPermissions());

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockEstablishmentReadService
                .Setup(s => s.GetAsync(1000, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ServiceResultDto<EstablishmentModel>
                {
                    ReturnValue = new EstablishmentModel { GovernanceModeId = 1 },
                    Status = eServiceResultStatus.Success
                });

            mockEstablishmentReadService
                .Setup(s => s.GetPermissibleLocalGovernorsAsync(1000, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<LookupDto>());

            // Act
            var controller = BuildController();
            var vm = await controller.CreateGovernorsViewModel(
                groupUId: null,
                establishmentUrn: estabUrn,
                establishmentModel: null,
                user: new GenericPrincipal(new GenericIdentity("UnitTestUser"), new string[0])
                );

            // Assert
            Assert.NotNull(vm);
            Assert.Equal(estabUrn, vm.EstablishmentUrn);
        }
    }
}
