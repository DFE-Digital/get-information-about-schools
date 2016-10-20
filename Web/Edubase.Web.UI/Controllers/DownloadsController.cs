using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace Edubase.Web.UI.Controllers
{
    public class DownloadsController : Controller
    {
        // GET: Downloads
        public ActionResult Index()
        {
            var client = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString).CreateCloudBlobClient();
            var blobs = client.GetContainerReference("public").GetDirectoryReference("zip").ListBlobs().Cast<CloudBlob>();
            return View(blobs);
        }
    }
}