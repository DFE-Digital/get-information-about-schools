using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class EditGroupDelegationInformationViewModelValidator : EdubaseAbstractValidator<EditGroupDelegationInformationViewModel>
    {
        public EditGroupDelegationInformationViewModelValidator()
        {
            RuleFor(x => x.DelegationInformation)
                .Must(x => x == null || x.Length <= 1000)
                .WithMessage("Must be 1000 characters or less")
                .WithSummaryMessage("Details must be 1000 characters or less");
        }
    }

    public class EditGroupCorporateContactViewModelValidator : EdubaseAbstractValidator<EditGroupCorporateContactViewModel>
    {
        public EditGroupCorporateContactViewModelValidator()
        {
            RuleFor(x => x.CorporateContact)
                .Must(x => x == null || x.Length <= 10)
                .WithMessage("Must be 10 characters or less")
                .WithSummaryMessage("Corporate contact must be 10 characters or less");
        }
    }
}