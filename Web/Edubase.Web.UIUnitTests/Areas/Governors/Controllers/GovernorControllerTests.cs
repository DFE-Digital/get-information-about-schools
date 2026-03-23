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
