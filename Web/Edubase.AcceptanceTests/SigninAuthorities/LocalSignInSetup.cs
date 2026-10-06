using Edubase.LocalDevelopment;

namespace Edubase.AcceptanceTests.SigninAuthorities;

// Invoked by this project's local Debug build, never by the web app or pipeline.
internal static class LocalSignInSetup
{
    public static int Main()
    {
        if (string.Equals(Environment.GetEnvironmentVariable("TF_BUILD"), "true", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase))
            return 0;

        var directory = LocalSignInCertificate.DirectoryPath;
        using var certificate = LocalSignInCertificate.GetOrCreate(directory);
        LocalSamlMetadata.Write(directory, LocalSamlMetadata.Issuer, certificate);

        Console.WriteLine("Local SAML metadata is ready. Website configuration has not been changed.");
        Console.WriteLine($"Local SASimulatorUri: {LocalSamlMetadata.BaseUri}");
        return 0;
    }
}
