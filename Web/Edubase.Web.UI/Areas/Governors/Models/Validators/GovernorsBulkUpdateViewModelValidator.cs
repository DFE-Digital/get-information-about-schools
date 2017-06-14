using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class GovernorsBulkUpdateViewModelValidator : AbstractValidator<GovernorsBulkUpdateViewModel>
    {
        public GovernorsBulkUpdateViewModelValidator()
        {
            RuleFor(x => x.BulkFile)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Please upload a file.")
                .Must(x => x.ContentLength > 0).WithMessage("Please upload a file.")
                .Must(x => x.FileName.EndsWith("csv") || x.FileName.EndsWith("xlsx") || x.FileName.EndsWith("xls")).WithMessage("Please upload a CSV or XLSX.");
        }
    }
}
