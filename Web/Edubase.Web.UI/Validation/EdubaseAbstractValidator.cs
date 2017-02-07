using FluentValidation.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Results;
using System.Web.Mvc;

namespace Edubase.Web.UI.Validation
{
    public abstract class EdubaseAbstractValidator<T> : AbstractValidator<T>, IValidatorInterceptor
    {
        public ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result)
        {
            controllerContext.Controller.ViewBag.FVErrors = result;
            return result;
        }

        public ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext) => validationContext;
    }
}