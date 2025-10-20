using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Models.Tools
{
    public class AmalgamateEstablishmentsModel
    {
        [Display(Name = "Establishment 1 URN")]
        public int? Establishment0Urn { get; set; }

        [Display(Name = "Establishment 2 URN")]
        public int? Establishment1Urn { get; set; }

        [Display(Name = "Establishment 3 URN")]
        public int? Establishment2Urn { get; set; }

        [Display(Name = "Establishment 4 URN")]
        public int? Establishment3Urn { get; set; }

        public string Establishment0Name { get; set; }

        public string Establishment1Name { get; set; }

        public string Establishment2Name { get; set; }

        public string Establishment3Name { get; set; }

        [Display(Name = "Merge date")]
        public DateTimeViewModel MergeDate { get; set; } = new DateTimeViewModel();

        public int? NewEstablishmentUrn { get; set; }
        public string NewEstablishmentName { get; set; }
        public string EstablishmentType { get; set; }

        public int? EstablishmentPhase { get; set; }

        public string LocalAuthorityId { get; set; }

        public string LocalAuthorityName { get; set; }

        public IEnumerable<SelectListItem> EstablishmentTypes { get; set; }

        public IEnumerable<SelectListItem> EstablishmentPhases { get; set; }

        public IEnumerable<SelectListItem> LocalAuthorities { get; set; }
    }
}
