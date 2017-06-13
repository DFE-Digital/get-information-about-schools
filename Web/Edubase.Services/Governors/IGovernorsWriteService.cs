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
    }
}
