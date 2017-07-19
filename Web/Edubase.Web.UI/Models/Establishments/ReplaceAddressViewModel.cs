using Edubase.Common;
using Edubase.Services.Domain;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models.Establishments
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ReplaceAddressViewModel : IEstablishmentPageViewModel
    {
        [JsonProperty]
        public string Street { get; set; }
        [JsonProperty]
        public string Locality { get; set; }
        [JsonProperty]
        public string Address3 { get; set; }
        [JsonProperty]
        public string Town { get; set; }
        [JsonProperty]
        public int? CountyId { get; set; }
        [JsonProperty]
        public string PostCode { get; set; }
        [JsonProperty]
        public int? CountryId { get; set; }
        public IEnumerable<SelectListItem> Counties { get; internal set; }
        public IEnumerable<SelectListItem> Countries { get; internal set; }
        [JsonProperty]
        public string SelectedUPRN { get; set; }
        public string ActionName { get; set; }

        public string AddressLookupResultJsonToken
        {
            get { return UriHelper.SerializeToUrlToken(LookupAddresses); }
            set { LookupAddresses = UriHelper.TryDeserializeUrlToken<IEnumerable<AddressLookupResult>>(value); }
        }

        public string CountriesJsonToken
        {
            get { return UriHelper.SerializeToUrlToken(Countries); }
            set { Countries = UriHelper.TryDeserializeUrlToken<IEnumerable<SelectListItem>>(value); }
        }

        public string CountiesJsonToken
        {
            get { return UriHelper.SerializeToUrlToken(Counties); }
            set { Counties = UriHelper.TryDeserializeUrlToken<IEnumerable<SelectListItem>>(value); }
        }

        public IEnumerable<AddressLookupResult> LookupAddresses { get; set; }
        public IEnumerable<SelectListItem> GetLookupAddressSelectListItems(string selectedUPRN) => LookupAddresses?.Select(x => new SelectListItem { Text = x.ToString(), Value = x.UPRN, Selected = selectedUPRN == x.UPRN });
        string IEstablishmentPageViewModel.SelectedTab { get; set; }
        int? IEstablishmentPageViewModel.Urn { get; set; }
        string IEstablishmentPageViewModel.Name { get; set; }
        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }
        string IEstablishmentPageViewModel.Layout { get; set; }

        public string Step { get; set; } = "enterpostcode";

        public ReplaceAddressViewModel()
        {

        }

        public ReplaceAddressViewModel(IEnumerable<SelectListItem> countries)
        {
            Countries = countries;
        }
    }
}