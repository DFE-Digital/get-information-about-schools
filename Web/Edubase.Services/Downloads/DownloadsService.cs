using Edubase.Services.Downloads.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Downloads
{
    public class DownloadsService : IDownloadsService
    {
        public async Task<FileDownload[]> GetListAsync(IPrincipal principal)
        {
            var client = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString).CreateCloudBlobClient();
            var blobs = client.GetContainerReference("public").GetDirectoryReference("zip").ListBlobs().Cast<CloudBlob>();

            return blobs.Select(x => new FileDownload
            {
                Description = x.Name.Substring(4, x.Name.Length - 8),
                FileSizeInBytes = x.Properties.Length,
                Url = x.Uri.ToString()
            }).ToArray();
        }
    }
}
