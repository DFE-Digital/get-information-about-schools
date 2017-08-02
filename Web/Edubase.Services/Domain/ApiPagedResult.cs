using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    /// <summary>
    /// Represents a page of a larger resultset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiPagedResult<T> where T : class
    {
        public List<T> Items { get; set; }

        public int Count { get; set; }
        
        public ApiPagedResult(int count, List<T> items)
        {
            Count = count;
            Items = items;
        }

        public ApiPagedResult()
        {

        }

    }
}
