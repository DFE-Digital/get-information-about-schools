using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch.Models
{
    public class AzureSearchResult<T> where T : class
    {
        internal Action CountAccessor { get; set; }

        public IList<T> Items { get; internal set; }
        
        private long? _count;

        public long? Count
        {
            get
            {
                CountAccessor?.Invoke();
                return _count;
            }
            private set { _count = value; }
        }

        public AzureSearchResult(DocumentSearchResult<T> result)
        {
            Count = result.Count;
            Items = result.Results.Select(x => x.Document).ToList();
        }

        public AzureSearchResult(long? count, IList<T> items)
        {
            Count = count;
            Items = items;
        }


    }
}
