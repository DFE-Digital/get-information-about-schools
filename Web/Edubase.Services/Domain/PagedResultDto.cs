using System;
using System.Collections.Generic;

namespace Edubase.Services.Domain
{
    public class PagedResultDto
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
        public int PageCount => (int) Math.Ceiling((double)Count / Take);

        /// <summary>
        /// Current page index based on the current Skip value
        /// </summary>
        public int CurrentPageIndex => (int) Math.Floor((double)Skip / Take) + 1;

        public PagedResultDto(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

    }


    public class PagedDto<T>
    {
        public IList<T> Items { get; set; } = new List<T>();

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

        /// <summary>
        /// Current page index based on the current Skip value
        /// </summary>
        public int CurrentPageIndex => (int)Math.Floor((double)Skip / Take) + 1;

        public PagedDto(int skip, int take, IList<T> items, int totalCount)
        {
            Skip = skip;
            Take = take;
            Items = items;
            Count = totalCount;
        }

        private PagedDto()
        {

        }

        private PagedDto(T result)
        {
            Skip = 0;
            Take = 1;
            Count = 1;
            Items.Add(result);
        }

        public static PagedDto<T> Empty => new PagedDto<T>();
        public static PagedDto<T> One(T result) => new PagedDto<T>(result);




    }
}
