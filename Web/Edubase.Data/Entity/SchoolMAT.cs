using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Data.Entity
{
    [Table("SchoolMAT")]
    public class SchoolMAT
    {
        [Key]
        public int URN { get; set; }
        public short LinkedUID { get; set; }
        public DateTime JoinedDate { get; set; }

        [ForeignKey("LinkedUID")]
        public virtual MAT MAT { get; set; }
    }
}
