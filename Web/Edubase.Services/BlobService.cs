using Edubase.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Edubase.Services;

public class BlobService : IBlobService
{
    private readonly CloudBlobClient _client;
    
    public class Base64String
    {
        public string Data { get; set; }

        public Base64String(string data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return Data;
        }
    }

    private struct PathComponents
    {
        public string ContainerName { get; set; }
        public string Path { get; set; }
    }

    public BlobService(CloudStorageAccount account)
    {
        _client = account.CreateCloudBlobClient();
        _client.DefaultRequestOptions.RetryPolicy = new ExponentialRetry();
    }

    /// <summary>
    /// Adds a blob as a memory stream to a zip archive 
    /// </summary>
    /// <param name="blobStream"></param>
    /// <param name="blobName"></param>
    /// <returns></returns>
    public async Task<MemoryStream> ArchiveBlobAsync(MemoryStream blobStream, string blobName)
    {
        var archiveStream = new MemoryStream();

        using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
        {
            var blobToArchive = archive.CreateEntry(blobName);

            using (var entryStream = blobToArchive.Open())
            {
              await blobStream.CopyToAsync(entryStream);
            }
        }

        archiveStream.Position = 0;
        
        return archiveStream;
    }

    /// <summary>
    /// Uploads a blob from a byte array
    /// </summary>
    /// <param name="sourceBytes"></param>
    /// <param name="mimeType"></param>
    /// <param name="destinationContainerName"></param>
    /// <param name="destinationBlobName"></param>
    /// <returns></returns>
    public async Task<string> UploadAsync(byte[] sourceBytes, string mimeType, string destinationContainerName, string destinationBlobName)
    {
        var blob = GetBlobReference(destinationContainerName, destinationBlobName);
        await blob.UploadFromByteArrayAsync(sourceBytes, 0, sourceBytes.Length);
        await SetContentMimeType(mimeType, blob);
        return blob.Uri.ToString();
    }

    /// <summary>
    /// Checks whether a given blob in a given container exists
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="blobName"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string containerName, string blobName)
    {
        var reference = GetBlobReference(containerName, blobName);
        return await reference.ExistsAsync();
    }

    /// <summary>
    /// Creates a random blob name.  Optionally appending an extension, e.g., a file extension.
    /// Returns a blob name which is a lower-cased guid with no hyphens
    /// </summary>
    /// <returns></returns>
    public string CreateRandomBlobName(string extensionToAppend = null)
    {
        if (extensionToAppend.Clean() != null && !extensionToAppend.StartsWith(".")) extensionToAppend = "." + extensionToAppend;
        return Guid.NewGuid().ToString("N").ToLower() + (extensionToAppend ?? string.Empty);
    }

    #region Helper methods

    private async Task SetContentMimeType(string mimeType, CloudBlockBlob blob)
    {
        blob.Properties.ContentType = mimeType;
        await blob.SetPropertiesAsync();
    }

    public CloudBlockBlob GetBlobReference(string destinationContainerName, string destinationBlobName)
    {
        var container = _client.GetContainerReference(destinationContainerName);
        var blob = container.GetBlockBlobReference(destinationBlobName);
        return blob;
    }

    /// <summary>
    /// Returns path components from an absolute path
    /// </summary>
    /// <param name="absolutePathWithContainerNameAndPrependingSlash"></param>
    /// <returns></returns>
    private PathComponents ExtractPathComponents(string absolutePathWithContainerNameAndPrependingSlash)
    {
        var path = absolutePathWithContainerNameAndPrependingSlash.Clean();
        var argName = nameof(absolutePathWithContainerNameAndPrependingSlash);
        const string FORWARD_SLASH = "/";
        
        if (path == null) throw new ArgumentNullException(argName);
        if (!path.StartsWith(FORWARD_SLASH)) throw new ArgumentException($"The argument '{argName}' does not have a prepending slash.  Are you sure this is an absolute path?");

        path = path.Substring(1);

        if (path.Length == 0) throw new ArgumentException($"Argument '{argName}' is not valid");
        if (path.Split(new[] { FORWARD_SLASH }, StringSplitOptions.RemoveEmptyEntries).Length <= 1)
            throw new ArgumentException($"Argument '{argName}' doesn't have a path component after the container name component");

        string containerName = path.Substring(0, path.IndexOf(FORWARD_SLASH));
        string blobName = path.Substring(path.IndexOf(FORWARD_SLASH) + 1);

        return new PathComponents { Path = blobName, ContainerName = containerName };
    }

    public string GetReadOnlySharedAccessUrl(string containerName, string blobName, DateTimeOffset expirationUtc)
    {
        var blob = GetBlobReference(containerName, blobName);
        var signature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
        {
            Permissions = SharedAccessBlobPermissions.Read,
            SharedAccessExpiryTime = expirationUtc
        });

        return blob.Uri + signature;
    }

    #endregion

}
