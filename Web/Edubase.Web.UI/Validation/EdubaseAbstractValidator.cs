using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public abstract class EdubaseAbstractValidator<T> : AbstractValidator<T>, IValidatorInterceptor
{
    public ValidationResult AfterMvcValidation(ActionContext actionContext, ValidationContext<T> validationContext, ValidationResult result)
    {
        result.EduBaseAddToModelState(actionContext.ModelState, prefix: null);
        return result;
    }

    public ValidationContext<T> BeforeMvcValidation(ActionContext actionContext, ValidationContext<T> validationContext)
    {
        return validationContext;
    }
}
