using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Helpers;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.RoleMapping
{
    public sealed class RoleMappingGovernorControllerTests
    {
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
    }
}
