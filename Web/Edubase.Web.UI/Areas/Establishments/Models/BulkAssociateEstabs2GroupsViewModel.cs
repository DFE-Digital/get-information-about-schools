using System.Web;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class BulkAssociateEstabs2GroupsViewModel
    {
        public HttpPostedFileBase BulkFile { get; set; }
        public BulkUpdateProgressModel Result { get; set; }
    }
}
