using System.Reflection;
using System.Threading.Tasks;
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
public class PropertyNameBinderHandler : PropertyBinderHandler
{
    private readonly ITypeConverter _converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyNameBinderHandler"/> class.
    /// </summary>
    /// <param name="converter">
    /// The type converter used to convert raw string values into the target property type.
    /// </param>
    public PropertyNameBinderHandler(ITypeConverter converter)
    {
        _converter = converter;
    }

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
        // Determine the property prefix (model name + property name)
        string propertyPrefix =
            string.IsNullOrEmpty(context.ModelName)
                ? property.Name
                : $"{context.ModelName}.{property.Name}";

        // Attempt to retrieve the value from the value provider
        ValueProviderResult valueResult =
            context.ValueProvider.GetValue(propertyPrefix);

        if (valueResult == ValueProviderResult.None)
        {
            // No value found, delegate to the next handler
            return await base.HandleAsync(context, model, property);
        }

        // Convert the raw value to the target property type
        object converted = _converter.Convert(valueResult.FirstValue, property.PropertyType);

        // Assign the converted value to the property
        property.SetValue(model, converted);

        return true;
    }
}
