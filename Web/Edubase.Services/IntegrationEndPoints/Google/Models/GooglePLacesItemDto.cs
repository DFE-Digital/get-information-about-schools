using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Google.Models
{
    public class GooglePlacesItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public GooglePlacesItemDto()
        {

        }

        public GooglePlacesItemDto(string id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
