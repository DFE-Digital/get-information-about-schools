using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators
{
    public class BulkUpdateViewModelValidator : AbstractValidator<BulkUpdateViewModel>
    {
        public BulkUpdateViewModelValidator()
        {
            RuleFor(x => x.BulkUpdateType)
                .NotEmpty().WithMessage("Please select a file type.");

            RuleFor(x => x.EffectiveDate)
                .Must(x => x.IsEmpty() || x.IsValid()).WithMessage("Select set a valid date");

            RuleFor(x => x.BulkFile)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Please upload a file.")
                .Must(x => x.ContentLength > 0).WithMessage("Please upload a file.")
                .Must(x => x.FileName.EndsWith("csv") || x.FileName.EndsWith("xlsx") || x.FileName.EndsWith("xls")).WithMessage("Please upload a CSV or XLSX.");
        }
    }
}