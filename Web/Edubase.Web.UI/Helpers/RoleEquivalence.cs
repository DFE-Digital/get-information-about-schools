using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Helpers
{
    public static class RoleEquivalence
    {
        private static readonly Dictionary<eLookupGovernorRole, List<eLookupGovernorRole>> equivalentRoles = new Dictionary<eLookupGovernorRole, List<eLookupGovernorRole>>
        {
            { eLookupGovernorRole.LocalGovernor, new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor, eLookupGovernorRole.Group_SharedLocalGovernor, eLookupGovernorRole.Establishment_SharedLocalGovernor }},
            { eLookupGovernorRole.ChairOfLocalGoverningBody, new List<eLookupGovernorRole> { eLookupGovernorRole.ChairOfLocalGoverningBody, eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody }}
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

        public static List<eLookupGovernorRole> GetEquivalentRole(eLookupGovernorRole role)
        {
            if (role == eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody ||
                role == eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody)
            {
                return new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody,
                    eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
                };
            }

            if (role == eLookupGovernorRole.Establishment_SharedLocalGovernor ||
                role == eLookupGovernorRole.Group_SharedLocalGovernor)
            {
                return new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.Group_SharedLocalGovernor,
                    eLookupGovernorRole.Establishment_SharedLocalGovernor
                };
            }

            return new List<eLookupGovernorRole> {role};
        }
    }
}