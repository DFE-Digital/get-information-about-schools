using Edubase.Services.Domain;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using System.Security.Principal;
using System.Threading.Tasks;
using System;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupsWriteApiService : IGroupsWriteService
    {
        private readonly HttpClientWrapper _httpClient;

        public GroupsWriteApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse> SaveAsync(SaveGroupDto dto, IPrincipal principal) => await _httpClient.PutAsync($"group", dto, principal);

        public async Task<ApiResponse<NumericResultDto>> SaveNewAsync(SaveGroupDto dto, IPrincipal principal)
            => (await _httpClient.PostAsync<NumericResultDto>($"group", dto, principal));
            
        public async Task<ValidationEnvelopeDto> ValidateAsync(SaveGroupDto dto, IPrincipal principal)
        {
            if (dto.IsNewEntity) return (await _httpClient.PostAsync<ValidationEnvelopeDto>($"group/validate", dto, principal)).GetResponse();
            else return (await _httpClient.PutAsync<ValidationEnvelopeDto>($"group/validate", dto, principal)).Response;
        }

        public async Task<ApiResponse> ConfirmGovernanceAsync(int uid, IPrincipal principal) => await _httpClient.PostAsync($"/group/{uid}/governance/confirm", null, principal);
    }
}
