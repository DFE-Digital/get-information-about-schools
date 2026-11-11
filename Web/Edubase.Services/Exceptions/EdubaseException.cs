using System;

namespace Edubase.Services.Exceptions
{
    /// <summary>
    /// Represents an exception that's purposeful; i.e., as a result of a business rule.
    /// These types of exceptions should either be handled or the message
    /// should be shown to the user.
    /// </summary>

    public class EdubaseException : Exception
    {
        public EdubaseException()
        {
        }

        public EdubaseException(string message) : base(message)
        {
        }

        public EdubaseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
