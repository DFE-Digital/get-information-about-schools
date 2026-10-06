using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Edubase.LocalDevelopment;

namespace Edubase.AcceptanceTests.SigninAuthorities;

internal static class LocalSamlMetadata
{
    internal static string BaseUri => new Uri(LocalSignInCertificate.DirectoryPath).AbsoluteUri.TrimEnd('/');
    internal static string Issuer => BaseUri + "/Metadata";

    internal static void Write(string directory, string issuer, X509Certificate2 certificate)
    {
        XNamespace md = "urn:oasis:names:tc:SAML:2.0:metadata";
        XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
        var entity = new XElement(md + "EntityDescriptor", new XAttribute("entityID", issuer),
            new XElement(md + "IDPSSODescriptor", new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol"),
                new XElement(md + "KeyDescriptor", new XAttribute("use", "signing"),
                    new XElement(ds + "KeyInfo", new XElement(ds + "X509Data",
                        new XElement(ds + "X509Certificate", Convert.ToBase64String(certificate.RawData))))),
                new XElement(md + "SingleSignOnService",
                    new XAttribute("Binding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"),
                    new XAttribute("Location", "http://localhost/local-signin"))));
        File.WriteAllText(Path.Combine(directory, "Metadata"), entity.ToString());
        File.WriteAllText(Path.Combine(directory, "Federation"), new XElement(md + "EntitiesDescriptor", entity).ToString());
    }
}
