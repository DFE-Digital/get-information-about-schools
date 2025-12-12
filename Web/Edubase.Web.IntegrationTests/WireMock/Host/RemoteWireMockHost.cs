using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService;

namespace Edubase.Web.IntegrationTests.WireMock.Host;

internal class RemoteWireMockHost : IWireMockHost
{
    private readonly WireMockNetRemoteClient _client;

    private bool _serverStarted = false;

    public RemoteWireMockHost(WireMockNetRemoteClient client)
    {
        Guard.ThrowIfNull(client, nameof(client));
        _client = client;
    }

    // TODO consider HealthCheck on IWireMockRemoteClient
    public Task StartAsync()
    {
        _serverStarted = true;
        return Task.CompletedTask;
    }

    public Task RegisterMappingAsync(MappingRequest mapping) => _client.PostMappingsAsync(mapping.Mapping);

    public async Task RegisterMappingsAsync(IEnumerable<MappingRequest> mappings)
    {
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
            // TODO ClearMappings and terminate server
        }
    }
}

