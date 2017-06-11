using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class NewAcademyResult
    {
        public int Urn { get; set; }
        public DateTime? OpeningDate { get; set; }
        public int? TypeId { get; set; }
    }
}
