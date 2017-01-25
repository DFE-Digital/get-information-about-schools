using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    [Serializable]
    public class AdditionalAddressModel : EstablishmentAddressModel
    {
        public Guid Id { get; set; }
        public string SiteName { get; set; }
        public bool IsRestricted { get; set; }
        public string TelephoneNumber { get; set; }

        public AdditionalAddressModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
