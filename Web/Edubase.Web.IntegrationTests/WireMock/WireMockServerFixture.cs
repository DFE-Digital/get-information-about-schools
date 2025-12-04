using Edubase.Web.IntegrationTests.WireMock.Host;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;
using Edubase.Web.IntegrationTests.WireMock.Options;

namespace Edubase.Web.IntegrationTests.WireMock;

public abstract class WireMockServerFixture : IAsyncLifetime
{
    private readonly WireMockServerOptions _options;
#nullable disable
    private IWireMockHost _wireMockHost;
    private IWireMockMappingService _mappingService;
#nullable enable
    public WireMockServerFixture(WireMockServerOptions options)
    {
        Guard.ThrowIfNull(options, nameof(options));
        _options = options;
    }

    public Uri ServerUri => _options.ServerAddress;

    public async Task InitializeAsync()
    {

        _wireMockHost = _options.ServerMode switch
        {
            WireMockServerMode.LocalProcess => new LocalProcessWireMockServerHost(_options),
            WireMockServerMode.Remote
            => new RemoteWireMockHost(
                    new WireMockNetRemoteClient(
                        new HttpClient
                        {
                            BaseAddress = _options.ServerAddress
                        },
                    new RemoteWireMockHostServerOptions(_options.ServerAddress))

                ),
            _ => throw new NotImplementedException($"Server mode {_options.ServerMode} is not implemented."),
        };
        await _wireMockHost.StartAsync();

        _mappingService = new WireMockMappingService(_wireMockHost);
    }

    public virtual Task OnInitialiseAsync(IWireMockHost host) => Task.CompletedTask;

    public async Task<HttpMappedResponses> RegisterHttpMapping(HttpMappingRequest request)
    {
        HttpMappedResponses responses =
            await _mappingService.RegisterMappingsAsync(
                files: request.Files);

        return responses;
    }

    public async Task DisposeAsync()
    {
        using (_wireMockHost)
        {
            await OnDisposeAsync(_wireMockHost);
        }
    }

    public virtual Task OnDisposeAsync(IWireMockHost host) => Task.CompletedTask;
}
