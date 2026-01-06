using System.Security.Cryptography.X509Certificates;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService;
using Edubase.Web.IntegrationTests.WireMock.Options;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace Edubase.Web.IntegrationTests.WireMock.Host;
internal class LocalProcessWireMockServerHost : IWireMockHost
{
    private readonly Uri _serverUri;
    private readonly Lazy<WireMockServer> _server;
    public LocalProcessWireMockServerHost(WireMockServerOptions serverOptions)
    {
        _serverUri = serverOptions.ServerAddress;
        _server = CreateWireMockServer(serverOptions);
    }

    public Uri Endpoint => _serverUri;

    public Task StartAsync()
    {
        if (!_server.IsValueCreated)
        {
            _ = _server.Value;
        }
        return Task.CompletedTask;
    }

    public Task RegisterMappingAsync(MappingRequest mapping)
    {
        _server.Value.WithMapping(mapping.Mapping);
        return Task.CompletedTask;
    }

    public async Task RegisterMappingsAsync(IEnumerable<MappingRequest> mappings)
    {
        Guard.ThrowIfNull(mappings, nameof(mappings));

        foreach (var mapping in mappings)
        {
            await RegisterMappingAsync(mapping);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_server.IsValueCreated)
            {
                _server.Value.Dispose();
            }

        }
    }

    private static Lazy<WireMockServer> CreateWireMockServer(WireMockServerOptions serverOptions)
    {
        Lazy<WireMockServer> serverFactory = new(() =>
        {
            WireMockCertificateSettings? cert =
                string.IsNullOrEmpty(serverOptions.CertificatePath) ?
                    null :
                        new()
                        {
                            X509Certificate = new X509Certificate2(
                                fileName: serverOptions.CertificatePath,
                                password: serverOptions.CertificatePassword ?? string.Empty)
                        };

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Port = serverOptions.Port,
                CertificateSettings = cert,
                UseSSL = serverOptions.EnableSecureConnection,
                StartAdminInterface = true,
                Logger = new WireMockConsoleLogger(),
                WatchStaticMappingsInSubdirectories = true,
                WatchStaticMappings = true
            });

            return server;
        });

        return serverFactory;
    }
}
