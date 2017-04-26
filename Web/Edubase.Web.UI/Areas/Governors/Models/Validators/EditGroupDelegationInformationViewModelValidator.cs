using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class EditGroupDelegationInformationViewModelValidator : EdubaseAbstractValidator<EditGroupDelegationInformationViewModel>
    {
        public EditGroupDelegationInformationViewModelValidator()
        {
            RuleFor(x => x.DelegationInformation)
                .Must(x => x.Length <= 1000)
                .WithMessage("Must be 1000 characters or less")
                .WithSummaryMessage("Delegation information must be 1000 characters or less");
        }
    }
}