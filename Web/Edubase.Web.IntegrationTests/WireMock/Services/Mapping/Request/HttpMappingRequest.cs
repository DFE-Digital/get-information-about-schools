namespace Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;
public record HttpMappingRequest
{
    public HttpMappingRequest(IEnumerable<HttpMappingFile> httpMappingFiles)
    {
        Guard.ThrowIfNullOrEmpty(httpMappingFiles, nameof(httpMappingFiles));

        // Check for duplicates by FileName
        var duplicateIds =
            httpMappingFiles
                .GroupBy((mapping) => mapping.Id)
                .Where(grouping => grouping.Count() > 1);

        if (duplicateIds.Any())
        {
            throw new ArgumentException($"Duplicate identifers for mappings detected: {string.Join(", ", duplicateIds)}");
        }

        Files = httpMappingFiles.ToList().AsReadOnly();
    }

    public IReadOnlyList<HttpMappingFile> Files { get; }

    public static HttpMappingRequest Create(IEnumerable<HttpMappingFile> httpMappingFiles) => new(httpMappingFiles);
}
