using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;

namespace Edubase.Services.Security.ClaimsIdentityConverters
{
    /// <summary>
    /// Responsible for converting the incoming external claim identity from the Stub IDP into the internally-used Claims Id.
    /// </summary>
    public class StubClaimsIdConverter
    {
        const string SA_USER_ID_SAML_ATTR = EduClaimTypes.SAUserId;
        
        public ClaimsIdentity Convert(ClaimsIdentity id)
        {
            var userIdClaim = id.FindFirst(SA_USER_ID_SAML_ATTR);
            
            Guard.IsNotNull(userIdClaim, () => new Exceptions.EduSecurityException("The user id SAML assertion / claim was not present"));
            Guard.IsNotNull(userIdClaim.Value.Clean(), () => new Exceptions.EduSecurityException("The user id SAML assertion / claim VALUE is empty or null"));

            var claims = new List<Claim>() { new Claim(EduClaimTypes.UserId, userIdClaim.Value) };
            
            return new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
        }

    }
}
