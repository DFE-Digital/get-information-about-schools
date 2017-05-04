using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Google.Models
{
    /// <summary>
    /// Represents a Google API exception. This is an unexpected exception, that should propagate to the generic handler and error reporting.
    /// </summary>
    [Serializable]
    public class GoogleApiException : Exception
    {
        public GoogleApiException() { }
        public GoogleApiException(string message) : base(message) { }
        public GoogleApiException(string message, Exception inner) : base(message, inner) { }
        protected GoogleApiException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
