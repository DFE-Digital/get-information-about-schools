namespace Edubase.AcceptanceTests.SigninAuthorities
{
    [Serializable]
    internal class CookieException : Exception
    {
        public CookieException()
        {
        }

        public CookieException(string? message) : base(message)
        {
        }

        public CookieException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}