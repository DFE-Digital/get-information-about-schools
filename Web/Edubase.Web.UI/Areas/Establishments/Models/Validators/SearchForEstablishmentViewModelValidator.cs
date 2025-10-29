using System.Linq;
using System.Security.Principal;
using Edubase.Common;
using Edubase.Services.Establishments;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class SearchForEstablishmentViewModelValidator : AbstractValidator<SearchForEstablishmentViewModel>
{
    public SearchForEstablishmentViewModelValidator(IEstablishmentReadService establishmentReadService, IPrincipal principal)
    {
        RuleFor(x => x.SearchUrn)
            .NotEmpty().WithMessage("Please enter a valid URN")
            .Must(x => x.ToInteger().HasValue)
                .WithMessage("Please enter a valid URN")
            .Must((model, input) =>
            {
                var inputUrn = input.ToInteger();
                return inputUrn.HasValue && inputUrn.Value != model.Urn;
            })
                .WithMessage("Establishment cannot link to itself")
            .MustAsync(async (model, input, ct) =>
            {
                var inputUrn = input.ToInteger();
                if (!inputUrn.HasValue || !model.Urn.HasValue)
                {
                    return true;
                }

                var links = await establishmentReadService.GetLinkedEstablishmentsAsync(model.Urn.Value, principal);
                return links.All(link => link.Urn != inputUrn.Value);
            })
                .WithMessage("This URN is already linked to this establishment")
            .MustAsync(async (model, input, ct) =>
            {
                var inputUrn = input.ToInteger();
                if (!inputUrn.HasValue)
                {
                    return false;
                }

                var result = await establishmentReadService.GetAsync(inputUrn.Value, principal);
                return result?.ReturnValue != null;
            })
                .WithMessage("Establishment not found");
    }
}
