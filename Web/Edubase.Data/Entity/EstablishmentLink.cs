using Edubase.Data.Entity.Lookups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Data.Entity
{
    public class EstablishmentLink : EdubaseEntity
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

        public LookupEstablishmentLinkType LinkType { get; set; }

        public int? LinkTypeId { get; set; }

        public DateTime? LinkEstablishedDate { get; set; }
    }
}
