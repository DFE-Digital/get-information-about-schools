using System.Linq;
using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators
{
    public class EditEstablishmentModelValidator : EdubaseAbstractValidator<EditEstablishmentModel>
    {
        public EditEstablishmentModelValidator(IEstablishmentReadService establishmentReadService)
        {
            When(x => x.ActionSpecifierParam == EditEstablishmentModel.ASSaveDetail, () =>
            {
                RuleFor(x => x.EducationPhaseId)
                    .Must((m, x) => !x.HasValue || (establishmentReadService.GetEstabType2EducationPhaseMap().AsInts()[m.TypeId.Value]).Contains(x.Value))
                    .WithMessage("Education phase is not valid for the selected type of establishment");
            });
        }
        
    }
}