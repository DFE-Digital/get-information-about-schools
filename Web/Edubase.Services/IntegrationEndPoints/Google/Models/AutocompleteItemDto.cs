using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Google.Models
{
    public class AutocompleteItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public AutocompleteItemDto()
        {

        }

        public AutocompleteItemDto(string id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
