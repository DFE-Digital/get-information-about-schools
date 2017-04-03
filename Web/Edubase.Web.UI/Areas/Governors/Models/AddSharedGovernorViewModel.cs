using System.ComponentModel;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Establishments;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class AddSharedGovernorViewModel : IEstablishmentPageViewModel
    {
        public GovernorModel Governor { get; set; }

        public string GovernorType { get; set; }

        [DisplayName("Date of appointment")]
        public DateTimeViewModel AppointmentStartDate { get; set; } = new DateTimeViewModel();

        [DisplayName("Date term ends")]
        public DateTimeViewModel AppointmentEndDate { get; set; } = new DateTimeViewModel();

        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }
    }
}