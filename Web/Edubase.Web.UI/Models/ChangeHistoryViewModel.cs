using System.Collections.Generic;
using Edubase.Common;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Texuna.ChangeHistory.Models;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Models.Search;

namespace Edubase.Web.UI.Models
{
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

        public bool NoResultsForName { get; set; }

        public int PageSize { get; set; } = 100;

        public int StartIndex { get; set; }

        public IPagedResult<ChangeHistorySearchItem> Changes { get; internal set; }

        public IPagedResult<EstablishmentChangeDto> EstablishmentChanges { get; internal set; }

        public long Count => (Changes?.Count).GetValueOrDefault();

        public bool IsEstablishmentSearch => SearchType == eSearchType.EstablishmentAll || SearchType == eSearchType.Text;

        public string EstablishmentName { get; set; }

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