using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService;

namespace Edubase.Web.IntegrationTests.WireMock.Host;
public interface IWireMockHost : IDisposable
{
    /*Uri? Endpoint { get; }*/
    Task StartAsync();
    Task RegisterMappingAsync(MappingRequest mapping);
    Task RegisterMappingsAsync(IEnumerable<MappingRequest> mapping);
}
