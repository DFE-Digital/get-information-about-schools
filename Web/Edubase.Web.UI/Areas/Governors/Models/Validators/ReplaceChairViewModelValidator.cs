using System.Linq;
using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class ReplaceChairViewModelValidator : EdubaseAbstractValidator<ReplaceChairViewModel>
    {
        public ReplaceChairViewModelValidator()
        {
            RuleFor(x => x.DateTermEnds)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Date term ends is invalid");

            RuleFor(x => x.SharedGovernors)
                .NotEmpty()
                .WithMessage("There are no shared governors available for this establishment")
                .WithSummaryMessage("There are no shared governors available for this establishment")
                .When(x => x.NewChairType == ReplaceChairViewModel.ChairType.SharedChair, ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.SharedGovernors.Single(s => s.Id == x.SelectedGovernorId).AppointmentEndDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Appointment end date is invalid")
                .When(x => x.SharedGovernors != null && x.NewChairType == ReplaceChairViewModel.ChairType.SharedChair, ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.NewLocalGovernor.AppointmentEndDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Appointment end date is invalid")
                .When(x => x.NewChairType == ReplaceChairViewModel.ChairType.LocalChair, ApplyConditionTo.CurrentValidator);
        }
    }
}
