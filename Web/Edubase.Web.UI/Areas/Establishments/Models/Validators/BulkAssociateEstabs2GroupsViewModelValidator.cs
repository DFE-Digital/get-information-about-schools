using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators
{
    public class BulkAssociateEstabs2GroupsViewModelValidator : AbstractValidator<BulkAssociateEstabs2GroupsViewModel>
    {
        public BulkAssociateEstabs2GroupsViewModelValidator()
        {
            RuleFor(x => x.BulkFile)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Please upload a file.")
                .Must(x => x.ContentLength > 0).WithMessage("Please upload a file.")
                .Must(x => x.FileName.EndsWith("csv") || x.FileName.EndsWith("xls")).WithMessage("Please upload a CSV or XLS file.");
        }
    }
}
