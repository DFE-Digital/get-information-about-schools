using Edubase.Services.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class CreateEstablishmentPermissionDto
    {
        public bool CanCreate { get; set; }
        [JsonProperty("typeIds")]
        public eLookupEstablishmentType[] Types { get; set; }
        public int[] LocalAuthorityIds { get; set; }

    }
}
