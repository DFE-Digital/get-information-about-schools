using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// Handler that binds properties directly by their name.
/// </summary>
/// <remarks>
/// This handler attempts to resolve values from the <see cref="ModelBindingContext.ValueProvider"/>
/// using the property name (or prefixed model name). If a value is found, it is converted
/// using the provided <see cref="ITypeConverter"/> and assigned to the property.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="PropertyNameBinderHandler"/> class.
/// </remarks>
/// <param name="converter">
/// The type converter used to convert raw string values into the target property type.
/// </param>
public class PropertyNameBinderHandler(ITypeConverter converter) : PropertyBinderHandler
{
    /// <summary>
    /// Attempts to bind a property by looking up its value in the <see cref="ModelBindingContext.ValueProvider"/>.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="model">The model instance being bound.</param>
    /// <param name="property">The property to bind.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the property was successfully bound,
    /// or <c>false</c> if binding was delegated to the next handler in the chain.
    /// </returns>
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Skip properties that cannot or should not be bound:
        // - Properties marked with [BindNever]
        // - Read-only properties (no setter)
        // - Indexers
        if (property.IsDefined(typeof(BindNeverAttribute), inherit: true)
            || !property.CanWrite
            || property.GetIndexParameters().Length > 0)
        {
            return await base.HandleAsync(context, model, property);
        }

        // Always build an exact key: ModelName.PropertyName.
        // This avoids prefix-matching issues where unrelated values bind to the wrong property.
        string key =
            string.IsNullOrEmpty(context.ModelName)
                ? property.Name
                : $"{context.ModelName}.{property.Name}";

        // Try to get the value from the ValueProvider using the exact key.
        ValueProviderResult valueResult =
            context.ValueProvider.GetValue(key);

        if (!valueResult.HasValues())
        {
            // No value found — pass to next handler.
            return await base.HandleAsync(context, model, property);
        }

        // Convert the raw string to the target type.
        object converted =
            converter.Convert(
                valueResult.FirstValue, property.PropertyType);

        // Assign the converted value.
        property.SetValue(model, converted);

        return true;
    }
}
