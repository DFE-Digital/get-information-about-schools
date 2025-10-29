using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class EditEstablishmentLinksViewModelValidator : AbstractValidator<EditEstablishmentLinksViewModel>
{
    public EditEstablishmentLinksViewModelValidator()
    {
        RuleFor(x => x.ActiveRecord.LinkTypeId)
            .NotNull()
            .WithMessage("Please select the relationship");

        RuleFor(x => x.ActiveRecord.LinkDateEditable)
            .Must(x => x.IsNotEmpty())
            .WithMessage("Please provide a valid date for the link");

        When(x => x.ActiveRecord.CreateReverseLink, () =>
        {
            RuleFor(x => x.ActiveRecord.ReverseLinkTypeId)
                .NotNull()
                .WithMessage("Please select the reverse link relationship");

            When(x => !x.ActiveRecord.ReverseLinkSameDate, () =>
            {
                RuleFor(x => x.ActiveRecord.ReverseLinkDateEditable)
                    .Must(x => x.IsNotEmpty())
                    .WithMessage("Please provide a valid date for the reverse link");
            });
        });
    }
}
