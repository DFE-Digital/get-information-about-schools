using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Edubase.Web.UI.Helpers
{
    public class AuthHelper
    {
        public static string GetRole()
        {
            var roles = ((ClaimsPrincipal)HttpContext.Current.User)
                .Claims.Where(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)
                .Select(x => x.Value).ToArray();
            return roles.FirstOrDefault();
        }
    }
}