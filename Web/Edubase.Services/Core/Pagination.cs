namespace Edubase.Services.Core
{
    using System;
    using System.Collections.Generic;
    using Domain;

    public class Pagination : IPagination
    {
        /// <summary>
        /// Total number of records available
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The page size
        /// </summary>
        public int Take { get; private set; }

        /// <summary>
        /// How many records to skip
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// How many pages based on the Take value / page size
        /// </summary>
        public int PageCount => (int)Math.Ceiling((double)Count / Take);

        public int FirstResultIndex => Skip + 1;

        public int LastResultIndex => Skip + Take > Count ? Count : Skip + Take;

        /// <summary>
        /// Current page index based on the current Skip value (one-based)
        /// </summary>
        public int CurrentPageIndex => (int)Math.Floor((double)Skip / Take) + 1;

        public Pagination()
        {

        }

        public Pagination(int skip, int take, int totalCount)
        {
            Skip = skip;
            Take = take;
            Count = totalCount;
        }
    }

    public class PaginatedResult<T> : Pagination, IPagedResult<T> where T : class
    {
        public IList<T> Items { get; set; } = new List<T>();

        public PaginatedResult()
        {

        }

        public PaginatedResult(int skip, int take, int totalCount) : base(skip, take, totalCount)
        {

        }

        public PaginatedResult(int skip, int take, int totalCount, IList<T> items) : base(skip, take, totalCount)
        {
            Items = items;
        }

        public PaginatedResult(int skip, int take, IPagedResult<T> inner) : this(skip, take, inner.Count, inner.Items)
        {
            
        }
    }

    public interface IPagedResult<T> where T : class
    {
        IList<T> Items { get; set; }
        int Count { get; set; }
    }
}
