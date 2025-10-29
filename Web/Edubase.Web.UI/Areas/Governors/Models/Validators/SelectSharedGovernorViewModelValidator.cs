using System.Linq;
using Edubase.Services.Enums;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators;

public class SelectSharedGovernorViewModelValidator : AbstractValidator<SelectSharedGovernorViewModel>
{
    public SelectSharedGovernorViewModelValidator()
    {
        When(x => EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
        {
            RuleFor(x => x.SelectedGovernorId)
                .Must((model, selectedId) => IsSelectedGovernorIdFound(model, selectedId))
                .WithMessage("Required")
                .WithSummaryMessage("Select a governor");
        });

        When(x => !EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
        {
            RuleFor(x => x.Governors)
                .Must(govs => govs != null && govs.Any(g => g.Selected))
                .WithMessage("Required")
                .WithSummaryMessage("Select at least one governor");
        });

        RuleFor(x => x)
            .Custom((model, context) =>
            {
                if (model.Governors == null) return;

                for (var index = 0; index < model.Governors.Count; index++)
                {
                    var governor = model.Governors[index];
                    if (!IsGovernorSelected(governor, model.SelectedGovernorId))
                        continue;

                    if (!governor.AppointmentStartDate.IsValid())
                    {
                        context.AddFailure(
                            $"Governors[{index}].AppointmentStartDate",
                            $"Enter a valid appointment start date for {governor.FullName} ({governor.Id})"
                        );
                    }

                    if (model.Role != eLookupGovernorRole.Establishment_SharedGovernanceProfessional &&
                        !governor.AppointmentEndDate.IsValid())
                    {
                        context.AddFailure(
                            $"Governors[{index}].AppointmentEndDate",
                            $"Enter a valid appointment end date for {governor.FullName} ({governor.Id})"
                        );
                    }
                }
            });
    }

    private static bool IsGovernorSelected(SharedGovernorViewModel governor, string selectedId)
    {
        return governor.Selected || governor.Id.ToString() == selectedId;
    }

    private static bool IsSelectedGovernorIdFound(SelectSharedGovernorViewModel model, string selectedId)
    {
        return int.TryParse(selectedId, out var id) &&
               model.Governors?.Any(g => g.Id == id) == true;
    }
}
