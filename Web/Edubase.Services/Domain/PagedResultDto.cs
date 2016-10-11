using System;

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
}
