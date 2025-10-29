using System.Linq;
using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Models;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class EditEstablishmentModelValidator : AbstractValidator<EditEstablishmentModel>
{
    public EditEstablishmentModelValidator(IEstablishmentReadService establishmentReadService)
    {
        When(x => x.ActionSpecifierParam == EditEstablishmentModel.ASSaveDetail, () =>
        {
            RuleFor(x => x.EducationPhaseId)
                .Must((model, phaseId) =>
                {
                    if (!phaseId.HasValue || !model.TypeId.HasValue)
                    {
                        return true;
                    }

                    var map = establishmentReadService.GetEstabType2EducationPhaseMap().AsInts();
                    return map.ContainsKey(model.TypeId.Value) &&
                           map[model.TypeId.Value].Contains(phaseId.Value);
                })
                .WithMessage("Education phase is not valid for the selected type of establishment");
        });
    }
}
