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
    [Serializable]
    public class TexunaApiSystemException : Exception
    {
        public TexunaApiSystemException() { }
        public TexunaApiSystemException(string message) : base(message) { }
        public TexunaApiSystemException(string message, Exception inner) : base(message, inner) { }
        protected TexunaApiSystemException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
