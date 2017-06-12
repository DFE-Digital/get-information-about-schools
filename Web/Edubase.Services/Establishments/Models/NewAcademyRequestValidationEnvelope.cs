using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class NewAcademyRequestValidationEnvelope : ValidationEnvelopeDto
    {
        public int? Urn { get; set; }
    }
}
