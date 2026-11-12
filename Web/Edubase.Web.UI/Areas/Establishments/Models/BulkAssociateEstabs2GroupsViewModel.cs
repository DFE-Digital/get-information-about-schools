using Edubase.Services.Domain;
using Microsoft.AspNetCore.Http;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class BulkAssociateEstabs2GroupsViewModel
    {
        public IFormFile BulkFile { get; set; }
        public BulkUpdateProgressModel Result { get; set; }
    }
}
