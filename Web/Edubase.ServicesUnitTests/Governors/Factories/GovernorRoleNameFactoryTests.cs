using System;
using System.Collections.Generic;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Factories;
using Xunit;

namespace Edubase.ServicesUnitTests.Governors
{
    public class GovernorRoleNameFactoryTests
    {
        public static IEnumerable<object[]> RoleEnumValues()
        {
            foreach (var role in Enum.GetValues(typeof(eLookupGovernorRole)))
            {
                yield return new object[] { role };
            }
        }

        [Theory]
        [MemberData(nameof(RoleEnumValues))]
        public void GetGovernorRoleName_DefaultIs_Singular_And_NotMidSentence_AndMemberPrefixRetained(eLookupGovernorRole role)
        {
            var defaultName = GovernorRoleNameFactory.Create(role);
            var singularName = GovernorRoleNameFactory.Create(
                role,
                pluraliseLabelIfApplicable: false,
                isMidSentence: false,
                removeMemberPrefix: false
            );

            Assert.Equal(singularName, defaultName);
        }


        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "Governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governor")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governor - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "Accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "Chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "Chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governor - establishment")]
        [InlineData(eLookupGovernorRole.Member, "Member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Member - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Member - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustee")]
        [InlineData(eLookupGovernorRole.NA, "Not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professional - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professional - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Local governance professional - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professional - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professional - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professional - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professional - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professional - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Singular_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: false);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "Governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governors")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governors - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "Accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "Chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "Chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governors - establishment")]
        [InlineData(eLookupGovernorRole.Member, "Members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Members - individuals")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Members - organisations")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustees")]
        [InlineData(eLookupGovernorRole.NA, "Not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professionals - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professionals - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Local governance professionals - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professionals - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professionals - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professionals - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professionals - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professionals - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Plural_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: true);
            Assert.Equal(expected, result);
        }



        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governor")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governor - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governor - establishment")]
        [InlineData(eLookupGovernorRole.Member, "member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "member - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "member - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "trustee")]
        [InlineData(eLookupGovernorRole.NA, "not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "shared governance professional - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "shared governance professional - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "local governance professional - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "governance professional - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "governance professional - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "governance professional - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "governance professional - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "governance professional - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Singular_AND_MidSentence_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: false, isMidSentence: true);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governors")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governors - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governors - establishment")]
        [InlineData(eLookupGovernorRole.Member, "members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "members - individuals")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "members - organisations")]
        [InlineData(eLookupGovernorRole.Trustee, "trustees")]
        [InlineData(eLookupGovernorRole.NA, "not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "shared governance professionals - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "shared governance professionals - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "local governance professionals - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "governance professionals - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "governance professionals - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "governance professionals - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "governance professionals - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "governance professionals - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Plural_AND_MidSentence_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: true, isMidSentence: true);
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "Governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governor")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governor - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "Accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "Chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "Chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governor - establishment")]
        [InlineData(eLookupGovernorRole.Member, "Member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustee")]
        [InlineData(eLookupGovernorRole.NA, "Not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professional - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professional - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Local governance professional - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professional - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professional - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professional - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professional - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professional - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Singular_AND_RemoveMemberPrefix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: false, removeMemberPrefix: true);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "Governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governors")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governors - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "Accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "Chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "Chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governors - establishment")]
        [InlineData(eLookupGovernorRole.Member, "Members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Individuals")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Organisations")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustees")]
        [InlineData(eLookupGovernorRole.NA, "Not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professionals - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professionals - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Local governance professionals - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professionals - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professionals - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professionals - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professionals - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professionals - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Plural_AND_RemoveMemberPrefix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: true, removeMemberPrefix: true);
            Assert.Equal(expected, result);
        }



        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governor")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governor - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governor - establishment")]
        [InlineData(eLookupGovernorRole.Member, "member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "trustee")]
        [InlineData(eLookupGovernorRole.NA, "not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "shared governance professional - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "shared governance professional - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "local governance professional - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "governance professional - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "governance professional - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "governance professional - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "governance professional - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "governance professional - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Singular_AND_MidSentence_AND_RemoveMemberPrefix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: false, isMidSentence: true, removeMemberPrefix: true);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governors")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governors - group")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing body - group")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing body - establishment")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governors - establishment")]
        [InlineData(eLookupGovernorRole.Member, "members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "individuals")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "organisations")]
        [InlineData(eLookupGovernorRole.Trustee, "trustees")]
        [InlineData(eLookupGovernorRole.NA, "not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "shared governance professionals - group")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "shared governance professionals - establishment")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "local governance professionals - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "governance professionals - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "governance professionals - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "governance professionals - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "governance professionals - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "governance professionals - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Plural_AND_MidSentence_AND_RemoveMemberPrefix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: true, isMidSentence: true, removeMemberPrefix: true);
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "Governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governor")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governor")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "Accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "Chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "Chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governor")]
        [InlineData(eLookupGovernorRole.Member, "Member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Member - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Member - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustee")]
        [InlineData(eLookupGovernorRole.NA, "Not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professional")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professional")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Local governance professional - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professional - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professional - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professional - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professional - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professional - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Singular_AND_RemoveGroupEstablishmentSuffix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: false, removeGroupEstablishmentSuffix: true);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "Governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governors")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governors")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "Accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "Chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "Chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governors")]
        [InlineData(eLookupGovernorRole.Member, "Members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Members - individuals")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Members - organisations")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustees")]
        [InlineData(eLookupGovernorRole.NA, "Not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "Shared governance professionals")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "Shared governance professionals")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "Local governance professionals - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "Governance professionals - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "Governance professionals - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "Governance professionals - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "Governance professionals - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "Governance professionals - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Plural_AND_RemoveGroupEstablishmentSuffix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: true, removeGroupEstablishmentSuffix: true);
            Assert.Equal(expected, result);
        }



        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governor")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governor")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governor")]
        [InlineData(eLookupGovernorRole.Member, "member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "member - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "member - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "trustee")]
        [InlineData(eLookupGovernorRole.NA, "not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "shared governance professional")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "shared governance professional")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "local governance professional - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "governance professional - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "governance professional - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "governance professional - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "governance professional - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "governance professional - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Singular_AND_MidSentence_AND_RemoveGroupEstablishmentSuffix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: false, isMidSentence: true, removeGroupEstablishmentSuffix: true);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.Governor, "governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governors")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governors")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governors")]
        [InlineData(eLookupGovernorRole.Member, "members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "members - individuals")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "members - organisations")]
        [InlineData(eLookupGovernorRole.Trustee, "trustees")]
        [InlineData(eLookupGovernorRole.NA, "not applicable")]
        [InlineData(eLookupGovernorRole.Group_SharedGovernanceProfessional, "shared governance professionals")]
        [InlineData(eLookupGovernorRole.Establishment_SharedGovernanceProfessional, "shared governance professionals")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToALocalAuthorityMaintainedSchool, "local governance professionals - local authority maintained school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAFederation, "governance professionals - federation")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, "governance professionals - individual academy or free school")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToAMat, "governance professionals - multi-academy trust (MAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASecureSat, "governance professionals - secure single-academy trust (SSAT)")]
        [InlineData(eLookupGovernorRole.GovernanceProfessionalToASat, "governance professionals - single-academy trust (SAT)")]
        public void GetGovernorRoleName_WHEN_Plural_AND_MidSentence_AND_RemoveGroupEstablishmentSuffix_THEN_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var result = GovernorRoleNameFactory.Create(role, pluraliseLabelIfApplicable: true, isMidSentence: true, removeGroupEstablishmentSuffix: true);
            Assert.Equal(expected, result);
        }

    }
}

