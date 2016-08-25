using System.Dynamic;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Web.Services.Api;
using Web.Services.Schools;

namespace Web.UnitTests
{
    [TestFixture]
    public class DescribeSchoolService
    {
        private SchoolService _sut;
        private Mock<ISchoolPermissions> _schoolPermissions;
        private Mock<IApiService> _apiService;

        [SetUp]
        public void SetUp()
        {
            _schoolPermissions = new Mock<ISchoolPermissions>();
            _apiService = new Mock<IApiService>();

            var fixture = new Fixture();
            fixture.Register(() => _schoolPermissions.Object);
            fixture.Register(() => _apiService.Object);

            _sut = fixture.Create<SchoolService>();
        }

        [Test]
        public void ShouldEnsureHasAccessToSchoolWhenGettingSchoolDetails()
        {
            // Act
            _sut.GetSchoolDetails(1);

            // Assert
            _schoolPermissions.Verify(o => o.EnsureHasAccessToSchool(1));
        }

        [Test]
        public void ShouldReturnSchoolDetailsFromApiService()
        {
            // Arrange
            var expected = new ExpandoObject();
            _apiService.Setup(o => o.GetSchoolById("1")).Returns(expected);

            // Act
            var actual = _sut.GetSchoolDetails(1);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}