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
            { eLookupGovernorRole.ChairOfLocalGoverningBody, new List<eLookupGovernorRole> { eLookupGovernorRole.ChairOfLocalGoverningBody, eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody }},
            { eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, new List<eLookupGovernorRole> { eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool,
                eLookupGovernorRole.Group_SharedGovernanceProfessional, eLookupGovernorRole.Establishment_SharedGovernanceProfessional }}
        };

        public static List<eLookupGovernorRole> GetEquivalentToLocalRole(eLookupGovernorRole role)
        {
            return equivalentRoles.ContainsKey(role) ? equivalentRoles[role] : new List<eLookupGovernorRole> { role };
        }

        public static eLookupGovernorRole? GetLocalEquivalentToSharedRole(eLookupGovernorRole role)
        {
            return equivalentRoles.Any(x => x.Value.Contains(role)) ? (eLookupGovernorRole?) equivalentRoles.Single(x => x.Value.Contains(role)).Key : null;
        }

        public static List<eLookupGovernorRole> GetEquivalentRole(eLookupGovernorRole role)
        {
            return role == eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody ||
                role == eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody
                ? new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody,
                    eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
                }
                : role == eLookupGovernorRole.Establishment_SharedGovernanceProfessional || role == eLookupGovernorRole.Group_SharedGovernanceProfessional
                ? new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.Group_SharedGovernanceProfessional,
                    eLookupGovernorRole.Establishment_SharedGovernanceProfessional
                }
                : role == eLookupGovernorRole.Establishment_SharedLocalGovernor ||
                role == eLookupGovernorRole.Group_SharedLocalGovernor
                ? new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.Group_SharedLocalGovernor,
                    eLookupGovernorRole.Establishment_SharedLocalGovernor
                }
                : new List<eLookupGovernorRole> {role};
        }
    }
}
