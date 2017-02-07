using Edubase.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Exceptions
{

    [Serializable]
    public class InvalidParameterException : EdubaseException
    {
        public InvalidParameterException() { }
        public InvalidParameterException(string message) : base(message) { }
        public InvalidParameterException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParameterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}