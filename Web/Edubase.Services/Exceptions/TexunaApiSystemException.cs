using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Exceptions
{
    /// <summary>
    /// Represents an exception from Texuna where the API raised some service error that is ___unhandle-able___ on our side.
    /// </summary>
    public class TexunaApiSystemException : Exception
    {
        public string ApiRequestJsonPayload { get; set; }

        public TexunaApiSystemException() { }
        public TexunaApiSystemException(string message) : base(message) { }
        public TexunaApiSystemException(string message, Exception inner) : base(message, inner) { }
        
        public TexunaApiSystemException(string message, string apiRequestJsonPayload) : base(message)
        {
            ApiRequestJsonPayload = apiRequestJsonPayload;
        }
    }
}
