using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// Handler that binds properties using <see cref="BindAliasAttribute"/> annotations.
/// </summary>
public sealed class AliasBinderHandler(ITypeConverter converter) : PropertyBinderHandler
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
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        BindAliasAttribute[] bindAliasAttributes = property.GetBindAliases();

        foreach (BindAliasAttribute alias in bindAliasAttributes)
        {
            ValueProviderResult valueResult =
                context.ValueProvider.GetValue(alias.Alias);

            if (!valueResult.HasValues()) { continue; }

            try
            {
                // Normalise multiple values into a single string.
                string combined = valueResult.ToCombinedString();
                // Convert to the target property type.
                object converted =
                    converter.Convert(combined, property.PropertyType);
                // Assign the converted value.
                property.SetValue(model, converted);

                return true;
            }
            catch
            {
                context.Result = ModelBindingResult.Failed();
            }
        }

        // Pass to next handler in the chain (ArrayBinderHandler, ListBinderHandler, etc.).
        return await base.HandleAsync(context, model, property);
    }
}
