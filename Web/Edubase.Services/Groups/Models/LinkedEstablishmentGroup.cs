using System;

namespace Edubase.Services.Groups.Models
{
    public class LinkedEstablishmentGroup
    {
        public int? Id { get; set; }
        public int? Urn { get; set; }
        public DateTime? JoinedDate { get; set; }
        public bool CCIsLeadCentre { get; set; }
    }
}
