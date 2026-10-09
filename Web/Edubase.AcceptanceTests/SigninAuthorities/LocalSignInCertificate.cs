using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Edubase.LocalDevelopment
{
    // Certificate storage belongs entirely to the acceptance-test project.
    internal static class LocalSignInCertificate
    {
        internal static string DirectoryPath
        {
            get
            {
                var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (directory != null)
                {
                    if (File.Exists(Path.Combine(directory.FullName, "EdubaseWeb.sln")))
                        return Path.Combine(directory.FullName, "Edubase.AcceptanceTests", ".local-signin");
                    directory = directory.Parent;
                }
                throw new InvalidOperationException("Local sign-in requires a GIAS source checkout.");
            }
        }

        internal static X509Certificate2 GetOrCreate(string directory)
        {
            directory = Path.GetFullPath(directory);
            // Synchronize concurrent local builds and test processes.
            string lockName;
            using (var sha = SHA256.Create())
                lockName = "Local\\GIAS-SAML-" + BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(directory.ToUpperInvariant()))).Replace("-", "");
            using (var mutex = new Mutex(false, lockName))
            {
                try
                {
                    if (!mutex.WaitOne(TimeSpan.FromSeconds(30)))
                        throw new TimeoutException("Timed out initializing the local sign-in certificate.");
                }
                catch (AbandonedMutexException) { /* The previous process exited; this process owns the lock. */ }
                try
                {
                    Directory.CreateDirectory(directory);
                    var path = Path.Combine(directory, "signing.pfx");
                    if (!File.Exists(path))
                    {
                        using (var rsa = RSA.Create(2048))
                        {
                            var request = new CertificateRequest("CN=GIAS local sign-in", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                            using (var generated = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddYears(10)))
                                File.WriteAllBytes(path, generated.Export(X509ContentType.Pfx));
                        }
                    }
                    return new X509Certificate2(path, string.Empty, X509KeyStorageFlags.EphemeralKeySet);
                }
                finally { mutex.ReleaseMutex(); }
            }
        }

        internal static X509Certificate2 GetPublicCertificate(string directory)
        {
            using (var certificate = GetOrCreate(directory))
                return new X509Certificate2(certificate.Export(X509ContentType.Cert));
        }
    }
}
