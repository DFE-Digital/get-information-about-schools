using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Groups.Models
{
    public class EstablishmentGroupModel
    {
        public int? Id { get; set; }
        public int EstablishmentUrn { get; set; }
        public DateTime? JoinedDate { get; set; }
        public bool CCIsLeadCentre { get; set; }
    }
}
