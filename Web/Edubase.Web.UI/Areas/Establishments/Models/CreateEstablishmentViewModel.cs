using System.Collections.Generic;
using System.Web.Mvc;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class CreateEstablishmentViewModel
    {
        public string Name { get; set; }
        public bool? GenerateEstabNumber { get; set; }
        public string EstablishmentNumber { get; set; }
        public int? LocalAuthorityId { get; set; }
        public int? EducationPhaseId { get; set; }
        public int EstablishmentTypeId { get; set; }
        
        public IEnumerable<SelectListItem> LocalAuthorities { get; set; }
        public IEnumerable<SelectListItem> EstablishmentTypes { get; set; }
        public IEnumerable<SelectListItem> EducationPhases { get; set; }
        public CreateEstablishmentPermissionDto CreateEstablishmentPermission { get; internal set; }

        public Dictionary<int, int[]> Type2PhaseMap { get; set; }
    }
}