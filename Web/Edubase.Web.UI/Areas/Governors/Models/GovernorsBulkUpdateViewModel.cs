using System.Web;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorsBulkUpdateViewModel
    {
        public HttpPostedFileBase BulkFile { get; set; }
        public FileDownloadDto ErrorLogDownload { get; set; }
        public bool WasSuccessful { get; set; }
        public bool BadFileType { get; set; }
    }
}
