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

        public async Task UpdateDatesAsync(int governorId, DateTime appointmentStartDate, DateTime appointmentEndDate, IPrincipal principal)
        {
            await _httpClient.PatchAsync($"governor/{governorId}", new
            {
                AppointmentStartDate = appointmentStartDate,
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
            var retVal = new GovernorBulkUpdateValidationResult();
            var json = (await _httpClient.PostMultipartAsync<string>("governor/bulk-update", null, fileName, principal)).GetResponse(); 

            // Had to hack this as the API peeps didn't conform to the YAML spec.
            if (json.IndexOf(nameof(FileDownloadDto.FileSizeInBytes), StringComparison.OrdinalIgnoreCase) > -1)
            {
                retVal.Success = false;
                retVal.ErrorLogFile = Newtonsoft.Json.JsonConvert.DeserializeObject<FileDownloadDto>(json);
            }
            else retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<GovernorBulkUpdateValidationResult>(json);
            return retVal;
        }

        public async Task<GovernorBulkUpdateValidationResult> BulkUpdateProcessRequestAsync(Guid id, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<GovernorBulkUpdateValidationResult>($"governor/bulk-update/{id}", null, principal)).GetResponse();
        }
    }
}
