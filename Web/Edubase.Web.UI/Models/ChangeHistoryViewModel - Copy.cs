//using System;
//using System.Collections.Generic;
//using System.Web.Mvc;
//using Edubase.Services.Domain;
//using Edubase.Services.Texuna.ChangeHistory.Models;
//using Edubase.Web.UI.Helpers.ModelBinding;
//using Edubase.Services.Enums;

//namespace Edubase.Web.UI.Models
//{
//    public class ChangeHistoryViewModel
//    {
//        public const string BIND_ALIAS_ESTABTYPEIDS = "e";
//        public const string BIND_ALIAS_FIELDS = "f";
//        public const string BIND_ALIAS_GROUPTYPEIDS = "g";

//        public const string DATE_FILTER_MODE_EFFECTIVE = "e";
//        public const string DATE_FILTER_MODE_APPLIED = "a";
//        public const string DATE_FILTER_MODE_APPROVED = "r";

//        public List<EstablishmentField> EstablishmentFields { get; internal set; }

//        [BindAlias(BIND_ALIAS_FIELDS)]
//        public string[] SelectedEstablishmentFields { get; set; } = new string[0];

//        public IEnumerable<LookupItemViewModel> EstablishmentTypes { get; set; }

//        [BindAlias(BIND_ALIAS_ESTABTYPEIDS)]
//        public List<int> SelectedEstablishmentTypeIds { get; set; } = new List<int>();

//        public IEnumerable<LookupItemViewModel> GroupTypes { get; set; }

//        [BindAlias(BIND_ALIAS_GROUPTYPEIDS)]
//        public List<int> SelectedGroupTypeIds { get; set; } = new List<int>();
//        public string SearchType { get; set; }
//        public bool IsGroupSearch => SearchType == "group";
//        public bool IsEstablishmentSearch => SearchType == "establishment";
//        public ApiPagedResult<ChangeHistorySearchItem> Results { get; internal set; }
//        public long Count => (Results?.Count).GetValueOrDefault();
//        public int PageSize { get; set; } = 100;
//        public int StartIndex { get; set; }
//        public int PageCount => (int)Math.Ceiling(Count / (double)PageSize);
//        public bool ClearResults { get; set; }
//        public string SelectedApproverId { get; set; }
//        public string SelectedSuggesterId { get; set; }
//        public IEnumerable<SelectListItem> Suggesters { get; internal set; }
//        public IEnumerable<SelectListItem> Approvers { get; internal set; }
//        public string DateFilterMode { get; set; }
//        public DateTimeViewModel DateFilterFrom { get; set; }
//        public DateTimeViewModel DateFilterTo { get; set; }
//        public eFileFormat DownloadFormat { get; set; }
//        public bool StartDownload { get; set; }
//    }
//}