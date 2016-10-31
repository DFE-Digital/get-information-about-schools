using Edubase.Data.Entity;
using System.Collections.Generic;
using Edubase.Common;
using System;

namespace Edubase.Web.UI.Models
{
    public class SearchResultsModel
    {
        public enum eSearchType
        {
            Unknown,
            URN,
            LAESTAB,
            LA,
            SchoolName
        }

        public string SearchTerm { get; set; }
        public List<Establishment> Results { get; set; }
        public string Error { get; set; }
        public eSearchType SearchType { get; set; }
        public int Count { get; set; }
        public int PageCount { get; private set; }
        public int PageSize { get; set; }
        public int StartIndex { get; set; }
        

        public SearchResultsModel(string searchTerm)
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