using Edubase.Services.Governors;
using System;
using System.Threading.Tasks;
using Edubase.Services.Governors.Models;
using System.Security.Principal;
using Edubase.Services.Domain;

namespace Edubase.Services.Texuna.Governors
{
    public class GovernorsWriteApiService : IGovernorsWriteService
    {
        private readonly HttpClientWrapper _httpClient;

        public GovernorsWriteApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DeleteAsync(int id, IPrincipal principal)
        {
            await _httpClient.DeleteAsync($"governor/{id}", null, principal);
        }

        public async Task<ApiResponse> UpdateDatesAsync(int governorId, DateTime appointmentStartDate, DateTime appointmentEndDate, IPrincipal principal)
        {
            return await _httpClient.PatchAsync($"governor/{governorId}", new
            {
                AppointmentStartDate = appointmentStartDate,
                AppointmentEndDate = appointmentEndDate
            }, principal);
        }

        public async Task<ApiResponse> UpdateDatesAsync(int governorId, DateTime appointmentEndDate, IPrincipal principal)
        {
            return await _httpClient.PatchAsync($"governor/{governorId}", new
            {
                AppointmentEndDate = appointmentEndDate
            }, principal);
        }

        public async Task<ApiResponse<int>> SaveAsync(GovernorModel model, IPrincipal principal)
        {
            if (model.IsNewEntity)
            {
                var response = await _httpClient.PostAsync<NumericResultDto>("governor", model, principal);
                return new ApiResponse<int>
                {
                    Success = response.Success,
                    Response = response.Response?.Value ?? 0,
                    Errors = response.Errors
                };
            }

            var postResponse = await _httpClient.PostAsync("governor", model, principal);
            return new ApiResponse<int>
            {
                Success = postResponse.Success,
                Response = model.Id.Value,
                Errors = postResponse.Errors
            };
        }

        public async Task<ValidationEnvelopeDto> ValidateAsync(GovernorModel model, IPrincipal principal)
         {
            return (await _httpClient.PostAsync<ValidationEnvelopeDto>("governor/validate", model, principal)).GetResponse();
        }

        public async Task<GovernorBulkUpdateValidationResult> BulkUpdateValidateAsync(string fileName, IPrincipal principal)
        {
            var apiResponse = (await _httpClient.PostMultipartAsync<GovernorBulkUpdateValidationResult>("governor/bulk-update", null, fileName, principal));
            if (apiResponse.Success) return apiResponse.GetResponse();
            else return new GovernorBulkUpdateValidationResult { Errors = apiResponse.Errors };
        }

        public async Task<ApiResponse> BulkUpdateProcessRequestAsync(string id, IPrincipal principal)
        {
            return (await _httpClient.PostAsync($"governor/bulk-update/{id}", null, principal));
        }

        public async Task<ApiResponse> AddSharedGovernorAppointmentAsync(int governorId, int establishmentUrn, DateTime appointmentStart, DateTime? appointmentEnd, IPrincipal principal)
        {
            return await _httpClient.PostAsync($"governor/{governorId}/establishment/{establishmentUrn}", new {startDate = appointmentStart, endDate = appointmentEnd}, principal);
        }

        public async Task<ApiResponse> UpdateSharedGovernorAppointmentAsync(int governorId, int establishmentUrn,
            DateTime appointmentStart, DateTime? appointmentEnd, IPrincipal principal)
        {
            return await _httpClient.PutAsync($"governor/{governorId}/establishment/{establishmentUrn}", new { startDate = appointmentStart, endDate = appointmentEnd }, principal);
        }

        public async Task DeleteSharedGovernorAppointmentAsync(int governorId, int establishmentUrn, IPrincipal principal)
        {
            await _httpClient.DeleteAsync($"governor/{governorId}/establishment/{establishmentUrn}", null, principal);
        }
    }
}
