using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity
{
    public class EstablishmentChangeHistory : EdubaseEntity
    {
        public int Id { get; set; }
        public int Urn { get; set; }
        [ForeignKey("Urn")]
        public Establishment Establishment { get; set; }
        public string Name { get; set; }
        public string NewValue { get; set; }
        public ApplicationUser OriginatorUser { get; set; }
        public string OriginatorUserId { get; set; }
        public ApplicationUser ApproverUser { get; set; }
        public string ApproverUserId { get; set; }
        public string OldValue { get; set; }

        /// <summary>
        /// Date the change actually occurred
        /// </summary>
        public DateTime? EffectiveDateUtc { get; set; }

        /// <summary>
        /// Date the change was first requested/done
        /// </summary>
        public DateTime? RequestedDateUtc { get; set; }
    }
}