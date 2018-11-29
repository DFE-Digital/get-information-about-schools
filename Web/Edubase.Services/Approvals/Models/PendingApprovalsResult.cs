namespace Edubase.Services.Approvals.Models
{
    public class PendingApprovalsResult
    {
        public int Count { get; set; }
        public PendingApprovalItem[] Items { get; set; }
    }
}
