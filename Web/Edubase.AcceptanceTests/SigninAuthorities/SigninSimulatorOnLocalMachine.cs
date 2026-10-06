using Edubase.LocalDevelopment;
using System.IO.Compression;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.Users;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public sealed class SigninSimulatorOnLocalMachine : ISignInAuthority
    {
        internal static string Issuer => LocalSamlMetadata.Issuer;
        private const string Audience = "http://edubase.gov";
        private static readonly Uri Authority = new("http://localhost/local-signin");
        private static readonly Lazy<X509Certificate2> LocalCertificate = new(() => LocalSignInCertificate.GetOrCreate(LocalSignInCertificate.DirectoryPath));
        private readonly HttpClient httpClient;
        private readonly X509Certificate2 certificate;

        public SigninSimulatorOnLocalMachine(HttpClient httpClient)
            : this(httpClient, null) { }

        internal SigninSimulatorOnLocalMachine(HttpClient httpClient, X509Certificate2? certificate)
        {
            ArgumentNullException.ThrowIfNull(httpClient);
            if (httpClient.BaseAddress is not { IsLoopback: true, Scheme: "https" })
                throw new ArgumentException("Local sign-in requires an HTTPS loopback website.", nameof(httpClient));
            this.httpClient = httpClient;
            this.certificate = certificate ?? LocalCertificate.Value;
        }

        public async Task SignIn(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrWhiteSpace(user.NameId);
            ArgumentException.ThrowIfNullOrWhiteSpace(user.AttributeStatementValue);

            // GIAS creates its normal SAML request and correlation cookie.
            using var login = await httpClient.GetAsync(WebRoutes.SignIn);
            var authorityLocation = GetRedirect(login);
            if (!authorityLocation.IsAbsoluteUri || authorityLocation.GetLeftPart(UriPartial.Path) != Authority.AbsoluteUri)
                throw new InvalidOperationException("Local SAML requires a local acceptance-test build to prepare metadata and a restarted Debug website using devsecrets.gias.config.alwaysignore.");
            var query = HttpUtility.ParseQueryString(authorityLocation.Query);
            var samlRequest = query["SAMLRequest"] ?? throw new InvalidOperationException("GIAS returned no SAML request.");
            var relayState = query["RelayState"] ?? throw new InvalidOperationException("GIAS returned no relay state.");
            var acs = new Uri(httpClient.BaseAddress!, "/Saml2/Acs");

            // No request goes to the authority URL: create the signed response here.
            var response = CreateSamlResponse(user, samlRequest, acs);
            using var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["SAMLResponse"] = response,
                ["RelayState"] = relayState
            });
            using var accepted = await httpClient.PostAsync(acs, form);
            var callback = new Uri(httpClient.BaseAddress!, GetRedirect(accepted));
            if (callback.GetLeftPart(UriPartial.Authority) != httpClient.BaseAddress!.GetLeftPart(UriPartial.Authority) ||
                callback.AbsolutePath != "/Account/ExternalLoginCallback")
                throw new InvalidOperationException("GIAS returned an unexpected SAML callback.");

            // The regular callback loads backend roles and issues the application cookie.
            using var completed = await httpClient.GetAsync(callback);
            GetRedirect(completed);
            if (!completed.Headers.TryGetValues("Set-Cookie", out var cookies) ||
                !cookies.Any(cookie => cookie.StartsWith(".AspNet.ApplicationCookie=", StringComparison.Ordinal)))
                throw new InvalidOperationException("GIAS did not issue an application cookie.");
            // DI gives this client a scenario-scoped CookieContainer, shared with the front end.
        }

        internal string CreateSamlResponse(User user, string encodedRequest, Uri acs)
        {
            using var compressed = new MemoryStream(Convert.FromBase64String(encodedRequest));
            using var inflated = new DeflateStream(compressed, CompressionMode.Decompress);
            using var reader = XmlReader.Create(inflated, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 65536
            });
            var request = XElement.Load(reader);
            XNamespace protocol = "urn:oasis:names:tc:SAML:2.0:protocol";
            XNamespace assertion = "urn:oasis:names:tc:SAML:2.0:assertion";
            var requestId = (string?)request.Attribute("ID");
            if (request.Name != protocol + "AuthnRequest" || string.IsNullOrWhiteSpace(requestId) ||
                (string?)request.Attribute("Destination") != Authority.AbsoluteUri ||
                (string?)request.Attribute("AssertionConsumerServiceURL") != acs.AbsoluteUri ||
                (string?)request.Element(assertion + "Issuer") != Audience)
                throw new InvalidOperationException("The SAML request does not match the local GIAS website.");

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.NameId),
                new Claim("http://www.edubase.gov.uk/SAUserId", user.AttributeStatementValue),
                new Claim("urn:oid:2.5.4.45", user.AttributeStatementValue)
            });
            var response = new Saml2Response(new EntityId(Issuer), certificate, acs,
                new Saml2Id(requestId), null, new Uri(Audience), new[] { identity });
            var document = new XmlDocument { PreserveWhitespace = true };
            document.LoadXml(response.ToXml());
            // The library normalizes a trailing slash onto Uri audiences; GIAS's ID has none.
            document.GetElementsByTagName("Audience", assertion.NamespaceName)[0]!.InnerText = Audience;
            document.Sign(certificate, true, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(document.OuterXml));
        }

        private static Uri GetRedirect(HttpResponseMessage response)
        {
            if ((response.StatusCode != HttpStatusCode.Redirect && response.StatusCode != HttpStatusCode.SeeOther) ||
                response.Headers.Location is null)
                throw new InvalidOperationException($"Expected a sign-in redirect, received {(int)response.StatusCode} from {response.RequestMessage?.RequestUri}.");
            return response.Headers.Location;
        }
    }
}
