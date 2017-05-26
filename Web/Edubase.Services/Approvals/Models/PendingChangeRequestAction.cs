using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Approvals.Models
{
    public class PendingChangeRequestAction
    {
        /// <summary>
        /// Array of PendingApprovalItem.id values
        /// </summary>
        public int[] Ids { get; set; }
        
        [JsonIgnore]
        public ePendingChangeRequestAction Action { get; set; }

        [JsonProperty("action")]
        public string ActionSpecifier { get; set; }

        public string RejectionReason { get; set; }
    }
}
