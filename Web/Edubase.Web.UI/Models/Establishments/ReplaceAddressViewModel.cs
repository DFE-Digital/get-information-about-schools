using System.Collections.Generic;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models.Establishments
{
    public class ReplaceAddressViewModel : IEstablishmentPageViewModel
    {
        public string Street { get; set; }
        public string Locality { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public int? CountyId { get; set; }
        public string PostCode { get; set; }
        public string UPRN { get; set; }
        public int? CountryId { get; set; }
        public IEnumerable<SelectListItem> Counties { get; internal set; }
        public IEnumerable<SelectListItem> Countries { get; internal set; }
        public string AddressLookupResultJsonToken { get; set; }

        string IEstablishmentPageViewModel.SelectedTab { get; set; }
        int? IEstablishmentPageViewModel.Urn { get; set; }
        string IEstablishmentPageViewModel.Name { get; set; }
        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }
        string IEstablishmentPageViewModel.Layout { get; set; }
    }
}