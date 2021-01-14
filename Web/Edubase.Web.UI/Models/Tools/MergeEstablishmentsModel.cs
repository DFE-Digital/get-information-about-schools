using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Tools
{
    public class MergeEstablishmentsModel
    {
        [Display(Name = "Lead establishment URN")]
        public int? LeadEstablishmentUrn { get; set; }

        [Display(Name = "Establishment 1 URN")]
        public int? Establishment1Urn { get; set; }

        [Display(Name = "Establishment 2 URN")]
        public int? Establishment2Urn { get; set; }

        [Display(Name = "Establishment 3 URN")]
        public int? Establishment3Urn { get; set; }

        public string LeadEstablishmentName { get; set; }

        public string Establishment1Name { get; set; }

        public string Establishment2Name { get; set; }

        public string Establishment3Name { get; set; }

        [Display(Name = "Merge date")]
        public DateTimeViewModel MergeDate { get; set; } = new DateTimeViewModel();

        public string EstablishmentType { get; set; }
    }
}
