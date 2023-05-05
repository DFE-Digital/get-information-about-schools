using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Edubase.Web.UI.Helpers;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers
{
    public class SecureAcademyUtilityTestsPartII
    {
        [Theory]
        [InlineData("YCS", "YCS")]
        [InlineData("ROLE_BACKOFFICE", "ROLE_BACKOFFICE")]
        [InlineData("EFADO", null)]
        public void
            GetSecureAcademy16To19Role_WhenUserIsCalled_ReturnsExpectedResult(string roleName, string expectedResult)
        {
            var mockPrincipal = GetMockPrincipalIdentity(roleName);

            var result = AcademyUtility.GetSecureAcademy16To19Role(mockPrincipal.Object);

            Assert.Equal(result, expectedResult);
        }


        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("EFADO", false)]
        [InlineData("ROLE_BACKOFFICE", true)]
        [InlineData("YCS", true)]
        public void IsPartOfManageSecureAcademy16To19UserRole_WhenCalled_ReturnsExpectedResult(string roleName,
            bool expectedResult)
        {
            var result = AcademyUtility.IsPartOfManageSecureAcademy16To19UserRole(roleName);

            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData("", "", true)]
        [InlineData(" ", " ", true)]
        [InlineData(null, null, true)]
        [InlineData("", null, true)]
        [InlineData(" ", null, true)]
        [InlineData(null, "", true)]
        [InlineData(null, " ", true)]
        [InlineData("someValue", "", false)]
        [InlineData("someValue", " ", false)]
        [InlineData("someValue", null, false)]
        [InlineData("", "someValue", false)]
        [InlineData(" ", "someValue", false)]
        [InlineData("YCS", "", true)]
        [InlineData("YCS", " ", true)]
        [InlineData("YCS", null, true)]
        [InlineData("YCS", "someValue", true)]
        [InlineData("ROLE_BACKOFFICE", "", true)]
        [InlineData("ROLE_BACKOFFICE", " ", true)]
        [InlineData("ROLE_BACKOFFICE", null, true)]
        [InlineData("ROLE_BACKOFFICE", "someValue", true)]
        [InlineData("EFADO", "", false)]
        [InlineData("EFADO", " ", false)]
        [InlineData("EFADO", null, false)]
        [InlineData("EFADO", "someValue", false)]
        public void DoesHaveAccessAuthorization_WhenCalled_ReturnsExpectedResult(string roleName,
            string establishmentTypeId, bool expectedResult)
        {
            var user = GetMockPrincipalIdentity(roleName);

            var result = AcademyUtility.DoesHaveAccessAuthorization(user.Object, roleName, establishmentTypeId);

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void AccessViolationException_WhenCalled_ReturnsAccessViolationException()
        {
            var result = AcademyUtility.GetAccessViolationException();

            Assert.IsType<AccessViolationException>(result);
            Assert.Equal("Attempt to access resource without the right authorization", result.Message);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public void IsUserSecureAcademy16To19User_WhenCalledWithNullOrWhitespaceRoleName_ReturnsExpectedResult(
            string roleName, bool expectedResult)
        {
            var result = AcademyUtility.IsUserSecureAcademy16To19User(roleName);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("EFADO", false)]
        [InlineData("YCS", true)]
        [InlineData("ROLE_BACKOFFICE", true)]
        public void IsUserSecureAcademy16To19User_WhenCalledWithNonNullOrNonWhitespaceRoleName_ReturnsExpectedResult
            (string roleName, bool expectedResult)
        {
            roleName = AcademyUtility.EncryptValue(roleName);
            var result = AcademyUtility.IsUserSecureAcademy16To19User(roleName);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("", false, "")]
        [InlineData(" ", false, " ")]
        [InlineData(null, false, null)]
        [InlineData("", true, "")]
        [InlineData(" ", true, " ")]
        [InlineData(null, true, null)]
        public void
            GetDecryptedEstablishmentTypeId_WhenCalledWithNullOrWhitespaceEstablishmentTypeId_ReturnsExpectedResult
            (string establishmentTypeId, bool isUserSecureAcademy16To19, string expectedResult)
        {
            var result = AcademyUtility.GetDecryptedEstablishmentTypeId(establishmentTypeId, isUserSecureAcademy16To19);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("someValue", false, "someValue", false)]
        [InlineData("someValue", true, "someValue", true)]
        [InlineData("46", false, "46", false)]
        [InlineData("46", true, "46", true)]
        public void
            GetDecryptedEstablishmentTypeId_WhenCalledWithNonNullOrNonWhitespaceEstablishmentTypeId_ReturnsExpectedResult
            (string establishmentTypeId, bool isUserSecureAcademy16To19, string expectedResult, bool shouldEncrypt)
        {
            establishmentTypeId =
                shouldEncrypt ? AcademyUtility.EncryptValue(establishmentTypeId) : establishmentTypeId;
            var result = AcademyUtility.GetDecryptedEstablishmentTypeId(establishmentTypeId, isUserSecureAcademy16To19);

            Assert.Equal(expectedResult, result);
        }

        private Mock<IPrincipal> GetMockPrincipalIdentity(string roleName)
        {
            var claims = new List<Claim>()
            {
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", $"{roleName}"),
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "SomeValue")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var mockPrincipal = new Mock<IPrincipal>();
            mockPrincipal.Setup(i => i.Identity).Returns(identity);
            return mockPrincipal;
        }
    }
}
