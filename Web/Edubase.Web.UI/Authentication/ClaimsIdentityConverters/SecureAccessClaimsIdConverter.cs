using System.Collections.Generic;
using System.Security;
using System.Security.Claims;
using Edubase.Common;
using Edubase.Services.Security;

namespace Edubase.Web.UI.Authentication.ClaimsIdentityConverters;

/// <summary>
/// Converts a SAML2-provided <see cref="ClaimsIdentity"/> into an application cookie identity.
/// Ensures required SAML claims are present and valid.
/// </summary>
public class SecureAccessClaimsIdConverter
{
    private const string SA_USER_ID_SAML_ATTR = "urn:oid:2.5.4.45";
    private const string USERNAME_SAML_ATTR = "urn:oid:0.9.2342.19200300.100.1.1";
    private const string FIRSTNAME_SAML_ATTR = "urn:oid:2.5.4.42";
    private const string LASTNAME_SAML_ATTR = "urn:oid:2.5.4.4";

    /// <summary>
    /// Converts the incoming SAML identity into an application cookie identity.
    /// Throws a <see cref="SecurityException"/> if the required user id claim is missing or invalid.
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

        // Always include the required UserId claim
        List<Claim> claims =
        [
            new Claim(EduClaimTypes.UserId, userIdClaim.Value)
        ];

        // Map optional SAML attributes to EduClaimTypes
        Dictionary<string, string> optionalClaimsMap = new()
        {
            { USERNAME_SAML_ATTR, EduClaimTypes.UserName },
            { FIRSTNAME_SAML_ATTR, EduClaimTypes.FirstName },
            { LASTNAME_SAML_ATTR, EduClaimTypes.LastName }
        };

        foreach (KeyValuePair<string, string> mapping in optionalClaimsMap)
        {
            Claim claim = id.FindFirst(mapping.Key);

            if (!string.IsNullOrEmpty(claim?.Value))
            {
                claims.Add(new Claim(mapping.Value, claim.Value));
            }
        }

        return new ClaimsIdentity(claims, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
    }
}
