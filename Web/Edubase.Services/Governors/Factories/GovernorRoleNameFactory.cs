using System.Collections.Generic;
using Edubase.Services.Enums;

namespace Edubase.Services.Governors.Factories
{
    public static class GovernorRoleNameFactory
    {
        private static Dictionary<eLookupGovernorRole, string> CreateLabelsDictionary(bool isPlural)
        {
            return new Dictionary<eLookupGovernorRole, string>
            {
                /*  1 */ { eLookupGovernorRole.ChairOfGovernors,                                         isPlural ? "Chairs of governors"                                                 : "Chair of governors" },
                /*  2 */ { eLookupGovernorRole.Governor,                                                 isPlural ? "Governors"                                                           : "Governor" },
                /*  3 */ { eLookupGovernorRole.ChairOfTrustees,                                          isPlural ? "Chairs of trustees"                                                  : "Chair of trustees" },
                /*  4 */ { eLookupGovernorRole.Trustee,                                                  isPlural ? "Trustees"                                                            : "Trustee" },
                /*  5 */ { eLookupGovernorRole.Member,                                                   isPlural ? "Members"                                                             : "Member" },
                /*  6 */ { eLookupGovernorRole.AccountingOfficer,                                        isPlural ? "Accounting officers"                                                 : "Accounting officer" },
                /*  7 */ { eLookupGovernorRole.ChiefFinancialOfficer,                                    isPlural ? "Chief financial officers"                                            : "Chief financial officer" },
                /*  8 */ { eLookupGovernorRole.ChairOfLocalGoverningBody,                                isPlural ? "Chairs of local governing body"                                      : "Chair of local governing body" },
                /*  9 */ { eLookupGovernorRole.LocalGovernor,                                            isPlural ? "Local governors"                                                     : "Local governor" },
                /* 10 */ { eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody,                    isPlural ? "Shared chairs of local governing body - group"                       : "Shared chair of local governing body - group" },
                /* 11 */ { eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody,            isPlural ? "Shared chairs of local governing body - establishment"               : "Shared chair of local governing body - establishment" },
                /* 12 */ { eLookupGovernorRole.Group_SharedLocalGovernor,                                isPlural ? "Shared local governors - group"                                      : "Shared local governor - group" },
                /* 13 */ { eLookupGovernorRole.Establishment_SharedLocalGovernor,                        isPlural ? "Shared local governors - establishment"                              : "Shared local governor - establishment" },
                /* 14 */ { eLookupGovernorRole.NA,                                                       isPlural ? "Not applicable"                                                      : "Not applicable" },
                /* 15 */ { eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool,  isPlural ? "Governance professionals - local authority maintained school"        : "Governance professional - local authority maintained school" },
                /* 16 */ { eLookupGovernorRole.GovernanceProfessionalToAFederation,                      isPlural ? "Governance professionals - federation"                               : "Governance professional - federation" },
                /* 17 */ { eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool,  isPlural ? "Local governance professionals - individual academy or free school"  : "Local governance professional - individual academy or free school" },
                /* 18 */ { eLookupGovernorRole.GovernanceProfessionalToAMat,                             isPlural ? "Governance professionals - multi-academy trust (MAT)"                : "Governance professional - multi-academy trust (MAT)" },
                /* 19 */ { eLookupGovernorRole.Group_SharedGovernanceProfessional,                       isPlural ? "Shared governance professionals - group"                             : "Shared governance professional - group" },
                /* 20 */ { eLookupGovernorRole.Establishment_SharedGovernanceProfessional,               isPlural ? "Shared governance professionals - establishment"                     : "Shared governance professional - establishment" },
                /* 21 */ { eLookupGovernorRole.Member_Individual,                                        isPlural ? "Members - individuals"                                               : "Member - individual" },
                /* 22 */ { eLookupGovernorRole.Member_Organisation,                                      isPlural ? "Members - organisations"                                             : "Member - organisation" },
                /* 23 */ { eLookupGovernorRole.GovernanceProfessionalToASecureSat,                       isPlural ? "Governance professionals - secure single-academy trust (SSAT)"       : "Governance professional - secure single-academy trust (SSAT)" },
                /* 24 */ { eLookupGovernorRole.GovernanceProfessionalToASat,                             isPlural ? "Governance professionals - single-academy trust (SAT)"               : "Governance professional - single-academy trust (SAT)" },
            };
        }

        private static readonly Dictionary<eLookupGovernorRole, string> SentenceCaseLabels = CreateLabelsDictionary(isPlural: false);
        private static readonly Dictionary<eLookupGovernorRole, string> PluralisedLabels = CreateLabelsDictionary(isPlural: true);

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
