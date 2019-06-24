using System;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models.Tools
{
    public class BulkAcademiesViewModel
    {
        public int? Urn { get; set; }
        public string Name { get; set; }
        public string EstablishmentType { get; set; }
        public DateTime? OpeningDate { get; set; }
        public string Address { get; set; }
    }

    public class BulkAcademiesCreateViewModel
    {
        public List<BulkAcademiesViewModel> EstablishmentsToAdd { get; set; }
        public BulkAcademiesViewModel EstablishmentSearch { get; set; }
        public int? Urn { get; set; }
    }
}
