using System.Collections.Generic;
using Edubase.Services.Enums;

namespace Edubase.Services.Governors.Factories
{
    public static class GovernorRoleNameFactory
    {
        private static readonly Dictionary<eLookupGovernorRole, string> SentenceCaseLabels = new Dictionary<eLookupGovernorRole, string>()
        {
            { eLookupGovernorRole.Governor, "Governor" },
            { eLookupGovernorRole.LocalGovernor, "Local governor" },
            { eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governor - group" },
            { eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body" },
            { eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - group" },
            { eLookupGovernorRole.AccountingOfficer, "Accounting officer" },
            { eLookupGovernorRole.ChairOfGovernors, "Chair of governors" },
            { eLookupGovernorRole.ChairOfTrustees, "Chair of trustees" },
            { eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer" },
            { eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - establishment" },
            { eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governor - establishment" },
            { eLookupGovernorRole.Member, "Member" },
            { eLookupGovernorRole.Member_Individual, "Member - individual" },
            { eLookupGovernorRole.Member_Organisation, "Member - organisation" },
            { eLookupGovernorRole.Trustee, "Trustee" },
            { eLookupGovernorRole.NA, "Not applicable" },
            { eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professional - group" },
            { eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professional - establishment" },
            { eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Governance professional to a local authority maintained school" },
            { eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professional to a federation" },
            { eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professional to an individual academy or free school" },
            { eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professional to a multi-academy trust (MAT)" },
            { eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professional to a secure single-academy trust (SSAT)" },
            { eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professional to a single-academy trust (SAT)" },
        };

        private static readonly Dictionary<eLookupGovernorRole, string> PluralisedLabels = new Dictionary<eLookupGovernorRole, string>()
        {
            { eLookupGovernorRole.Governor, "Governors" },
            { eLookupGovernorRole.LocalGovernor, "Local governors" },
            { eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governors - group" },
            { eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governors - establishment" },
            { eLookupGovernorRole.Member, "Members" },
            { eLookupGovernorRole.Member_Individual, "Members - individuals" },
            { eLookupGovernorRole.Member_Organisation, "Members - organisations" },
            { eLookupGovernorRole.Trustee, "Trustees" },
            { eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professionals - group" },
            { eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professionals - establishment" },
            { eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Governance professionals to a local authority maintained school" },
            { eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professionals to a federation" },
            { eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professionals to an individual academy or free school" },
            { eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professionals to a multi-academy trust (MAT)" },
            { eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professionals to a secure single-academy trust (SSAT)" },
            { eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professionals to a single-academy trust (SAT)" },
        };

        public static string Create(
            eLookupGovernorRole role,
            bool pluraliseLabelIfApplicable = false,
            bool removeMemberPrefix = false,
            bool isMidSentence = false,
            bool removeGroupEstablishmentSuffix = false
        )
        {
            string governorLabel = null;

            // Default to using sentence case label
            if (SentenceCaseLabels.ContainsKey(role))
            {
                governorLabel = SentenceCaseLabels[role];
            }
            else
            {
                // We're expecting the singular label to always be present
                throw new KeyNotFoundException($"No label found for governor role {role}");
            }

            // If we want plural and it's found, use that instead
            if(pluraliseLabelIfApplicable && PluralisedLabels.ContainsKey(role))
            {
                governorLabel = PluralisedLabels[role];
            }

            // Some areas of the UI do not want the "Member - " prefix
            if (removeMemberPrefix)
            {
                governorLabel = governorLabel.Replace("Member - ", "");
                governorLabel = governorLabel.Replace("Members - ", "");

                // Capitalise first character to restore sentence case
                governorLabel = governorLabel.Substring(0, 1).ToUpper() + governorLabel.Substring(1);
            }

            // Some areas of the UI do not want the "- Group" or "- Establishment" suffix
            if (removeGroupEstablishmentSuffix)
            {
                governorLabel = governorLabel.Replace(" - group", "");
                governorLabel = governorLabel.Replace(" - establishment", "");
            }

            if (isMidSentence)
            {
                // lowercase first letter
                governorLabel = governorLabel.Substring(0, 1).ToLower() + governorLabel.Substring(1);
            }

            return governorLabel;
        }
    }
}
