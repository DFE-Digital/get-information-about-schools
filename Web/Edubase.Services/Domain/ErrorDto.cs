using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class ErrorDto
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Fields { get; set; }
    }
}
