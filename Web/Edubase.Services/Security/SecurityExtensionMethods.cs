using Edubase.Common;
using Edubase.Services.Security.Permissions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Edubase.Services.Security
{
    public static class SecurityExtensionMethods
    {
        public static EditEstablishmentPermissions GetEditEstablishmentPermissions(this ClaimsPrincipal principal) 
            => principal.GetFromClaim<EditEstablishmentPermissions, NoEditEstablishmentPermissions>(EduClaimTypes.EditEstablishment);

        public static CreateEstablishmentPermissions GetCreateEstablishmentPermissions(this ClaimsPrincipal principal) 
            => principal.GetFromClaim<CreateEstablishmentPermissions, NoCreateEstablishmentPermissions>(EduClaimTypes.CreateEstablishment);

        public static CreateGroupPermissions GetCreateGroupPermissions(this ClaimsPrincipal principal)
                    => principal.GetFromClaim<CreateGroupPermissions, NoCreateGroupPermissions>(EduClaimTypes.CreateGroup);

        public static EditGroupPermissions GetEditGroupPermissions(this ClaimsPrincipal principal)
                            => principal.GetFromClaim<EditGroupPermissions, NoEditGroupPermissions>(EduClaimTypes.EditGroup);

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
