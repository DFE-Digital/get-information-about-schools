using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Google.Models
{
    internal class AutocompleteApiQueryResponse
    {
        public List<Prediction> predictions { get; set; }
        public string status { get; set; }
    }
}
