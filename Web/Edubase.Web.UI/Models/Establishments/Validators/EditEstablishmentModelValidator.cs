using Edubase.Common;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System.Linq;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services;

namespace Edubase.Web.UI.Models.Validators
{
    public class EditEstablishmentModelValidator : EdubaseAbstractValidator<EditEstablishmentModel>
    {
        private ICachedLookupService _lookupService;

        public EditEstablishmentModelValidator(ICachedLookupService lookupService, IEstablishmentReadService establishmentReadService)
        {
            _lookupService = lookupService;

            When(x => x.Action == EditEstablishmentModel.eAction.SaveDetails, () =>
            {
                RuleFor(x => x.EducationPhaseId)
                    .Must((m, x) => !x.HasValue || (establishmentReadService.GetEstabType2EducationPhaseMap().AsInts()[m.TypeId.Value]).Contains(x.Value))
                    .WithMessage("Education phase is not valid for the selected type of establishment");
            });
        }
        
    }
}