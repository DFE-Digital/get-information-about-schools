using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace Edubase.Services
{
    public interface IBlobService
    {
        string CreateRandomBlobName(string extensionToAppend = null);
        Task<bool> ExistsAsync(string absolutePathWithContainerNameAndPrependingSlash);
        Task<bool> ExistsAsync(string containerName, string blobName);
        Task<MemoryStream> GetBlobAsStreamAsync(string absolutePath);
        Task<MemoryStream> ArchiveBlobAsync(MemoryStream blobStream, string blobName);
        CloudBlockBlob GetBlobReference(string destinationContainerName, string destinationBlobName);
        BlobService Initialise(string containerName, bool fullPublicReadAccess = false, bool ensurePermissions = false);
        Task<string> UploadAsync(byte[] sourceBytes, string mimeType, string absolutePathWithContainerNameAndPrependingSlash);
        Task<string> UploadAsync(Stream sourceFileStream, string mimeType, string absolutePath);
        Task<string> UploadAsync(string sourceFileName, string mimeType, string absolutePathWithContainerNameAndPrependingSlash, string friendlyName);
        Task<string> UploadAsync(BlobService.Base64String source, string mimeType, string absolutePathWithContainerNameAndPrependingSlash);
        Task<string> UploadAsync(string sourceFileName, string mimeType, string destinationContainerName, string destinationBlobName, string friendlyName);
        Task<string> UploadAsync(byte[] sourceBytes, string mimeType, string destinationContainerName, string destinationBlobName);
        Task<string> UploadAsync(BlobService.Base64String source, string mimeType, string destinationContainerName, string destinationBlobName);
        string GetReadOnlySharedAccessUrl(string containerName, string blobName, DateTimeOffset expirationUtc);
    }
}
