using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class AddressViewModel
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string CityOrTown { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Locality { get; set; }
        public string PostCode { get; set; }
        public string Easting { get; set; }
        public string Northing { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}