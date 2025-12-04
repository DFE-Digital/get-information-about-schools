namespace Edubase.Web.IntegrationTests.WireMock.Host;
public sealed class RemoteWireMockHostServerOptions
{
    public RemoteWireMockHostServerOptions(Uri serverAddress)
    {
        Guard.ThrowIfNull(serverAddress, nameof(serverAddress));

        Endpoint = serverAddress;
    }

    public string AdminEndpoint { get; set; } = "/__admin";
    public string MappingEndpoint { get; set; } = "/__admin/mappings";
    public Uri Endpoint { get; set; }
}
