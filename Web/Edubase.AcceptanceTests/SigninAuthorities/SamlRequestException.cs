namespace Edubase.AcceptanceTests.SigninAuthorities
{
    [Serializable]
    internal class SamlRequestException : Exception
    {
        public SamlRequestException()
        {
        }

        public SamlRequestException(string? message) : base(message)
        {
        }

        public SamlRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}