using Edubase.Web.UI.Areas.Establishments.Models.Validators;
using Edubase.Web.UI.Models;
using System.Web;
using static Edubase.Services.Establishments.Models.BulkUpdateDto;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class BulkUpdateViewModel
    {
        public DateTimeViewModel EffectiveDate { get; set; } = new DateTimeViewModel();
        public HttpPostedFileBase BulkFile { get; set; }
        public eBulkUpdateType? BulkUpdateType { get; set; }
        public BulkUpdateProgressModel Result { get; internal set; }
        public bool CanOverrideCRProcess { get; set; }
        public bool OverrideCRProcess { get; set; }

        public BulkUpdateViewModel()
        {

        }

        public BulkUpdateViewModel(bool canOverrideCRProcess)
        {
            CanOverrideCRProcess = canOverrideCRProcess;
        }
    }
}
