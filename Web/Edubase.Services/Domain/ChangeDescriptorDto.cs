using Edubase.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class ChangeDescriptorDto
    {
        [JsonProperty("fieldName")]
        public string Name { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
