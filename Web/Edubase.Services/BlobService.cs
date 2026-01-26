using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Edubase.Services;

/// <summary>
/// Provides helper methods for interacting with Azure Blob Storage,
/// including uploading, archiving, existence checks, and generating SAS URLs.
/// </summary>
public class BlobService : IBlobService
{
    private readonly BlobServiceClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobService"/> class.
    /// </summary>
    /// <param name="connectionString">
    /// The Azure Storage connection string used to create the underlying <see cref="BlobServiceClient"/>.
    /// </param>
    public BlobService(string connectionString)
    {
        _client = new BlobServiceClient(connectionString);
    }

    /// <summary>
    /// Retrieves a <see cref="BlobClient"/> for the specified container and blob name.
    /// </summary>
    /// <param name="containerName">The name of the blob container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <returns>A <see cref="BlobClient"/> instance for the specified blob.</returns>
    public BlobClient GetBlobClient(string containerName, string blobName)
    {
        BlobContainerClient containerClient =
            _client.GetBlobContainerClient(containerName);

        BlobClient blobClient =
            containerClient.GetBlobClient(blobName);

        return blobClient;
    }

    /// <summary>
    /// Creates a ZIP archive containing the provided blob stream.
    /// </summary>
    /// <param name="blobStream">The stream representing the blob content.</param>
    /// <param name="blobName">The name of the blob to use inside the archive.</param>
    /// <returns>A <see cref="MemoryStream"/> containing the ZIP archive.</returns>
    public async Task<MemoryStream> ArchiveBlobAsync(MemoryStream blobStream, string blobName)
    {
        MemoryStream archiveStream = new();

        using (ZipArchive archive =
            new(archiveStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            ZipArchiveEntry entry = archive.CreateEntry(blobName);

            using (Stream entryStream = entry.Open())
            {
                blobStream.Position = 0;
                await blobStream.CopyToAsync(entryStream);
            }
        }

        archiveStream.Position = 0;
        return archiveStream;
    }

    /// <summary>
    /// Uploads a blob to Azure Blob Storage.
    /// </summary>
    /// <param name="sourceBytes">The byte array containing the blob content.</param>
    /// <param name="mimeType">The MIME type of the blob.</param>
    /// <param name="containerName">The name of the container to upload to.</param>
    /// <param name="blobName">The name of the blob to create.</param>
    /// <returns>The absolute URI of the uploaded blob.</returns>
    public async Task<string> UploadAsync(byte[] sourceBytes, string mimeType, string containerName, string blobName)
    {
        BlobContainerClient containerClient =
            _client.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync();

        BlobClient blobClient =
            containerClient.GetBlobClient(blobName);

        using (MemoryStream ms = new(sourceBytes))
        {
            BlobHttpHeaders headers = new() { ContentType = mimeType };
            await blobClient.UploadAsync(ms, headers);
        }

        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// Determines whether a blob exists in the specified container.
    /// </summary>
    /// <param name="containerName">The name of the blob container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <returns><c>true</c> if the blob exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> ExistsAsync(string containerName, string blobName)
    {
        BlobContainerClient containerClient = _client.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        Response<bool> exists = await blobClient.ExistsAsync();
        return exists.Value;
    }

    /// <summary>
    /// Generates a random blob name, optionally appending a file extension.
    /// </summary>
    /// <param name="extensionToAppend">The file extension to append (with or without a leading dot).</param>
    /// <returns>A unique blob name.</returns>
    public string CreateRandomBlobName(string extensionToAppend = null)
    {
        if (!string.IsNullOrWhiteSpace(extensionToAppend) && !extensionToAppend.StartsWith("."))
        {
            extensionToAppend = "." + extensionToAppend;
        }

        return Guid.NewGuid().ToString("N").ToLower() + (extensionToAppend ?? string.Empty);
    }

    /// <summary>
    /// Generates a read-only Shared Access Signature (SAS) URL for a blob.
    /// </summary>
    /// <param name="containerName">The name of the blob container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="expirationUtc">The UTC expiration time for the SAS token.</param>
    /// <returns>A read-only SAS URL granting temporary access to the blob.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <see cref="BlobClient"/> cannot generate a SAS URI.
    /// </exception>
    public string GetReadOnlySharedAccessUrl(string containerName, string blobName, DateTimeOffset expirationUtc)
    {
        BlobContainerClient containerClient = _client.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException(
                "BlobClient cannot generate SAS. Ensure the client was created with a connection string.");
        }

        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expirationUtc
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}
