using Edubase.Web.IntegrationTests.WireMock;
using Edubase.Web.IntegrationTests.WireMock.Options;

namespace Edubase.Web.IntegrationTests;

public sealed class EdubaseApiServerFixture : WireMockServerFixture
{
    public EdubaseApiServerFixture() : base(
        new WireMockServerOptions()
        {
            ServerMode = WireMockServerMode.LocalProcess,
            Domain = "localhost",
            Port = 8443,
            EnableSecureConnection = true,
            CertificatePassword = "yourpassword",
            CertificatePath = "wiremock-cert.pfx",
        })
    {
    }
}
