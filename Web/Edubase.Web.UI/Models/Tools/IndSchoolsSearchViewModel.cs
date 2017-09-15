using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Data.Entity;
using System.Web.Mvc;
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
        
        [BindAlias(BindAliasForActionName)]
        public string ActionName { get; set; }

        public string Mode { get; set; } // welfare => 'w' || general => 'g'

        public bool IsWelfareMode => Mode == "w"; // filter by welfare action date
        public bool IsGeneralMode => Mode == "g"; // filter by general action date

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

        public IEnumerable<SelectListItem> LocalAuthoritySetSelectList => LocalAuthoritySets.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = SelectedLocalAuthoritySetId == x.Id });

        public IndSchoolsSearchViewModel SetResults(PaginatedResult<EstablishmentSearchResultModel> results)
        {
            Results = results;
            return this;
        }
    }
}