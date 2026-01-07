using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// Provides model binding support for properties of type <see cref="List{T}"/>.
/// <para>
/// Simple element types (e.g., <see cref="string"/>, <see cref="int"/>, <see cref="Enum"/>) are bound
/// using comma‑delimited values and the configured <see cref="ITypeConverter"/>.
/// </para>
/// <para>
/// Complex element types (e.g., custom view models) are bound using the default ASP.NET Core model binder,
/// with support for indexed keys such as <c>Property[0].ChildProp</c>.
/// </para>
/// </summary>
public sealed class ListBinderHandler(ITypeConverter converter) : PropertyBinderHandler
{
    /// <summary>
    /// Attempts to bind a property when its type is a generic <see cref="List{T}"/>.
    /// Delegates to either <see cref="BindSimpleListAsync"/> or <see cref="BindComplexListAsync"/>
    /// depending on the element type.
    /// </summary>
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Skip if the property is not a List<T>.
        if (!property.IsList())
        {
            return await base.HandleAsync(context, model, property);
        }

        // Determine the list's element type.
        Type elementType =
            property.PropertyType.GetGenericArguments()[0];

        // Route to simple or complex list binding.
        return elementType.IsSimpleType()
            ? await BindSimpleListAsync(context, model, property)
            : await BindComplexListAsync(context, model, property, elementType);
    }

    /// <summary>
    /// Binds a list whose element type is a simple type.
    /// </summary>
    private async Task<bool> BindSimpleListAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Build the prefix for the list property (e.g. "Tags" or "Codes").
        string prefix =
            context.BuildPropertyPrefix(property);

        // Retrieve all values for this prefix.
        ValueProviderResult values =
            context.ValueProvider.GetValue(prefix);

        // If no values exist, allow the next handler to try.
        if (!values.HasValues())
        {
            return await base.HandleAsync(context, model, property);
        }

        try
        {
            // Combine values into a comma‑delimited string.
            string combined = values.ToCombinedString();

            // Convert into the target list type.
            object converted =
                converter.Convert(combined, property.PropertyType);

            // Assign the list to the model.
            property.SetValue(model, converted);
            return true;
        }
        catch
        {
            // Mark binding as failed and allow fallback.
            context.Result = ModelBindingResult.Failed();
            return await base.HandleAsync(context, model, property);
        }
    }

    /// <summary>
    /// Binds a list whose element type is a complex type.
    /// </summary>
    private async Task<bool> BindComplexListAsync(
        ModelBindingContext context,
        object model,
        PropertyInfo property,
        Type elementType)
    {
        // Build the prefix for the list property (e.g. "Establishments").
        string propertyPrefix =
            context.BuildPropertyPrefix(property);

        // Create a binder for the list element type.
        IModelBinder elementBinder =
            context.CreateBinder(elementType);

        // Retrieve metadata for the element type.
        ModelMetadata metadata =
            context.GetMetadata(elementType);

        // Create an empty list instance using the type factory.
        ITypeFactory typeFactory =
            context.ActionContext.HttpContext.RequestServices
                .GetRequiredService<ITypeFactory>();

        // The list property on which to bind the discovered list values.
        IList list =
            (IList) typeFactory
                .CreateInstance(property.PropertyType)!;

        // Enumerate all indexed prefixes (e.g. "Establishments[0]", "Establishments[1]").
        foreach (string elementPrefix in
            context.EnumerateElementPrefixes(propertyPrefix))
        {
            // Create a binding context for each element.
            DefaultModelBindingContext elementContext =
                context.CreateBindingContext(metadata, elementPrefix);

            // Bind the element using the default binder.
            await elementBinder.BindModelAsync(elementContext);

            // Add successfully bound elements to the list.
            if (elementContext.Result.IsModelSet)
            {
                list.Add(elementContext.Result.Model);
            }
        }

        // If no elements were bound, allow the next handler to try.
        if (list.Count == 0)
        {
            return await base.HandleAsync(context, model, property);
        }

        // Assign the populated list to the model.
        property.SetValue(model, list);
        return true;
    }
}
