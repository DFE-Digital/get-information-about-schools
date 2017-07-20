using Edubase.Services.Domain;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using FluentValidation.Attributes;
using System.Web;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    [Validator(typeof(GovernorsBulkUpdateViewModelValidator))]
    public class GovernorsBulkUpdateViewModel
    {
        public HttpPostedFileBase BulkFile { get; set; }
        public FileDownloadDto ErrorLogDownload { get; set; }
        public bool WasSuccessful { get; set; }
        public bool BadFileType { get; set; }
        public string TemplateUri { get; set; }
    }
}
