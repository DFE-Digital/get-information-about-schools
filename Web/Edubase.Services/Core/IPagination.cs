using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Core
{
    public interface IPagination
    {
        /// <summary>
        /// Total number of records available
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The page size
        /// </summary>
        int Take { get; }

        /// <summary>
        /// How many records to skip
        /// </summary>
        int Skip { get; }

        /// <summary>
        /// How many pages based on the Take value / page size
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Current page index based on the current Skip value (one-based)
        /// </summary>
        int CurrentPageIndex { get; }

        int FirstResultIndex { get; }

        int LastResultIndex { get; }
    }
}
