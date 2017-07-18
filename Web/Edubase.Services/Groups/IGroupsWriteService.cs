using Edubase.Services.Domain;
using Edubase.Services.Groups.Models;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Groups
{
    public interface IGroupsWriteService
    {
        /// <summary>
        /// Saves a group and its linked establishments
        /// </summary>
        /// <param name="model"></param>
        /// <param name="linkedEstablishments"></param>
        /// <param name="principal"></param>
        /// <returns>The UID of the group</returns>
        Task<ApiResponse> SaveAsync(SaveGroupDto dto, IPrincipal principal);
        Task<ApiResponse<NumericResultDto>> SaveNewAsync(SaveGroupDto dto, IPrincipal principal);
        Task<ValidationEnvelopeDto> ValidateAsync(SaveGroupDto dto, IPrincipal principal);
        Task<ApiResponse> ConfirmGovernanceAsync(int uid, IPrincipal principal);
    }
}