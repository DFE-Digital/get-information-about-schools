using FluentValidation.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Results;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Validation
{
    public abstract class EdubaseAbstractValidator<T> : AbstractValidator<T>, IValidatorInterceptor
    {
        public ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result)
        {
            result.EduBaseAddToModelState(controllerContext.Controller.ViewData.ModelState, null);
            //controllerContext.Controller.ViewBag.FVErrors = result;
            return result;
        }

        public ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext) => validationContext;
    }
}