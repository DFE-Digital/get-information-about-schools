using Edubase.Services.Domain;
using Edubase.Services.Security;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Security
{
    public class SecurityApiService : ISecurityService
    {
        private readonly HttpClientWrapper _httpClient;

        public SecurityApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public IPrincipal CreateAnonymousPrincipal() => new GenericPrincipal(new GenericIdentity("ANON"), new string[0]);

        public IPrincipal CreateSystemPrincipal()
        {
            throw new NotImplementedException(nameof(CreateSystemPrincipal) + " not yet implemented");
        }

        /// <summary>
        /// TODO: TEXCHANGE; temporary hack to enable the system to call lookup/dictionary APIs with the highest
        /// level of permissions.
        /// </summary>
        /// <returns></returns>
        public IPrincipal CreateBackOfficePrincipal()
        {
            var claims = new List<Claim>() { new Claim(EduClaimTypes.UserId, "777406") };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie));
        }

        public async Task<CreateGroupPermissionDto> GetCreateGroupPermissionAsync(IPrincipal principal)
        {
            return await _httpClient.GetAsync<CreateGroupPermissionDto>($"security/group/create-permission", principal);
        }


        public async Task<CreateEstablishmentPermissionDto> GetCreateEstablishmentPermissionAsync(IPrincipal principal)
        {
            return await _httpClient.GetAsync<CreateEstablishmentPermissionDto>($"establishment/cancreate", principal);
        }

    }
}
