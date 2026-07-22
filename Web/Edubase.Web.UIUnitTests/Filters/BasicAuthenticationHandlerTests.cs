using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Filters
{
    public class BasicAuthenticationHandlerTests
    {
        private const string ConfigKey = "AppSettings:BasicAuthCredentials";
        private const string Username = "user";
        private const string Password = "pass";

        private static IConfiguration CreateConfiguration(string credentials) => new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>(ConfigKey, credentials) }).Build();

        private class TestBasicAuthenticationHandler : Edubase.Web.UI.Filters.BasicAuthenticationHandler
        {
            public TestBasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, UrlEncoder encoder, IConfiguration configuration)
                : base(options, NullLoggerFactory.Instance, encoder, configuration)
            {
            }

            public Task<AuthenticateResult> InvokeHandleAuthenticateAsync() => base.HandleAuthenticateAsync();

            public Task InvokeHandleChallengeAsync(AuthenticationProperties props) => base.HandleChallengeAsync(props);
        }

        private static async Task<TestBasicAuthenticationHandler> CreateInitializedHandler(HttpContext context, IConfiguration configuration)
        {
            var handler = new TestBasicAuthenticationHandler(CreateOptionsMonitor(), UrlEncoder.Default, configuration);
            var scheme = new AuthenticationScheme("Basic", "Basic", typeof(TestBasicAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);
            return handler;
        }

        private static IOptionsMonitor<AuthenticationSchemeOptions> CreateOptionsMonitor()
        {
            var mock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            var opts = new AuthenticationSchemeOptions();
            mock.Setup(m => m.CurrentValue).Returns(opts);
            mock.Setup(m => m.Get(It.IsAny<string>())).Returns(opts);
            return mock.Object;
        }

        [Fact]
        public async Task HandleAuthenticateAsync_NoAuthorizationHeader_ReturnsNoResult()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var configuration = CreateConfiguration($"{Username}:{Password}");
            var handler = await CreateInitializedHandler(context, configuration);

            // Act
            var result = await handler.InvokeHandleAuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.None);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_NonBasicScheme_ReturnsNoResult()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer sometoken";
            var configuration = CreateConfiguration($"{Username}:{Password}");
            var handler = await CreateInitializedHandler(context, configuration);

            // Act
            var result = await handler.InvokeHandleAuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.None);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_InvalidBase64_ReturnsFailWithFormatMessage()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Basic not-base64!";
            var configuration = CreateConfiguration($"{Username}:{Password}");
            var handler = await CreateInitializedHandler(context, configuration);

            // Act
            var result = await handler.InvokeHandleAuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid Authorization Header Format", result.Failure.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WrongCredentials_ReturnsFailWithInvalidCredentialsMessage()
        {
            // Arrange
            var wrong = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:wrongpass"));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Basic {wrong}";
            var configuration = CreateConfiguration($"{Username}:{Password}");
            var handler = await CreateInitializedHandler(context, configuration);

            // Act
            var result = await handler.InvokeHandleAuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid Username or Password", result.Failure.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ValidCredentials_ReturnsSuccessWithClaims()
        {
            // Arrange
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Basic {encoded}";
            var configuration = CreateConfiguration($"{Username}:{Password}");
            var handler = await CreateInitializedHandler(context, configuration);

            // Act
            var result = await handler.InvokeHandleAuthenticateAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Principal);
            var idClaim = result.Principal.FindFirst(ClaimTypes.NameIdentifier);
            var nameClaim = result.Principal.FindFirst(ClaimTypes.Name);
            Assert.NotNull(idClaim);
            Assert.NotNull(nameClaim);
            Assert.Equal(Username, idClaim.Value);
            Assert.Equal(Username, nameClaim.Value);
        }

        [Fact]
        public async Task HandleChallengeAsync_Sets401HeaderAndWritesBody()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var ms = new MemoryStream();
            context.Response.Body = ms;
            var configuration = CreateConfiguration($"{Username}:{Password}");
            var handler = await CreateInitializedHandler(context, configuration);

            // Act
            await handler.InvokeHandleChallengeAsync(new AuthenticationProperties());

            // Assert - status code
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

            // Assert - WWW-Authenticate header
            Assert.True(context.Response.Headers.ContainsKey("WWW-Authenticate"));
            Assert.Contains("Basic realm=\"Secure Website\"", context.Response.Headers["WWW-Authenticate"].ToString());

            // Assert - body content
            ms.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(ms, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            Assert.Equal("Unauthorized Access.", body);
        }
    }
}
