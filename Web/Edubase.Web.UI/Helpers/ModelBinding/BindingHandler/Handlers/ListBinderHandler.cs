using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// Handler that binds List&lt;T&gt; properties by converting values from the ValueProvider.
/// Delegates conversion logic to <see cref="DefaultTypeConverter"/>.
/// </summary>
public sealed class ListBinderHandler(
    ITypeConverter converter) : PropertyBinderHandler
{
    /// <summary>
    /// Attempts to bind a property when its type is a generic <see cref="List{T}"/>.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ModelBindingContext"/> containing metadata, value providers, and binding state.
    /// </param>
    /// <param name="model">
    /// The model instance whose property is being bound.
    /// </param>
    /// <param name="property">
    /// The <see cref="PropertyInfo"/> representing the property to bind.
    /// </param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the <see cref="List{T}"/> property was successfully bound,
    /// or <c>false</c> if binding was delegated to the next handler in the chain.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method only processes properties whose type is a generic <see cref="List{T}"/>.
    /// It retrieves values from the <see cref="ModelBindingContext.ValueProvider"/> using the property name
    /// (or prefixed model name). If values are found, they are normalized into a comma-delimited string
    /// and converted into the target list type using the configured type converter.
    /// </para>
    /// <para>
    /// If no values are found, or if conversion fails, the request is delegated to the next handler
    /// in the chain (e.g., <c>ComplexTypeBinderHandler</c>, <c>PropertyNameBinderHandler</c>, etc.).
    /// </para>
    /// </remarks>
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Only handle List<T> properties
        if (!property.PropertyType.IsGenericType ||
            property.PropertyType.GetGenericTypeDefinition() != typeof(List<>))
        {
            return await base.HandleAsync(context, model, property);
        }

        string propertyPrefix = string.IsNullOrEmpty(context.ModelName)
            ? property.Name
            : $"{context.ModelName}.{property.Name}";

        ValueProviderResult valueResult =
            context.ValueProvider.GetValue(propertyPrefix);

        if (valueResult == ValueProviderResult.None)
        {
            return await base.HandleAsync(context, model, property);
        }

        try
        {
            // Delegate conversion to DefaultTypeConverter
            object converted =
                converter.Convert(string.Join(
                    ",", valueResult.Values), property.PropertyType);

            property.SetValue(model, converted);
            return true;
        }
        catch{
            context.Result = ModelBindingResult.Failed();
            return await base.HandleAsync(context, model, property);
        }
    }
}
