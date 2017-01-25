using Edubase.Data.Entity;
using System.Collections.Generic;
using Edubase.Common;
using System;
using Edubase.Services.Groups.Models;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Models
{
    public class GroupSearchResultsModel
    {
        public string SearchTerm { get; set; }
        public IList<SearchGroupDocument> Results { get; set; } = new List<SearchGroupDocument>();
        public string Error { get; set; }
        public string SearchType { get; set; }
        public long Count { get; set; }
        public int PageCount { get; private set; }
        public int PageSize { get; set; }
        public int StartIndex { get; set; }
        public List<LookupItemViewModel> GroupTypes { get; set; }

        public GroupSearchResultsModel(string searchTerm)
        {
            SearchTerm = searchTerm.Clean();
        }

        public void CalculatePageStats(int pageSize)
        {
            PageSize = pageSize;
            PageCount = (int)Math.Ceiling(Count / (double)pageSize);
        }

        
    }
}