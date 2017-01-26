using Edubase.Common.Spatial;
using Edubase.Services.Establishments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSearchPayload
    {
        public enum eSortBy
        {
            /// <summary>
            /// The distance from the centre-point of the location supplied (nearest first)
            /// </summary>
            Distance,

            /// <summary>
            /// Alphabetical on the Establishment name (ascending)
            /// </summary>
            NameAlphabeticalAZ,

            /// <summary>
            /// Alphabetical on the Establishment name (descending)
            /// </summary>
            NameAlphabeticalZA
        }

        public EstablishmentSearchPayload()
        {

        }

        public EstablishmentSearchPayload(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public string Text { get; set; }

        public EstablishmentSearchFilters Filters { get; set; } = new EstablishmentSearchFilters();

        public int Skip { get; set; }

        public int Take { get; set; } = 10;

        public int? RadiusInMiles { get; set; }

        public LatLon GeoSearchLocation { get; set; }

        public eSortBy SortBy { get; set; }

        public int[] SENIds { get; set; } = new int[0];
    }
}
