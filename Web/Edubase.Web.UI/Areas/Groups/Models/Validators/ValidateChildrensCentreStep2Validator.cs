using Edubase.Services.Groups;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Groups.Models.Validators;

public class ValidateChildrensCentreStep2Validator : AbstractValidator<ValidateChildrensCentreStep2>
{
    public ValidateChildrensCentreStep2Validator(IGroupReadService groupReadService, ISecurityService securityService)
    {
        RuleFor(x => x.LocalAuthorityId)
            .NotNull()
            .WithMessage("This field is mandatory")
            .WithSummaryMessage("Please select a local authority for the group");

        RuleFor(x => x.OpenDate)
            .Must(x => !x.IsEmpty())
            .WithMessage("Open date missing. Please enter the date")
            .Must(x => x.IsValid() || x.IsEmpty())
            .WithMessage("Open date is invalid. Please enter a valid date");

        RuleFor(x => x.GroupName)
            .NotEmpty()
            .WithMessage("Enter a group name")
            .MustAsync(async (model, name, ct) =>
            {
                if (!model.LocalAuthorityId.HasValue)
                    return true; // Skip uniqueness check if LocalAuthorityId is missing

                var principal = securityService.CreateAnonymousPrincipal();
                return !await groupReadService.ExistsAsync(principal, name: name, localAuthorityId: model.LocalAuthorityId.Value);
            })
            .WithMessage("Group name already exists at this authority, please select another name");
    }
}
