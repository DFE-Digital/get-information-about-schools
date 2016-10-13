using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class HomepageViewModel
    {
        public bool AllowSchoolCreation { get; set; }
        public bool AllowTrustCreation { get; set; }
        public bool AllowApprovals { get; set; }
        public int PendingApprovalsCount { get; set; }
    }
}