using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Edubase.AcceptanceTests.Users;
using Edubase.LocalDevelopment;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.WebSso;
using Xunit;

namespace Edubase.AcceptanceTests.SigninAuthorities;

public sealed class SigninSimulatorOnLocalMachineTests
{
    private static readonly Uri Website = new("https://localhost:44309");
    private static readonly Uri Acs = new(Website, "/Saml2/Acs");
    private static readonly User TestUser = new() { NameId = "local-user", AttributeStatementValue = "123" };

    [Fact]
    public void ExistingWebsiteConfigurationCanLoadFileMetadata()
    {
        var directory = Path.Combine(Path.GetTempPath(), "gias-metadata-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        using var certificate = Certificate();
        try
        {
            var baseUri = new Uri(directory).AbsoluteUri.TrimEnd('/');
            var issuer = baseUri + "/Metadata";
            LocalSamlMetadata.Write(directory, issuer, certificate);
            var sp = new SPOptions { EntityId = new EntityId("http://edubase.gov") };
            var options = new Options(sp);
            options.IdentityProviders.Add(new IdentityProvider(new EntityId(issuer), sp)
            {
                SingleSignOnServiceUrl = new Uri(baseUri),
                Binding = Saml2BindingType.HttpRedirect,
                AllowUnsolicitedAuthnResponse = true
            });
            _ = new Federation(baseUri + "/Federation", true, options);
            var loaded = options.IdentityProviders[new EntityId(issuer)];
            Assert.Equal("http://localhost/local-signin", loaded.SingleSignOnServiceUrl.AbsoluteUri);
            Assert.NotEmpty(loaded.SigningKeys);
        }
        finally
        {
            File.Delete(Path.Combine(directory, "Metadata"));
            File.Delete(Path.Combine(directory, "Federation"));
            Directory.Delete(directory);
        }
    }

    [Fact]
    public void WebsiteCertificateContainsNoPrivateKey()
    {
        using var client = new HttpClient { BaseAddress = Website };
        _ = new SigninSimulatorOnLocalMachine(client);
        using var publicCertificate = LocalSignInCertificate.GetPublicCertificate(LocalSignInCertificate.DirectoryPath);
        Assert.False(publicCertificate.HasPrivateKey);
        Assert.True(publicCertificate.NotAfter.ToUniversalTime() > DateTime.UtcNow);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WebsiteAndTestsShareCertificateRegardlessOfStartOrder(bool websiteFirst)
    {
        var directory = Path.Combine(Path.GetTempPath(), "gias-local-signin-" + Guid.NewGuid().ToString("N"));
        try
        {
            using var first = websiteFirst
                ? LocalSignInCertificate.GetPublicCertificate(directory)
                : LocalSignInCertificate.GetOrCreate(directory);
            using var second = websiteFirst
                ? LocalSignInCertificate.GetOrCreate(directory)
                : LocalSignInCertificate.GetPublicCertificate(directory);
            Assert.Equal(first.Thumbprint, second.Thumbprint);
            Assert.NotEqual(first.HasPrivateKey, second.HasPrivateKey);
        }
        finally
        {
            File.Delete(Path.Combine(directory, "signing.pfx"));
            Directory.Delete(directory);
        }
    }

    [Theory]
    [InlineData(HttpStatusCode.Redirect)]
    [InlineData(HttpStatusCode.SeeOther)]
    public async Task CompletesSamlExchangeThroughNormalWebsiteEndpoints(HttpStatusCode redirectStatus)
    {
        using var certificate = Certificate();
        var request = Request();
        var calls = new List<string>();
        using var handler = new Handler(async message =>
        {
            calls.Add(message.RequestUri!.AbsolutePath);
            Assert.Equal(Website.Authority, message.RequestUri.Authority);
            if (calls.Count == 1)
                return Redirect("http://localhost/local-signin?SAMLRequest=" + Uri.EscapeDataString(Encode(request)) + "&RelayState=correlation", redirectStatus);
            if (calls.Count == 2)
            {
                Assert.Equal(HttpMethod.Post, message.Method);
                var form = HttpUtility.ParseQueryString(await message.Content!.ReadAsStringAsync());
                Assert.Equal("correlation", form["RelayState"]);
                var claims = Validate(form["SAMLResponse"]!, request, certificate);
                Assert.Contains(claims, claim => claim.Type == "http://www.edubase.gov.uk/SAUserId" && claim.Value == "123");
                return Redirect("/Account/ExternalLoginCallback", redirectStatus);
            }
            var response = Redirect("/", redirectStatus);
            response.Headers.Add("Set-Cookie", ".AspNet.ApplicationCookie=application-session; path=/; secure; HttpOnly");
            return response;
        });
        using var client = new HttpClient(handler) { BaseAddress = Website };
        await new SigninSimulatorOnLocalMachine(client, certificate).SignIn(TestUser);
        Assert.Equal(new[] { "/Account/Login", "/Saml2/Acs", "/Account/ExternalLoginCallback" }, calls);
    }

    [Fact]
    public void RejectsTamperedResponseAndUntrustedCertificate()
    {
        using var certificate = Certificate();
        using var otherCertificate = Certificate();
        using var client = new HttpClient { BaseAddress = Website };
        var simulator = new SigninSimulatorOnLocalMachine(client, certificate);
        var request = Request();
        var encoded = simulator.CreateSamlResponse(TestUser, Encode(request), Acs);
        Assert.ThrowsAny<Exception>(() => Validate(encoded, request, otherCertificate));
        var tampered = Encoding.UTF8.GetString(Convert.FromBase64String(encoded)).Replace(
            ">123<", ">456<", StringComparison.Ordinal);
        Assert.ThrowsAny<Exception>(() => Validate(Convert.ToBase64String(Encoding.UTF8.GetBytes(tampered)), request, certificate));
    }

    [Fact]
    public void RejectsRequestForDifferentWebsite()
    {
        using var certificate = Certificate();
        using var client = new HttpClient { BaseAddress = Website };
        var request = Request();
        request.AssertionConsumerServiceUrl = new Uri("https://example.com/Saml2/Acs");
        Assert.Throws<InvalidOperationException>(() => new SigninSimulatorOnLocalMachine(client, certificate)
            .CreateSamlResponse(TestUser, Encode(request), Acs));
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://localhost:44309")]
    public void RequiresLocalHttpsWebsite(string address)
    {
        using var certificate = Certificate();
        using var client = new HttpClient { BaseAddress = new Uri(address) };
        Assert.Throws<ArgumentException>(() => new SigninSimulatorOnLocalMachine(client, certificate));
    }

    private static System.Security.Claims.Claim[] Validate(string encoded, Saml2AuthenticationRequest request, X509Certificate2 certificate)
    {
        var sp = new SPOptions { EntityId = new EntityId("http://edubase.gov") };
        var options = new Options(sp);
        var idp = new IdentityProvider(new EntityId(SigninSimulatorOnLocalMachine.Issuer), sp)
        {
            SingleSignOnServiceUrl = new Uri("http://localhost/local-signin"),
            Binding = Saml2BindingType.HttpRedirect,
            AllowUnsolicitedAuthnResponse = false
        };
        idp.SigningKeys.AddConfiguredKey(certificate);
        options.IdentityProviders.Add(idp);
        var response = Saml2Response.Read(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)), request.Id, options);
        return response.GetClaims(options).SelectMany(identity => identity.Claims).ToArray();
    }

    private static Saml2AuthenticationRequest Request() => new()
    {
        Issuer = new EntityId("http://edubase.gov"),
        DestinationUrl = new Uri("http://localhost/local-signin"),
        AssertionConsumerServiceUrl = Acs
    };

    private static string Encode(Saml2AuthenticationRequest request)
    {
        using var buffer = new MemoryStream();
        using (var deflate = new DeflateStream(buffer, CompressionLevel.Optimal, true))
            deflate.Write(Encoding.UTF8.GetBytes(request.ToXml()));
        return Convert.ToBase64String(buffer.ToArray());
    }

    private static X509Certificate2 Certificate()
    {
        using var key = RSA.Create(2048);
        return new CertificateRequest("CN=local-test", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            .CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
    }

    private static HttpResponseMessage Redirect(string location, HttpStatusCode status = HttpStatusCode.Redirect)
    {
        var response = new HttpResponseMessage(status);
        response.Headers.Location = new Uri(location, UriKind.RelativeOrAbsolute);
        return response;
    }

    private sealed class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => respond(request);
    }
}
