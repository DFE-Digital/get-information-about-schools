using Edubase.Data.Entity.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity
{
    public class Company
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupUID { get; set; }
        public string GroupId { get; set; }
        public string Name { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public GroupType GroupType { get; set; }
        public int? GroupTypeId { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string GroupStatus { get; set; }
        public string GroupStatusCode { get; set; }
        public DateTime? OpenDate { get; set; }
        public string Address { get; set; }

        /// <summary>
        /// Temporary!
        /// </summary>
        [Obsolete, NotMapped]
        public int EstablishmentCount { get; set; }
    }
}
