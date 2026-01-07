using System.Collections.Generic;
using System.Security;
using System.Security.Claims;
using Edubase.Common;
using Edubase.Services.Security;
using Microsoft.AspNet.Identity;

namespace Edubase.Web.UI.Authentication.ClaimsIdentityConverters;

/// <summary>
/// Converts a SAML2-provided <see cref="ClaimsIdentity"/> into an application cookie identity.
/// Ensures the required SAML user id claim is present and valid.
/// </summary>
public class StubClaimsIdentityConverter
{
    private const string SA_USER_ID_SAML_ATTR = EduClaimTypes.SAUserId;

    /// <summary>
    /// Converts the incoming SAML identity into an application cookie identity.
    /// Throws a <see cref="SecurityException"/> if the required claim is missing or invalid.
    /// </summary>
    /// <param name="id">The incoming SAML claims identity.</param>
    /// <returns>A new <see cref="ClaimsIdentity"/> for the application cookie scheme.</returns>
    public ClaimsIdentity Convert(ClaimsIdentity id)
    {
        Claim userIdClaim = id.FindFirst(SA_USER_ID_SAML_ATTR);

        Guard.IsNotNull(userIdClaim, () =>
            new SecurityException("The user id SAML assertion/claim was not present"));

        Guard.IsNotNull(userIdClaim.Value.Clean(), () =>
            new SecurityException("The user id SAML assertion/claim VALUE is empty or null"));

        List<Claim> claims =
        [
            new Claim(EduClaimTypes.UserId, userIdClaim.Value)
        ];

        return new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
    }
}
