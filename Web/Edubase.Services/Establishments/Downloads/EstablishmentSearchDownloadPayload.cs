using Edubase.Services.Domain;
using Edubase.Services.Establishments.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edubase.Services.Establishments.Downloads
{
    public class EstablishmentSearchDownloadPayload : SearchDownloadDto<EstablishmentSearchPayload>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public eDataSet DataSet { get; set; }

        public bool IncludeEmailAddresses { get; set; }
        public bool IncludeIEBTFields { get; set; }
        public bool IncludeBringUpFields { get; set; }
        public bool IncludeChildrensCentreFields { get; set; }
        public bool IncludeLinks { get; set; }
        public string[] SelectedFields { get; set; }
    }
}
