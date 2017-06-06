using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Helpers
{
    public class MvcAuthorizeRolesAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public MvcAuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }

    public class HttpAuthorizeRolesAttribute : System.Web.Http.AuthorizeAttribute
    {
        public HttpAuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}