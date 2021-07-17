using Edubase.Services.Approvals.Models;
using Edubase.Services.Core;

namespace Edubase.Web.UI.Models.Tools
{
    public class ApprovalsViewModel : PaginatedResult<PendingApprovalItem>
    {
        public ApprovalsViewModel()
        {
            Skip = 0;
            Take = 100;
        }
        public PaginatedResult<PendingApprovalItem> ApprovalItems { get; set; }

        public string SortBy { get; set; } = "effectiveDateUtc-asc";

        public bool SortedAscending { get; set; } = true;
    }
}
