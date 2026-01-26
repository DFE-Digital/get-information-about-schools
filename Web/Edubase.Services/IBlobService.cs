using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Edubase.Services;

/// <summary>
/// Defines operations for interacting with Azure Blob Storage,
/// including uploading, archiving, existence checks, and generating SAS URLs.
/// </summary>
public interface IBlobService
{
    /// <summary>
    /// Creates a ZIP archive containing the provided blob stream.
    /// </summary>
    /// <param name="blobStream">The stream representing the blob content.</param>
    /// <param name="blobName">The name of the blob inside the archive.</param>
    /// <returns>
    /// A <see cref="MemoryStream"/> containing the ZIP archive.
    /// </returns>
    Task<MemoryStream> ArchiveBlobAsync(MemoryStream blobStream, string blobName);

    /// <summary>
    /// Uploads a blob to Azure Blob Storage.
    /// </summary>
    /// <param name="sourceBytes">The byte array containing the blob content.</param>
    /// <param name="mimeType">The MIME type of the blob.</param>
    /// <param name="containerName">The name of the container to upload to.</param>
    /// <param name="blobName">The name of the blob to create.</param>
    /// <returns>
    /// The absolute URI of the uploaded blob.
    /// </returns>
    Task<string> UploadAsync(byte[] sourceBytes, string mimeType, string containerName, string blobName);

    /// <summary>
    /// Determines whether a blob exists in the specified container.
    /// </summary>
    /// <param name="containerName">The name of the blob container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <returns>
    /// <c>true</c> if the blob exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ExistsAsync(string containerName, string blobName);

    /// <summary>
    /// Generates a random blob name, optionally appending a file extension.
    /// </summary>
    /// <param name="extensionToAppend">
    /// The file extension to append (with or without a leading dot).
    /// </param>
    /// <returns>
    /// A unique blob name suitable for use in Azure Blob Storage.
    /// </returns>
    string CreateRandomBlobName(string extensionToAppend = null);

    /// <summary>
    /// Generates a read-only Shared Access Signature (SAS) URL for a blob.
    /// </summary>
    /// <param name="containerName">The name of the blob container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="expirationUtc">The UTC expiration time for the SAS token.</param>
    /// <returns>
    /// A read-only SAS URL granting temporary access to the blob.
    /// </returns>
    string GetReadOnlySharedAccessUrl(string containerName, string blobName, DateTimeOffset expirationUtc);

    /// <summary>
    /// Retrieves a <see cref="BlobClient"/> for the specified container and blob name.
    /// </summary>
    /// <param name="containerName">The name of the blob container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <returns>
    /// A <see cref="BlobClient"/> instance for interacting with the blob.
    /// </returns>
    BlobClient GetBlobClient(string containerName, string blobName);
}
