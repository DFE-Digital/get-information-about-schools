using System;
using System.Runtime.Serialization;

namespace Edubase.Services.Exceptions
{
    public class PermissionDeniedException : EduSecurityException
    {
        public PermissionDeniedException()
        {
        }

        public PermissionDeniedException(string message) : base(message)
        {
        }

        public PermissionDeniedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PermissionDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
