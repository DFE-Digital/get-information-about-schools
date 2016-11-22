using Edubase.Common;
using Edubase.Services.Security.Permissions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Security
{
    public static class SecurityExtensionMethods
    {
        public static EditEstablishmentPermissions GetEditEstablishmentPermissions(this ClaimsPrincipal principal) 
            => principal.GetFromClaim<EditEstablishmentPermissions>(EduClaimTypes.EditEstablishment);

        public static CreateEstablishmentPermissions GetCreateEstablishmentPermissions(this ClaimsPrincipal principal) 
            => principal.GetFromClaim<CreateEstablishmentPermissions>(EduClaimTypes.CreateEstablishment);

        public static CreateGroupPermissions GetCreateGroupPermissions(this ClaimsPrincipal principal)
                    => principal.GetFromClaim<CreateGroupPermissions>(EduClaimTypes.EditEstablishment);

        public static EditGroupPermissions GetEditGroupPermissions(this ClaimsPrincipal principal)
                            => principal.GetFromClaim<EditGroupPermissions>(EduClaimTypes.EditGroup);

        private static T GetFromClaim<T>(this ClaimsPrincipal principal, string claimType) where T : new()
        {
            Guard.IsNotNull(principal, () => new ArgumentNullException(nameof(principal)));

            var token = principal.FindFirst(claimType)?.Value;
            if (token != null) return UriHelper.DeserializeUrlToken<T>(token);
            else return new T();
        }

        internal static ClaimsPrincipal AsClaimsPrincipal(this IPrincipal principal)
        {
            Guard.Is<ClaimsPrincipal>(principal, $"The passed principal is of the wrong type ({principal.GetType().Name})");
            return (ClaimsPrincipal)principal;
        }

    }
}
