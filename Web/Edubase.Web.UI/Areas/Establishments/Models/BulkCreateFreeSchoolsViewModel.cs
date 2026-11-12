using Edubase.Services.Domain;
using Microsoft.AspNetCore.Http;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class BulkCreateFreeSchoolsViewModel
    {
        public IFormFile BulkFile { get; set; }
        public BulkCreateFreeSchoolsResult Result { get; set; }
    }
}
