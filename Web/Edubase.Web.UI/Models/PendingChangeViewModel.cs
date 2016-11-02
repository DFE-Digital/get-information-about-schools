using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class PendingChangeViewModel
    {
        public string DataField { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}