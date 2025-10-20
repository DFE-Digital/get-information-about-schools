using Edubase.Common;
using Edubase.Common.Spatial;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class AdditionalAddressViewModel
    {
        public int? Id { get; set; }
        public string SiteName { get; set; }
        public int? CountryId { get; set; }
        public string UPRN { get; set; }
        public string Street { get; set; }
        public string Locality { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public int? CountyId { get; set; }
        public string PostCode { get; set; }
        public LatLon Location { get; set; }

        public string CountyName { get; set; }
        public string CountryName { get; set; }

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ", Street, Locality, Address3, Town, CountyName, PostCode);
    }
}
