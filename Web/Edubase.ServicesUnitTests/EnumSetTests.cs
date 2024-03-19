using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Edubase.Services.Enums.UnitTests
{
    public class EnumSetTests
    {
        public static IEnumerable<object[]> ELookupGovernorRoles = EnumSets
            .eGovernanceProfessionalRoles
            .Select(role => new object[] { role });

        [Theory]
        [MemberData(nameof(ELookupGovernorRoles))]
        public void AllGovernanceProfessionalRolesAreSingularRoles(eLookupGovernorRole role)
        {
            Assert.Contains(role, EnumSets.eSingularGovernorRoles);
        }
    }
}
