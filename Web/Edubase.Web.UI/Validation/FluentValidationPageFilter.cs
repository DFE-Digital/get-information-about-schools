using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Validation
{
    public class FluentValidationPageFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is Controller && !Attribute.IsDefined(context.Controller.GetType(), typeof(ApiControllerAttribute)))
            {
                foreach (var argument in context.ActionArguments.Values)
                {
                    await ValidateAsync(argument, string.Empty, context, new HashSet<object>(ReferenceEqualityComparer.Instance));
                }
            }

            await next();
        }

        private static async Task ValidateAsync(object model, string prefix, ActionExecutingContext context, HashSet<object> visited)
        {
            if (model == null || model is string || model.GetType().IsValueType || !visited.Add(model))
            {
                return;
            }

            if (model is IEnumerable collection)
            {
                var index = 0;
                foreach (var item in collection)
                {
                    await ValidateAsync(item, $"{prefix}[{index++}]", context, visited);
                }

                return;
            }

            var modelType = model.GetType();
            if (modelType.Namespace?.StartsWith("Edubase.Web.UI", StringComparison.Ordinal) != true)
            {
                return;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                var validationContextType = typeof(ValidationContext<>).MakeGenericType(modelType);
                var validationContext = (IValidationContext) Activator.CreateInstance(validationContextType, model);
                var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
                result.EduBaseAddToModelState(context.ModelState, prefix, true);
            }

            foreach (var property in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetIndexParameters().Length > 0 ||
                    property.DeclaringType.GetField($"<{property.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance) == null)
                {
                    continue;
                }

                var propertyPrefix = ModelNames.CreatePropertyModelName(prefix, property.Name);
                await ValidateAsync(property.GetValue(model), propertyPrefix, context, visited);
            }
        }
    }
}
