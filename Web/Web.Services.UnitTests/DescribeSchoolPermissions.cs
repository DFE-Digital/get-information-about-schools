using System;
using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Web.Identity;
using Web.Services.Schools;

namespace Web.UnitTests
{
    [TestFixture]
    public class DescribeSchoolPermissions
    {
        private SchoolPermissions _sut;
        private Mock<IUserIdentity> _userIdentity;

        [SetUp]
        public void SetUp()
        {
            _userIdentity = new Mock<IUserIdentity>();

            var fixture = new Fixture();
            fixture.Register(() => _userIdentity.Object);

            _sut = fixture.Create<SchoolPermissions>();
        }

        [Test]
        [TestCase("1", new[] {1})]
        [TestCase("1,2", new[] {1,2})]
        [TestCase("1,3,5", new[] {1,3,5})]
        [TestCase("", new int[] {})]
        public void ShouldReturnAccessibleSchoolIds(string claimValue, IEnumerable<int> expected)
        {
            // Arrange
            _userIdentity.Setup(o => o.FindFirstClaim(IdentityConstants.AccessibleSchoolIdsClaimTypeName))
                .Returns(new Claim(IdentityConstants.AccessibleSchoolIdsClaimTypeName, claimValue));

            // Act
            var actual = _sut.GetAccessibleSchoolIds();

            // Assert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldReturnNoAccessibleSchoolIdsWhenClaimNotPresent()
        {
            // Act
            var result = _sut.GetAccessibleSchoolIds();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserHasNoAccessToSchool()
        {
            // Arrange
            _userIdentity.Setup(o => o.FindFirstClaim(IdentityConstants.AccessibleSchoolIdsClaimTypeName))
                .Returns(new Claim(IdentityConstants.AccessibleSchoolIdsClaimTypeName, ""));

            // Assert
            Assert.Throws<UnauthorizedAccessException>(() => _sut.EnsureHasAccessToSchool(1));
        }

        [Test]
        public void ShouldNotThrowExceptionWhenUserHasAccessToSchool()
        {
            // Arrange
            _userIdentity.Setup(o => o.FindFirstClaim(IdentityConstants.AccessibleSchoolIdsClaimTypeName))
                .Returns(new Claim(IdentityConstants.AccessibleSchoolIdsClaimTypeName, "1"));

            // Assert
            _sut.EnsureHasAccessToSchool(1);
        }

        [Test]
        public void ShouldNotThrowExceptionWhenUserHasAccessToAllSchools()
        {
            // Arrange
            _userIdentity.Setup(o => o.IsInRole(IdentityConstants.AccessAllSchoolsRoleName)).Returns(true);

            // Assert
            _sut.EnsureHasAccessToSchool(1);
        }
    }
}
