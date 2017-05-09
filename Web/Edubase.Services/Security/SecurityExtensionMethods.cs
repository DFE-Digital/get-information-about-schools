using Edubase.Common;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Edubase.Services.Security
{
    public static class SecurityExtensionMethods
    {

        private static T GetFromClaim<T, TDefault>(this ClaimsPrincipal principal, string claimType) where TDefault : T, new()
        {
            Guard.IsNotNull(principal, () => new ArgumentNullException(nameof(principal)));

            var token = principal.FindFirst(claimType)?.Value;
            if (token != null) return UriHelper.DeserializeUrlToken<T>(token);
            else return new TDefault();
        }

        internal static ClaimsPrincipal AsClaimsPrincipal(this IPrincipal principal)
        {
            Guard.Is<ClaimsPrincipal>(principal, $"The passed principal is of the wrong type ({principal.GetType().Name})");
            return (ClaimsPrincipal)principal;
        }

        /// <summary>
        /// Returns whether a principal is in at least one of the roles supplied.
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static bool InRole(this IPrincipal principal, params string[] roles)
            => roles.Any(x => principal.IsInRole(x));
    }
}
