using System.Security.Cryptography.X509Certificates;

namespace Edubase.LocalDevelopment
{
    internal static class LocalSignInCertificate
    {
        private static string WebDirectory
        {
            get
            {
                var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (directory != null)
                {
                    if (File.Exists(Path.Join(directory.FullName, "EdubaseWeb.sln")))
                        return directory.FullName;
                    directory = directory.Parent;
                }
                throw new InvalidOperationException("Local sign-in requires a GIAS source checkout.");
            }
        }

        internal static string DirectoryPath =>
            Path.Join(WebDirectory, "Edubase.AcceptanceTests", ".local-signin");

        // Read the shared development certificate directly; never copy or generate a private key.
        internal static X509Certificate2 Load()
        {
            var path = Path.Join(Directory.GetParent(WebDirectory)!.FullName,
                "devsecrets.gias.signing.pfx.alwaysignore");
            return new X509Certificate2(path, string.Empty, X509KeyStorageFlags.EphemeralKeySet);
        }
    }
}
