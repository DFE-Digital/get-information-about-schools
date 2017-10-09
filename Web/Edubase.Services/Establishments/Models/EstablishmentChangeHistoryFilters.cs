using System;

namespace Edubase.Services.Establishments.Models
{
    public class EstablishmentChangeHistoryFilters
    {
        public DateTime? DateAppliedFrom { get; set; }
        public DateTime? DateAppliedTo { get; set; }

        public DateTime? DateApprovedFrom { get; set; }
        public DateTime? DateApprovedTo { get; set; }

        public DateTime? DateEffectiveFrom { get; set; }
        public DateTime? DateEffectiveTo { get; set; }

        public string ApprovedBy { get; set; }
        public string SuggestedBy { get; set; }

        public string[] FieldsUpdated { get; set; }

    }
}
