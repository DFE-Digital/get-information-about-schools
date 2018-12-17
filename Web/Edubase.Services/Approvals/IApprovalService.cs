using Edubase.Services.Approvals.Models;
using Edubase.Services.Domain;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Approvals
{
    public interface IApprovalService
    {
        Task<PendingApprovalsResult> GetAsync(int skip, int take, string sortBy, IPrincipal principal);
        Task<int> CountAsync(IPrincipal principal);
        Task<ApiResponse> ActionAsync(PendingChangeRequestAction payload, IPrincipal principal);
    }
}
