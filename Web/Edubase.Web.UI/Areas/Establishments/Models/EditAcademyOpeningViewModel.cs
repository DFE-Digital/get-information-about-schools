using System;
using System.ComponentModel.DataAnnotations;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class EditAcademyOpeningViewModel
    {
        public int Urn { get; set; }

        public string EstablishmentName { get; set; }

        public string EstablishmentType { get; set; }

        public string PredecessorUrn { get; set; }

        public string PredecessorName { get; set; }

        [Display(Name = "Opening date")]
        public DateTimeViewModel OpeningDate { get; set; } = new DateTimeViewModel();

        public DateTime OpenDate { get; set; }

        public bool IsSearching { get; set; } = false;

        public string EstablishmentTypeId { get; set; }
        public string ReturnTo { get; set; }
    }
}
