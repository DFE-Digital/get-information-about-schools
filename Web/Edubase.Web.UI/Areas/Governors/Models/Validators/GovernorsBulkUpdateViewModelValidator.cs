using System;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators;

public class GovernorsBulkUpdateViewModelValidator : AbstractValidator<GovernorsBulkUpdateViewModel>
{
    public GovernorsBulkUpdateViewModelValidator()
    {
        RuleFor(x => x.BulkFile)
            .NotNull().WithMessage("Please upload a file.")
            .DependentRules(() =>
            {
                RuleFor(x => x.BulkFile.ContentLength)
                    .GreaterThan(0).WithMessage("Please upload a file.");

                RuleFor(x => x.BulkFile.FileName)
                    .Must(name => name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Please upload an XLSX file");
            });
    }
}
