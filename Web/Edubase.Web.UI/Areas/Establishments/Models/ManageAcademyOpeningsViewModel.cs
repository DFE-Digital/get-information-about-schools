using System.Collections.Generic;
using Edubase.Services.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IEnumerable<EditAcademyOpeningViewModel> AllAcademyOpenings { get; set; } = new List<EditAcademyOpeningViewModel>();
        public string PageTitle { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> MonthOptions { get; set; } = new List<SelectListItem>();
        public string SelectedMonth { get; set; }
        public string EstablishmentTypeId { get; set; }
        public string CurrentRouteName { get; set; }
    }
}
