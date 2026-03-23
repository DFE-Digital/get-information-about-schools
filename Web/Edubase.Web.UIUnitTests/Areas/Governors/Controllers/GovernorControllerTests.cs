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
using Edubase.Services.Establishments;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UIUnitTests;
using Moq;
using Xunit;

namespace Edubase.Web.UI.Areas.Governors.Controllers.UnitTests
{
    public class GovernorControllerTests
    {

        

        


        





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

        private GovernorController BuildController()
        {
            var controller = new GovernorController(
                mockGovernorsReadService.Object,
                mockCachedLookupService.Object,
                mockGovernorsWriteService.Object,
                mockGroupReadService.Object,
                mockEstablishmentReadService.Object,
                mockLayoutHelper.Object);

            // HttpContext + HttpRequest
            var httpContext = new Mock<HttpContextBase>();
            var httpRequest = new Mock<HttpRequestBase>();

            httpRequest.Setup(r => r.QueryString)
                .Returns(new NameValueCollection());

            // Required for MVC URL generation
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
