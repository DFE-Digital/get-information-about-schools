using Edubase.Services.Domain;
using Edubase.Services.Security;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Security
{
    public class SecurityApiService : ISecurityService
    {
        private readonly HttpClientWrapper _httpClient;

        public SecurityApiService(HttpClientWrapper httpClient) { _httpClient = httpClient; }

        public IPrincipal CreateAnonymousPrincipal() => new GenericPrincipal(new GenericIdentity("ANON"), new string[0]);
        
        public async Task<CreateGroupPermissionDto> GetCreateGroupPermissionAsync(IPrincipal principal)
            => await _httpClient.GetAsync<CreateGroupPermissionDto>($"security/group/create-permission", principal);
        
        public async Task<CreateEstablishmentPermissionDto> GetCreateEstablishmentPermissionAsync(IPrincipal principal)
            => await _httpClient.GetAsync<CreateEstablishmentPermissionDto>($"establishment/cancreate", principal);

        public async Task<string[]> GetRolesAsync(IPrincipal principal) => await _httpClient.GetAsync<string[]>($"security/role-names", principal);
    }
}
