using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Google.Models
{
    internal class Geometry
    {
        public Location location { get; set; }
        public Viewport viewport { get; set; }
    }
}
