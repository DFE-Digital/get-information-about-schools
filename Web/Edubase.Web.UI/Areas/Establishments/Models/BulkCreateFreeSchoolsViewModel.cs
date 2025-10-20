using System.Web;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class BulkCreateFreeSchoolsViewModel
    {
        public HttpPostedFileBase BulkFile { get; set; }
        public BulkCreateFreeSchoolsResult Result { get; set; }
    }
}
