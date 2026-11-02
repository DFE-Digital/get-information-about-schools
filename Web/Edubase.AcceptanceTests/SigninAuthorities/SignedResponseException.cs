namespace Edubase.AcceptanceTests.SigninAuthorities
{
    [Serializable]
    internal class SignedResponseException : Exception
    {
        public SignedResponseException()
        {
        }

        public SignedResponseException(string? message) : base(message)
        {
        }

        public SignedResponseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}