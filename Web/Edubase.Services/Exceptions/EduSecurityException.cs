using System;

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
    }
}
