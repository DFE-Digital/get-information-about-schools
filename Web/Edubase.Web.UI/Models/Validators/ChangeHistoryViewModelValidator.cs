using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Models.Validators
{

    public class ChangeHistoryViewModelValidator : EdubaseAbstractValidator<ChangeHistoryViewModel>
    {
        public ChangeHistoryViewModelValidator()
        {
            RuleFor(x => x.DateFilterFrom)
                .Must((model, x) => model.DateFilterTo.ToDateTime() > x.ToDateTime())
                .WithSummaryMessage("Please enter a From date earlier than the To date")
                .When(x => x.DateFilterFrom != null && x.DateFilterTo != null && x.DateFilterFrom.IsValid() && x.DateFilterTo.IsValid());

            RuleFor(x => x.DateFilterTo)
                .Must((model, x) => model.DateFilterFrom.ToDateTime() < x.ToDateTime())
                .WithSummaryMessage("Please enter a To date later than the From date")
                .When(x => x.DateFilterFrom != null && x.DateFilterTo != null && x.DateFilterFrom.IsValid() && x.DateFilterTo.IsValid());
        }
    }
}
