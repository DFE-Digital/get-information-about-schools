using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch.Models
{
    public class ApiSearchResult<T> where T : class
    {
        public List<T> Items { get; set; }

        public long Count { get; set; }

        public ApiSearchResult(DocumentSearchResult<T> result)
        {
            Count = result.Count.GetValueOrDefault();
            Items = result.Results.Select(x => x.Document).ToList();
        }

        public ApiSearchResult(long count, List<T> items)
        {
            Count = count;
            Items = items;
        }

        public ApiSearchResult()
        {

        }

    }
}
