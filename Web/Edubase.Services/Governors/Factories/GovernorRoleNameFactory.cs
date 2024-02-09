using System.Collections.Generic;
using Edubase.Common.Text;
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
            { eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professional for a secure single-academy trust (SSAT)" },
            { eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professional for a single-academy trust (SAT)" },
        };

        private static readonly Dictionary<eLookupGovernorRole, string> PluralisedLabels = new Dictionary<eLookupGovernorRole, string>()
        {
            { eLookupGovernorRole.Governor, "Governors" },
            { eLookupGovernorRole.LocalGovernor, "Local governors" },
            { eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governors - group" },
            { eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governors - establishment" },
            { eLookupGovernorRole.Member, "Members" },
            { eLookupGovernorRole.Member_Individual, "Members - individual" },
            { eLookupGovernorRole.Member_Organisation, "Members - organisation" },
            { eLookupGovernorRole.Trustee, "Trustees" },
            { eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professionals - group" },
            { eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professionals - establishment" },
            { eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Governance professionals to a local authority maintained school" },
            { eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professionals to a federation" },
            { eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professionals to an individual academy or free school" },
            { eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professionals to a multi-academy trust (MAT)" },
            { eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professionals for a secure single-academy trust (SSAT)" },
            { eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professionals for a single-academy trust (SAT)" },
        };

        public static string Create(eLookupGovernorRole role, eTextCase textCase = eTextCase.SentenceCase, bool pluraliseLabelIfApplicable = false)
        {
            string governorLabel = null;

            // Default to using sentence case label
            if (SentenceCaseLabels.ContainsKey(role))
            {
                governorLabel = SentenceCaseLabels[role];
            }

            // If we want plural and it's found, use that instead
            if(pluraliseLabelIfApplicable && PluralisedLabels.ContainsKey(role))
            {
                governorLabel = PluralisedLabels[role];
            }

            // Optionally convert to lower case if we don't want sentence case
            if(textCase != eTextCase.SentenceCase)
            {
                // TODO: Reconsider this implementation as it inadvertently lower-cases acronyms like SAT/SSAT/MAT/DfE
                governorLabel = governorLabel.ToLower();
            }

            return governorLabel;
        }
    }
}
