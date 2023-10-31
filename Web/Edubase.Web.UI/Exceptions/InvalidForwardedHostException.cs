using Edubase.Services.Exceptions;
using System;

namespace Edubase.Web.UI.Exceptions
{
    [Serializable]
    public class InvalidForwardedHostException : Exception
    {
        public InvalidForwardedHostException() { }
        public InvalidForwardedHostException(string message) : base(message) { }
        public InvalidForwardedHostException(string message, Exception inner) : base(message, inner) { }

        protected InvalidForwardedHostException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context
        ) : base(info, context)
        {
        }
    }
}
