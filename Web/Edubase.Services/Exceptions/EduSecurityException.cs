using System;
using System.Runtime.Serialization;

namespace Edubase.Services.Exceptions
{
    public class EduSecurityException : EdubaseException
    {
        public EduSecurityException()
        {
        }

        public EduSecurityException(string message) : base(message)
        {
        }

        public EduSecurityException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EduSecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
