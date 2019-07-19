using System;
using Newtonsoft.Json;

namespace Edubase.Services.Establishments.Models
{
    public class NewAcademyRequest
    {
        public int PredecessorEstablishmentUrn { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime OpeningDate { get; set; }

        public int TypeId { get; set; }
    }
}
