using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models.Tools
{
    public class BulkAcademiesViewModel
    {
        public int? Urn { get; set; }
        public string Name { get; set; }
        public int? EstablishmentTypeId { get; set; }
        public DateTimeViewModel OpeningDate { get; set; } = new DateTimeViewModel();
        public string Address { get; set; }
    }

    public class BulkAcademiesCreateViewModel
    {
        public List<BulkAcademiesViewModel> EstablishmentsToAdd { get; set; }
        public BulkAcademiesViewModel EstablishmentSearch { get; set; }
        public int? Urn { get; set; }
        public IEnumerable<SelectListItem> EstablishmentTypes { get; set; }
    }
}
