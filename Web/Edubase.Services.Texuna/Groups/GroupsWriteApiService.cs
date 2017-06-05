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

        public async Task<int> SaveAsync(SaveGroupDto dto, IPrincipal principal)
        {
            if (dto.IsNewEntity) return (await _httpClient.PostAsync<NumericResultDto>($"group", dto, principal)).Response.Value;
            else
            {
                await _httpClient.PutAsync($"group", dto, principal);
                return dto.GetGroupUId().Value;
            }
        }

        public async Task<ValidationEnvelopeDto> ValidateAsync(SaveGroupDto dto, IPrincipal principal)
        {
            if (dto.IsNewEntity) return (await _httpClient.PostAsync<ValidationEnvelopeDto>($"group/validate", dto, principal)).Response;
            else return (await _httpClient.PutAsync<ValidationEnvelopeDto>($"group/validate", dto, principal)).Response;
        }
    }
}
