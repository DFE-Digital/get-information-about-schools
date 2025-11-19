using System;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;

/// <summary>
/// Handler that binds simple types (string, primitives, enums, DateTime, Guid, etc.)
/// both at the property level within complex models and at the top-level model level.
/// </summary>
/// <remarks>
/// This handler checks if a property or model type is simple and attempts to bind its value
/// directly from the <see cref="ModelBindingContext.ValueProvider"/>.
/// If binding succeeds, the property or model is set and the handler short-circuits.
/// Otherwise, the request is passed to the next handler in the chain.
/// </remarks>
public class SimpleTypeBinderHandler : IPropertyBinderHandler
{
    private IPropertyBinderHandler _next;

    /// <summary>
    /// Sets the next handler in the chain of responsibility.
    /// </summary>
    /// <param name="next">The next handler to invoke if this handler cannot bind.</param>
    /// <returns>The handler that was set as next.</returns>
    public IPropertyBinderHandler SetNext(IPropertyBinderHandler next)
    {
        _next = next;
        return next;
    }

    /// <summary>
    /// Attempts to bind a simple type property.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="model">The model instance being bound.</param>
    /// <param name="property">The property to bind.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the property was successfully bound,
    /// or <c>false</c> if binding failed and should be passed to the next handler.
    /// </returns>
    public async Task<bool> HandleAsync(ModelBindingContext context, object model, PropertyInfo property)
    {
        if (property.PropertyType.IsSimpleType())
        {
            ValueProviderResult valueResult =
                context.ValueProvider.GetValue(property.Name);

            if (valueResult != ValueProviderResult.None)
            {
                try
                {
                    object convertedValue =
                        property.PropertyType == typeof(string)
                            ? valueResult.FirstValue
                            : Convert.ChangeType(valueResult.FirstValue,
                                Nullable.GetUnderlyingType(property.PropertyType) ??
                                property.PropertyType);

                    property.SetValue(model, convertedValue);
                    return true;
                }
                catch{
                    context.Result = ModelBindingResult.Failed();
                }
            }
        }

        return _next != null &&
            await _next.HandleAsync(context, model, property);
    }

    /// <summary>
    /// Binds a top-level simple model directly (e.g. string, int, DateTime).
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the model was successfully bound,
    /// or <c>false</c> if the type is not simple.
    /// </returns>
    public async Task<bool> BindModelAsync(ModelBindingContext context)
    {
        Type modelType = context.ModelType;

        if (!modelType.IsSimpleType())
        {
            return false;
        }

        ValueProviderResult valueResult =
            context.ValueProvider.GetValue(context.ModelName);

        if (valueResult == ValueProviderResult.None)
        {
            context.Result = ModelBindingResult.Failed();
            return true;
        }

        try
        {
            object convertedValue =
                modelType == typeof(string)
                    ? valueResult.FirstValue
                    : Convert.ChangeType(
                        valueResult.FirstValue,
                        Nullable.GetUnderlyingType(modelType) ?? modelType);

            context.Result =
                ModelBindingResult.Success(convertedValue);

            return true;
        }
        catch{
            context.Result = ModelBindingResult.Failed();
            return true;
        }
    }
}
