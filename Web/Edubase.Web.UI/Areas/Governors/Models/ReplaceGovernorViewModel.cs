using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class ReplaceGovernorViewModel
    {
        public int? GID { get; set; }
        public string Name { get; set; }
        public DateTimeViewModel AppointmentEndDate { get; set; } = new DateTimeViewModel();
    }
}
