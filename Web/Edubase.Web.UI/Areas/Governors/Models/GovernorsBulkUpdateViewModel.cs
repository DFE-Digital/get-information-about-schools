using Edubase.Services.Domain;
using Microsoft.AspNetCore.Http;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorsBulkUpdateViewModel
    {
        public IFormFile BulkFile { get; set; }
        public FileDownloadDto ErrorLogDownload { get; set; }
        public bool WasSuccessful { get; set; }
        public bool BadFileType { get; set; }
    }
}
