using System.Net;
using Newtonsoft.Json;

namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;
public record HttpMappedResponse
{
    private readonly string _body;

    public HttpMappedResponse(
        ClientMappingId mappingId,
        int statusCode,
        string? body = null)
    {
        Guard.ThrowIfNull(mappingId, nameof(mappingId));
        Id = mappingId.Value;
        StatusCode = statusCode;
        _body = body ?? string.Empty;
    }

    public HttpMappedResponse(
        ClientMappingId mappingId,
        HttpStatusCode code,
        string body) : this(mappingId, (int) code, body)
    { }

    public string Id { get; }
    public int StatusCode { get; }

    public TBody GetResponseBody<TBody>()
    {
        if (string.IsNullOrWhiteSpace(_body))
        {
            throw new ArgumentException("Response body is empty.");
        }

        return JsonConvert.DeserializeObject<TBody>(_body)
            ?? throw new ArgumentException($"Failed to deserialize body to {typeof(TBody).Name}");
    }

    public static HttpMappedResponse Create(ClientMappingId clientId, int code, string body) => new(clientId, code, body);
}
