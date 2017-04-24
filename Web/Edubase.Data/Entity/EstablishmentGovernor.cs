using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Edm.Values;

namespace Edubase.Data.Entity
{
    public class EstablishmentGovernor : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EstabishmentUrn { get; set; }

        public int GovernorId { get; set; }

        public DateTime? AppointmentStartDate { get; set; }

        public DateTime? AppointmentEndDate { get; set; }



        [ForeignKey("EstabishmentUrn")]
        public Establishment Establishment { get; set; }

        [ForeignKey("GovernorId")]
        public Governor Governor { get; set; }

        public override int? GetId() => Id;
    }
}
