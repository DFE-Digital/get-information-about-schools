using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Web.IntegrationTests.WireMock.Options;
public sealed class WireMockServerOptions
{
    public WireMockServerMode ServerMode { get; set; } = WireMockServerMode.Remote;
    public string Domain { get; set; } = "localhost";
    public int Port { get; set; } = 8443;
    // TODO CertificateOptions
    public bool EnableSecureConnection { get; set; } = true;
    public string? CertificatePath { get; set; } = null;
    public string? CertificatePassword { get; set; } = null;
    // TODO should validator live here - not configurable
    public string Protocol => EnableSecureConnection ? "https" : "http";
    public Uri ServerAddress
    {
        get
        {
            if (!Uri.TryCreate($"{Protocol}://{Domain}:{Port}", UriKind.Absolute, out var serverAddress))
            {
                throw new ArgumentException("Unable to construct uri from options");
            }
            return serverAddress;
        }
    }
}
