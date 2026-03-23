using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UIUnitTests.Areas.Governors.Helpers;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.AddOrEdit
{
    public class EditGovernorControllerTests : GovernorControllerTestBase
    {
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
                estabId = estabId,
                dto = dto,
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
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

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

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

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

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

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

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

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

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

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

            WireEditHelper.WireEdit(
                estabId = estabId,
                dto = dto,
                mockRead: mockGovernorsReadService,
                mockCache: mockCachedLookupService,
                mockLayout: mockLayoutHelper
            );

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
    }
}
