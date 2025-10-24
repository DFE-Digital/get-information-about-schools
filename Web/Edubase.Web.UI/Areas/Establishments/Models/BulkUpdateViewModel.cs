using Microsoft.AspNetCore.Http;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models;
using static Edubase.Services.Establishments.Models.BulkUpdateDto;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class BulkUpdateViewModel
    {
        public DateTimeViewModel EffectiveDate { get; set; } = new DateTimeViewModel();
        public IFormFile BulkFile { get; set; }
        public eBulkUpdateType? BulkUpdateType { get; set; }
        public BulkUpdateProgressModel Result { get; internal set; }
        public bool CanOverrideCRProcess { get; set; }
        public bool OverrideCRProcess { get; set; }

        public BulkUpdateViewModel() { }

        public BulkUpdateViewModel(bool canOverrideCRProcess)
        {
            CanOverrideCRProcess = canOverrideCRProcess;
        }
    }
}
