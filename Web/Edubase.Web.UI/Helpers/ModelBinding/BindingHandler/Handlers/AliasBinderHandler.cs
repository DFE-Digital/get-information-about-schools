using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// Handler that binds properties using BindAlias attributes.
/// </summary>
public sealed class AliasBinderHandler(
    ITypeConverter converter) : PropertyBinderHandler
{
    /// <summary>
    /// Attempts to bind a property using any <see cref="BindAliasAttribute"/> defined on it.
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
    /// A task that resolves to <c>true</c> if the property was successfully bound using an alias,
    /// or <c>false</c> if binding was delegated to the next handler in the chain.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method inspects the property for any <see cref="BindAliasAttribute"/> instances.
    /// For each alias, it attempts to retrieve a value from the <see cref="ModelBindingContext.ValueProvider"/>.
    /// If a value is found, it is normalized (joined into a comma-separated string if multiple values exist),
    /// converted to the target property type using the configured type converter, and assigned to the property.
    /// </para>
    /// <para>
    /// If no alias values are found or conversion fails, the request is delegated to the next handler
    /// in the chain (e.g., <c>ArrayBinderHandler</c>, <c>ListBinderHandler</c>, etc.).
    /// </para>
    /// </remarks>
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        foreach (var alias in property.GetCustomAttributes<BindAliasAttribute>(true))
        {
            ValueProviderResult valueResult =
                context.ValueProvider.GetValue(alias.Alias);

            if (valueResult == ValueProviderResult.None) { continue; }

            try
            {
                // Instead of handling arrays/lists here, just normalise the alias
                // and delegate to the rest of the chain.
                string combined = string.Join(",", valueResult.Values);

                // Use the converter for simple types
                object converted =
                    converter.Convert(combined, property.PropertyType);

                property.SetValue(model, converted);
                return true;
            }
            catch{
                context.Result = ModelBindingResult.Failed();
            }
        }

        // Pass to next handler in the chain (ArrayBinderHandler, ListBinderHandler, etc.)
        return await base.HandleAsync(context, model, property);
    }
}
