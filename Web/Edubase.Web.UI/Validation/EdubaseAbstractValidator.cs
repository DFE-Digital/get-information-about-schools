using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Results;
using System.Web.Mvc;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Validation
{
    public abstract class EdubaseAbstractValidator<T> : AbstractValidator<T>
    {
        public ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext<T> validationContext, ValidationResult result)
        {
            result.EduBaseAddToModelState(controllerContext.Controller.ViewData.ModelState, null);
            //controllerContext.Controller.ViewBag.FVErrors = result;
            return result;
        }

        public ValidationContext<T> BeforeMvcValidation(ControllerContext controllerContext,
            ValidationContext<T> validationContext)
        {
            return validationContext;
        }
    }
}
