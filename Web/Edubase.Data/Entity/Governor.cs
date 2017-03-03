using Edubase.Common;
using Edubase.Data.Entity.ComplexTypes;
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
        public int? EstablishmentUrn { get; set; }

        [ForeignKey("EstablishmentUrn")]
        public Establishment Establishment { get; set; }
        public Person Person { get; set; } = new Person();
        public Person PreviousPerson { get; set; } = new Person();
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public int? RoleId { get; set; }
        public LookupGovernorRole Role { get; set; }
        public int? AppointingBodyId { get; set; }
        public LookupGovernorAppointingBody AppointingBody { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? DOB { get; set; }
        public string Nationality { get; set; }
        public string PostCode { get; set; }
        public string TelephoneNumber { get; set; }

        public int? GroupUID { get; set; }
        [ForeignKey("GroupUID")]
        public GroupCollection Group { get; set; }

        public override int? GetId() => Id;
    }
}
