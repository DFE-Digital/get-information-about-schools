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
            { eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governor" },
            { eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body" },
            { eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body" },
            { eLookupGovernorRole.AccountingOfficer, "Accounting officer" },
            { eLookupGovernorRole.ChairOfGovernors, "Chair of governors" },
            { eLookupGovernorRole.ChairOfTrustees, "Chair of trustees" },
            { eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer" },
            { eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body" },
            { eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governor" },
            { eLookupGovernorRole.Member, "Member" },
            { eLookupGovernorRole.Member_Individual, "Member - individual" },
            { eLookupGovernorRole.Member_Organisation, "Member - organisation" },
            { eLookupGovernorRole.Trustee, "Trustee" },
            { eLookupGovernorRole.NA, "N a" },
            { eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professional" },
            { eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professional" },
            { eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Governance professional to a local authority maintained school" },
            { eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professional to a federation" },
            { eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professional to an individual academy or free school" },
            { eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professional to a MAT" },
        };

        private static readonly Dictionary<eLookupGovernorRole, string> PluralisedLabels = new Dictionary<eLookupGovernorRole, string>()
        {
            { eLookupGovernorRole.Governor, "Governors" },
            { eLookupGovernorRole.LocalGovernor, "Local governors" },
            { eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governors" },
            { eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governors" },
            { eLookupGovernorRole.Member, "Members" },
            { eLookupGovernorRole.Member_Individual, "Members - individual" },
            { eLookupGovernorRole.Member_Organisation, "Members - organisation" },
            { eLookupGovernorRole.Trustee, "Trustees" }
        };

        public static string Create(eLookupGovernorRole role, eTextCase textCase = eTextCase.SentenceCase, bool pluraliseLabelIfApplicable = false)
        {
            var governorlabel = pluraliseLabelIfApplicable ?
                PluralisedLabels.ContainsKey(role) ? PluralisedLabels[role] : SentenceCaseLabels[role]
                : SentenceCaseLabels[role];
            return textCase == eTextCase.SentenceCase ? governorlabel : governorlabel.ToLower();
        }
    }
}
