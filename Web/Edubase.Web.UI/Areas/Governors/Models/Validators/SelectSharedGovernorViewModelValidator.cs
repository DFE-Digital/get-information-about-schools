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
            // This section for "singular" governor roles (i.e., roles that can only have one appointee)
            // On the web UI, these are displayed as radio buttons and the `Selected` value is ignored.
            // Instead, the property `SelectedGovernorId` is used to track which governor is selected.
            When(x => EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure the governor ID is selected and valid
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsSelectedGovernorIdFound)
                    .WithMessage("Required")
                    .WithSummaryMessage("Select a governor");
            });

            // This section for roles that can have multiple appointees.
            // On the web UI, these are displayed as checkboxes and the `Selected` value for each governor
            // is used to track which governors are selected (the `SelectedGovernorId` is ignored).
            // In this case, validations need to be applied to all selected governors (not just a single "selected" governor).
            When(x => !EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure at least one governor is selected (can be multiple, and at least one needs to be selected)
                RuleFor(x => x.Governors)
                    .Must(x => x.Any(g => g.Selected))
                    .WithMessage("Required")
                    .WithSummaryMessage("Select at least one governor");
            });


            // Custom validation for appointment end date.
            // Using a custom rule is needed due to the hijinks/headaches with the `model.SelectedGovernorId`
            // property versus `governor.Selected` property, and how the rules do not seem to share
            // between the parent property in `RuleForEach` and the child property (governor) in `model.Governors`.
            RuleFor(x => x)
                .Custom((model, context) =>
                {
                    // Due to the use of `SelectedGovernorId`, hijinks are required
                    // to attach validations to the selected governor with positional index,
                    // and have the error messages display next to the correct governor.
                    for (var index = 0; index < model.Governors.Count; index++)
                    {
                        var currentGovernor = model.Governors[index];
                        if (!IsGovernorSelected(currentGovernor, model.SelectedGovernorId))
                        {
                            // Skip governors that are not selected
                            continue;
                        }

                        // Validate appointment start date for all _shared_ establishment-side roles.
                        if (!currentGovernor.AppointmentStartDate.IsValid())
                        {
                            context.AddFailure(
                                $"Governors[{index}].{nameof(currentGovernor.AppointmentStartDate)}",
                                $"Enter a valid appointment start date for {currentGovernor.FullName} ({currentGovernor.Id})"
                            );
                        }

                        // Validate appointment end date roles, for all _shared_ establishment-side roles except `shared governance professional - establishment`.
                        // Note that end date is optional for this role, per #230911.
                        if (
                            model.Role != eLookupGovernorRole.Establishment_SharedGovernanceProfessional
                            && !currentGovernor.AppointmentEndDate.IsValid()
                        )
                        {
                            context.AddFailure(
                                $"Governors[{index}].{nameof(currentGovernor.AppointmentEndDate)}",
                                $"Enter a valid appointment end date for {currentGovernor.FullName} ({currentGovernor.Id})"
                            );
                        }
                    }
                });


        }

        private static bool IsGovernorSelected(SharedGovernorViewModel currentGovernor, string modelSelectedGovernorId)
        {
            // Note two ways to detect if current governor is selected:
            // 1. "Singular" governor roles use `model.SelectedGovernorId` to track the selected governor.
            // 2. "Multiple"/non-singular governor roles use `governor.Selected` to track the selected governor.

            // Consider making this more robust by checking if the governor is selected based on the governor role
            // (e.g., if the governor role is singular, then the governor is selected if the governor ID matches the selected governor ID).
            return currentGovernor.Selected || currentGovernor.Id.ToString() == modelSelectedGovernorId;
        }

        private static bool IsSelectedGovernorIdFound(SelectSharedGovernorViewModel model, string selectedId)
        {
            return int.TryParse(selectedId, out var governorId)
                   && model.Governors.SingleOrDefault(g => g.Id == governorId) != null;
        }
    }
}
