namespace Edubase.Web.UI.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Common;
    using Helpers.ModelBinding;
    using Search;
    using Services.Core;
    using Services.Texuna.ChangeHistory.Models;

    public class ChangeHistoryViewModel
    {
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

        public const string DATE_FILTER_MODE_EFFECTIVE = "e";
        public const string DATE_FILTER_MODE_APPLIED = "a";
        public const string DATE_FILTER_MODE_APPROVED = "r";

        public enum Tab
        {
            Establishments,
            Groups
        }

        [BindAlias(BIND_ALIAS_FIELDS)]
        public string[] SelectedEstablishmentFields { get; set; } = new string[0];

        [BindAlias(BIND_ALIAS_ESTABTYPEIDS)]
        public List<int> SelectedEstablishmentTypeIds { get; set; } = new List<int>();

        public Tab SelectedTab { get; set; }

        public eSearchType? SearchType { get; set; }

        public SearchPayloadViewModel TextSearchModel { get; set; } = new SearchPayloadViewModel();
        public SearchPayloadViewModel GroupSearchModel { get; set; } = new SearchPayloadViewModel();

        public bool NoResultsForName { get; set; }

        public int PageSize { get; set; } = 100;

        public int StartIndex { get; set; }

        public PaginatedResult<ChangeHistorySearchItem> Changes { get; internal set; }

        public long Count => (Changes?.Count).GetValueOrDefault();
            
        public bool SingleEstablishment { get; set; }
        public bool SingleGroup { get; set; }

        public bool IsEstablishmentSearch => SearchType == eSearchType.EstablishmentAll || SearchType == eSearchType.Text;

        public string EstablishmentName { get; set; }
        public string GroupName { get; set; }

        public DateTimeViewModel DateFilterFrom { get; set; }

        public DateTimeViewModel DateFilterTo { get; set; }

        public string DateFilterMode { get; set; } = DATE_FILTER_MODE_APPLIED;

        public IEnumerable<SelectListItem> DateFilterOptions => new List<SelectListItem>
        {
            new SelectListItem {Text = "Effective", Value = DATE_FILTER_MODE_EFFECTIVE},
            new SelectListItem {Text = "Applied", Value = DATE_FILTER_MODE_APPLIED},
            new SelectListItem {Text = "Approved", Value = DATE_FILTER_MODE_APPROVED}
        };

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
    }
}