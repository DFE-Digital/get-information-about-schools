using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.RoleAllowed
{
    public sealed class RoleAllowedGovernorControllerTests : GovernorControllerTestBase
    {
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
            var currentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int) eLookupGovernorRole.Group_SharedGovernanceProfessional }, };
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
            var currentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody }, };
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
            var currentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int) eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody }, };
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
            applicableRoles.AddRange(currentGovernors.Select(g => (eLookupGovernorRole) g.RoleId));

            var governorsDetails = new GovernorsDetailsDto
            {
                CurrentGovernors = currentGovernors,
                ApplicableRoles = applicableRoles,
                HistoricalGovernors = new List<GovernorModel>(),
                HasFullAccess = true
            };

            mockGovernorsReadService.Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(governorsDetails);

            var controller = BuildController();
            var actualResult = await controller.RoleAllowed(newGovernorRole, null, null, null, false);

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
