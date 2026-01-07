using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// Handles binding of complex types, including arrays of complex types,
/// by delegating to the appropriate ASP.NET Core model binders.
/// </summary>
/// <remarks>
/// <para>
/// This handler participates in a chain-of-responsibility model binding pipeline.
/// It is responsible for binding non-simple types such as nested view models,
/// arrays of complex types, and POCO objects.
/// </para>
/// <para>
/// Simple types, lists, and arrays of simple types are intentionally skipped
/// so that other handlers (e.g., <see cref="ListBinderHandler"/>) can process them.
/// </para>
/// </remarks>
public sealed class ComplexTypeBinderHandler : PropertyBinderHandler
{
    private readonly IModelMetadataProvider _metadataProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexTypeBinderHandler"/> class.
    /// </summary>
    public ComplexTypeBinderHandler(IModelMetadataProvider metadataProvider)
    {
        _metadataProvider = metadataProvider;
    }

    /// <summary>
    /// Attempts to bind a complex property by delegating to the default model binder
    /// or by explicitly binding arrays of complex types.
    /// </summary>
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Skip simple or non-bindable properties.
        if (property.ShouldSkipProperty())
        {
            return await base.HandleAsync(context, model, property);
        }

        // If this is an array of complex objects, bind each element individually.
        if (property.IsComplexArray())
        {
            Type elementType =
                property.PropertyType.GetElementType()!;

            return await BindArrayAsync(
                context, model, property, elementType);
        }

        // Otherwise treat it as a nested complex object.
        return await BindNestedAsync(context, model, property);
    }

    /// <summary>
    /// Binds an array of complex types using indexed prefixes.
    /// </summary>
    private async Task<bool> BindArrayAsync(
        ModelBindingContext context, object model, PropertyInfo property, Type elementType)
    {
        // Build the prefix for the array property (e.g. "Schools[0].Addresses").
        string prefix = context.BuildPropertyPrefix(property);

        IModelBinder binder = context.CreateBinder(elementType);
        ModelMetadata metadata = context.GetMetadata(elementType);

        List<object> items = [];

        // Enumerate all indexed prefixes (e.g. "Addresses[0]", "Addresses[1]").
        foreach (string elementPrefix in
            context.EnumerateIndexedPrefixes(prefix, elementType))
        {
            // Create a binding context for each array element.
            DefaultModelBindingContext elementContext =
                context.CreateBindingContext(metadata, elementPrefix);

            await binder.BindModelAsync(elementContext);

            if (elementContext.Result.IsModelSet)
            {
                items.Add(elementContext.Result.Model!);
            }
        }

        // If no elements were bound, return false.
        if (items.Count == 0)
        {
            return false;
        }

        // Build the final array and assign it to the model.
        Array array = Array.CreateInstance(elementType, items.Count);
        items.CopyTo((object[]) array, 0);
        property.SetValue(model, array);

        return true;
    }

    /// <summary>
    /// Binds a nested complex type using the default model binder.
    /// </summary>
    private async Task<bool> BindNestedAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Build the prefix for this complex property (e.g. "School.Address").
        string prefix =
            context.BuildPropertyPrefix(property);

        ModelMetadata metadata =
            _metadataProvider
                .GetMetadataForType(property.PropertyType);

        IModelBinder binder =
            context.CreateBinder(property.PropertyType);

        // Create a binding context for the nested object.
        DefaultModelBindingContext nestedContext =
            context.CreateBindingContext(metadata, prefix);

        await binder.BindModelAsync(nestedContext);

        // Resolve the nested model instance (existing or newly created).
        object nestedModel =
            property.ResolveNestedModel(nestedContext);

        if (nestedModel == null)
        {
            return false;
        }

        // Bind each child property of the nested object.
        foreach (PropertyInfo childProp in
            property.PropertyType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance))
        {
            ModelMetadata childMetadata =
                _metadataProvider
                    .GetMetadataForType(childProp.PropertyType);

            // Create a new binding context for each child property.
            DefaultModelBindingContext childContext =
                context.CreateBindingContext(childMetadata, prefix);

            // Recursively bind the child property.
            await HandleAsync(childContext, nestedModel, childProp);
        }

        // Assign the fully bound nested model back to the parent.
        property.SetValue(model, nestedModel);
        return true;
    }
}
