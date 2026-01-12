using System.Security;
using System.Security.Claims;
using Edubase.Services.Security;
using Edubase.Web.UI.Authentication.ClaimsIdentityConverters;
using Xunit;

namespace Edubase.Web.UIUnitTests.Authentication.ClaimsIdentityConverters;

public class SecureAccessClaimsIdConverterTests
{
    private readonly SecureAccessClaimsIdConverter _converter = new();

    private const string SA_USER_ID_SAML_ATTR = "urn:oid:2.5.4.45";
    private const string USERNAME_SAML_ATTR = "urn:oid:0.9.2342.19200300.100.1.1";
    private const string FIRSTNAME_SAML_ATTR = "urn:oid:2.5.4.42";
    private const string LASTNAME_SAML_ATTR = "urn:oid:2.5.4.4";

    [Fact]
    public void Convert_Throws_WhenUserIdClaimMissing()
    {
        // Arrange
        var identity = new ClaimsIdentity();

        // Act & Assert
        Assert.Throws<SecurityException>(() => _converter.Convert(identity));
    }

    [Fact]
    public void Convert_Throws_WhenUserIdClaimValueIsEmpty()
    {
        // Arrange
        var identity = new ClaimsIdentity(
        [
            new Claim(SA_USER_ID_SAML_ATTR, "")
        ]);

        // Act & Assert
        Assert.Throws<SecurityException>(() => _converter.Convert(identity));
    }

    [Fact]
    public void Convert_ReturnsIdentity_WithRequiredUserIdClaim()
    {
        // Arrange
        var identity = new ClaimsIdentity(
        [
            new Claim(SA_USER_ID_SAML_ATTR, "12345")
        ]);

        // Act
        var result = _converter.Convert(identity);

        // Assert
        var userIdClaim = result.FindFirst(EduClaimTypes.UserId);
        Assert.NotNull(userIdClaim);
        Assert.Equal("12345", userIdClaim.Value);
    }

    [Fact]
    public void Convert_MapsOptionalClaims_WhenPresent()
    {
        // Arrange
        var identity = new ClaimsIdentity(
        [
            new Claim(SA_USER_ID_SAML_ATTR, "12345"),
            new Claim(USERNAME_SAML_ATTR, "jdoe"),
            new Claim(FIRSTNAME_SAML_ATTR, "John"),
            new Claim(LASTNAME_SAML_ATTR, "Doe")
        ]);

        // Act
        var result = _converter.Convert(identity);

        // Assert
        Assert.Equal("jdoe", result.FindFirst(EduClaimTypes.UserName)?.Value);
        Assert.Equal("John", result.FindFirst(EduClaimTypes.FirstName)?.Value);
        Assert.Equal("Doe", result.FindFirst(EduClaimTypes.LastName)?.Value);
    }

    [Fact]
    public void Convert_DoesNotAddOptionalClaims_WhenMissing()
    {
        // Arrange
        var identity = new ClaimsIdentity(
        [
            new Claim(SA_USER_ID_SAML_ATTR, "12345")
        ]);

        // Act
        var result = _converter.Convert(identity);

        // Assert
        Assert.Null(result.FindFirst(EduClaimTypes.UserName));
        Assert.Null(result.FindFirst(EduClaimTypes.FirstName));
        Assert.Null(result.FindFirst(EduClaimTypes.LastName));
    }

    [Fact]
    public void Convert_SetsAuthenticationType_ToApplicationCookie()
    {
        // Arrange
        var identity = new ClaimsIdentity(
        [
            new Claim(SA_USER_ID_SAML_ATTR, "12345")
        ]);

        // Act
        var result = _converter.Convert(identity);

        // Assert
        Assert.Equal(
            Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie,
            result.AuthenticationType
        );
    }
}

