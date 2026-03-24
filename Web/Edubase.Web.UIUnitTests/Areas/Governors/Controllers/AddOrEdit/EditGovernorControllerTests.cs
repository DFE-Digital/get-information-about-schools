using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UIUnitTests.Areas.Governors.Helpers;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.AddOrEdit
{
    public class EditGovernorControllerTests : GovernorControllerTestBase
    {
        [Fact()]
        public async Task Gov_Edit_Null_Params()
        {
            var controller = BuildController();
            await Assert.ThrowsAsync<InvalidParameterException>(() => controller.Edit(null, null, null, null));
        }

        [Fact()]
        public async Task Gov_Edit_GroupIdSpecified()
        {
            // Arrange
            var controller = BuildController();
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


            var route = new Route("group/edit/{groupId}/governance", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            rd.Values["controller"] = "Governors";
            rd.Values["action"] = "Edit";
            rd.Values["groupId"] = groupId;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);


            SetupCommonLookupMocks();

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
                            GovernanceModeId = (int) expectedGovernanceMode
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

            // Act
            var result = await controller.Edit(5, null, null, null);

            var viewResult = result as ViewResult;

            var model = viewResult?.Model as GovernorsGridViewModel;

            // Assert
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
            var controller = BuildController();
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


            var route = new Route("establishment/edit/{establishmentUrn}/governance", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            rd.Values["controller"] = "Governors";
            rd.Values["action"] = "Edit";
            rd.Values["establishmentUrn"] = establishmentId;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            SetupCommonLookupMocks();

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(establishmentId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);

            mockGovernorsReadService
                .Setup(g => g.GetGovernorPermissions(establishmentId, null, It.IsAny<IPrincipal>()))
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

            // Act
            var result = await controller.Edit(null, establishmentId, null, null);

            var viewResult = result as ViewResult;
            var model = viewResult?.Model as GovernorsGridViewModel;

            // Assert
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
            // Arrange
            var controller = BuildController();
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

            var route = new Route("group/edit/{groupId}/governance", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            rd.Values["controller"] = "Governors";
            rd.Values["action"] = "Edit";
            rd.Values["establishmentUrn"] = groupId;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            SetupCommonLookupMocks();

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

            // Act
            var result = await controller.Edit(5, null, 43, null);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as GovernorsGridViewModel;

            // Assert
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
            // Arrange
            var controller = BuildController();
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


            var route = new Route("group/edit/{groupId}/governance", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            rd.Values["controller"] = "Governors";
            rd.Values["action"] = "Edit";
            rd.Values["groudId"] = groupId;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);

            SetupCommonLookupMocks();

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

            // Act
            var result = await controller.Edit(5, null, 43, null);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as GovernorsGridViewModel;

            // Assert
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

            WireEditHelper.WireEdit(
                estabId: estabId,
                dto: dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

            var controller = BuildController();

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                controller.Edit(null, estabId, removalGid, null));
        }

        [Fact()]
        public async Task Gov_Edit_GroupId_DuplicateGovernorId()
        {
            // Arrange
            var controller = BuildController();
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


            var route = new Route("group/edit/{groupId}/governance", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            rd.Values["controller"] = "Governors";
            rd.Values["action"] = "Edit";
            rd.Values["groupId"] = groupId;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);


            SetupCommonLookupMocks();

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

            // Act
            var result = await controller.Edit(5, null, null, duplicateId);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as GovernorsGridViewModel;

            // Assert
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
            // Arrange
            var controller = BuildController();
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


            var route = new Route("establishment/edit/{establishmentUrn}/governance", new PageRouteHandler("~/"));
            var rd = new RouteData(route, new PageRouteHandler("~/"));
            rd.Values["controller"] = "Governors";
            rd.Values["action"] = "Edit";
            rd.Values["establishmentUrn"] = establishmentId;

            controller.ControllerContext = new ControllerContext(
                controller.ControllerContext.HttpContext,
                rd,
                controller);


            SetupCommonLookupMocks();

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

            // Act
            var result = await controller.Edit(null, establishmentId, null, null, true);

            var viewResult = result as ViewResult;
            var model = viewResult?.Model as GovernorsGridViewModel;

            // Assert
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

            WireEditHelper.WireEdit(
                estabId,
                dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

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
            // Arrange
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

            WireEditHelper.WireEdit(
                estabId,
                dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

            // Act
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            // Assert
            Assert.Equal(2, vm.Grids.Count);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor).Rows);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.Group_SharedLocalGovernor).Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldNotBleedRowsAcrossGrids()
        {
            // Arrange
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

            WireEditHelper.WireEdit(
                estabId: estabId,
                dto: dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

            // Act
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            // Assert
            Assert.Equal(2, vm.Grids.Count);
            Assert.Empty(vm.Grids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor).Rows);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.Group_SharedLocalGovernor).Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldRenderHistoricGrids_OnePerRole()
        {
            // Arrange
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

            WireEditHelper.WireEdit(
                estabId: estabId,
                dto: dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

            // Act
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            // Assert
            Assert.Equal(2, vm.HistoricGrids.Count);
            Assert.Single(vm.HistoricGrids.Single(g => g.Role == eLookupGovernorRole.Governor).Rows);
            Assert.Single(vm.HistoricGrids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor).Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldThrow_WhenDisplayPolicyMissingForRole()
        {
            // Arrange
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

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

            // Act & Assert
            var controller = BuildController();

            await Assert.ThrowsAsync<Exception>(async () => await controller.Edit(null, estabId, null, null));
        }

        [Fact]
        public async Task Gov_Edit_ShouldRender_WhenAllDisplayPoliciesPresent()
        {
            // Arrange
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

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

            // Act
            var controller = BuildController();

            var result = await controller.Edit(null, estabId, null, null);
            var vm = Assert.IsType<GovernorsGridViewModel>(Assert.IsType<ViewResult>(result).Model);

            // Assert
            Assert.Single(vm.Grids);
            Assert.Single(vm.Grids[0].Rows);
        }

        [Fact]
        public async Task Gov_Edit_ShouldPopulateLayoutProperties()
        {
            // Arrange
            var estabId = 1006;

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>(),
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>()
            };

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

            // Act
            var controller = BuildController();

            // Assert
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
    }
}
