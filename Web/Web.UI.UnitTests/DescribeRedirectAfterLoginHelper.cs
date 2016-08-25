using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Web.Identity;
using Web.UI.Helpers;
using Web.UI.Utils;

namespace Web.UI.UnitTests
{
    public class DescribeRedirectAfterLoginHelper
    {
        private RedirectAfterLoginHelper _sut;
        private Mock<IUserIdentity> _userIdentity;
        private Mock<IRequestContext> _requestContext;
        private RequestContext _requestContextImpl;
        private Mock<HttpContextBase> _httpContext;

        [SetUp]
        public void SetUp()
        {
            _userIdentity = new Mock<IUserIdentity>();
            _requestContext = new Mock<IRequestContext>();

            _httpContext = new Mock<HttpContextBase>();

            _requestContextImpl = new RequestContext
            {
                HttpContext = _httpContext.Object,
                RouteData = new RouteData()
            };

            _requestContext.Setup(o => o.GetContext()).Returns(_requestContextImpl);

            var fixture = new Fixture();
            fixture.Register(() => _userIdentity.Object);
            fixture.Register(() => _requestContext.Object);

            _sut = fixture.Create<RedirectAfterLoginHelper>();
        }

        [Test]
        public void ShouldRedirectToReturnUrlWhenSpecified()
        {
            // Arrange
            const string returnUrl = "/Schools";

            // Act
            var actual = _sut.GetResult(returnUrl) as RedirectResult;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Url, Is.EqualTo("/Schools"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ShouldRedirectToHomeIndexWhenNotInRoleWithNoReturnUrl(string returnUrl)
        {
            // Arrange
            var expected = new Dictionary<string, object> {

                { "action", "Index"},
                { "controller", "Home"},
            };

            // Act
            var actual = _sut.GetResult(returnUrl) as RedirectToRouteResult;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RouteValues, Is.EqualTo(expected ));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ShouldRedirectToSearchWhenInAccessAllSchoolsRoleWithNoReturnUrl(string returnUrl)
        {
            // Arrange
            _userIdentity.Setup(o => o.IsInRole(IdentityConstants.AccessAllSchoolsRoleName)).Returns(true);

            var expected = new Dictionary<string, object> {

                { "action", "Index"},
                { "controller", "Search"},
            };

            // Act
            var actual = _sut.GetResult(returnUrl) as RedirectToRouteResult;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RouteValues, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldRedirectToReturnUrlWhenSpecifiedWhileInAccessAllSchoolsRole()
        {
            // Arrange
            const string returnUrl = "/Schools";
            _userIdentity.Setup(o => o.IsInRole(IdentityConstants.AccessAllSchoolsRoleName)).Returns(true);

            // Act
            var actual = _sut.GetResult(returnUrl) as RedirectResult;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Url, Is.EqualTo(returnUrl));
        }

        [Test]
        public void ShouldNotRedirectWhenUrlIsNotLocalUrl()
        {
            // Arrange
            var expected = new Dictionary<string, object> {

                { "action", "Index"},
                { "controller", "Home"},
            };

            // Act
            var actual = _sut.GetResult("http://www.google.com") as RedirectToRouteResult;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RouteValues, Is.EqualTo(expected));
        }
    }
}
