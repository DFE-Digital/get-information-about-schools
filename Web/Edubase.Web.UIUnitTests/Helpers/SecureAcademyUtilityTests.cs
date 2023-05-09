using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Exceptions;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UIUnitTests.Helpers.Data;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers
{
    public class SecureAcademyUtilityTests
    {
        [Theory]
        [InlineData(null, "Manage academy openings")]
        [InlineData("", "Manage academy openings")]
        [InlineData(" ", "Manage academy openings")]
        [InlineData("46", "Manage secure academy 16-19 openings")]
        public void GetAcademyOpeningPageTitle_WhenCalledWithValidValues_ReturnsCorrectTitle(string establishmentTypeId,
            string expectedPageTitle)
        {
            var result = AcademyUtility.GetAcademyOpeningPageTitle(establishmentTypeId);

            Assert.Equal(result, expectedPageTitle);
        }

        [Fact]
        public void GetAcademyOpeningPageTitle_WhenCalledWithInvalidValues_ThrowsArgumentException() =>
            Assert.Throws<ArgumentException>(() => AcademyUtility.GetAcademyOpeningPageTitle("48"));


        [Theory]
        [InlineData("46", 2)]
        [InlineData("48", 6)]
        [InlineData(null, 6)]
        [InlineData("", 6)]
        [InlineData(" ", 6)]
        public void FilterEstablishmentsIfSecureAcademy16To19_WhenCalled_ReturnsCorrectlyFilteredEstablishment
            (string establishmentTypeId, int expectedResult)
        {
            var establishments = DataUtility.GetEstablishmentLookupDto();

            var result = AcademyUtility.FilterEstablishmentsIfSecureAcademy16To19
                (establishments, establishmentTypeId);

            Assert.Equal(result.Count(), expectedResult);
        }

        [Theory]
        [InlineData("45")]
        [InlineData("47")]
        public void GetEstablishmentSearchFilters_WhenCalledWithInvalidValues_ThrowsArgumentException(
            string establishmentTypeId)
        {
            Assert.Throws<ArgumentException>(() =>
                AcademyUtility.GetEstablishmentSearchFilters(DateTime.Now.Date.AddDays(-1), DateTime.Now,
                    establishmentTypeId));
        }

        [Theory]
        [InlineData("46")]
        public void GetEstablishmentSearchFilters_WhenCalledWithValidSecure16To19Values_ReturnsExpectedFilters(
            string establishmentTypeId)
        {
            var result = AcademyUtility.GetEstablishmentSearchFilters(DateTime.Now.Date.AddDays(-1), DateTime.Now,
                establishmentTypeId);

            Assert.IsType<EstablishmentSearchFilters>(result);
            Assert.Single(result.TypeIds);
            Assert.Equal(result.TypeIds.FirstOrDefault(), int.Parse(establishmentTypeId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetEstablishmentSearchFilters_WhenCalledWithValidNonSecure16To19Values_ReturnsExpectedFilters(
            string establishmentTypeId)
        {
            var result = AcademyUtility.GetEstablishmentSearchFilters(DateTime.Now.Date.AddDays(-1),
                DateTime.Now, establishmentTypeId);

            Assert.IsType<EstablishmentSearchFilters>(result);
            Assert.Equal(5, result.TypeIds.Length);
            Assert.Equal(result.TypeIds,  new[] { 36,37,38,39,40 });
        }

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
        [InlineData("YCS", "46", true)]
        [InlineData("YCS", "48", false)]
        [InlineData("ROLE_BACKOFFICE", "", true)]
        [InlineData("ROLE_BACKOFFICE", " ", true)]
        [InlineData("ROLE_BACKOFFICE", null, true)]
        [InlineData("ROLE_BACKOFFICE", "46", true)]
        [InlineData("ROLE_BACKOFFICE", "48", false)]
        [InlineData("EFADO", "", true)]
        [InlineData("EFADO", " ", true)]
        [InlineData("EFADO", null, true)]
        [InlineData("EFADO", "46", false)]
        [InlineData("EFADO", "48", false)]
        public void DoesHaveAccessAuthorization_WhenCalled_ReturnsExpectedResult(string roleName,
            string establishmentTypeId, bool expectedResult)
        {
            var user = GetMockPrincipalIdentity(roleName);

            var result = AcademyUtility.DoesHaveAccessAuthorization(user.Object, establishmentTypeId);

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void GetPermissionDeniedException_WhenCalled_ReturnsGetPermissionDeniedException()
        {
            var result = AcademyUtility.GetPermissionDeniedException();

            Assert.IsType<PermissionDeniedException>(result);
            Assert.Equal("Attempt to access a resource without the right permission or value", result.Message);
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
