using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System.Linq;

namespace Edubase.Web.UI.Models.Establishments.Validators
{
    public class EditEstablishmentLinksViewModelValidator : EdubaseAbstractValidator<EditEstablishmentLinksViewModel>
    {
        public EditEstablishmentLinksViewModelValidator()
        {
            RuleFor(x => x.ActiveRecord.LinkTypeId).NotNull().WithMessage("Please select a link type");
            RuleFor(x => x.ActiveRecord.LinkDateEditable).Must(x => x.IsNotEmpty()).WithMessage("Please provide a valid date for the link");

            When(x => x.ActiveRecord.CreateReverseLink, () =>
            {
                RuleFor(x => x.ActiveRecord.ReverseLinkTypeId).NotNull().WithMessage("Please select a link type");

                RuleFor(x => x.ActiveRecord.ReverseLinkDateEditable).Must(x => x.IsNotEmpty())
                    .WithMessage("Please provide a valid date for the link")
                    .When(x => !x.ActiveRecord.ReverseLinkSameDate);
            });
        }
    }
}