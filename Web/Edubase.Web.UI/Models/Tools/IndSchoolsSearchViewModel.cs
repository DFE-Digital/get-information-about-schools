using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Data.Entity;
using Edubase.Services.Core;

namespace Edubase.Web.UI.Models.Tools
{
    public class IndSchoolsSearchViewModel
    {
        public class LASetViewModel
        {
            public string Id { get; set; }
            public int[] LocalAuthorityIds { get; set; }
            public string Name { get; set; }

            public LASetViewModel()
            {

            }

            public LASetViewModel(LocalAuthoritySet laSet)
            {
                LocalAuthorityIds = laSet.Ids;
                Name = laSet.Title;
                Id = laSet.RowKey;
            }
        }

        public const string ActionSearch = "search";
        public const string ActionSaveSet = "saveset";
        public const string BindAliasForActionName = "a";
        public const string BindAliasForSelectedLocalAuthorityIds = "d";
        public const string SpecifierDateOfActionWelfare = "w";
        public const string SpecifierDateOfActionGeneral = "g";

        [BindAlias(BindAliasForActionName)]
        public string ActionName { get; set; }

        [Required(ErrorMessage = "Please select an action")]
        public string Mode { get; set; } = SpecifierDateOfActionGeneral;
        public bool IsWelfareMode => Mode == SpecifierDateOfActionWelfare;
        public bool IsGeneralMode => Mode == SpecifierDateOfActionGeneral;

        public DateTimeViewModel MinDate { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel MaxDate { get; set; } = new DateTimeViewModel();

        public int Skip { get; set; }
        public int Take { get; set; } = 50;

        [BindAlias(BindAliasForSelectedLocalAuthorityIds)]
        public List<int> SelectedLocalAuthorityIds { get; set; } = new List<int>();
        public PaginatedResult<EstablishmentSearchResultModel> Results { get; internal set; }

        public bool HasResults => Results != null && Results.Count > 0 && Results.Items.Count > 0;

        public IEnumerable<LookupDto> LocalAuthorities { get; internal set; }
        public IEnumerable<EstablishmentLookupDto> EstablishmentTypes { get; internal set; }
        public IEnumerable<LookupDto> EstablishmentStatuses { get; internal set; }
        public string SelectedLocalAuthoritySetId { get; set; }
        public IEnumerable<LASetViewModel> LocalAuthoritySets { get; set; }

        public IEnumerable<SelectListItem> LocalAuthoritySetSelectList 
            => LocalAuthoritySets.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = SelectedLocalAuthoritySetId == x.Id });
        
        public IndSchoolsSearchViewModel SetResults(PaginatedResult<EstablishmentSearchResultModel> results)
        {
            Results = results;
            return this;
        }
    }
}