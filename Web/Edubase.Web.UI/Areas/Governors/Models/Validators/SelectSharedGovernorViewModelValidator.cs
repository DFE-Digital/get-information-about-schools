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
                RuleFor(x => x)
                    .Must(x => x.SelectedGovernorId != null && x.Governors.Count(g => g.Id == int.Parse(x.SelectedGovernorId)) == 1)
                    .WithMessage("Required")
                    .WithSummaryMessage("You must select a governor");

                RuleFor(x => x)
                    .Must(x => x.SelectedGovernorId != null && x.Governors.Single(g => g.Id == int.Parse(x.SelectedGovernorId)).AppointmentStartDate.IsValid())
                    .WithMessage("Required")
                    .WithSummaryMessage("An appointment start date is required");

                RuleFor(x => x)
                    .Must(x =>
                    {
                        if(x.Role == eLookupGovernorRole.Establishment_SharedGovernanceProfessional)
                        {
                            // End date is mandatory for shared governors at the establishment level,
                            // But is optional for shared governance professional - establishment (per #230911).
                            return true;
                        }

                        return x.SelectedGovernorId != null
                               && x.Governors.Single(g => g.Id == int.Parse(x.SelectedGovernorId)).AppointmentEndDate.IsValid();
                    })
                    .WithMessage("Required")
                    .WithSummaryMessage("An appointment end date is required");
            });

            When(x => !EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                RuleFor(x => x)
                    .Must(x => x.Governors.Any(g => g.Selected))
                    .WithMessage("Required")
                    .WithSummaryMessage("You must select a governor");

                RuleFor(x => x)
                    .Must(x => x.Governors.Where(g => g.Selected).All(g => g.AppointmentStartDate.IsValid()))
                    .WithMessage("Required")
                    .WithSummaryMessage("An appointment start date is required");

                RuleFor(x => x)
                    .Must(x =>
                    {
                        if(x.Role == eLookupGovernorRole.Establishment_SharedGovernanceProfessional)
                        {
                            // End date is mandatory for shared governors at the establishment level,
                            // But is optional for shared governance professional - establishment (per #230911).
                            return true;
                        }

                        return x.SelectedGovernorId != null
                               && x.Governors.Single(g => g.Id == int.Parse(x.SelectedGovernorId)).AppointmentEndDate.IsValid();
                    })
                    .WithMessage("Required")
                    .WithSummaryMessage("An appointment end date is required");
            });
        }
    }
}
