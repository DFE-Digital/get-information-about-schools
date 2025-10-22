using System;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class CreateEditGovernorViewModelValidator : GovernorViewModelValidator<CreateEditGovernorViewModel>
    {
        public CreateEditGovernorViewModelValidator()
        {
            When(x => x.ReplaceGovernorViewModel.GID.HasValue, () =>
            {
                RuleFor(x => x.ReplaceGovernorViewModel.AppointmentEndDate)
                    .Must(x => x.IsValid())
                    .WithMessage("Required")
                    .WithSummaryMessage("Appointment end date is required for the governor you're replacing")
                    .When(x => !x.ReplaceGovernorViewModel.AppointmentEndDate.IsValid(), ApplyConditionTo.CurrentValidator)
                    
                    .Must(x => x.IsValid() && x.ToDateTime().Value < DateTime.Today)
                    .WithMessage("Date must be before today")
                    .WithSummaryMessage("Appointment end date must be before today")
                    .When(x => x.ReplaceGovernorViewModel.AppointmentEndDate.IsValid() && x.ReplaceGovernorViewModel.AppointmentEndDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)

                    .Must((model, x) => x.ToDateTime().Value < model.AppointmentStartDate.ToDateTime().Value)
                    .WithMessage("Date must be before Appointment start date")
                    .WithSummaryMessage("Appointment end date must be before Appointment start date")
                    .When(x => x.ReplaceGovernorViewModel.AppointmentEndDate.IsNotEmpty() && x.AppointmentStartDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator);
            });
        }
    }
}
