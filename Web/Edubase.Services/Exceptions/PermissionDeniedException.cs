using System;

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
    }
}
