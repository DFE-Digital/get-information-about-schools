using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace Edubase.Services;

public interface IBlobService
{
    string CreateRandomBlobName(string extensionToAppend = null);
    Task<bool> ExistsAsync(string containerName, string blobName);
    Task<MemoryStream> ArchiveBlobAsync(MemoryStream blobStream, string blobName);
    CloudBlockBlob GetBlobReference(string destinationContainerName, string destinationBlobName);
    Task<string> UploadAsync(byte[] sourceBytes, string mimeType, string destinationContainerName, string destinationBlobName);
    string GetReadOnlySharedAccessUrl(string containerName, string blobName, DateTimeOffset expirationUtc);
}
