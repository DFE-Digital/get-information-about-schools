using FluentValidation;

namespace Edubase.Web.UI.Models.Validators;

public class ChangeHistoryViewModelValidator : AbstractValidator<ChangeHistoryViewModel>
{
    public ChangeHistoryViewModelValidator()
    {
        When(x =>
            x.DateFilterFrom != null &&
            x.DateFilterTo != null &&
            x.DateFilterFrom.IsValid() &&
            x.DateFilterTo.IsValid(), () =>
            {
                RuleFor(x => x.DateFilterFrom)
                    .Must((model, fromDate) => model.DateFilterTo.ToDateTime() > fromDate.ToDateTime())
                    .WithMessage("Please enter a From date earlier than the To date")
                    .WithSummaryMessage("Please enter a From date earlier than the To date");

                RuleFor(x => x.DateFilterTo)
                    .Must((model, toDate) => model.DateFilterFrom.ToDateTime() < toDate.ToDateTime())
                    .WithMessage("Please enter a To date later than the From date")
                    .WithSummaryMessage("Please enter a To date later than the From date");
            });
    }
}
