using Edubase.Data.Entity;
using System.Collections.Generic;
using Edubase.Common;
using System;

namespace Edubase.Web.UI.Models
{
    public class MATASSearchResultsModel
    {
        public string SearchTerm { get; set; }
        public List<Company> Results { get; set; }
        public string Error { get; set; }
        public string SearchType { get; set; }
        public int Count { get; set; }
        public int PageCount { get; private set; }
        public int PageSize { get; set; }
        public int StartIndex { get; set; }

        public MATASSearchResultsModel(string searchTerm)
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