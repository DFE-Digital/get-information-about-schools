namespace Edubase.AcceptanceTests.SigninAuthorities
{
    [Serializable]
    internal class AcceptanceTestSetupException : Exception
    {
        public AcceptanceTestSetupException()
        {
        }

        public AcceptanceTestSetupException(string? message) : base(message)
        {
        }

        public AcceptanceTestSetupException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}