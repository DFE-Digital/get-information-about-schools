using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.ReplaceChair
{
    public class ReplaceChairControllerGetTests : GovernorControllerTestBase
    {
        [Fact()]
        public async Task Gov_Get_ReplaceChair()
        {
            var controller = BuildController();
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

            mockGovernorsReadService
                .Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorDetailsDto);

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governor);

            mockGovernorsReadService
                .Setup(g => g.GetSharedGovernorsAsync(estabId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new List<GovernorModel>());

            mockGovernorsReadService
                .Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new GovernorDisplayPolicy());

            mockLayoutHelper
                .Setup(l => l.PopulateLayoutProperties
                    (It.IsAny<ReplaceChairViewModel>(),
                        estabId,
                        null,
                        It.IsAny<IPrincipal>(),
                        It.IsAny<Action<EstablishmentModel>>(),
                        It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);

            SetupCommonLookupMocks();

            var result = await controller.ReplaceChair(estabId, gid);
            var viewResult = result as ViewResult;

            Assert.NotNull(viewResult);

            var model = viewResult.Model as ReplaceChairViewModel;
            Assert.NotNull(model); ;
            Assert.Equal(gid, model.ExistingGovernorId);
            Assert.Equal((eLookupGovernorRole) governor.RoleId, model.Role);
        }

        [Fact]
        public async Task Gov_ReplaceChair_Get_ShouldRender_WithPolicies()
        {
            // Arrange
            var estabId = 2001;
            var gid = 3001;

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
                .ReturnsAsync(CreateExistingChair(gid));

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

            SetupCommonLookupMocks();

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
            Assert.Equal((eLookupGovernorRole) (int) eLookupGovernorRole.ChairOfLocalGoverningBody, vm.Role);
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

            SetupCommonLookupMocks();

            // Overrides
            mockCachedLookupService
                .Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(titles);

            mockCachedLookupService
                .Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(bodies);

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
                .ReturnsAsync(CreateSharedChair(gid, 123));

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

            SetupCommonLookupMocks();

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
            var controller = BuildController();
            var estabUrn = 50000;
            var existingChairId = 900;

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
            controller = BuildController(qs);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateSharedChair(existingChairId, estabUrn));

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

            SetupCommonLookupMocks();

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
            var controller = BuildController();
            var estabUrn = 7777;
            var existingChairId = 123;

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["d"] = "10";
            qs["m"] = "11";
            qs["y"] = "2030";
            qs["ri"] = "true";

            controller = BuildController(qs);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(existingChairId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(CreateExistingChair(existingChairId));

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

            SetupCommonLookupMocks();

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
    }
}
