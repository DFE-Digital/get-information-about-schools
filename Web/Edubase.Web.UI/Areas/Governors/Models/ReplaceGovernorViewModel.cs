using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class ReplaceGovernorViewModel
    {
        public int? GID { get; set; }
        public string Name { get; set; }
        public DateTimeViewModel AppointmentEndDate { get; set; } = new DateTimeViewModel();
    }
}