using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Helpers
{
    public static class SharedLocalRoleEquivalence
    {
        private static readonly Dictionary<eLookupGovernorRole, List<eLookupGovernorRole>> equivalentRoles = new Dictionary<eLookupGovernorRole, List<eLookupGovernorRole>>
        {
            { eLookupGovernorRole.LocalGovernor, new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor, eLookupGovernorRole.Establishment_SharedLocalGovernor, eLookupGovernorRole.Group_SharedLocalGovernor }},
            { eLookupGovernorRole.ChairOfLocalGoverningBody, new List<eLookupGovernorRole> { eLookupGovernorRole.ChairOfLocalGoverningBody, eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody }}
        };

        public static List<eLookupGovernorRole> GetEquivalentToLocalRole(eLookupGovernorRole role)
        {
            return equivalentRoles.ContainsKey(role) ? equivalentRoles[role] : new List<eLookupGovernorRole> { role };
        }

        public static eLookupGovernorRole? GetLocalEquivalentToSharedRole(eLookupGovernorRole role)
        {
           if (equivalentRoles.Any(x => x.Value.Contains(role)))
           {
               return equivalentRoles.Single(x => x.Value.Contains(role)).Key;
           }

            return null;
        }
    }
}