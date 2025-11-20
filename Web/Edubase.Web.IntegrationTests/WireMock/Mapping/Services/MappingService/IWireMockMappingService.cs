using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;

namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService;
public interface IWireMockMappingService
{
    Task<HttpMappedResponses> RegisterMappingsAsync(IEnumerable<HttpMappingFile> files);
}
