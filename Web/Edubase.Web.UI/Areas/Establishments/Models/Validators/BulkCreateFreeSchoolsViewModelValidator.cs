using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators
{
    public class BulkCreateFreeSchoolsViewModelValidator : AbstractValidator<BulkCreateFreeSchoolsViewModel>
    {
        public BulkCreateFreeSchoolsViewModelValidator()
        {
            RuleFor(x => x.BulkFile)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Please upload a file.")
                .Must(x => x.ContentLength > 0).WithMessage("Please upload a file.")
                .Must(x => x.FileName.EndsWith("xlsx")).WithMessage("Please upload an XLSX file.");
        }
    }
}