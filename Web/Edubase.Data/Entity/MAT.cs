using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity
{
    [Table("MAT")]
    public class MAT
    {
        [Key]
        public short GroupUID { get; set; }

        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string GroupTypeCode { get; set; }
        public string GroupType { get; set; }
        public string ClosedDate { get; set; }
        public string GroupStatusCode { get; set; }
        public string GroupStatus { get; set; }


    }
}
