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
        public int TrustGroupUID { get; set; }

        [ForeignKey("Establishment")]
        public int EstablishmentUrn { get; set; }

        public GroupCollection Group { get; set; }

        public DateTime? JoinedDate { get; set; }
    }
}
