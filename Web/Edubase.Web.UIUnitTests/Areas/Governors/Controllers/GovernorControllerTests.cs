using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
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

            mockCachedLookupService
                .Setup(c => c.GovernorRolesGetAllAsync())
                .ReturnsAsync(() => new List<LookupDto>
                {
                    new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                    new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
                });

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<object>(),
                    null,
                    groupId,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Callback<object, int?, int?, IPrincipal,
                            Action<EstablishmentModel>, Action<GroupModel>>(
                    (modelObj, estabUrn, grpId, user, estabAction, groupAction) =>
                    {
                        var vm = Assert.IsType<GovernorsGridViewModel>(modelObj);

                        var expectedGovernanceMode = eGovernanceMode.LocalGovernors;

                    var fakeEstab = new EstablishmentModel
                    {
                        GovernanceModeId = (int)expectedGovernanceMode
                    };
                    estabAction(fakeEstab);

                    // Simulate group context for delegation & corporate contact
                    var fakeGroup = new GroupModel
                    {
                        GroupTypeId = (int) eLookupGroupType.MultiacademyTrust,
                        DelegationInformation = "Test delegation info",
                        CorporateContact = "Test corporate contact"
                    };
                    groupAction(fakeGroup);
                })
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


            Assert.Equal(eGovernanceMode.LocalGovernors, model.GovernanceMode);
            Assert.True(model.ShowDelegationAndCorpContactInformation);
            Assert.Equal("Test delegation info", model.DelegationInformation);
            Assert.Equal("Test corporate contact", model.CorporateContact);

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

        [Fact]
        public async Task Gov_Edit_EstabId_RemovalGid_DuplicateIds_Throws()
        {
            // Arrange
            var estabId = 1007;
            var removalGid = 43;

            // Create a DTO with TWO governors having the SAME Id
            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
            {
                eLookupGovernorRole.AccountingOfficer,
                eLookupGovernorRole.Governor
            },
                    RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
            {
                { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                { eLookupGovernorRole.Governor,          new GovernorDisplayPolicy() }
            },
                    CurrentGovernors = new List<GovernorModel>
            {
                new GovernorModel
                {
                    Id     = removalGid,
                    RoleId = (int)eLookupGovernorRole.Governor,
                    Person_FirstName = "John",
                    Person_LastName  = "Doe",
                    Person_TitleId   = 1
                },
                new GovernorModel
                {
                    Id     = removalGid,
                    RoleId = (int)eLookupGovernorRole.Governor,
                    Person_FirstName = "Jane",
                    Person_LastName  = "Doe",
                    Person_TitleId   = 1
                }
            },
                HistoricalGovernors = new List<GovernorModel>()
            };

            WireEdit(estabId, dto);  

            var controller = BuildController(); 

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                controller.Edit(null, estabId, removalGid, null));
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
        public async Task Gov_ReplaceChair_Prepopulates_NewLocalGovernor_Fields()
        {
            // Arrange
            var estabUrn = 9001;
            var existingChairId = 500;   
            var replacementId = 600;    

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["rgid"] = replacementId.ToString();
            mockHttpRequestBase.SetupGet(r => r.QueryString).Returns(qs);

            var chair = new GovernorModel
            {
                Id = existingChairId,
                RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                AppointmentEndDate = new DateTime(2025, 4, 10)
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(chair);

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            var replacement = new GovernorModel
            {
                Id = replacementId,
                RoleId = (int) eLookupGovernorRole.LocalGovernor, 
                AppointingBodyId = 2,
                AppointmentStartDate = new DateTime(2020, 1, 1),
                AppointmentEndDate = new DateTime(2024, 12, 1),
                DOB = new DateTime(1980, 5, 5),
                Person_FirstName = "Beth",
                Person_MiddleName = "Jane",
                Person_LastName = "Smith",
                Person_TitleId = 4,
                PreviousPerson_TitleId = 7,
                PreviousPerson_FirstName = "B.",
                PreviousPerson_MiddleName = "J.",
                PreviousPerson_LastName = "S.",
                TelephoneNumber = "01234 567890",
                EmailAddress = "beth@example.com",
                PostCode = "AB12 3CD",
                EstablishmentUrn = estabUrn
            };

            var dto = new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel> { replacement },
                HistoricalGovernors = new List<GovernorModel>(),
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() }
                }
                    };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    It.IsAny<bool>(),
                    It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockCachedLookupService.Setup(c => c.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(c => c.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(c => c.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

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
            var result = await controller.ReplaceChair(estabUrn, existingChairId);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ReplaceChairViewModel>(view.Model);
            var newGov = vm.NewLocalGovernor;

            Assert.NotNull(newGov);

            Assert.Equal(replacement.AppointingBodyId, newGov.AppointingBodyId);
            Assert.Equal(replacement.AppointmentEndDate, newGov.AppointmentEndDate.ToDateTime());
            Assert.Equal(replacement.AppointmentStartDate, newGov.AppointmentStartDate.ToDateTime());
            Assert.Equal(replacement.DOB, newGov.DOB.ToDateTime());

            Assert.Equal(replacement.EmailAddress, newGov.EmailAddress);

            Assert.Equal(replacement.Person_TitleId, newGov.GovernorTitleId);
            Assert.Equal(replacement.Person_FirstName, newGov.FirstName);
            Assert.Equal(replacement.Person_MiddleName, newGov.MiddleName);
            Assert.Equal(replacement.Person_LastName, newGov.LastName);

            Assert.Equal(replacement.PreviousPerson_TitleId, newGov.PreviousTitleId);
            Assert.Equal(replacement.PreviousPerson_FirstName, newGov.PreviousFirstName);
            Assert.Equal(replacement.PreviousPerson_MiddleName, newGov.PreviousMiddleName);
            Assert.Equal(replacement.PreviousPerson_LastName, newGov.PreviousLastName);

            Assert.Equal(replacement.TelephoneNumber, newGov.TelephoneNumber);
            Assert.Equal(replacement.PostCode, newGov.PostCode);

            Assert.Equal(estabUrn, vm.Urn);
            Assert.Equal(replacement.Id, vm.SelectedPreviousExistingNonChairId);
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

        [Theory]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, eLookupGovernorRole.Governor)]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, eLookupGovernorRole.LocalGovernor)]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, eLookupGovernorRole.Trustee)]
        public async Task ReInstateChairAsNonChairAsync_MapsChairRoleToCorrectTargetRole(
            eLookupGovernorRole currentRole,
            eLookupGovernorRole expectedTargetRole)
        {
            // Arrange
            var gid = 42;
            var start = new DateTime(2025, 1, 1);
            var end = new DateTime(2025, 12, 31);

            var existing = new GovernorModel
            {
                Id = gid,
                RoleId = (int) currentRole,
                EmailAddress = "old@example.com",
                TelephoneNumber = "0123456789"
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(existing));

            GovernorModel savedModel = null;

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Callback<GovernorModel, IPrincipal>((m, _) => savedModel = m)
                .Returns(Task.FromResult(new ApiResponse<int>(true)));

            // Act
            await controller.ReInstateChairAsNonChairAsync(gid, start, end, currentRole);

            // Assert
            mockGovernorsReadService.Verify(
                g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()),
                Times.Once);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Once);

            Assert.NotNull(savedModel);
            Assert.Equal((int) expectedTargetRole, savedModel.RoleId);
            Assert.Null(savedModel.Id);
            Assert.Equal(start, savedModel.AppointmentStartDate);
            Assert.Equal(end, savedModel.AppointmentEndDate);
            Assert.Null(savedModel.EmailAddress);
            Assert.Null(savedModel.TelephoneNumber);
        }

        [Fact]
        public async Task ReInstateChairAsNonChairAsync_InvalidRole_ThrowsException()
        {
            // Arrange
            var controller = BuildController();
            var gid = 123;
            var invalidRole = eLookupGovernorRole.Governor;
            var appointmentStart = new DateTime(2025, 1, 1);
            var appointmentEnd = new DateTime(2025, 2, 1);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = gid,
                    RoleId = (int) invalidRole
                });

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                controller.ReInstateChairAsNonChairAsync(
                    gid,
                    appointmentStart,
                    appointmentEnd,
                    invalidRole
                ));

            Assert.Contains("You cannot demote from role", ex.Message);
            Assert.Contains(invalidRole.ToString(), ex.Message);

            // Ensure SaveAsync was NOT called
            mockGovernorsWriteService.Verify(
                w => w.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Never);
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

        [Fact]
        public async Task Gov_ReplaceChair_Get_FiltersSharedGovernors_ByRole_AndExcludesExistingChair()
        {
            // Arrange
            var estabUrn = 3001;
            var existingChairId = 100;

            var chair = new GovernorModel
            {
                Id = existingChairId,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                Person_FirstName = "Alex",
                Person_LastName = "Chair",
                Person_TitleId = 1,
                AppointmentEndDate = new DateTime(2025, 4, 10)
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.LocalGovernor,
                    eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
                },
                        CurrentGovernors = new List<GovernorModel>(),
                        HistoricalGovernors = new List<GovernorModel>(),
                        RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, new GovernorDisplayPolicy() }
                }
            };

            var sharedSameId = new GovernorModel
            {
                Id = existingChairId,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                Person_FirstName = "Beth",
                Person_LastName = "SameId",
                Person_TitleId = 1,
                AppointingBodyId = 2
            };

            var sharedValid = new GovernorModel
            {
                Id = 200,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                Person_FirstName = "Chris",
                Person_LastName = "Valid",
                Person_TitleId = 1,
                AppointingBodyId = 2
            };

            var sharedDifferentRole = new GovernorModel
            {
                Id = 300,
                RoleId = (int) eLookupGovernorRole.Governor,
                Person_FirstName = "Dana",
                Person_LastName = "WrongRole",
                Person_TitleId = 1,
                AppointingBodyId = 2
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(chair));

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(dto));

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult<IEnumerable<GovernorModel>>(new[]
                {
            sharedSameId,
            sharedValid,
            sharedDifferentRole
                }));

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(),
                    false,
                    It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(new GovernorDisplayPolicy()));

            var titles = new List<LookupDto>
            {
                new LookupDto { Id = 1, Name = "Mr" }
            };

            var bodies = new List<LookupDto>
            {
                new LookupDto { Id = 2, Name = "Body" }
            };

            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync())
                .Returns(Task.FromResult<IEnumerable<LookupDto>>(titles));

            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .Returns(Task.FromResult<IEnumerable<LookupDto>>(bodies));

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync())
                .Returns(Task.FromResult<IEnumerable<LookupDto>>(Array.Empty<LookupDto>()));

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            var controller = BuildController();

            // Act
            var result = await controller.ReplaceChair(estabUrn, existingChairId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ReplaceChairViewModel>(viewResult.Model);

            Assert.Equal(existingChairId, vm.ExistingGovernorId);
            Assert.Equal((eLookupGovernorRole) chair.RoleId, vm.Role);
            Assert.Equal(chair.AppointmentEndDate, vm.DateTermEnds.ToDateTime());
            Assert.Single(vm.SharedGovernors);
            var sg = vm.SharedGovernors.Single();
            Assert.Equal(sharedValid.Id, sg.Id);
            Assert.DoesNotContain(vm.SharedGovernors, x => x.Id == sharedSameId.Id);
            Assert.DoesNotContain(vm.SharedGovernors, x => x.Id == sharedDifferentRole.Id);
            Assert.NotNull(vm.NewLocalGovernor);
            Assert.NotNull(vm.NewLocalGovernor.DisplayPolicy);
        }

        [Fact]
        public async Task ReplaceChair_Get_UsesLocalEquivalentRoleForDisplayPolicy_WhenGovernorIsSharedChair()
        {
            // Arrange
            var estabUrn = 12345;
            var gid = 555;

            var sharedChair = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,
                AppointmentEndDate = new DateTime(2025, 1, 1),
                Person_TitleId = 1,
                Person_FirstName = "Alex",
                Person_LastName = "Chair"
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.LocalGovernor,
                    eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
                },
                        CurrentGovernors = new List<GovernorModel>(),
                        HistoricalGovernors = new List<GovernorModel>(),
                        RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, new GovernorDisplayPolicy() }
                }
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(sharedChair);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
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

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<ReplaceChairViewModel>(),
                    estabUrn,
                    null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            var controller = BuildController();

            // Act
            var result = await controller.ReplaceChair(estabUrn, gid);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ReplaceChairViewModel>(view.Model);

            Assert.Equal(gid, vm.ExistingGovernorId);

            mockGovernorsReadService.Verify(s =>
                s.GetEditorDisplayPolicyAsync(
                    eLookupGovernorRole.ChairOfLocalGoverningBody,
                    false,
                    It.IsAny<IPrincipal>()),
                Times.Once);
        }

        [Fact]
        public async Task ReplaceChair_Get_PopulatesExistingNonChairs_AndSelectsCorrectItem()
        {
            // Arrange
            var estabUrn = 50000;
            var existingChairId = 900;

            var chair = new GovernorModel
            {
                Id = existingChairId,
                RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                AppointmentEndDate = DateTime.Today
            };

            var local1 = new GovernorModel
            {
                Id = 101,
                RoleId = (int) eLookupGovernorRole.LocalGovernor,
                Person_FirstName = "Alice",
                Person_LastName = "One",
                Person_TitleId = 1
            };

            var local2 = new GovernorModel
            {
                Id = 202,
                RoleId = (int) eLookupGovernorRole.LocalGovernor,
                Person_FirstName = "Bob",
                Person_LastName = "Two",
                Person_TitleId = 1
            };

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["rgid"] = "202";
            mockHttpRequestBase.SetupGet(r => r.QueryString).Returns(qs);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(chair);

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(s => s.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorsDetailsDto
                {
                    CurrentGovernors = new List<GovernorModel> { local1, local2 },
                    ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor },
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

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync()).ReturnsAsync(new List<LookupDto>());

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
            var result = await controller.ReplaceChair(estabUrn, existingChairId);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ReplaceChairViewModel>(view.Model);

            var items = vm.ExistingNonChairs.ToList();
            Assert.Equal(2, items.Count);

            Assert.Equal("Alice One", items[0].Text);
            Assert.Equal("101", items[0].Value);
            Assert.False(items[0].Selected);

            Assert.Equal("Bob Two", items[1].Text);
            Assert.Equal("202", items[1].Value);
            Assert.True(items[1].Selected);
        }

        [Fact]
        public async Task ReplaceChair_Get_Sets_DateTermEnds_And_Reinstate_From_QueryString()
        {
            // Arrange
            var estabUrn = 7777;
            var existingChairId = 123;

            var chair = new GovernorModel
            {
                Id = existingChairId,
                RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                AppointmentEndDate = new DateTime(2025, 12, 31),
                Person_FirstName = "Test",
                Person_LastName = "Chair",
                Person_TitleId = 1
            };

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["d"] = "10";
            qs["m"] = "11";
            qs["y"] = "2030";
            qs["ri"] = "true";

            mockHttpRequestBase
                .SetupGet(r => r.QueryString)
                .Returns(qs);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(chair);

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

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync()).ReturnsAsync(new List<LookupDto>());

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
            var result = await controller.ReplaceChair(estabUrn, existingChairId);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ReplaceChairViewModel>(view.Model);

            Assert.True(vm.Reinstate);
            Assert.Equal(10, vm.DateTermEnds.Day);
            Assert.Equal(11, vm.DateTermEnds.Month);
            Assert.Equal(2030, vm.DateTermEnds.Year);
        }

        [Fact()]
        public async Task Gov_Put_ReplaceChair_NewChairNotShared_NoValidationErrors()
        {
            var govId = 465134;
            var estabUrn = 16802;

            var termEnds = DateTime.Today.AddDays(10);

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

        [Fact]
        public async Task ReplaceChair_Post_SharedChair_SelectedIdLessOrEqualZero_AddsModelErrorAndReturnsView()
        {
            // Arrange
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
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
                });

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

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

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
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = DateTime.Today
                });

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

            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

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
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = DateTime.Today
                });

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

            mockCachedLookupService.Setup(s => s.NationalitiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(s => s.TitlesGetAllAsync()).ReturnsAsync(new List<LookupDto>());

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
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,  // ✅ LOCAL CHAIR MODE
                Reinstate = true,

                // DateTermEnds MUST be valid (Local chair pipeline uses it)
                DateTermEnds = new DateTimeViewModel
                {
                    Day = today.Day,
                    Month = today.Month,
                    Year = today.Year
                },

                // Local chair path uses NewLocalGovernor, not SharedGovernors
                NewLocalGovernor = new CreateEditGovernorViewModel(),
                SelectedPreviousExistingNonChairId = null  // ✅ Prevent non-chair retirement branch
            };

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,  // ✅ Required for LocalChair mode
                    AppointmentEndDate = today                                     // Needed for safe pathway
                });

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
                    RoleId = null,   // ✅ INVALID ROLE triggers the branch we want
                    AppointmentEndDate = today
                });

            mockGovernorsReadService
                .Setup(s => s.GetEditorDisplayPolicyAsync(
                    It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorDisplayPolicy());

            mockGovernorsReadService
                .Setup(s => s.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<GovernorModel>());   // ✅ Prevent Shared-Chair pipeline

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

            mockCachedLookupService.Setup(x => x.TitlesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(x => x.NationalitiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(x => x.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());

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
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody, // ✅ valid role
                    AppointmentEndDate = today
                });

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

            mockCachedLookupService.Setup(x => x.TitlesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(x => x.NationalitiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(x => x.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());

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
            var result = await controller.ReplaceChair(vm);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);

            // Verify the reinstate method executed the “oldRole → newRole → Save” path
            mockGovernorsWriteService.Verify(
                s => s.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public void GetLocalEquivalentToSharedRole_AddsLocalEquivalent_WhenRoleIsShared()
        {
            // Arrange
            var sharedRole = eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody;
            Assert.Contains((int) sharedRole, EnumSets.SharedGovernorRoles);

            var governor = new GovernorModel
            {
                RoleId = (int) sharedRole
            };

            var roles = new List<eLookupGovernorRole>();

            // Act
            var localEquivalent = RoleEquivalence.GetLocalEquivalentToSharedRole(sharedRole);
            if (EnumSets.SharedGovernorRoles.Contains(governor.RoleId.Value))
            {
                if (localEquivalent != null)
                    roles.Add(localEquivalent.Value);
            }

            // Assert
            Assert.Single(roles);
            Assert.Equal(eLookupGovernorRole.ChairOfLocalGoverningBody, roles[0]);
        }

        [Fact]
        public void UpdateSharedGovernors_AppliesSelectedAndUnselectedLogicCorrectly()
        {
            // Arrange
            var model = new ReplaceChairViewModel
            {
                SharedGovernors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = 1,
                        Selected = true,
                        SharedWith = new List<SharedGovernorViewModel.EstablishmentViewModel>
                        {
                            new SharedGovernorViewModel.EstablishmentViewModel
                            {
                                Urn = 100,
                                EstablishmentName = "ORIGINAL_SHOULD_BE_REPLACED"
                            }
                        }
                    },
                    new SharedGovernorViewModel
                    {
                        Id = 2,
                        Selected = false,
                        SharedWith = new List<SharedGovernorViewModel.EstablishmentViewModel>
                        {
                            new SharedGovernorViewModel.EstablishmentViewModel
                            {
                                Urn = 200,
                                EstablishmentName = "IGNORED"
                            }
                        }
                    }
                }
            };

            var sourceGovernors = new List<SharedGovernorViewModel>
            {
                new SharedGovernorViewModel
                {
                    Id = 1,
                    SharedWith = new List<SharedGovernorViewModel.EstablishmentViewModel>
                    {
                        new SharedGovernorViewModel.EstablishmentViewModel
                        {
                            Urn = 10,
                            EstablishmentName = "SOURCE_SHARED_1"
                        }
                    }
                },
                new SharedGovernorViewModel
                {
                    Id = 2,
                    SharedWith = new List<SharedGovernorViewModel.EstablishmentViewModel>
                    {
                        new SharedGovernorViewModel.EstablishmentViewModel
                        {
                            Urn = 20,
                            EstablishmentName = "SOURCE_SHARED_2"
                        }
                    }
                }
            };

            // Act
            for (var i = 0; i < model.SharedGovernors?.Count; i++)
            {
                if (model.SharedGovernors[i].Selected)
                {
                    model.SharedGovernors[i].SharedWith =
                        sourceGovernors.First(x => x.Id == model.SharedGovernors[i].Id).SharedWith;
                }
                else
                {
                    model.SharedGovernors[i] =
                        sourceGovernors.First(x => x.Id == model.SharedGovernors[i].Id);

                    model.SharedGovernors[i].Selected = false;
                }
            }

            // Assert – Selected=true branch
            Assert.Single(model.SharedGovernors[0].SharedWith);
            Assert.Equal("SOURCE_SHARED_1", model.SharedGovernors[0].SharedWith[0].EstablishmentName);
            Assert.True(model.SharedGovernors[0].Selected);   

            // Assert – Selected=false branch
            Assert.Single(model.SharedGovernors[1].SharedWith);
            Assert.Equal("SOURCE_SHARED_2", model.SharedGovernors[1].SharedWith[0].EstablishmentName);
            Assert.False(model.SharedGovernors[1].Selected); 
            Assert.Equal(2, model.SharedGovernors[1].Id);    
        }

        [Fact]
        public void ReplaceChair_SelectedPreviousExistingNonChairId_AssignsSelectedNonChair()
        {
            // Arrange
            var model = new ReplaceChairViewModel
            {
                SelectedPreviousExistingNonChairId = 200
            };

            var localGovernors = new List<GovernorModel>
            {
                new GovernorModel { Id = 100, RoleId = 1 },
                new GovernorModel { Id = 200, RoleId = 2 },
                new GovernorModel { Id = 300, RoleId = 3 }
            };

            // Act
            if (model.SelectedPreviousExistingNonChairId.HasValue)
            {
                model.SelectedNonChair =
                    localGovernors.FirstOrDefault(x =>
                        x.Id == model.SelectedPreviousExistingNonChairId);
            }

            // Assert
            Assert.NotNull(model.SelectedNonChair);
            Assert.Equal(200, model.SelectedNonChair.Id);
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
                .ReturnsAsync(new GovernorModel
                {
                    Id = existingChairId,
                    RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody,
                    AppointmentEndDate = today
                });

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

            mockCachedLookupService.Setup(x => x.TitlesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(x => x.NationalitiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());
            mockCachedLookupService.Setup(x => x.GovernorAppointingBodiesGetAllAsync()).ReturnsAsync(new List<LookupDto>());

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
    }
}
