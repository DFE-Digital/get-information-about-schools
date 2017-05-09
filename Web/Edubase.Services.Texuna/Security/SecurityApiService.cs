using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Security.ClaimsIdentityConverters;
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
    }
}
