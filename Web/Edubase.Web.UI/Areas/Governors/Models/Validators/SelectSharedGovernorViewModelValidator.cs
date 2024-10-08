using System.Linq;
using Edubase.Services.Enums;
using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class SelectSharedGovernorViewModelValidator : EdubaseAbstractValidator<SelectSharedGovernorViewModel>
    {
        public SelectSharedGovernorViewModelValidator()
        {
            When(x => EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure the governor ID is selected and valid
                RuleFor(x => x.SelectedGovernorId)
                    .NotNull()
                    .WithMessage("Required")
                    .WithSummaryMessage("You must select a governor")
                    .Must(IsValidGovernorSelected)
                    .WithMessage("Required")
                    .WithSummaryMessage("You must select a governor");

                // Validate appointment start date
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsAppointmentStartDateValid)
                    .When(x => x.SelectedGovernorId != null)
                    .WithMessage("Required")
                    .WithSummaryMessage("An appointment start date is required");

                // Validate appointment end date
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsAppointmentEndDateValid)
                    .When(x => x.SelectedGovernorId != null &&
                               x.Role != eLookupGovernorRole.Establishment_SharedGovernanceProfessional)
                    .WithMessage("An appointment end date is required");
            });


            // When the role may have multiple appointees (i.e., is not "singular"),
            // validations need to be performed on the list of governors
            When(x => !EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure at least one governor is selected (can be multiple, and at least one needs to be selected)
                RuleFor(x => x.Governors)
                    .Must(x => x.Any(g => g.Selected))
                    .WithMessage("Required")
                    .WithSummaryMessage("At least one governor must be selected");

                RuleForEach(z => z.Governors)
                    .ChildRules(x => x
                        .RuleFor(y => y.AppointmentStartDate)
                        .Must(y => y.IsValid())
                        .When(y => y.Selected)
                        .WithMessage("Required")
                        .WithSummaryMessage("An appointment start date is required")
                    )
                    // TODO: This rule is not required for the Establishment_SharedGovernanceProfessional role
                    .ChildRules(x => x
                        .RuleFor(y => y.AppointmentEndDate)
                        .Must(y => y.IsValid())
                        .When(y => y.Selected)
                        .WithMessage("Required")
                        .WithSummaryMessage("An appointment end date is required")
                    );
            });
        }

        private static bool IsValidGovernorSelected(SelectSharedGovernorViewModel model, string selectedId)
        {
            if (!int.TryParse(selectedId, out int governorId))
            {
                return false;
            }

            var selectedGovernorRecord = model
                .Governors
                .SingleOrDefault(g => g.Id == governorId);

            return selectedGovernorRecord != null;
        }

        private static bool IsAppointmentStartDateValid(SelectSharedGovernorViewModel model, string selectedId)
        {
            if (!int.TryParse(selectedId, out int governorId))
            {
                return false;
            }

            var selectedGovernorRecord = model
                .Governors
                .SingleOrDefault(g => g.Id == governorId);

            return selectedGovernorRecord?.AppointmentStartDate.IsValid() ?? false;

        }

        private static bool IsAppointmentEndDateValid(SelectSharedGovernorViewModel model, string selectedId)
        {
            if (int.TryParse(selectedId, out int governorId))
            {
                return model.Governors.SingleOrDefault(g => g.Id == governorId)?.AppointmentEndDate.IsValid() ?? false;
            }

            return false;
        }
    }
}
