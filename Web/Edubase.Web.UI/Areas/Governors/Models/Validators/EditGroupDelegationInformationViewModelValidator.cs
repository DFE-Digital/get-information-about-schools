using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators;

public class EditGroupDelegationInformationViewModelValidator : AbstractValidator<EditGroupDelegationInformationViewModel>
{
    public EditGroupDelegationInformationViewModelValidator()
    {
        RuleFor(x => x.DelegationInformation)
            .MaximumLength(1000)
            .WithMessage("Must be 1000 characters or less")
            .WithSummaryMessage("Details must be 1000 characters or less");
    }
}

public class EditGroupCorporateContactViewModelValidator : AbstractValidator<EditGroupCorporateContactViewModel>
{
    public EditGroupCorporateContactViewModelValidator()
    {
        RuleFor(x => x.CorporateContact)
            .MaximumLength(10)
            .WithMessage("Must be 10 characters or less")
            .WithSummaryMessage("Corporate contact must be 10 characters or less");
    }
}
