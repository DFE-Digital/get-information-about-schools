using Edubase.Services.Enums;

namespace Edubase.Web.UI.Models
{
    using System.Collections.Generic;

    using Common;
    using Edubase.Web.UI.Views.TagHelpers.Models;
    using Helpers.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Search;
    using Services.Core;
    using Services.Texuna.ChangeHistory.Models;

    public class ChangeHistoryViewModel : PaginatedResult<ChangeHistorySearchItem>
    {

        public ChangeHistoryViewModel()
        {
            Skip = 0;
            Take = 100;
        }

        private Dictionary<int, eTextSearchType> _textSearchTypeMap = new Dictionary<int, eTextSearchType>
        {
            { 5, eTextSearchType.URN },
            { 6, eTextSearchType.URN },
            { 7, eTextSearchType.LAESTAB },
            { 8, eTextSearchType.UKPRN }
        };

        public const string BIND_ALIAS_ESTABTYPEIDS = "e";
        public const string BIND_ALIAS_FIELDS = "f";
        public const string BIND_ALIAS_GROUPTYPEIDS = "g";
        public const string BIND_ALIAS_SUGGESTED_BY = "s";
        public const string BIND_ALIAS_APPROVED_BY = "a";
        public const string BIND_ALIAS_DOWNLOADFORMAT = "df";

        public const string DATE_FILTER_MODE_EFFECTIVE = "e";
        public const string DATE_FILTER_MODE_APPLIED = "a";
        public const string DATE_FILTER_MODE_APPROVED = "r";

        public enum Tab
        {
            Establishments,
            Groups
        }

        [BindAlias(BIND_ALIAS_FIELDS)]
        public List<string> SelectedEstablishmentFields { get; set; } = new List<string>();

        [BindAlias(BIND_ALIAS_ESTABTYPEIDS)]
        public List<int> SelectedEstablishmentTypeIds { get; set; } = new List<int>();

        [BindAlias(BIND_ALIAS_GROUPTYPEIDS)]
        public List<int> SelectedGroupTypeIds { get; set; } = new List<int>();

        public Tab? SelectedTab { get; set; } = Tab.Establishments;

        public eSearchType? SearchType { get; set; }

        public SearchPayloadViewModel TextSearchModel { get; set; } = new SearchPayloadViewModel();
        public SearchPayloadViewModel GroupSearchModel { get; set; } = new SearchPayloadViewModel();

        public bool NoResultsForName { get; set; }
        public bool GroupSearchError { get; set; }

        public bool SingleEstablishment { get; set; }
        public bool SingleGroup { get; set; }

        public bool IsEstablishmentSearch => SearchType == eSearchType.EstablishmentAll || SearchType == eSearchType.Text;

        public string EstablishmentName { get; set; }
        public string GroupName { get; set; }

        public DateTimeViewModel DateFilterFrom { get; set; }

        public DateTimeViewModel DateFilterTo { get; set; }

        public string DateFilterMode { get; set; } = DATE_FILTER_MODE_APPLIED;

        [BindAlias(BIND_ALIAS_SUGGESTED_BY)]
        public string SuggestedBy { get; set; }

        [BindAlias(BIND_ALIAS_APPROVED_BY)]
        public string ApprovedBy { get; set; }

        [BindAlias(BIND_ALIAS_DOWNLOADFORMAT)]
        public eFileFormat? DownloadFormat { get; set; }

        public string Sortby { get; set; }
        public bool DisplayErrors { get; set; }

        public IEnumerable<SelectListItem> DateFilterOptions => new List<SelectListItem>
        {
            new SelectListItem {Text = "Effective", Value = DATE_FILTER_MODE_EFFECTIVE},
            new SelectListItem {Text = "Applied", Value = DATE_FILTER_MODE_APPLIED},
            new SelectListItem {Text = "Approved", Value = DATE_FILTER_MODE_APPROVED}
        };

        public IEnumerable<LookupItemViewModel> EstablishmentTypes { get; set; } = new List<LookupItemViewModel>();
        public IEnumerable<StringLookupItemViewModel> EstablishmentFields { get; set; } = new List<StringLookupItemViewModel>();
        public IEnumerable<LookupItemViewModel> GroupTypes { get; set; } = new List<LookupItemViewModel>();

        public IEnumerable<SelectListItem> SuggesterGroups { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ApproverGroups { get; set; } = new List<SelectListItem>();

        public eTextSearchType TextSearchType
        {
            get
            {
                var text = TextSearchModel.Text.RemoveSubstring("/").Clean();
                return text.IsInteger() ? _textSearchTypeMap.Get(text.Length, eTextSearchType.Unknown) : eTextSearchType.EstablishmentName;
            }
        }

        public enum eTextSearchType
        {
            Unknown,
            URN,
            LAESTAB,
            EstablishmentName,
            UKPRN
        }

        public SelectorViewModel SelectorItems { get; set; }
    }
}
