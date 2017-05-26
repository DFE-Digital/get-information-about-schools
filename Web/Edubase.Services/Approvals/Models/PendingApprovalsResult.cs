using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Approvals.Models
{
    public class PendingApprovalsResult
    {
        public int Count { get; set; }
        public PendingApprovalItem[] Items { get; set; }
    }
}
