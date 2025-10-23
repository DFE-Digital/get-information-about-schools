using Edubase.Web.UI.Helpers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Validation
{
    public abstract class EdubaseAbstractValidator<T> : AbstractValidator<T>, IValidatorInterceptor
    {
        public ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext<T> validationContext, ValidationResult result)
        {
            result.EduBaseAddToModelState(controllerContext.ModelState, null);
            return result;
        }

        public ValidationContext<T> BeforeMvcValidation(ControllerContext controllerContext, ValidationContext<T> validationContext)
        {
            return validationContext;
        }
    }
}
