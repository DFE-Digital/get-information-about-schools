using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class ApiSearchResult<T> where T : class
    {
        public List<T> Items { get; set; }

        public long Count { get; set; }
        
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
