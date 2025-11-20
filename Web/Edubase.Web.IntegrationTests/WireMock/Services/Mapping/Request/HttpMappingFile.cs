namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;

public record HttpMappingFile
{
    public HttpMappingFile(string? id, string fileName)
    {
        ClientId = id?.Trim() ?? Guid.NewGuid().ToString();

        Guard.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        FileName = fileName;
    }

    public string ClientId { get; }
    public string FileName { get; }
}

