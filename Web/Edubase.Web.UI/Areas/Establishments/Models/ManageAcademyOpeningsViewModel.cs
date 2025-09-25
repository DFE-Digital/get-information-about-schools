using System.Collections.Generic;
using System.Web.Mvc;
using Edubase.Services.Core;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class ManageAcademyOpeningsViewModel : PaginatedResult<EditAcademyOpeningViewModel>
    {
        public ManageAcademyOpeningsViewModel()
        {
            Skip = 0;
            Take = 50;
        }
        public PaginatedResult<EditAcademyOpeningViewModel> AcademyOpenings { get; set; }
        public string PageTitle { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> MonthOptions { get; set; } = new List<SelectListItem>();
        public string SelectedMonth { get; set; }
    }
}
