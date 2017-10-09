using System;
namespace Edubase.Services.Domain
{
    using System.Collections.Generic;
    using Core;


    /// <summary>
    /// Represents a page of a larger resultset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiPagedResult<T> : IPagedResult<T> where T : class
    {
        public IList<T> Items { get; set; }

        public int Count { get; set; }
        
        public ApiPagedResult(int count, IList<T> items)
        {
            Count = count;
            Items = items;
        }

        public ApiPagedResult()
        {

        }

    }
}
