using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
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
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UIUnitTests;
using Moq;
using Xunit;

namespace Edubase.Web.UI.Areas.Governors.Controllers.UnitTests
{
    public class GovernorControllerTests: IDisposable
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

        public GovernorControllerTests()
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
        public async Task Gov_Edit_Null_Params()
        {
            await Assert.ThrowsAsync<InvalidParameterException>(() => controller.Edit(null, null, null, null));
        }

        [Fact()]
        public async Task Gov_Edit_GroupIdSpecified()
        {
            var groupId = 5;
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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(
                It.IsAny<GovernorsGridViewModel>(),
                null,
                groupId,
                It.IsAny<IPrincipal>(),
                It.IsAny<Action<EstablishmentModel>>(),
                It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync())
                .ReturnsAsync(() => new List<LookupDto>
                {
                    new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                    new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
                });

            var result = await controller.Edit(5, null, null, null);

            var viewResult = result as ViewResult;

            var model = viewResult?.Model as GovernorsGridViewModel;

            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.False(model.GovernorShared);
            Assert.Null(model.RemovalGid);
            Assert.Equal(groupId, model.GroupUId);
            Assert.Null(model.EstablishmentUrn);

            Assert.Equal(governorDetailsDto.ApplicableRoles.Count, model.GovernorRoles.Count);

            Assert.False(viewResult.ViewData.Keys.Contains("DuplicateGovernor"));
            Assert.True(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact()]
        public async Task Gov_Edit_EstabIdSpecified()
        {
            var establishmentId = 23;
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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(establishmentId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(establishmentId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            mockLayoutHelper.Setup(
                l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(),
                establishmentId,
                null,
                It.IsAny<IPrincipal>(),
                It.IsAny<Action<EstablishmentModel>>(),
                It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await controller.Edit(null, establishmentId, null, null);

            var viewResult = result as ViewResult;
            var model = viewResult?.Model as GovernorsGridViewModel;

            Assert.NotNull(viewResult);
            Assert.NotNull(model);

            Assert.False(model.GovernorShared);
            Assert.Null(model.RemovalGid);
            Assert.Null(model.GroupUId);
            Assert.Equal(establishmentId, model.EstablishmentUrn);

            Assert.Equal(governorDetailsDto.ApplicableRoles.Count, model.GovernorRoles.Count);

            Assert.False(viewResult.ViewData.Keys.Contains("DuplicateGovernor"));
            Assert.True(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact()]
        public async Task Gov_Edit_GroupId_RemovalGid_GidExists()
        {
            var groupId = 5;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>
                {
                    new GovernorModel
                    {
                        Id = 43,
                        RoleId = (int)eLookupGovernorRole.Establishment_SharedLocalGovernor
                    }
                },
                HistoricalGovernors = new List<GovernorModel>()
            };

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(),
                null, groupId, It.IsAny<IPrincipal>(),
                It.IsAny<Action<EstablishmentModel>>(),
                It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);


            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await controller.Edit(5, null, 43, null);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as GovernorsGridViewModel;

            Assert.NotNull(viewResult);
            Assert.NotNull(model);

            Assert.True(model.GovernorShared);
            Assert.Equal(43, model.RemovalGid);
            Assert.Equal(groupId, model.GroupUId);

            Assert.Equal(governorDetailsDto.ApplicableRoles.Count, model.GovernorRoles.Count);

            Assert.False(viewResult.ViewData.Keys.Contains("DuplicateGovernor"));
            Assert.True(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact()]
        public async Task Gov_Edit_GroupId_RemovalGid_GidDoesNotExist()
        {
            var groupId = 5;
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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(
                It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await controller.Edit(5, null, 43, null);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as GovernorsGridViewModel;

            Assert.NotNull(viewResult);
            Assert.NotNull(model);

            Assert.False(model.GovernorShared);
            Assert.Equal(43, model.RemovalGid);
            Assert.Equal(groupId, model.GroupUId);

            Assert.Equal(governorDetailsDto.ApplicableRoles.Count, model.GovernorRoles.Count);

            Assert.False(viewResult.ViewData.Keys.Contains("DuplicateGovernor"));
            Assert.True(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact()]
        public async Task Gov_Edit_GroupId_DuplicateGovernorId()
        {
            var groupId = 5;
            var duplicateId = 13;
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
                Id = duplicateId
            };

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(duplicateId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governor);
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(
                It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await controller.Edit(5, null, null, duplicateId);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as GovernorsGridViewModel;

            Assert.NotNull(viewResult);
            Assert.NotNull(model);

            Assert.False(model.GovernorShared);
            Assert.Null(model.RemovalGid);
            Assert.Equal(groupId, model.GroupUId);

            Assert.Equal(governorDetailsDto.ApplicableRoles.Count, model.GovernorRoles.Count);

            Assert.True(viewResult.ViewData.Keys.Contains("DuplicateGovernor"));
            Assert.Equal(governor, viewResult.ViewData["DuplicateGovernor"]);
            Assert.True(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact()]
        public async Task Gov_Edit_RoleExists()
        {
            var establishmentId = 23;
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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(establishmentId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(establishmentId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(
                It.IsAny<GovernorsGridViewModel>(), establishmentId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await controller.Edit(null, establishmentId, null, null, true);

            var viewResult = result as ViewResult;
            var model = viewResult?.Model as GovernorsGridViewModel;


            Assert.NotNull(viewResult);
            Assert.NotNull(model);

            Assert.False(model.GovernorShared);
            Assert.Null(model.RemovalGid);
            Assert.Null(model.GroupUId);
            Assert.Equal(establishmentId, model.EstablishmentUrn);

            Assert.Equal(governorDetailsDto.ApplicableRoles.Count, model.GovernorRoles.Count);

            Assert.False(viewResult.ViewData.Keys.Contains("DuplicateGovernor"));
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Single(viewResult.ViewData.ModelState["role"].Errors);
        }

        [Fact()]
        public void Gov_AddEditOrReplace_NullParams()
        {
            mockControllerContext.SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));
            Assert.ThrowsAsync<EdubaseException>(() => controller.AddEditOrReplace(null, null, null, null));
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

        /// <summary>
        /// Every chair of local governing body role, combined with every chair of local governing body role.
        /// At the time of writing, equivalent to:
        ///     [InlineData( eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody,          eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody          )]
        ///     [InlineData( eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody,          eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody  )]
        ///     [InlineData( eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,  eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody          )]
        ///     [InlineData( eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,  eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody  )]
        /// </summary>
        public static IEnumerable<object[]> PairwiseChairOfLocalGoverningBodyRoles =>
        (
            from preExistingRole in EnumSets.eChairOfLocalGoverningBodyRoles
            from newRole in EnumSets.eChairOfLocalGoverningBodyRoles
            select new object[] { preExistingRole, newRole }
        );

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

            // Expecting to redirect back, rejecting the proposed add/edit
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);
            Assert.Equal("EstabEditGovernance", redirectResult.RouteName);
        }


        /// <summary>
        /// Every governance professional role, combined with every type of governance professional.
        /// </summary>
        public static IEnumerable<object[]> PairwiseGovernanceProfessionalRoles =>
        (
            from preExistingRole in EnumSets.eGovernanceProfessionalRoles
            from newRole in EnumSets.eGovernanceProfessionalRoles
            select new object[] { preExistingRole, newRole }
        );


        public static IEnumerable<object[]> PairwiseGovernanceProfessionalRolesAllowedCombinations
        {
            get
            {
                var allData = new List<object[]>
                {
                    // - #198772 / #193913 : MAT can have "one-each" of "Shared governance professional - group" and "Governance professional to a multi-academy trust (MAT)"
                    new object[] {eLookupGovernorRole.Group_SharedGovernanceProfessional, eLookupGovernorRole.GovernanceProfessionalToAMat},
                    new object[] {eLookupGovernorRole.GovernanceProfessionalToAMat, eLookupGovernorRole.Group_SharedGovernanceProfessional},

                    // - #198772 / #197361 : Establishment within a SAT can have "one-each" of "Governance professional to an individual academy or free school" and "Governance professional for single-academy trust (SAT)"
                    new object[] {eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, eLookupGovernorRole.GovernanceProfessionalToASat},
                    new object[] {eLookupGovernorRole.GovernanceProfessionalToASat, eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool},

                    // - #198239: System should allow adding a Governance professional to a federation if a record for Governance professional to a local authority maintained is already recorded.
                    new object[] {eLookupGovernorRole.GovernanceProfessionalToAFederation, eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool},
                    new object[] {eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, eLookupGovernorRole.GovernanceProfessionalToAFederation},
                };
                return allData;
            }
        }

        public static IEnumerable<object[]> ForbiddenCombinationsofGovernanceProfessionalRoles => PairwiseGovernanceProfessionalRoles
            .Where(allPairsPair => !PairwiseGovernanceProfessionalRolesAllowedCombinations.Any(innerPair =>
                allPairsPair[0].Equals(innerPair[0])
                && allPairsPair[1].Equals(innerPair[1])));


        [Theory()]
        [MemberData(nameof(ForbiddenCombinationsofGovernanceProfessionalRoles))]
        public async Task Gov_AddEditOrReplace_RoleSpecified_GovernanceProfessional_RoleAlreadyExists_DisallowedThereforeReject(eLookupGovernorRole preExistingGovernorRole, eLookupGovernorRole newGovernorRole)
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

            // original requirement: Only a single governance professional may be attached
            // Expecting to redirect back, rejecting the proposed add/edit
            var redirectResult = result as RedirectToRouteResult;

            Assert.NotNull(redirectResult);
            Assert.Equal("EstabEditGovernance", redirectResult.RouteName);
        }


        [Fact()]
        public async Task Gov_DeleteOrRetireGovernor_NoAction()
        {
            await Assert.ThrowsAsync<InvalidParameterException>(() => controller.DeleteOrRetireGovernor(new GovernorsGridViewModel()));
        }

        [Fact()]
        public async Task Gov_DeleteOrRetireGovernor_Save_ApiError()
        {
            var groupId = 2436;
            var governorId = 6224;
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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });

            mockCachedLookupService.Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(false)
                {
                    Errors = new[]
                    {
                        new ApiError {Code = "Test", Fields = errorKey, Message = errorMessage}
                    }
                });

            var result = await controller.DeleteOrRetireGovernor(new GovernorsGridViewModel { GroupUId = groupId, Action = "Save", RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now), RemovalGid = governorId });

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Single(viewResult.ViewData.ModelState);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(errorKey));
            Assert.Single(viewResult.ViewData.ModelState[errorKey].Errors);
            Assert.Equal(errorMessage, viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage);
        }

        [Fact()]
        public async Task Gov_DeleteOrRetireGovernor_Save_UnknownApiError()
        {
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

            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(false)
                {
                    Errors = new ApiError[0]
                });

            await Assert.ThrowsAsync<TexunaApiSystemException>(() =>
            controller.DeleteOrRetireGovernor(new GovernorsGridViewModel {
                GroupUId = groupId, Action = "Save", RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now), RemovalGid = governorId
            }));
        }

        [Theory()]
        [InlineData(16513, null)]
        [InlineData(null, 81681)]
        public async Task Gov_DeleteOrRetireGovernor_Group_Save_OK(int? estabId, int? groupId)
        {
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

        [Fact()]
        public async Task Gov_DeleteOrRetireGovernor_SharedGov_Save_ApiError()
        {
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

        [Fact()]
        public async Task Gov_DeleteOrRetireGovernor_SharedGov_Save_Estab_OK()
        {
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

        [Fact()]
        public async Task Gov_DeleteOrRetireGovernor_Estab_Remove_Shared_OK()
        {
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

        [Fact()]
        public async Task Gov_DeleteOrRetireGovernor_Estab_Remove_NonShared_OK()
        {
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

        [Fact()]
        public async Task Gov_Get_ReplaceChair()
        {
            var estabId = 364631;
            var gid = 135454;

            var governor = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
            };

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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            mockGovernorsReadService.Setup(g => g.GetSharedGovernorsAsync(estabId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new List<GovernorModel>());
            mockGovernorsReadService.Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<ReplaceChairViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);


            var result = await controller.ReplaceChair(estabId, gid);
            var viewResult = result as ViewResult;

            Assert.NotNull(viewResult);

            var model = viewResult.Model as ReplaceChairViewModel;
            Assert.NotNull(model); ;
            Assert.Equal(gid, model.ExistingGovernorId);
            Assert.Equal((eLookupGovernorRole) governor.RoleId, model.Role);
        }

        [Fact()]
        public async Task Gov_Put_ReplaceChair_NewChairShared()
        {
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

            mockGovernorsWriteService.Setup(g => g.AddSharedGovernorAppointmentAsync(newGovId, estabUrn, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse(true));

            var result = await controller.ReplaceChair(model);
            var redirectResult = result as RedirectResult;
            Assert.NotNull(redirectResult);
            Assert.Contains("#school-governance", redirectResult.Url);

            mockGovernorsWriteService.Verify(g => g.AddSharedGovernorAppointmentAsync(newGovId, estabUrn, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact()]
        public async Task Gov_Put_ReplaceChair_NewChairNotShared_NoValidationErrors()
        {
            var govId = 465134;
            var estabUrn = 16802;

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

            mockGovernorsWriteService.Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ValidationEnvelopeDto { Errors = new List<ApiError>() });
            mockGovernorsWriteService.Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse<int>(true));

            var result = await controller.ReplaceChair(model);
            var redirectResult = result as RedirectResult;
            Assert.NotNull(redirectResult);
            Assert.Contains("#school-governance", redirectResult.Url);
        }

        [Fact()]
        public async Task Gov_Put_ReplaceChair_NewChairNotShared_ValidationErrors()
        {
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

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);


            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(model.ExistingGovernorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => existingGov);
            mockGovernorsReadService.Setup(g => g.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>())).ReturnsAsync(() => new List<GovernorModel>());
            mockGovernorsReadService.Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(It.IsAny<ReplaceChairViewModel>(), estabUrn, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);


            var result = await controller.ReplaceChair(model);
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var modelResult = viewResult.Model as ReplaceChairViewModel;
            Assert.NotNull(modelResult);
            Assert.Equal(model, modelResult);
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
