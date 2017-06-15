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
                .Must(x => x.FileName.EndsWith("xlsx")).WithMessage("Please upload a XLSX file");
        }
    }
}
