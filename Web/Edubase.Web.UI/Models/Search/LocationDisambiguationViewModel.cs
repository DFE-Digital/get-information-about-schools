using Edubase.Services.Geo;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models.Search
{
    public class LocationDisambiguationViewModel
    {
        public List<PlaceDto> MatchingLocations { get; set; } = new List<PlaceDto>();

        public string SearchText { get; set; }

        public LocationDisambiguationViewModel()
        {

        }
        
    }
}