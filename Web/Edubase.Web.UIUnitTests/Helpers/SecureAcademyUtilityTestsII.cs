using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Edubase.Web.UI.Helpers;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers
{
    public class SecureAcademyUtilityTestsII
    {
        [Theory]
        [InlineData("YCS", "YCS")]
        [InlineData("ROLE_BACKOFFICE", "ROLE_BACKOFFICE")]
        [InlineData("EFADO", null)]
        public void GetSecureAcademy16To19Role_WhenUserIsCalled_ReturnsExpectedResult
            (string roleName, string expectedResult)
        {
            var mockPrincipal = GetMockPrincipalIdentity(roleName);

            var result = AcademyUtility.GetAuthorizedRole(mockPrincipal.Object);

            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData("someValue", "", false)]
        [InlineData("someValue", " ", false)]
        [InlineData("someValue", null, false)]
        [InlineData("YCS", "", false)]
        [InlineData("YCS", " ", false)]
        [InlineData("YCS", null, false)]
        [InlineData("YCS", "W3XrRxuGnzvRuqxllYzLlg==", true)]
        [InlineData("ROLE_BACKOFFICE", "", true)]
        [InlineData("ROLE_BACKOFFICE", " ", true)]
        [InlineData("ROLE_BACKOFFICE", null, true)]
        [InlineData("ROLE_BACKOFFICE", "W3XrRxuGnzvRuqxllYzLlg==", true)]
        [InlineData("EFADO", "",  true)]
        [InlineData("EFADO", " ", true)]
        [InlineData("EFADO", null, true)]
        [InlineData("EFADO", "W3XrRxuGnzvRuqxllYzLlg==", false)]
        public void DoesHaveAccessAuthorization_WhenCalled_ReturnsExpectedResult(string roleName,
            string establishmentTypeId, bool expectedResult)
        {
            var user = GetMockPrincipalIdentity(roleName);

            var result = AcademyUtility.DoesHaveAccessAuthorization(user.Object, establishmentTypeId);

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
        [InlineData("EFADO", false)]
        [InlineData("YCS", true)]
        [InlineData("ROLE_BACKOFFICE", true)]
        public void IsUserSecureAcademy16To19User_WhenCalled_ReturnsExpectedResult(
            string roleName, bool expectedResult)
        {
            var result = AcademyUtility.IsUserSecureAcademy16To19User(roleName);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("",  "")]
        [InlineData(" ", " ")]
        [InlineData(null, null)]
        [InlineData("W3XrRxuGnzvRuqxllYzLlg==", "46")]

        public void
            GetDecryptedEstablishmentTypeId_WhenCalled_ReturnsExpectedResult
            (string establishmentTypeId, string expectedResult)
        {
            var result = AcademyUtility.GetDecryptedEstablishmentTypeId(establishmentTypeId);

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
