using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity
{
    public class EstablishmentApprovalQueue : EdubaseEntity
    {
        public int Id { get; set; }
        public int Urn { get; set; }
        [ForeignKey("Urn")]
        public Establishment Establishment { get; set; }
        public string Name { get; set; }
        public string NewValue { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string RejectionReason { get; set; }
        public ApplicationUser OriginatorUser { get; set; }
        public string OriginatorUserId { get; set; }
        public ApplicationUser ProcessorUser { get; set; }
        public string ProcessorUserId { get; set; }
        public string OldValue { get; set; }
        public DateTime? ProcessedDateUtc { get; set; }
    }
}