﻿using System.Collections.Generic;
using System.Web.Mvc;
using Edubase.Services.Domain;
using System.Linq;

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


        public string[] RecognisedWarningCodes { get; set; } = new[]
        {
            ApiWarningCodes.ESTABLISHMENT_WITH_SAME_NAME_LA_FOUND
        };

        public List<ApiWarning> WarningsToProcess { get; private set; } = new List<ApiWarning>();

        public bool ProcessedWarnings { get; set; }

        public void SetWarnings(ValidationEnvelopeDto envelope)
        {
            if (!ProcessedWarnings && !envelope.Errors.Any())
            {
                var warnings = envelope.Warnings;
                warnings = warnings ?? new List<ApiWarning>();
                warnings = warnings.Where(x => RecognisedWarningCodes.Contains(x.Code) == true).ToList();
                WarningsToProcess = warnings;
                ProcessedWarnings = true;
            }
        }
    }
}