using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common.Text;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Factories;
using Edubase.Services.Nomenclature;
using Xunit;

namespace Edubase.ServicesUnitTests.Nomenclature
{
    public class NomenclatureServiceTests
    {
        [Theory]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "Accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "Chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "Chair of local governing body")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "Chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "Chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "Shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governor")]
        [InlineData(eLookupGovernorRole.Governor, "Governor")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governor")]
        [InlineData(eLookupGovernorRole.Member, "Member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Member - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Member - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustee")]
        [InlineData(eLookupGovernorRole.NA, "N a")]
        public void GetGovernorRoleName_SentenceCase_PluraliseFalse_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var sut1 = new NomenclatureService();
            var sut = new GovernorRoleNameFactory(false);

            var result1 = sut1.GetGovernorRoleName(role, eTextCase.SentenceCase, false);
            var result = sut.Create(role);

            Assert.Equal(expected, result1);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "Shared local governors")]
        [InlineData(eLookupGovernorRole.Governor, "Governors")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "Shared chair of local governing bodies")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "Shared local governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "Local governors")]
        [InlineData(eLookupGovernorRole.Member, "Members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "Members - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "Members - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "Trustees")]
        [InlineData(eLookupGovernorRole.NA, "N as")]
        public void GetGovernorRoleName_SentenceCase_PluraliseTrue_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var sut1 = new NomenclatureService();

            var result1 = sut1.GetGovernorRoleName(role, eTextCase.SentenceCase, true);

            Assert.Equal(expected, result1);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officer")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing body")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officer")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governor")]
        [InlineData(eLookupGovernorRole.Governor, "governor")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing body")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governor")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governor")]
        [InlineData(eLookupGovernorRole.Member, "member")]
        [InlineData(eLookupGovernorRole.Member_Individual, "member - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "member - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "trustee")]
        [InlineData(eLookupGovernorRole.NA, "n a")]
        public void GetGovernorRoleName_Lowercase_PluraliseFalse_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var sut1 = new NomenclatureService();

            var result1 = sut1.GetGovernorRoleName(role, eTextCase.Lowerase, false);

            Assert.Equal(expected, result1);
        }

        [Theory]
        [InlineData(eLookupGovernorRole.AccountingOfficer, "accounting officers")]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, "chair of governors")]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, "chair of local governing bodies")]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, "chair of trustees")]
        [InlineData(eLookupGovernorRole.ChiefFinancialOfficer, "chief financial officers")]
        [InlineData(eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, "shared chair of local governing bodies")]
        [InlineData(eLookupGovernorRole.Establishment_SharedLocalGovernor, "shared local governors")]
        [InlineData(eLookupGovernorRole.Governor, "governors")]
        [InlineData(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, "shared chair of local governing bodies")]
        [InlineData(eLookupGovernorRole.Group_SharedLocalGovernor, "shared local governors")]
        [InlineData(eLookupGovernorRole.LocalGovernor, "local governors")]
        [InlineData(eLookupGovernorRole.Member, "members")]
        [InlineData(eLookupGovernorRole.Member_Individual, "members - individual")]
        [InlineData(eLookupGovernorRole.Member_Organisation, "members - organisation")]
        [InlineData(eLookupGovernorRole.Trustee, "trustees")]
        [InlineData(eLookupGovernorRole.NA, "n as")]
        public void GetGovernorRoleName_Lowercase_PluraliseTrue_ReturnsExpected(eLookupGovernorRole role, string expected)
        {
            var sut1 = new NomenclatureService();

            var result1 = sut1.GetGovernorRoleName(role, eTextCase.Lowerase, true);

            Assert.Equal(expected, result1);
        }

    }
}
