using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity
{
    public class Estab2Estab : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Establishment Establishment { get; set; }

        [ForeignKey("Establishment")]
        public int? Establishment_Urn { get; set; }

        public Establishment LinkedEstablishment { get; set; }

        [ForeignKey("LinkedEstablishment")]
        public int? LinkedEstablishment_Urn { get; set; }

        public string LinkName { get; set; }

        public string LinkType { get; set; }

        public DateTime? LinkEstablishedDate { get; set; }
    }
}
