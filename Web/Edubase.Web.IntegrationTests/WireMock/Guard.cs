namespace Edubase.Web.IntegrationTests.WireMock;
internal static class Guard
{
    internal static void ThrowIfNull<T>(T value, string paramName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    internal static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"{paramName} cannot be null or empty");
        }
    }

    internal static void ThrowIfNullOrEmpty<T>(IEnumerable<T> values, string paramName)
    {
        if (values is null || !values.Any())
        {
            throw new ArgumentException("Enumeration cannot be null or empty");
        }
    }
}
