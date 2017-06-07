using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Models.Establishments
{
    public class CreateEstablishmentViewModel
    {
        public string Name { get; set; }
        public bool? GenerateEstabNumber { get; set; }
        public string EstablishmentNumber { get; set; }
        public int? LocalAuthorityId { get; set; }
        public int? EducationPhaseId { get; set; }
        public int? EstablishmentTypeId { get; set; }
        
        public IEnumerable<SelectListItem> LocalAuthorities { get; set; }
        public IEnumerable<SelectListItem> EstablishmentTypes { get; set; }
        public IEnumerable<SelectListItem> EducationPhases { get; set; }
        public CreateEstablishmentPermissionDto CreateEstablishmentPermission { get; internal set; }
    }
}