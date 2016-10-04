using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Web;

namespace Edubase.Web.UI.Identity
{
    [ExcludeFromCodeCoverage]
    public class UserIdentity : IUserIdentity
    {
        public Claim FindFirstClaim(string type)
        {
            return ((ClaimsIdentity) HttpContext.Current.User.Identity).FindFirst(type);
        }

        public bool IsInRole(string role)
        {
            return HttpContext.Current.User.IsInRole(role);
        }
    }
}