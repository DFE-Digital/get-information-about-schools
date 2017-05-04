using Edubase.Services.Domain;
using Edubase.Services.IntegrationEndPoints.Google.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Search
{
    public class LocationDisambiguationViewModel
    {
        public List<AutocompleteItemDto> MatchingLocations { get; set; } = new List<AutocompleteItemDto>();
        public string SearchText { get; set; }

        public LocationDisambiguationViewModel()
        {

        }
        
    }
}