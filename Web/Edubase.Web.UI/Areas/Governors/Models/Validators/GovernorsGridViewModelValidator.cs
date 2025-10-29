using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators;

public class GovernorsGridViewModelValidator : AbstractValidator<GovernorsGridViewModel>
{
    public GovernorsGridViewModelValidator()
    {
        When(x => x.Action == "Save", () =>
        {
            RuleFor(x => x.RemovalAppointmentEndDate)
                .Must(x => x.IsValid())
                .WithMessage("Invalid date")
                .When(x => x.RemovalAppointmentEndDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.RemovalAppointmentEndDate)
                .Must(x => !x.IsEmpty())
                .WithMessage("Required")
                .When(x => x.RemovalAppointmentEndDate.IsEmpty(), ApplyConditionTo.CurrentValidator);
        });
    }
}
