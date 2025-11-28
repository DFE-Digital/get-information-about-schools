namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;
public record HttpMappedResponses
{
    private readonly List<HttpMappedResponse> _responses;
    public HttpMappedResponses(IEnumerable<HttpMappedResponse> responses)
    {
        Guard.ThrowIfNullOrEmpty(responses, nameof(responses));
        _responses = responses.ToList();
    }

    public IReadOnlyCollection<HttpMappedResponse> Responses => _responses.AsReadOnly();

    public HttpMappedResponse GetResponseById(string id)
    {
        Guard.ThrowIfNullOrWhiteSpace(id, nameof(id));

        var response = _responses.SingleOrDefault(t => t.Id.Equals(id, StringComparison.Ordinal));

        return response is null ?
            throw new ArgumentException($"Unable to find mapped response with identifier: {id}") :
                response;
    }
};
