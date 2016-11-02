using Edubase.Data.Entity;
using Edubase.Web.UI.Areas.Governors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;

namespace Edubase.Web.UI.Models
{
    public class AdvancedSearchViewModel
    {
        private Dictionary<int, eTextSearchType> _textSearchTypeMap = new Dictionary<int, eTextSearchType>
        {
            { 6, eTextSearchType.URN },
            { 7, eTextSearchType.LAESTAB },
            { 8, eTextSearchType.UKPRN }
        };

        public class Payload
        {
            public string Text { get; set; }
            public string AutoSuggestValue { get; set; }
            public int? AutoSuggestValueAsInt => AutoSuggestValue.ToInteger();
        }
        

        public enum eSearchType
        {
            Text,
            Location,
            LocalAuthority,
            Trust,
            Governor
        }

        public enum eTextSearchType
        {
            Unknown,
            URN,
            LAESTAB,
            EstablishmentName,
            UKPRN
        }

        public List<Establishment> Results { get; set; }
        public string Error { get; set; }

        public eTextSearchType TextSearchType => TextSearchModel.Text.IsInteger() 
            ? _textSearchTypeMap.Get(TextSearchModel.Text.Length, eTextSearchType.Unknown) 
            : eTextSearchType.EstablishmentName;

        public int Count { get; set; }
        public int PageCount => (int)Math.Ceiling(Count / (double)PageSize);
        public int PageSize { get; set; } = 50;
        public int StartIndex { get; set; }
        public bool HasError => !Error.IsNullOrEmpty();

        public AdvancedSearchViewModel()
        {
        }
        

        public SearchModel GovernorSearchModel { get; set; } = new SearchModel();

        public Payload TextSearchModel { get; set; } = new Payload();
        public Payload LocationSearchModel { get; set; } = new Payload();
        public Payload LocalAuthoritySearchModel { get; set; } = new Payload();
        public Payload TrustSearchModel { get; set; } = new Payload();

        public eSearchType SearchType { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
    }
}