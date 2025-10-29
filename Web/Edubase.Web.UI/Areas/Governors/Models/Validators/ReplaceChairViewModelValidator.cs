using System.Linq;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators;

public class ReplaceChairViewModelValidator : AbstractValidator<ReplaceChairViewModel>
{
    public ReplaceChairViewModelValidator()
    {
        RuleFor(x => x.DateTermEnds)
            .Must(x => x.IsValid())
            .WithMessage("This date is invalid")
            .WithSummaryMessage("Date term ends is invalid");

        When(x => x.NewChairType == ReplaceChairViewModel.ChairType.SharedChair, () =>
        {
            RuleFor(x => x.SharedGovernors)
                .NotEmpty()
                .WithMessage("There are no shared governors available for this establishment")
                .WithSummaryMessage("There are no shared governors available for this establishment");

            RuleFor(x => x)
                .Custom((model, context) =>
                {
                    var selectedGovernor = model.SharedGovernors?.FirstOrDefault(s => s.Id == model.SelectedGovernorId);
                    if (selectedGovernor == null || !selectedGovernor.AppointmentEndDate.IsValid())
                    {
                        context.AddFailure("AppointmentEndDate", "Appointment end date is invalid");
                    }
                });
        });

        When(x => x.NewChairType == ReplaceChairViewModel.ChairType.LocalChair, () =>
        {
            RuleFor(x => x.NewLocalGovernor.AppointmentEndDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Appointment end date is invalid");
        });
    }
}
