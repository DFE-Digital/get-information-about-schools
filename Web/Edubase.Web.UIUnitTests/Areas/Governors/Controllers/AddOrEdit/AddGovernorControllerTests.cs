using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas;
using Edubase.Web.UI.Areas.Governors.Controllers;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.AddOrEdit
{
    public class AddGovernorControllerTests : IDisposable
    {
        private readonly GovernorController controller;
        private readonly Mock<ICachedLookupService> mockCachedLookupService;

        private readonly Mock<IGovernorsReadService> mockGovernorsReadService = new Mock<IGovernorsReadService>(MockBehavior.Strict);
        private readonly Mock<IGovernorsWriteService> mockGovernorsWriteService = new Mock<IGovernorsWriteService>(MockBehavior.Strict);
        private readonly Mock<IGroupReadService> mockGroupReadService = new Mock<IGroupReadService>(MockBehavior.Strict);
        private readonly Mock<IEstablishmentReadService> mockEstablishmentReadService = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
        private readonly Mock<ILayoutHelper> mockLayoutHelper = new Mock<ILayoutHelper>(MockBehavior.Strict);
        private readonly Mock<UrlHelper> mockUrlHelper = new Mock<UrlHelper>(MockBehavior.Loose);
        private readonly Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
        private readonly Mock<HttpRequestBase> mockHttpRequestBase = new Mock<HttpRequestBase>(MockBehavior.Strict);
        private readonly Mock<HttpContextBase> mockHttpContextBase = new Mock<HttpContextBase>(MockBehavior.Strict);
        private readonly Mock<IPrincipal> mockPrincipal = new Mock<IPrincipal>(MockBehavior.Strict);
        private readonly Mock<IIdentity> mockIdentity = new Mock<IIdentity>(MockBehavior.Strict);
        private readonly Mock<IGovernorsGridViewModelFactory> mockGovernorGridViewModelFactory = new Mock<IGovernorsGridViewModelFactory>(MockBehavior.Strict);

        private bool disposedValue;

        public AddGovernorControllerTests()
        {
            mockCachedLookupService = MockHelper.SetupCachedLookupService();

            mockEstablishmentReadService.Setup(e => e.GetEstabType2EducationPhaseMap())
                .Returns(new Dictionary<eLookupEstablishmentType, eLookupEducationPhase[]>());

            mockUrlHelper.Setup(u => u.RouteUrl(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("fake url");

            controller = new GovernorController(
                mockGovernorsReadService.Object,
                mockCachedLookupService.Object,
                mockGovernorsWriteService.Object,
                mockGroupReadService.Object,
                mockEstablishmentReadService.Object,
                mockLayoutHelper.Object);

            SetupController();
        }

        protected void SetupController()
        {
            SetupHttpRequest();
            controller.ControllerContext = mockControllerContext.Object;
            mockControllerContext.SetupGet(c => c.Controller).Returns(controller);
            controller.Url = mockUrlHelper.Object;
        }

        private void SetupHttpRequest()
        {
            mockHttpRequestBase.SetupGet(x => x.QueryString)
                .Returns(HttpUtility.ParseQueryString(string.Empty));
            mockHttpContextBase.SetupGet(x => x.Request)
                .Returns(mockHttpRequestBase.Object);
            mockHttpContextBase.SetupGet(x => x.User)
                .Returns(mockPrincipal.Object);
            mockControllerContext.SetupGet(x => x.HttpContext)
                .Returns(mockHttpContextBase.Object);
            mockControllerContext.SetupGet(x => x.IsChildAction)
                .Returns(false);
            mockControllerContext.SetupGet(x => x.RouteData)
                .Returns(new System.Web.Routing.RouteData());
            mockPrincipal.SetupGet(x => x.Identity)
                .Returns(mockIdentity.Object);
        }

        [Fact()]
        public async Task Gov_AddEditOrReplace_NullParams()
        {
            mockControllerContext.SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));
            await Assert.ThrowsAsync<EdubaseException>(() => controller.AddEditOrReplace(null, null, null, null));
        }

        [Fact()]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Single_NotShared_AlreadyExists()
        {
            var estabUrn = 4;
            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int) eLookupGovernorRole.ChairOfGovernors } }
            });
            mockControllerContext.SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));

            var result = await controller.AddEditOrReplace(null, estabUrn, eLookupGovernorRole.ChairOfGovernors, null);

            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);
            Assert.Equal("EstabEditGovernance", redirectResult.RouteName);
        }

        [Fact()]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Single_NotShared_DoesntExist()
        {
            var estabUrn = 4;
            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>()
            });
            mockControllerContext.SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<CreateEditGovernorViewModel>(), estabUrn, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsReadService.Setup(g => g.GetEditorDisplayPolicyAsync(eLookupGovernorRole.ChairOfGovernors, false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());

            var result = await controller.AddEditOrReplace(null, estabUrn, eLookupGovernorRole.ChairOfGovernors, null);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            var model = viewResult.Model as CreateEditGovernorViewModel;
            Assert.NotNull(model);
            Assert.Equal(eLookupGovernorRole.ChairOfGovernors, model.GovernorRole);
            Assert.Equal(estabUrn, model.EstablishmentUrn);
            Assert.Null(model.GroupUId);
        }

        [Fact()]
        public async Task Gov_AddEditOrReplace_GIDSpecified()
        {
            var estabUrn = 4;
            var governorId = 1032;

            var governor = new GovernorModel
            {
                Id = governorId,
                RoleId = (int) eLookupGovernorRole.Governor
            };

            mockControllerContext.SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));
            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<CreateEditGovernorViewModel>(), estabUrn, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsReadService.Setup(g => g.GetEditorDisplayPolicyAsync(eLookupGovernorRole.Governor, false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());

            var result = await controller.AddEditOrReplace(null, estabUrn, null, 1032);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            var model = viewResult.Model as CreateEditGovernorViewModel;
            Assert.NotNull(model);
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Edit, model.Mode);
        }

        [Fact()]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Shared()
        {
            var estabUrn = 4;
            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>()
            });
            mockControllerContext.SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));

            var result = await controller.AddEditOrReplace(null, estabUrn, eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, null);

            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);
            Assert.Equal("SelectSharedGovernor", redirectResult.RouteName);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_ReplaceMode_NoRole_GidSpecified_RedirectsToEstabReplaceChair()
        {
            // Arrange
            var estabUrn = 123;
            var gid = 456;

            // Make the route URL contain "/Replace/" so replaceMode == true
            mockControllerContext
                .SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("establishment/edit/{establishmentUrn}/governance/Replace/{gid}",
                              new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["d"] = "10";
            qs["m"] = "12";
            qs["y"] = "2024";
            mockHttpRequestBase
                .SetupGet(r => r.QueryString)
                .Returns(qs);

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

            mockControllerContext
                .SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("group/edit/{groupUId}/governance", new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

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

            // Force replaceMode = true by setting a route URL with "/Replace/" in it
            mockControllerContext
                .SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("establishment/edit/{establishmentUrn}/governance/Replace/{gid}",
                              new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

            // Provide query string with gid2 = replacementGovernorId
            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["gid2"] = replacementGovernorId.ToString();
            mockHttpRequestBase
                .SetupGet(r => r.QueryString)
                .Returns(qs);

            // The governor being edited/replaced (a Chair of Trustees)
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

            // The list of existing governors/trustees available to be selected as replacement
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
                AppointmentEndDate   = new DateTime(2024, 1, 1),
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

            // Used twice: once inside RoleAllowed, once inside the replace block
            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            // Allow the ChairOfTrustees role (RoleAllowed should return true)
            // by not having conflicting roles in dto.CurrentGovernors (we only have Governor/Trustee)
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

            // Act: establishment context, replace mode (via route), ChairOfTrustees role, with gid
            var result = await controller.AddEditOrReplace(
                groupUId: null,
                establishmentUrn: estabUrn,
                role: eLookupGovernorRole.ChairOfTrustees,
                gid: gid);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);

            // We are in Replace mode, not Create/Edit
            Assert.Equal(CreateEditGovernorViewModel.EditMode.Replace, vm.Mode);

            // ReplaceGovernorViewModel should be populated from the chair model
            Assert.Equal(gid, vm.ReplaceGovernorViewModel.GID);
            Assert.NotNull(vm.ReplaceGovernorViewModel.AppointmentEndDate);
            Assert.Equal(model.AppointmentEndDate, vm.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime());
            Assert.False(string.IsNullOrWhiteSpace(vm.ReplaceGovernorViewModel.Name));

            // Existing governors list built from governorsOrTrustees (Governor + Trustee)
            Assert.NotNull(vm.ExistingGovernors);
            var existingList = vm.ExistingGovernors.ToList();
            Assert.Equal(2, existingList.Count);

            // The replacement governor from gid2 should be selected and prepopulated
            var selectedItem = Assert.Single(existingList.Where(i => i.Selected));
            Assert.Equal(replacementGovernorId.ToString(), selectedItem.Value);

            Assert.NotNull(vm.SelectedGovernor);
            Assert.Equal(replacementGovernorId, vm.SelectedGovernor.Id);
            // PrepopulateFields should have copied basic fields from SelectedGovernor
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

            mockControllerContext.SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("establishment/{establishmentUrn}/governance/Replace/{gid}",
                              new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["gid2"] = replacementId.ToString();
            mockHttpRequestBase.SetupGet(r => r.QueryString).Returns(qs);

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
                DOB = new DateTime(1980,1,1)
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

            mockControllerContext.SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("establishment/{establishmentUrn}/governance/Replace/{gid}",
                              new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

            mockHttpRequestBase.SetupGet(r => r.QueryString)
                .Returns(HttpUtility.ParseQueryString(string.Empty));

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

            mockControllerContext.SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("establishment/{urn}/governance", new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

            mockHttpRequestBase.SetupGet(r => r.QueryString)
                .Returns(HttpUtility.ParseQueryString(string.Empty));

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
                    eLookupGovernorRole.Governor, false, It.IsAny<IPrincipal>()))
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

            mockControllerContext.SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("establishment/{urn}/governance", new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

            mockHttpRequestBase.SetupGet(r => r.QueryString)
                .Returns(HttpUtility.ParseQueryString(string.Empty));

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
                    eLookupGovernorRole.Governor, false, It.IsAny<IPrincipal>()))
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

            mockControllerContext.SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("establishment/{establishmentUrn}/governance/Replace/{gid}",
                              new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["gid2"] = replacementId.ToString();
            qs["d"] = "10";
            qs["m"] = "12";
            qs["y"] = "2024";
            qs["rag"] = "true";
            mockHttpRequestBase.SetupGet(r => r.QueryString).Returns(qs);

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
        ///     [InlineData( eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody,          eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody          )]
        ///     [InlineData( eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody,          eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody  )]
        ///     [InlineData( eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,  eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody          )]
        ///     [InlineData( eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,  eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody  )]
        /// </summary>
        public static IEnumerable<object[]> PairwiseChairOfLocalGoverningBodyRoles =>
            from preExistingRole in EnumSets.eChairOfLocalGoverningBodyRoles
            from newRole in EnumSets.eChairOfLocalGoverningBodyRoles
            select new object[] { preExistingRole, newRole };

        [Theory()]
        [MemberData(nameof(PairwiseChairOfLocalGoverningBodyRoles))]
        public async Task Gov_AddEditOrReplace_RoleSpecified_ChairOfLocalGoverningBody_RoleAlreadyExists(eLookupGovernorRole preExistingGovernorRole, eLookupGovernorRole newGovernorRole)
        {
            var estabUrn = 4;
            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel>()
                    {
                        new GovernorModel() {RoleId = (int) preExistingGovernorRole}
                    }
                });
            mockControllerContext.SetupGet(c => c.RouteData)
                .Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));

            var result = await controller.AddEditOrReplace(null, estabUrn, newGovernorRole, null);

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

        [Theory()]
        [MemberData(nameof(ForbiddenCombinationsOfGovernanceProfessionalRoles))]
        public async Task Gov_AddEditOrReplace_RoleSpecified_GovernanceProfessional_RoleAlreadyExists_DisallowedThereforeReject(eLookupGovernorRole preExistingGovernorRole, eLookupGovernorRole newGovernorRole)
        {

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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(governorsDetails);

            var result = await controller.RoleAllowed(newGovernorRole, null, null, null, false);

            Assert.False(result);
        }

        [Fact()]
        public async Task Gov_AddEditOrReplace_Create_Defaults_OriginalChairOfTrustees_ToNo()
        {
            // Arrange
            var groupId = 42;

            var governorDetailsDto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>(),
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.ChairOfTrustees },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.ChairOfTrustees, new GovernorDisplayPolicy() }
                }
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);

            mockControllerContext
                .SetupGet(c => c.RouteData)
                .Returns(new RouteData(
                    new Route("", new PageRouteHandler("~/")),
                    new PageRouteHandler("~/")));

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
            var result = await controller.AddEditOrReplace(groupId, null, eLookupGovernorRole.ChairOfTrustees, null);

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

            var governor = new GovernorModel
            {
                Id = governorId,
                RoleId = (int) eLookupGovernorRole.ChairOfTrustees,
                IsOriginalSignatoryMember = true,
                IsOriginalChairOfTrustees = true
            };

            mockControllerContext
                .SetupGet(c => c.RouteData)
                .Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));

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
            var result = await controller.AddEditOrReplace(null, estabUrn, null, governorId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<CreateEditGovernorViewModel>(viewResult.Model);

            Assert.True(vm.IsOriginalSignatoryMember);
            Assert.True(vm.IsOriginalChairOfTrustees);

            // Do NOT assert vm.DisplayPolicy.AppointingBodyId (frontend-only rule - JS)
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_EditMode_MapsFieldsCorrectly()
        {
            // Arrange
            var estabUrn = 1010;

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
                .Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), It.IsAny<bool>(), It.IsAny<IPrincipal>()))
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

            // Act
            await controller.AddEditOrReplace(vm);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_ReplaceMode_SetsAppointmentStartDateCorrectly()
        {
            // Arrange
            var estabUrn = 2020;

            var endDate = new DateTime(2024, 6, 15);

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
                .Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), It.IsAny<bool>(), It.IsAny<IPrincipal>()))
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

            // Act
            await controller.AddEditOrReplace(vm);
        }

        [Fact]
        public async Task Gov_AddEditOrReplace_Post_DuplicateSharedGovernor_RedirectsToGroupEdit()
        {
            // Arrange
            var groupId = 900;

            var vm = new CreateEditGovernorViewModel
            {
                GroupUId = groupId,
                GovernorRole = eLookupGovernorRole.Establishment_SharedLocalGovernor, // shared role
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
                .Returns(Task.FromResult(new GovernorsDetailsDto
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
                }));

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Establishment_SharedLocalGovernor,
                    true,
                    It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ValidationEnvelopeDto()));

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse<int>(true) { Response = 999 }));

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
                .Returns(Task.FromResult(new GovernorsDetailsDto
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
                }));

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Establishment_SharedLocalGovernor,
                    true,
                    It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ValidationEnvelopeDto()));

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse<int>(true) { Response = 1000 }));

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

            // Display policy needed early in POST
            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.Governor,
                    false,          // groupUId.HasValue == false
                    It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            // Validation passes – so we enter the ModelState.IsValid block
            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ValidationEnvelopeDto())); // no errors

            // Save fails – triggers ErrorsToModelState
            var apiError = new ApiError
            {
                Fields = "Person_FirstName",
                Message = "First name is invalid"
            };

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse<int>(false)
                {
                    Errors = new[] { apiError }
                }));

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
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ValidationEnvelopeDto())); // no errors

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse<int>(true) { Response = 888 }));

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
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ValidationEnvelopeDto()));

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(oldGovernorId, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new GovernorModel { Id = oldGovernorId }));

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse<int>(true) { Response = 999 }));

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

            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.Governor,
                EstablishmentUrn = estabUrn,
                SelectedPreviousGovernorId = previousId,          // REQUIRED
                ReinstateAsGovernor = false,                      // IMPORTANT for this test
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
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            // Validation passes
            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ValidationEnvelopeDto()));

            // Main save succeeds
            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse<int>(true) { Response = 999 }));

            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse(true)));

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
            // We should have retired the previous governor with the replacement end date
            mockGovernorsWriteService.Verify(
                g => g.UpdateDatesAsync(previousId, replacementEndDate, It.IsAny<IPrincipal>()),
                Times.Once);

            // No extra save for ReInstateChairAsNonChairAsync (only main SaveAsync)
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
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ValidationEnvelopeDto()));

            mockGovernorsWriteService
                .SetupSequence(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse<int>(true) { Response = 500 })) // first save
                .Returns(Task.FromResult(new ApiResponse<int>(true) { Response = 501 })); // second save (reinstated governor)

            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse(true)));

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(oldChairId, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new GovernorModel
                {
                    Id = oldChairId,
                    AppointmentEndDate = oldChairEndDate
                }));

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

        [Theory()]
        [InlineData(null, 153513)]
        [InlineData(16851, null)]
        public async Task Gov_Post_AddEditOrReplace_ApiError(int? estabId, int? groupId)
        {
            var errorKey = "test";
            var errorMessage = "test message";

            mockGovernorsReadService.Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), It.IsAny<bool>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<CreateEditGovernorViewModel>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsWriteService
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ValidationEnvelopeDto()
                {
                    Errors =
                    {
                        new ApiError {Code = "Test", Fields = errorKey, Message = errorMessage}
                    }
                });

            var result = await controller.AddEditOrReplace(new CreateEditGovernorViewModel { EstablishmentUrn = estabId, GroupUId = groupId });

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var model = viewResult.Model as CreateEditGovernorViewModel;
            Assert.NotNull(model);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(errorKey));
            Assert.Single(viewResult.ViewData.ModelState[errorKey].Errors);
            Assert.Equal(errorMessage, viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    controller.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
