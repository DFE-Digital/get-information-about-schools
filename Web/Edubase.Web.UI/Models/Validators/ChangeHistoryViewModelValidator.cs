using Edubase.Web.UI.Models.Search;
using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Models.Validators
{

    public class ChangeHistoryViewModelValidator : EdubaseAbstractValidator<ChangeHistoryViewModel>
    {
        public ChangeHistoryViewModelValidator()
        {
            RuleFor(x => x)
                .Must(x => x.DateFilterTo.ToDateTime() > x.DateFilterFrom.ToDateTime())
                .WithSummaryMessage("To date cannot be prior to From date")
                .When(x => x.DateFilterFrom != null && x.DateFilterTo != null && x.DateFilterFrom.IsValid() && x.DateFilterTo.IsValid());

            RuleFor(x => x.DateFilterMode)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithSummaryMessage("Date mode must be selected")
                .When(x => x.SearchType == eSearchType.EstablishmentAll);
        }
    }
}