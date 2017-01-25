using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Security.Permissions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Data.Entity;
using Edubase.Data.Identity;
using Microsoft.AspNet.Identity;

namespace Edubase.Services.Security
{
    public class SecurityService : ISecurityService
    {
        public EditEstablishmentPermissions GetEditEstablishmentPermission(IPrincipal principal)
            => principal.AsClaimsPrincipal().GetEditEstablishmentPermissions();
        
        public CreateEstablishmentPermissions GetCreateEstablishmentPermission(IPrincipal principal) 
            => principal.AsClaimsPrincipal().GetCreateEstablishmentPermissions();

        public CreateGroupPermissions GetCreateGroupPermission(IPrincipal principal)
            => principal.AsClaimsPrincipal().GetCreateGroupPermissions();

        public EditGroupPermissions GetEditGroupPermission(IPrincipal principal)
            => principal.AsClaimsPrincipal().GetEditGroupPermissions();

        public async Task<ClaimsIdentity> LoginAsync(ClaimsIdentity id, IClaimsIdConverter claimsIdConverter, ApplicationUserManager userManager)
        {
            string userName = id.GetUserId();
            string userId;
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = userName };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded) userId = user.Id;
                else throw new Exception("Error creating user: " + string.Join(", ", result.Errors));
            }
            else userId = user.Id;

            return claimsIdConverter.Convert(id, userId);
        }

        /// <summary>
        /// Creates a system principal whose permissions are the same as Admin.
        /// This allows internal services to gain full permissions.
        /// </summary>
        /// <returns></returns>
        public IPrincipal CreateSystemPrincipal() => new GenericPrincipal(new GenericIdentity("SYSTEM"), new[] { EdubaseRoles.Admin });
    }
}
