using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class CompanyProfileDto
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public DateTime? IncorporationDate { get; set; }
        public string Address { get; set; }
    }
}
