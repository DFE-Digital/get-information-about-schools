using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Services.Security.Permissions;
using System.Security.Claims;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Security
{
    public class SecurityApiService : ISecurityService
    {
        public IPrincipal CreateAnonymousPrincipal() => new GenericPrincipal(new GenericIdentity("ANON"), new string[0]);

        public IPrincipal CreateSystemPrincipal()
        {
            throw new NotImplementedException(nameof(CreateSystemPrincipal) + " not yet implemented");
        }

        public CreateEstablishmentPermissions GetCreateEstablishmentPermission(IPrincipal principal)
        {
            throw new NotImplementedException(nameof(GetCreateEstablishmentPermission) + " not yet implemented");
        }

        public CreateGroupPermissions GetCreateGroupPermission(IPrincipal principal)
        {
            throw new NotImplementedException(nameof(GetCreateGroupPermission) + " not yet implemented");
        }

        public EditEstablishmentPermissions GetEditEstablishmentPermission(IPrincipal principal)
        {
            throw new NotImplementedException(nameof(GetEditEstablishmentPermission) + " not yet implemented");
        }

        public EditGroupPermissions GetEditGroupPermission(IPrincipal principal)
        {
            throw new NotImplementedException(nameof(GetEditGroupPermission) + " not yet implemented");
        }

        public string GetUserId(IPrincipal principal)
        {
            throw new NotImplementedException(nameof(GetUserId) + " not yet implemented");
        }

        public Task<ClaimsIdentity> LoginAsync(ClaimsIdentity id, IClaimsIdConverter claimsIdConverter, Edubase.Data.Identity.ApplicationUserManager userManager) // TODO: TEXCHANGE; remove dep on Edubase.Data 
        {
            throw new NotImplementedException(nameof(LoginAsync) + " not yet implemented");
        }
    }
}
