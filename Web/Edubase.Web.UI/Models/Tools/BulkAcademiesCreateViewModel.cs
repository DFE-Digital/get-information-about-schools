using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Tools
{
    public class BulkAcademyViewModel
    {
        public int? Urn { get; set; }
        public string Name { get; set; }
        public int? EstablishmentTypeId { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTimeViewModel OpeningDateView {
            get => new DateTimeViewModel(OpeningDate);
            set => OpeningDate = value.ToDateTime();
        }
        public string Address { get; set; }
        public int? ReplacedUrn { get; set; }
    }

    public class BulkAcademiesViewModel
    {
        public List<BulkAcademyViewModel> ItemsToAdd { get; set; }
        public BulkAcademyViewModel FoundItem { get; set; }
        public int? SearchUrn { get; set; }
        public IEnumerable<SelectListItem> ItemTypes { get; set; }
        public IEnumerable<SelectListItem> FilteredItemTypes { get; set; }
        public Guid ProgressGuid { get; set; }
        public bool IsComplete { get; set; }
    }
}
