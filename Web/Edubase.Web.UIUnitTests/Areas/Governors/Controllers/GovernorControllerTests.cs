using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
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
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Establishment_SharedLocalGovernor, new GovernorDisplayPolicy() }
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

            mockCachedLookupService.Setup(c => c.TitlesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Code = "04", Id = 5, Name = "Dr" },
                new LookupDto { Code = "05", Id = 6, Name = "Prof" },
                new LookupDto { Code = "14", Id = 15, Name = "Captain" }
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
                    theoryData.Add((eLookupGovernorRole)combination[0], (eLookupGovernorRole)combination[1]);
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


        [Fact]
        public async Task Gov_Edit_ShouldNotDuplicateGovernors_WhenLocalAndSharedExistForSamePerson()
        {
            // Arrange
            var estabId = 123456;

            var samePerson_GroupShared = new GovernorModel
            {
                Id = 2001,
                RoleId = (int) eLookupGovernorRole.Group_SharedLocalGovernor,
                Person_FirstName = "Alex",
                Person_MiddleName = "J",
                Person_LastName = "Taylor",
                DOB = new DateTime(1980, 1, 1),
                Person_TitleId = 1
            };

            var samePerson_EstabShared = new GovernorModel
            {
                Id = 2002,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedLocalGovernor,
                Person_FirstName = "Alex",
                Person_MiddleName = "J",
                Person_LastName = "Taylor",
                DOB = new DateTime(1980, 1, 1),
                Person_TitleId = 1
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.LocalGovernor,
                    eLookupGovernorRole.Group_SharedLocalGovernor,
                    eLookupGovernorRole.Establishment_SharedLocalGovernor
                },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Group_SharedLocalGovernor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Establishment_SharedLocalGovernor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel> { samePerson_GroupShared, samePerson_EstabShared },
                HistoricalGovernors = new List<GovernorModel>()
            };

            WireEdit(estabId, dto);
            var controller = BuildController();

            // Act
            var actionResult = await controller.Edit(null, estabId, null, null);
            var viewResult = Assert.IsType<ViewResult>(actionResult);
            var model = Assert.IsType<GovernorsGridViewModel>(viewResult.Model);

            // Assert 
            var allRows = model.Grids.SelectMany(g => g.Rows).ToList();
            Assert.Equal(2, allRows.Count);
            Assert.Contains(allRows, r => ((GovernorModel) r.Model).RoleId == (int) eLookupGovernorRole.Group_SharedLocalGovernor);
            Assert.Contains(allRows, r => ((GovernorModel) r.Model).RoleId == (int) eLookupGovernorRole.Establishment_SharedLocalGovernor);
        }

        [Fact]
        public async Task Gov_Edit_ShouldCreateOneGridPerRoleId()
        {
            var estabId = 1001;

            var gLocal = new GovernorModel { Id = 1, RoleId = (int) eLookupGovernorRole.LocalGovernor, Person_FirstName = "A" };
            var gShared = new GovernorModel { Id = 2, RoleId = (int) eLookupGovernorRole.Group_SharedLocalGovernor, Person_FirstName = "B" };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.LocalGovernor, eLookupGovernorRole.Group_SharedLocalGovernor
                },
                CurrentGovernors = new List<GovernorModel> { gLocal, gShared },
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Group_SharedLocalGovernor, new GovernorDisplayPolicy() }
                }
            };

            WireEdit(estabId, dto);
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            Assert.Equal(2, vm.Grids.Count);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor).Rows);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.Group_SharedLocalGovernor).Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldNotBleedRowsAcrossGrids()
        {
            var estabId = 1002;

            var gShared = new GovernorModel
            {
                Id = 10,
                RoleId = (int) eLookupGovernorRole.Group_SharedLocalGovernor,
                Person_FirstName = "Alex"
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.LocalGovernor, eLookupGovernorRole.Group_SharedLocalGovernor
                },
                CurrentGovernors = new List<GovernorModel> { gShared },
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Group_SharedLocalGovernor, new GovernorDisplayPolicy() }
                }
            };

            WireEdit(estabId, dto);
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            Assert.Equal(2, vm.Grids.Count);
            Assert.Empty(vm.Grids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor).Rows);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.Group_SharedLocalGovernor).Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldRenderHistoricGrids_OnePerRole()
        {
            var estabId = 1003;

            var h1 = new GovernorModel { Id = 100, RoleId = (int) eLookupGovernorRole.Governor, Person_FirstName = "H1" };
            var h2 = new GovernorModel { Id = 101, RoleId = (int) eLookupGovernorRole.LocalGovernor, Person_FirstName = "H2" };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.Governor, eLookupGovernorRole.LocalGovernor },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel> { h1, h2 },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() }
                }
            };

            WireEdit(estabId, dto);
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            Assert.Equal(2, vm.HistoricGrids.Count);
            Assert.Single(vm.HistoricGrids.Single(g => g.Role == eLookupGovernorRole.Governor).Rows);
            Assert.Single(vm.HistoricGrids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor).Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldThrow_WhenDisplayPolicyMissingForRole()
        {
            var estabId = 1004;

            var g = new GovernorModel
            {
                Id = 7,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedLocalGovernor,
                Person_FirstName = "P"
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.Establishment_SharedLocalGovernor },
                CurrentGovernors = new List<GovernorModel> { g },
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>() // Missing policy
            };

            WireEdit(estabId, dto);
            var controller = BuildController();

            await Assert.ThrowsAsync<Exception>(async () => await controller.Edit(null, estabId, null, null));
        }

        [Fact]
        public async Task Gov_Edit_ShouldRender_WhenAllDisplayPoliciesPresent()
        {
            var estabId = 1005;

            var g = new GovernorModel
            {
                Id = 8,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedLocalGovernor,
                Person_FirstName = "P"
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.Establishment_SharedLocalGovernor },
                CurrentGovernors = new List<GovernorModel> { g },
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.Establishment_SharedLocalGovernor, new GovernorDisplayPolicy() }
                }
            };

            WireEdit(estabId, dto);
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            Assert.Single(vm.Grids);
            Assert.Single(vm.Grids[0].Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldPopulateLayoutProperties()
        {
            var estabId = 1006;

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>(),
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>()
            };

            WireEdit(estabId, dto);
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            Assert.IsType<ViewResult>(result);

            mockLayoutHelper.Verify(l => l.PopulateLayoutProperties(
                It.IsAny<GovernorsGridViewModel>(),
                estabId,
                null,
                It.IsAny<IPrincipal>(),
                It.IsAny<Action<Edubase.Services.Establishments.Models.EstablishmentModel>>(),
                It.IsAny<Action<Edubase.Services.Groups.Models.GroupModel>>()), Times.Once);
        }

        [Fact]
        public async Task Gov_ReplaceChair_Get_ShouldRender_WithPolicies()
        {
            // Arrange
            var estabId = 2001;
            var gid = 3001;

            var chair = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.Governor },
                CurrentGovernors = new List<GovernorModel>(),      
                HistoricalGovernors = new List<GovernorModel>(),    
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                }
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(chair);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>()); 

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(
                It.IsAny<ReplaceChairViewModel>(),
                estabId,
                null,
                It.IsAny<IPrincipal>(),
                It.IsAny<Action<Edubase.Services.Establishments.Models.EstablishmentModel>>(),
                It.IsAny<Action<Edubase.Services.Groups.Models.GroupModel>>()))
                .Returns(Task.CompletedTask);

            var controller = BuildController();

            // Act
            var result = await controller.ReplaceChair(estabId, gid);
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ReplaceChairViewModel>(viewResult.Model);

            // Assert
            Assert.Equal(gid, vm.ExistingGovernorId);
            Assert.Equal((eLookupGovernorRole) chair.RoleId, vm.Role);
        }

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

            mockGovernorsReadService.Setup(s =>
                s.GetGovernorAsync(newGovId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = newGovId,
                    RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                    AppointmentEndDate = DateTime.Today.AddYears(1),
                    AppointmentStartDate = DateTime.Today.AddYears(-2),
                    Appointments = new[]
                    {
                new GovernorAppointment
                {
                    EstablishmentUrn = estabUrn,
                    AppointmentStartDate = DateTime.Today.AddYears(-2),
                    AppointmentEndDate = DateTime.Today.AddYears(1)
                }
                    }
                });

            mockGovernorsReadService.Setup(s =>
                s.GetGovernorAsync(oldGovId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = oldGovId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = DateTime.Today
                });

            mockGovernorsWriteService.Setup(s =>
                s.UpdateDatesAsync(oldGovId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

            // REQUIRED mocks for ReplaceChair POST
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

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync()).ReturnsAsync(new List<LookupDto>());

            var controller = BuildController(); // uses FakeUrlHelper or stub URL

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

            // mock list + permission
            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabId, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorPermissions(estabId, groupId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorPermissions());

            // delete call
            mockGovernorsWriteService
                .Setup(s => s.DeleteAsync(governorId, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new ApiResponse(true)));

            var controller = BuildController();

            // IMPORTANT: we must set GovernorShared = false and use Action="Remove"
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

            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(newGovId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = newGovId,
                    RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                    AppointmentEndDate = DateTime.Now.AddYears(1)
                });

            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(govId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = govId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = DateTime.Now
                });

            mockGovernorsWriteService
                .Setup(g => g.UpdateDatesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse(true));

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

            mockGovernorsWriteService.Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ValidationEnvelopeDto { Errors = new List<ApiError>() });

            mockGovernorsWriteService.Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(),
                It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<int>(true));

            mockGovernorsWriteService.Setup(a =>
                    a.SaveAsync(It.Is<GovernorModel>(g => g.RoleId == (int) eLookupGovernorRole.LocalGovernor),
                        It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ApiResponse<int>(true));

            mockGovernorsReadService.Setup(g => g.GetGovernorAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Person_FirstName = "Tom",
                    Person_LastName = "Smith",
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = DateTime.Today.AddMonths(6),
                    DOB = DateTime.Today.AddYears(-47)
                });

            var result = await controller.ReplaceChair(model);
            var redirectResult = Assert.IsType<RedirectResult>(result);
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

        // Only a single chair of a local governing body may be attached (either directly, or via shared role)
        [Fact]
        public async Task RoleAllowed_NewLocalChair_Permitted_WhenNoLocalOrSharedChair()
        {
            var currentGovernors = new List<GovernorModel> { };
            var newGovernorRole = eLookupGovernorRole.ChairOfLocalGoverningBody;

            await AssertAddingNewRoleIsPermitted(currentGovernors, newGovernorRole);
        }

        // #231733: Adding multiple `Shared governance professional - group` is now permitted.
        [Fact]
        public async Task RoleAllowed_NewSharedGovProGroup_Permitted_WhenPreExistingSharedGovProGroup()
        {
            var currentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int)eLookupGovernorRole.Group_SharedGovernanceProfessional }, };
            var newGovernorRole = eLookupGovernorRole.Group_SharedGovernanceProfessional;

            await AssertAddingNewRoleIsPermitted(currentGovernors, newGovernorRole);
        }

        // Only a single chair of a local governing body may be attached (either directly, or via shared role)
        [Fact]
        public async Task RoleAllowed_NewSharedChairGroup_Permitted_WhenNoLocalOrSharedChairGroup()
        {
            var currentGovernors = new List<GovernorModel> { };
            var newGovernorRole = eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody;

            await AssertAddingNewRoleIsPermitted(currentGovernors, newGovernorRole);
        }

        // Only a single chair of a local governing body may be attached (either directly, or via shared role)
        [Fact]
        public async Task RoleAllowed_NewLocalChair_Forbidden_WhenPreexistingLocalChair()
        {
            var currentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int)eLookupGovernorRole.ChairOfLocalGoverningBody }, };
            var newGovernorRole = eLookupGovernorRole.ChairOfLocalGoverningBody;

            await AssertAddingNewRoleIsForbidden(currentGovernors, newGovernorRole);
        }

        // Additional single chair of a local governing body may be attached (either directly, or via shared role)
        [Fact]
        public async Task RoleAllowed_NewSharedChairGroup_Allowed_WhenPreexistingLocalChair()
        {
            var currentGovernors = new List<GovernorModel>
            {
                new GovernorModel { RoleId = (int)eLookupGovernorRole.ChairOfLocalGoverningBody },
            };
            var newGovernorRole = eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody;

            await AssertAddingNewRoleIsPermitted(currentGovernors, newGovernorRole);
        }


        // Only a single chair of a local governing body may be attached (either directly, or via shared role)
        [Fact]
        public async Task RoleAllowed_NewLocalChair_Forbidden_WhenPreexistingSharedChairGroup()
        {
            var currentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int)eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody }, };
            var newGovernorRole = eLookupGovernorRole.ChairOfLocalGoverningBody;

            await AssertAddingNewRoleIsForbidden(currentGovernors, newGovernorRole);
        }


        [Theory]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, eLookupGovernorRole.Group_SharedGovernanceProfessional)]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, eLookupGovernorRole.GovernanceProfessionalToAMat)]
        public async Task RoleAllowed_ShouldReturnTrue_WhenEither_SharedGovernanceProfessionalGroup_or_MAT_added_AndOtherExists(eLookupGovernorRole firstGovernanceProfessional, eLookupGovernorRole secondGovernanceProfessional)
        {
            var currentGovernors = new List<GovernorModel>
            {
                new GovernorModel { RoleId = (int)firstGovernanceProfessional }
            };
            var newGovernorRole = secondGovernanceProfessional;

            await AssertAddingNewRoleIsPermitted(currentGovernors, newGovernorRole);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool)]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, eLookupGovernorRole.GovernanceProfessionalToASat)]
        public async Task RoleAllowed_ShouldReturnFalse_WhenEither_SharedGovernanceProfessionalSAT_or_FreeSchool_added_AndOtherExists(eLookupGovernorRole firstGovernanceProfessional, eLookupGovernorRole secondGovernanceProfessional)
        {
            var currentGovernors = new List<GovernorModel>
            {
                new GovernorModel { RoleId = (int)firstGovernanceProfessional }
            };
            var newGovernorRole = secondGovernanceProfessional;

            await AssertAddingNewRoleIsPermitted(currentGovernors, newGovernorRole);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, eLookupGovernorRole.GovernanceProfessionalToAMat)]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, eLookupGovernorRole.GovernanceProfessionalToASat)]
        public async Task RoleAllowed_ShouldReturnFalse_When_MAT_AddedAndOtherExists(eLookupGovernorRole firstGovernanceProfessional, eLookupGovernorRole secondGovernanceProfessional)
        {
            var currentGovernors = new List<GovernorModel>
            {
                new GovernorModel { RoleId = (int)firstGovernanceProfessional }
            };
            var newGovernorRole = secondGovernanceProfessional;

            await AssertAddingNewRoleIsForbidden(currentGovernors, newGovernorRole);
        }


        private async Task AssertAddingNewRoleIsForbidden(List<GovernorModel> currentGovernors,
            eLookupGovernorRole newGovernorRole)
        {
            await AssertAddingNewRoleAcceptedOrForbidden(currentGovernors, newGovernorRole, false);
        }

        private async Task AssertAddingNewRoleIsPermitted(List<GovernorModel> currentGovernors,
            eLookupGovernorRole newGovernorRole)
        {
            await AssertAddingNewRoleAcceptedOrForbidden(currentGovernors, newGovernorRole, true);
        }

        private async Task AssertAddingNewRoleAcceptedOrForbidden(List<GovernorModel> currentGovernors, eLookupGovernorRole newGovernorRole,
            bool expectedResult)
        {
            var applicableRoles = new List<eLookupGovernorRole> { newGovernorRole };
            applicableRoles.AddRange(currentGovernors.Select(g => (eLookupGovernorRole)g.RoleId));

            var governorsDetails = new GovernorsDetailsDto
            {
                CurrentGovernors = currentGovernors,
                ApplicableRoles = applicableRoles,
                HistoricalGovernors = new List<GovernorModel>(),
                HasFullAccess = true
            };

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(governorsDetails);

            var actualResult = await controller.RoleAllowed(newGovernorRole, null, null, null, false);

            Assert.Equal(expectedResult, actualResult);
        }

        private GovernorController BuildController()
        {
            var controller = new GovernorController(
                mockGovernorsReadService.Object,
                mockCachedLookupService.Object,
                mockGovernorsWriteService.Object,
                mockGroupReadService.Object,
                mockEstablishmentReadService.Object,
                mockLayoutHelper.Object);

            // -----------------------------
            // HttpContext + HttpRequest
            // -----------------------------
            var httpContext = new Mock<HttpContextBase>();
            var httpRequest = new Mock<HttpRequestBase>();

            httpRequest.Setup(r => r.QueryString)
                .Returns(new NameValueCollection());

            // ⭐ REQUIRED FOR MVC URL GENERATION
            httpRequest.Setup(r => r.ApplicationPath).Returns("/");
            httpRequest.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/");
            httpRequest.Setup(r => r.Path).Returns("/");
            httpRequest.Setup(r => r.RawUrl).Returns("/");
            httpRequest.Setup(r => r.PhysicalApplicationPath).Returns("C:\\FakeApp\\");

            httpContext.Setup(c => c.Request).Returns(httpRequest.Object);

            var routeData = new RouteData();
            var requestContext = new RequestContext(httpContext.Object, routeData);

            controller.ControllerContext = new ControllerContext(requestContext, controller);

            RouteTable.Routes.Clear();

            RouteTable.Routes.MapRoute(
                name: "EstabDetails",
                url: "establishment/{id}",
                defaults: new { controller = "Establishment", action = "Details" }
            );

            RouteTable.Routes.MapRoute(
                name: "GroupDetails",
                url: "group/{id}",
                defaults: new { controller = "Group", action = "Details" }
            );

            controller.Url = new FakeUrlHelper();

            return controller;
        }

        private void WireEdit(int estabId, GovernorsDetailsDto dto)
        {
            // Data + permissions
            mockGovernorsReadService.Setup(s => s.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);
            mockGovernorsReadService.Setup(s => s.GetGovernorPermissions(estabId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorPermissions());

            // Lookups used by Edit()
            mockCachedLookupService.Setup(s => s.GovernorRolesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            // Layout
            mockLayoutHelper.Setup(l => l.PopulateLayoutProperties(
                It.IsAny<GovernorsGridViewModel>(),
                estabId,
                null,
                It.IsAny<IPrincipal>(),
                It.IsAny<Action<Edubase.Services.Establishments.Models.EstablishmentModel>>(),
                It.IsAny<Action<Edubase.Services.Groups.Models.GroupModel>>()))
                .Returns(Task.CompletedTask);
        }

        internal sealed class FakeUrlHelper : UrlHelper
        {
            public FakeUrlHelper() : base(new RequestContext(new FakeHttpContext(), new RouteData())) { }

            public override string RouteUrl(string routeName, object routeValues)
            {
                return "/fake-url";
            }
        }

        internal sealed class FakeHttpContext : HttpContextBase
        {
            private readonly HttpRequestBase _request = new FakeHttpRequest();
            public override HttpRequestBase Request => _request;
        }

        internal sealed class FakeHttpRequest : HttpRequestBase
        {
            public override string ApplicationPath => "/";
            public override string AppRelativeCurrentExecutionFilePath => "~/";
            public override string Path => "/";
            public override string RawUrl => "/";
        }
    }
}
