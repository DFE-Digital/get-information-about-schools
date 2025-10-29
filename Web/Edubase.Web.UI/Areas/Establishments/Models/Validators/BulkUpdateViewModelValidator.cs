using System.IO;
using System.Linq;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class BulkUpdateViewModelValidator : AbstractValidator<BulkUpdateViewModel>
{
    public BulkUpdateViewModelValidator()
    {
        RuleFor(x => x.BulkUpdateType)
            .NotEmpty().WithMessage("Please select a file type.");

        RuleFor(x => x.EffectiveDate)
            .Must(x => x.IsEmpty() || x.IsValid())
            .WithMessage("Select a valid date");

        RuleFor(x => x.BulkFile)
            .NotNull().WithMessage("Please upload a file.")
            .Must(file => file.Length > 0).WithMessage("Uploaded file is empty.")
            .Must(file =>
            {
                var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                return allowedExtensions.Contains(extension);
            })
            .WithMessage("Please upload a CSV or Excel file (.csv, .xlsx, .xls).");
    }
}
