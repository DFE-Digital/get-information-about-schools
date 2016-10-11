using Edubase.Data.Entity.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity
{
    public class Governor : EdubaseEntity
    {
        public int Id { get; set; }
        public int EstablishmentUrn { get; set; }
        [ForeignKey("EstablishmentUrn")]
        public Establishment Establishment { get; set; }
        public string Title { get; set; }
        public string Forename1 { get; set; }
        public string Forename2 { get; set; }
        public string Surname { get; set; }
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public int? RoleId { get; set; }
        public GovernorRole Role { get; set; }
        public int? GovernorAppointingBodyId { get; set; }
        public GovernorAppointingBody GovernorAppointingBody { get; set; }
    }
}
