using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class GovernorsGridViewModelValidator : EdubaseAbstractValidator<GovernorsGridViewModel>
    {
        public GovernorsGridViewModelValidator()
        {
            When(x => x.Action == "Save", () =>
            {
                RuleFor(x => x.RemovalAppointmentEndDate)
                    .Must(x => x.IsValid())
                    .WithMessage("Invalid date")
                    .WithSummaryMessage("Date term ends is invalid")
                    .When(x => x.RemovalAppointmentEndDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)

                    .Must(x => !x.IsEmpty())
                    .WithMessage("Required")
                    .WithSummaryMessage("Date term ends is required")
                    .When(x => x.RemovalAppointmentEndDate.IsEmpty(), ApplyConditionTo.CurrentValidator);
            });
        }
        
    }
}