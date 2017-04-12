using Edubase.Services.Domain;
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


        public AddressDto Address { get; set; } = new AddressDto();
        public string HeadTitle { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public int? Urn { get; set; }
    }
}
