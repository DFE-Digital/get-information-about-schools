using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class GovernorsBulkUpdateViewModelValidator : AbstractValidator<GovernorsBulkUpdateViewModel>
    {
        public GovernorsBulkUpdateViewModelValidator()
        {
            RuleFor(x => x.BulkFile)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Please upload a file.")
                .Must(x => x.Length > 0).WithMessage("Please upload a file.")
                .Must(x => x.FileName.EndsWith("xlsx")).WithMessage("Please upload an XLSX file");
        }
    }
}
