using Microsoft.AspNetCore.Authorization;

namespace Edubase.Web.UI.Helpers
{
    public class MvcAuthorizeRolesAttribute : AuthorizeAttribute
    {
        public MvcAuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }

        public MvcAuthorizeRolesAttribute(string roles) : base()
        {
            Roles = roles;
        }
    }

    public class HttpAuthorizeRolesAttribute : AuthorizeAttribute
    {
        public HttpAuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }

        public HttpAuthorizeRolesAttribute(string roles) : base()
        {
            Roles = roles;
        }
    }
}
