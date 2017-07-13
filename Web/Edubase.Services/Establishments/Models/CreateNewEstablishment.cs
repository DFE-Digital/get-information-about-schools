using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class NewEstablishmentModel
    {
        public string Name { get; set; }
        [JsonIgnore]
        public bool? GenerateEstabNumber { get; set; }
        public string EstablishmentNumber { get; set; }
        public int? LocalAuthorityId { get; set; }
        public int? EducationPhaseId { get; set; }
        [JsonProperty("typeId")]
        public int? EstablishmentTypeId { get; set; }
    }
}
