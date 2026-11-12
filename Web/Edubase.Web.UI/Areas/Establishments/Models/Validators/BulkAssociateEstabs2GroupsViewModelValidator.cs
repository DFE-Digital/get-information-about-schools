using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators
{
    public class BulkAssociateEstabs2GroupsViewModelValidator : AbstractValidator<BulkAssociateEstabs2GroupsViewModel>
    {
        public BulkAssociateEstabs2GroupsViewModelValidator()
        {
            RuleFor(x => x.BulkFile)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Please upload a file.")
                .Must(x => x.Length > 0).WithMessage("Please upload a file.")
                .Must(x => x.FileName.EndsWith("csv") || x.FileName.EndsWith("xls")).WithMessage("Please upload a CSV or XLS file.");
        }
    }
}
