namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;

public record HttpMappingFile
{
    public HttpMappingFile(string? id, string fileName)
    {
        Id = id?.Trim() ?? Guid.NewGuid().ToString();

        Guard.ThrowIfNullOrEmpty(fileName, nameof(fileName));
        FileName = fileName;
    }

    public HttpMappingFile(string fileName) : this(id: null, fileName) { }

    public string Id { get; }
    public string FileName { get; }
}

