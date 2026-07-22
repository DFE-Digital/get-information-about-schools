using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using System;

namespace Edubase.Web.UIUnitTests.Filters
{
    public class BasicAuthenticationGateMiddlewareTests
    {
        private const string BasicAuthenticationScheme = "BasicAuthentication";

        private static DefaultHttpContext CreateHttpContextWithAuthService(Mock<IAuthenticationService> authServiceMock)
        {
            var context = new DefaultHttpContext();
            var services = new ServiceCollection();
            services.AddSingleton(authServiceMock.Object);
            context.RequestServices = services.BuildServiceProvider();
            return context;
        }

        [Fact]
        public async Task InvokeAsync_WhenAuthenticationSucceeds_CallsNext()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity()), BasicAuthenticationScheme);
            authServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), BasicAuthenticationScheme))
                .ReturnsAsync(AuthenticateResult.Success(ticket));

            // Challenge should not be called when authentication succeeds.
            authServiceMock
                .Setup(s => s.ChallengeAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Throws(new InvalidOperationException("Challenge should not be called for successful authentication"));

            var context = CreateHttpContextWithAuthService(authServiceMock);

            var nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new Edubase.Web.UI.Filters.BasicAuthenticationGateMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
            authServiceMock.Verify(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), BasicAuthenticationScheme), Times.Once);
            authServiceMock.Verify(s => s.ChallengeAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_WhenAuthenticationFails_CallsChallengeAndDoesNotCallNext()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);

            authServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), BasicAuthenticationScheme))
                .ReturnsAsync(AuthenticateResult.Fail("fail"));

            var challengeCalled = false;
            authServiceMock
                .Setup(s => s.ChallengeAsync(It.IsAny<HttpContext>(), BasicAuthenticationScheme, It.IsAny<AuthenticationProperties>()))
                .Callback(() => challengeCalled = true)
                .Returns(Task.CompletedTask);

            var context = CreateHttpContextWithAuthService(authServiceMock);

            var nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new Edubase.Web.UI.Filters.BasicAuthenticationGateMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.False(nextCalled);
            Assert.True(challengeCalled);
            authServiceMock.Verify(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), BasicAuthenticationScheme), Times.Once);
            authServiceMock.Verify(s => s.ChallengeAsync(It.IsAny<HttpContext>(), BasicAuthenticationScheme, It.IsAny<AuthenticationProperties>()), Times.Once);
        }
    }
}
