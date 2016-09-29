using Edubase.Data.Entity;
using Edubase.Data.Stubs;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models
{
    public class MATSearchResultsViewModel
    {
        public IList<MAT> Results { get; set; }
        public string SearchTerm { get; set; }
        public string Error { get; set; }

        public MATSearchResultsViewModel(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public MATSearchResultsViewModel()
        {

        }
    }
}