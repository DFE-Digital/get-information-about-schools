﻿using Edubase.Data.Entity;
using Edubase.Web.UI.Areas.Governors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;
using Edubase.Data.Entity.Lookups;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Services.Establishments.Search;

namespace Edubase.Web.UI.Models
{
    public class AdvancedSearchViewModel
    {
        private Dictionary<int, eTextSearchType> _textSearchTypeMap = new Dictionary<int, eTextSearchType>
        {
            { 5, eTextSearchType.URN },
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

        public IList<SearchEstablishmentDocument> Results { get; set; }
        public string Error { get; set; }

        public eTextSearchType TextSearchType => TextSearchModel.Text.IsInteger()
            ? _textSearchTypeMap.Get(TextSearchModel.Text.Length, eTextSearchType.Unknown)
            : eTextSearchType.EstablishmentName;

        public long Count { get; set; }
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
        
        public Payload TrustSearchModel { get; set; } = new Payload();

        public eSearchType SearchType { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }


        public IEnumerable<LookupItemViewModel> EstablishmentTypes { get; set; }
        [BindAlias("t")]
        public List<int> SelectedEstablishmentTypeIds { get; set; } = new List<int>();

        public IEnumerable<LookupItemViewModel> EstablishmentStatuses { get; set; }
        [BindAlias("s")]
        public List<int> SelectedEstablishmentStatusIds { get; set; } = new List<int>();

        public IEnumerable<LookupItemViewModel> EducationPhases { get; set; }
        [BindAlias("p")]
        public List<int> SelectedEducationPhaseIds { get; set; } = new List<int>();

        public IEnumerable<LookupItemViewModel> LocalAuthorties { get; set; }
        [BindAlias("l")]
        public List<int> SelectedLocalAuthorityIds { get; set; } = new List<int>();

        public IEnumerable<LookupItemViewModel> ReligiousCharacters { get; set; }
        [BindAlias("r")]
        public List<int> SelectedReligiousCharacterIds { get; set; } = new List<int>();



    }
}