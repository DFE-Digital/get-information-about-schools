using Edubase.Common;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Edubase.Services.Groups.Search.GroupSearchPayload;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class GroupSearchViewModel
    {
        public SearchPayloadViewModel GroupSearchModel { get; set; } = new SearchPayloadViewModel();

        public IList<SearchGroupDocument> Results { get; set; } = new List<SearchGroupDocument>();

        public List<LookupItemViewModel> GroupTypes { get; set; }

        public long Count { get; set; }

        public int PageCount => (int)Math.Ceiling(Count / (double)PageSize);

        public int PageSize { get; set; } = 50;

        public int StartIndex { get; set; }

        public string Error { get; set; }

        public bool HasError => !Error.IsNullOrEmpty();

        public char? SortBy { get; set; }

        public eSortBy SortOption => SortBy == 'z' ? eSortBy.NameAlphabeticalZA : eSortBy.NameAlphabeticalAZ;

        [BindAlias("t")]
        public List<int> SelectedGroupTypeIds { get; set; } = new List<int>();

        [BindAlias("g")]
        public bool GoToDetailPageOnOneResult { get; set; }

    }
}