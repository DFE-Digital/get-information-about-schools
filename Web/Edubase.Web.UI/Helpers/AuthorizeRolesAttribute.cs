using System;
using Microsoft.AspNetCore.Authorization;

namespace Edubase.Web.UI.Helpers
{
    public class MvcAuthorizeRolesAttribute : AuthorizeAttribute
    {
        public MvcAuthorizeRolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        public MvcAuthorizeRolesAttribute(string roles)
        {
            Roles = roles;
        }
    }

    public class HttpAuthorizeRolesAttribute : AuthorizeAttribute
    {
        public HttpAuthorizeRolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        public HttpAuthorizeRolesAttribute(string roles)
        {
            Roles = roles;
        }
    }
}
