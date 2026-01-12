using System.Collections.Generic;
using System.Security;
using System.Security.Claims;
using Edubase.Common;
using Edubase.Services.Security;

namespace Edubase.Web.UI.Authentication.ClaimsIdentityConverters
{
    /// <summary>
    /// Converts a SAML2-provided <see cref="ClaimsIdentity"/> into an application cookie identity.
    /// Ensures required SAML claims are present, validated, and mapped to application-specific claim types.
    /// </summary>
    public class SecureAccessClaimsIdConverter
    {
        /// <summary>
        /// SAML attribute containing the Secure Access user identifier.
        /// </summary>
        private const string SA_USER_ID_SAML_ATTR = "urn:oid:2.5.4.45";

        /// <summary>
        /// SAML attribute containing the username.
        /// </summary>
        private const string USERNAME_SAML_ATTR = "urn:oid:0.9.2342.19200300.100.1.1";

        /// <summary>
        /// SAML attribute containing the user's first name.
        /// </summary>
        private const string FIRSTNAME_SAML_ATTR = "urn:oid:2.5.4.42";

        /// <summary>
        /// SAML attribute containing the user's last name.
        /// </summary>
        private const string LASTNAME_SAML_ATTR = "urn:oid:2.5.4.4";

        /// <summary>
        /// Converts the incoming SAML identity into an application cookie identity.
        /// Validates the presence of the required user ID claim and maps optional SAML attributes.
        /// </summary>
        /// <param name="id">The incoming SAML claims identity.</param>
        /// <returns>A new <see cref="ClaimsIdentity"/> configured for the application cookie scheme.</returns>
        /// <exception cref="SecurityException">
        /// Thrown when the required user ID SAML claim is missing or contains an invalid value.
        /// </exception>
        public ClaimsIdentity Convert(ClaimsIdentity id)
        {
            // Retrieve the mandatory user ID claim from SAML
            Claim userIdClaim = id.FindFirst(SA_USER_ID_SAML_ATTR);

            // Ensure the claim exists
            Guard.IsNotNull(userIdClaim, () =>
                new SecurityException("The user id SAML assertion/claim was not present"));

            // Ensure the claim value is not null or empty
            Guard.IsNotNull(userIdClaim.Value.Clean(), () =>
                new SecurityException("The user id SAML assertion/claim VALUE is empty or null"));

            // Always include the required UserId claim in the resulting identity
            List<Claim> claims =
            [
                new Claim(EduClaimTypes.UserId, userIdClaim.Value)
            ];

            // Map optional SAML attributes to application-specific claim types
            Dictionary<string, string> optionalClaimsMap = new()
            {
                { USERNAME_SAML_ATTR, EduClaimTypes.UserName },
                { FIRSTNAME_SAML_ATTR, EduClaimTypes.FirstName },
                { LASTNAME_SAML_ATTR, EduClaimTypes.LastName }
            };

            // Iterate through optional mappings and add claims when present
            foreach (KeyValuePair<string, string> mapping in optionalClaimsMap)
            {
                Claim claim = id.FindFirst(mapping.Key);

                // Only add the claim if a non-empty value exists
                if (!string.IsNullOrEmpty(claim?.Value))
                {
                    claims.Add(new Claim(mapping.Value, claim.Value));
                }
            }

            // Return a new identity configured for the application cookie authentication scheme
            return new ClaimsIdentity(
                claims,
                Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie
            );
        }
    }
}
