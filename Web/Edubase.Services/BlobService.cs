namespace Edubase.Services
{
    using Common;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    public class BlobService : IBlobService
    {
        private CloudBlobClient _client;
        
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

        #region Constructors

        public BlobService(CloudStorageAccount account)
        {
            _client = account.CreateCloudBlobClient();
            _client.DefaultRequestOptions.RetryPolicy = new ExponentialRetry();
        }

        #endregion

        /// <summary>
        /// Initialises the container and optionally sets permissions.  This method is ideal for eager initialisation.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fullPublicReadAccess">Whether public 'blob' access is to be set</param>
        /// <param name="ensurePermissions">If the container exists, whether to re-assert the permissions.</param>
        public BlobService Initialise(string containerName, bool fullPublicReadAccess = false, bool ensurePermissions = false)
        {
            var result = _client.GetContainerReference(containerName);
            if (!result.CreateIfNotExists(fullPublicReadAccess
                ? BlobContainerPublicAccessType.Blob : BlobContainerPublicAccessType.Off)
                && ensurePermissions)
            {
                // The container exists and we need to check the permissions
                var perm = result.GetPermissions();
                if (perm.PublicAccess != BlobContainerPublicAccessType.Blob && fullPublicReadAccess) // the permission isn't 'public blob read', but we want it
                {
                    result.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }); // open up public access
                }
                else if (perm.PublicAccess == BlobContainerPublicAccessType.Blob && !fullPublicReadAccess) // the permission is currently allowing anon read, but we don't want it.
                {
                    result.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off }); // close public access
                }
            }
            return this;
        }

        /// <summary>
        /// Uploads a blob from a local file
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="mimeType"></param>
        /// <param name="destinationContainerName"></param>
        /// <param name="destinationBlobName"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(string sourceFileName, string mimeType, string destinationContainerName, string destinationBlobName, string friendlyName)
        {
            var blob = GetBlobReference(destinationContainerName, destinationBlobName);
            if (friendlyName != null) blob.Properties.ContentDisposition = "attachment; filename=" + friendlyName;
            using (var fs = new FileStream(sourceFileName, FileMode.Open))
                await blob.UploadFromStreamAsync(fs);
            SetContentMimeType(mimeType, blob);
            return blob.Uri.ToString();
        }

        /// <summary>
        /// Uploads a blob from a memory stream
        /// </summary>
        /// <param name="sourceFileStream"></param>
        /// <param name="mimeType"></param>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(Stream sourceFileStream, string mimeType, string absolutePath)
        {
            var pathComponents = ExtractPathComponents(absolutePath);
            var blob = GetBlobReference(pathComponents.ContainerName, pathComponents.Path);
            await blob.UploadFromStreamAsync(sourceFileStream);
            SetContentMimeType(mimeType, blob);
            return blob.Uri.ToString();
        }

        /// <summary>
        /// Get a blob as a memory stream
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public async Task<MemoryStream> GetBlobAsStreamAsync(string absolutePath)
        {
            var blobStream = new MemoryStream();
            var pathComponents = ExtractPathComponents(absolutePath.ToLower());
            var blob = GetBlobReference(pathComponents.ContainerName, pathComponents.Path);
            await blob.DownloadToStreamAsync(blobStream);

            return blobStream;
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
                var fileToArchive = archive.CreateEntry(blobName);

                using (var entryStream = fileToArchive.Open())
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
            SetContentMimeType(mimeType, blob);
            return blob.Uri.ToString();
        }

        /// <summary>
        /// Uploads a blob from a base64 encoded string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mimeType"></param>
        /// <param name="destinationContainerName"></param>
        /// <param name="destinationBlobName"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(Base64String source, string mimeType, string destinationContainerName, string destinationBlobName)
        {
            return await UploadAsync(Convert.FromBase64String(source.Data), mimeType, destinationContainerName, destinationBlobName);
        }


        /// <summary>
        /// Uploads a blob from a local file
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="mimeType"></param>
        /// <param name="destinationContainerName"></param>
        /// <param name="destinationBlobName"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(string sourceFileName, string mimeType, string absolutePathWithContainerNameAndPrependingSlash, string friendlyName)
        {
            var components = ExtractPathComponents(absolutePathWithContainerNameAndPrependingSlash);
            var blob = GetBlobReference(components.ContainerName, components.Path);
            if (friendlyName != null) blob.Properties.ContentDisposition = "attachment; filename=" + friendlyName;
            using (var fs = new FileStream(sourceFileName, FileMode.Open))
                await blob.UploadFromStreamAsync(fs);
            SetContentMimeType(mimeType, blob);
            return blob.Uri.ToString();
        }

        /// <summary>
        /// Uploads a blob from a byte array
        /// </summary>
        /// <param name="sourceBytes"></param>
        /// <param name="mimeType"></param>
        /// <param name="destinationContainerName"></param>
        /// <param name="destinationBlobName"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(byte[] sourceBytes, string mimeType, string absolutePathWithContainerNameAndPrependingSlash)
        {
            var components = ExtractPathComponents(absolutePathWithContainerNameAndPrependingSlash);
            var blob = GetBlobReference(components.ContainerName, components.Path);
            await blob.UploadFromByteArrayAsync(sourceBytes, 0, sourceBytes.Length);
            SetContentMimeType(mimeType, blob);
            return blob.Uri.ToString();
        }

        /// <summary>
        /// Uploads a blob from a base64 encoded string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mimeType"></param>
        /// <param name="destinationContainerName"></param>
        /// <param name="destinationBlobName"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(Base64String source, string mimeType, string absolutePathWithContainerNameAndPrependingSlash)
        {
            var components = ExtractPathComponents(absolutePathWithContainerNameAndPrependingSlash);
            return await UploadAsync(Convert.FromBase64String(source.Data), mimeType, components.ContainerName, components.Path);
        }

        /// <summary>
        /// Checks whether a blob exists at the location supplied by the absolute path.
        /// An absolute path is the full path after the domain and port, IT MUST INCLUDE THE PREPENDING SLASH. e.g., /test/folder/file.png., not: test/folder/file.png.
        /// </summary>
        /// <param name="absolutePathWithContainerNameAndPrependingSlash">The absolute path to the blob INCLUDING the container name</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string absolutePathWithContainerNameAndPrependingSlash)
        {
            var components = ExtractPathComponents(absolutePathWithContainerNameAndPrependingSlash);
            return await ExistsAsync(components.ContainerName, components.Path);
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

        private void SetContentMimeType(string mimeType, CloudBlockBlob blob)
        {
            blob.Properties.ContentType = mimeType;
            blob.SetProperties();
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
}
