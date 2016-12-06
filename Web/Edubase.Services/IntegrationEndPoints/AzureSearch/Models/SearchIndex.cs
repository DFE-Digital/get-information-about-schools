using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch.Models
{
    public class SearchIndex
    {
        public string Name { get; set; }
        public string SuggesterName { get; set; }
        public List<SearchIndexField> Fields { get; set; } = new List<SearchIndexField>();
    }
}
