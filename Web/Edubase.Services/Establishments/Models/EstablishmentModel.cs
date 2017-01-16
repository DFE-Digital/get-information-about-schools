using Edubase.Common.Spatial;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;

namespace Edubase.Services.Establishments.Models
{
    public class EstablishmentModel : EstablishmentModelBase
    {
        /// <summary>
        /// List of addresses visible to the principal
        /// </summary>
        public List<AdditionalAddressModel> AdditionalAddresses { get; set; }
        
        public ChildrensCentreLocalAuthorityDto CCLAContactDetail { get; set; }

        public LatLon Location { get; set; }

        public override LatLon Coordinate => Location;

        /// <summary>
        /// The number of additional addresses specified, including
        /// any hidden ones.
        /// </summary>
        public int AdditionalAddressesCount { get; set; }
    }
}
