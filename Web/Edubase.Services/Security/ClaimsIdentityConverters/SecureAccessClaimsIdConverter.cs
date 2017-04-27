using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Edubase.Common;

namespace Edubase.Services.Security.ClaimsIdentityConverters
{
    public class SecureAccessClaimsIdConverter
    {
        const string SA_USER_ID_SAML_ATTR = "urn:oid:2.5.4.45";
        const string USERNAME_SAML_ATTR = "urn:oid:0.9.2342.19200300.100.1.1";
        const string FIRSTNAME_SAML_ATTR = "urn:oid:2.5.4.42";
        const string LASTNAME_SAML_ATTR = "urn:oid:2.5.4.4";

        public ClaimsIdentity Convert(ClaimsIdentity id)
        {
            var userIdClaim = id.FindFirst(SA_USER_ID_SAML_ATTR);
            var userNameClaim = id.FindFirst(USERNAME_SAML_ATTR);
            var firstNameClaim = id.FindFirst(FIRSTNAME_SAML_ATTR);
            var lastNameClaim = id.FindFirst(LASTNAME_SAML_ATTR);

            Guard.IsNotNull(userIdClaim, () => new Exceptions.EduSecurityException("The user id SAML assertion / claim was not present"));
            Guard.IsNotNull(userIdClaim.Value.Clean(), () => new Exceptions.EduSecurityException("The user id SAML assertion / claim VALUE is empty or null"));

            var claims = new List<Claim>() { new Claim(EduClaimTypes.UserId, userIdClaim.Value) };
            if (userNameClaim?.Value != null) claims.Add(new Claim(EduClaimTypes.UserName, userNameClaim.Value));
            if (firstNameClaim?.Value != null) claims.Add(new Claim(EduClaimTypes.FirstName, firstNameClaim.Value));
            if (lastNameClaim?.Value != null) claims.Add(new Claim(EduClaimTypes.LastName, lastNameClaim.Value));

            return new ClaimsIdentity(claims, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}
