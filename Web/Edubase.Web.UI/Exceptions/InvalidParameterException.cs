using Edubase.Services.Exceptions;
using System;

namespace Edubase.Web.UI.Exceptions
{
    [Serializable]
    public class InvalidParameterException : EdubaseException
    {
        public InvalidParameterException() { }
        public InvalidParameterException(string message) : base(message) { }
        public InvalidParameterException(string message, Exception inner) : base(message, inner) { }
    }
}
