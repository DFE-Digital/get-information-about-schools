using Edubase.Services.Governors.Models;
using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services.Governors
{
    public interface IGovernorsWriteService
    {
        Task<ApiResponse<int>> SaveAsync(GovernorModel model, IPrincipal principal);
        Task DeleteAsync(int id, IPrincipal principal);
        Task UpdateDatesAsync(int governorId, DateTime appointmentStartDate, DateTime appointmentEndDate, IPrincipal principal);
        Task<ValidationEnvelopeDto> ValidateAsync(GovernorModel model, IPrincipal principal);

        /// <summary>
        /// Supplies the file and validates it's correct
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        Task<GovernorBulkUpdateValidationResult> BulkUpdateValidateAsync(string fileName, IPrincipal principal);
        
        /// <summary>
        /// Processes a previously uploaded file
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        Task<ApiResponse> BulkUpdateProcessRequestAsync(string id, IPrincipal principal);

        Task<ApiResponse> AddSharedGovernorAppointmentAsync(int governorId, int establishmentUrn, DateTime appointmentStart, DateTime? appointmentEnd, IPrincipal principal);
        Task<ApiResponse> UpdateSharedGovernorAppointmentAsync(int governorId, int establishmentUrn, DateTime appointmentStart, DateTime? appointmentEnd, IPrincipal principal);
        Task DeleteSharedGovernorAppointmentAsync(int governorId, int establishmentUrn, IPrincipal principal);
    }
}
