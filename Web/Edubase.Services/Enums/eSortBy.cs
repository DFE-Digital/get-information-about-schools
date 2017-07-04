using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Enums
{
    public enum eSortBy
    {
        /// <summary>
        /// Alphabetical on the group name (ascending)
        /// </summary>
        NameAlphabeticalAZ,

        /// <summary>
        /// Alphabetical on the group name (descending)
        /// </summary>
        NameAlphabeticalZA,

        /// <summary>
        /// The distance from the centre-point of the location supplied (nearest first)
        /// </summary>
        Distance
    }
}
