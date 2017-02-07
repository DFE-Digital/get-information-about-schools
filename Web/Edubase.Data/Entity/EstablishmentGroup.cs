using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity
{
    public class EstablishmentGroup : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Establishment Establishment { get; set; }

        [ForeignKey("Group")]
        public int GroupUID { get; set; }

        [ForeignKey("Establishment")]
        public int EstablishmentUrn { get; set; }

        public virtual GroupCollection Group { get; set; }

        public DateTime? JoinedDate { get; set; }

        public bool CCIsLeadCentre { get; set; }
    }
}
