using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class BulkCreateAcademiesProgress
    {
        public Guid Id { get; set; }
        public bool IsComplete { get; set; }
        public NewAcademyResult[] Result { get; set; }
    }
}
