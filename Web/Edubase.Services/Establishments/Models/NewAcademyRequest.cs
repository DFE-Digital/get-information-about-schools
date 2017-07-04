using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class NewAcademyRequest
    {
        public int PredecessorEstablishmentUrn { get; set; }
        public DateTime OpeningDate { get; set; }
        public int TypeId { get; set; }
    }
}
