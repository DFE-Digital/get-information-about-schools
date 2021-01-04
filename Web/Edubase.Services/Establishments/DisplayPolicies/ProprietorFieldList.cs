using Newtonsoft.Json;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class ProprietorFieldList<T>
    {
        #region Fields

        [JsonProperty("name")]
        public T Name { get; set; }

        [JsonProperty("street")]
        public T Street { get; set; }

        [JsonProperty("locality")]
        public T Locality { get; set; }

        [JsonProperty("address3")]
        public T Address3 { get; set; }

        [JsonProperty("town")]
        public T Town { get; set; }

        [JsonProperty("countyId")]
        public T CountyId { get; set; }

        [JsonProperty("postcode")]
        public T Postcode { get; set; }

        [JsonProperty("telephoneNumber")]
        public T TelephoneNumber { get; set; }

        [JsonProperty("email")]
        public T Email { get; set; }

        #endregion
    }
}
