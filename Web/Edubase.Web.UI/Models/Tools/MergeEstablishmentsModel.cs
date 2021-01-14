using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Tools
{
    public class MergeEstablishmentsModel
    {
        public int? LeadEstablishmentUrn { get; set; }

        public int? Establishment1Urn { get; set; }

        public int? Establishment2Urn { get; set; }

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
