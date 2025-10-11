using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Groups.Models;
using Edubase.Services.Texuna.Lookup;
using Newtonsoft.Json;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AddOrReplaceAddressViewModel : IEstablishmentPageViewModel
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

        [JsonProperty]
        public int? Easting { get; set; }

        [JsonProperty]
        public int? Northing { get; set; }

        public string TownLabel => CountryId.GetValueOrDefault() == Constants.COUNTRY_ID_UK ? "Town" : "Town / City";

        public string PostCodeLabel => CountryId.GetValueOrDefault() == Constants.COUNTRY_ID_UK ? "Postcode" : "Postcode / Zipcode";

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

        /// <summary>
        /// Target address to replace; i.e., main address or alternative address
        /// </summary>
        [JsonProperty]
        public string Target { get; set; }

        public IEnumerable<AddressLookupResult> LookupAddresses { get; set; }
        public IEnumerable<SelectListItem> GetLookupAddressSelectListItems(string selectedUPRN) => LookupAddresses?.Select(x => new SelectListItem { Text = x.ToString(), Value = x.UPRN, Selected = selectedUPRN == x.UPRN });
        string IEstablishmentPageViewModel.SelectedTab { get; set; }
        int? IEstablishmentPageViewModel.Urn { get; set; }
        string IEstablishmentPageViewModel.Name { get; set; }
        string IEstablishmentPageViewModel.TypeName { get; set; }
        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }
        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }
        string IEstablishmentPageViewModel.Layout { get; set; }

        public string Step { get; set; } = "enterpostcode";
        

        public AddOrReplaceAddressViewModel()
        {

        }

        public AddOrReplaceAddressViewModel(IEnumerable<SelectListItem> countries, IEnumerable<SelectListItem> counties, string target)
        {
            Countries = countries;
            Counties = counties;
            Target = target;
        }
    }
}