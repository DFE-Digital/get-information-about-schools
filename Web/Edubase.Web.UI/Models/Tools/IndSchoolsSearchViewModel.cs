using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Helpers.ModelBinding;

namespace Edubase.Web.UI.Models.Tools
{
    public class IndSchoolsSearchViewModel
    {
        public const string ActionSearch = "search";

        [BindAlias("a")]
        public string ActionName { get; set; }

        public string Mode { get; set; } // welfare => 'w' || general => 'g'

        public bool IsWelfareMode => Mode == "w"; // filter by welfare action date
        public bool IsGeneralMode => Mode == "g"; // filter by general action date

        public DateTimeViewModel MinDate { get; set; } = new DateTimeViewModel();
        public DateTimeViewModel MaxDate { get; set; } = new DateTimeViewModel();

        public int Skip { get; set; }
        public int Take { get; set; }

        [BindAlias("l")]
        public List<int> SelectedLocalAuthorityIds { get; set; } = new List<int>();
        public ApiPagedResult<EstablishmentSearchResultModel> Results { get; internal set; }

        public bool HasResults => Results != null && Results.Count > 0 && Results.Items.Count > 0;
        
        public IEnumerable<LookupDto> LocalAuthorities { get; internal set; }
        public IEnumerable<EstablishmentLookupDto> EstablishmentTypes { get; internal set; }
        public IEnumerable<LookupDto> EstablishmentStatuses { get; internal set; }
    }
}