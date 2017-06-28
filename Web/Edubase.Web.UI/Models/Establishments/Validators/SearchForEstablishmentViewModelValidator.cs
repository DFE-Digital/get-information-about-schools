using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System.Linq;
using System.Security.Principal;
using Edubase.Common;

namespace Edubase.Web.UI.Models.Establishments.Validators
{
    public class SearchForEstablishmentViewModelValidator : EdubaseAbstractValidator<SearchForEstablishmentViewModel>
    {
        public SearchForEstablishmentViewModelValidator(IEstablishmentReadService establishmentReadService, IPrincipal principal)
        {
            RuleFor(x => x.SearchUrn)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(x => x.ToInteger().HasValue)
                .WithMessage("Please enter a valid URN")
                
                .Must((vm,x) => x.ToInteger() != vm.Urn)
                .WithMessage("Establishment cannot link to itself")
                
                .MustAsync(async (vm, x, ct) =>
                {
                    var links = await establishmentReadService.GetLinkedEstablishmentsAsync(vm.Urn.Value, principal);
                    return !links.Any(a => a.Urn == x.ToInteger());
                }).WithMessage("This URN is already linked to this establishment")
                
                .MustAsync(async (vm, x, ct) => (await establishmentReadService.GetAsync(x.ToInteger().Value, principal)).ReturnValue != null)
                .WithMessage("Establishment not found");
        }
    }
}