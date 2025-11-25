using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;

/// <summary>
/// Provides extension methods to simplify common string manipulation and value lookups
/// used during model binding.
/// </summary>
public static class ModelBindingExtensions
{
    /// <summary>
    /// Builds a property prefix from the current model name and property.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="property">The property for which to build the prefix.</param>
    /// <returns>
    /// A string representing the property prefix, e.g. "Parent.PropertyName" or just "PropertyName"
    /// if the model name is empty.
    /// </returns>
    public static string BuildPropertyPrefix(
        this ModelBindingContext context, PropertyInfo property) =>
            string.IsNullOrEmpty(context.ModelName)
                ? property.Name
                : $"{context.ModelName}.{property.Name}";

    /// <summary>
    /// Builds an element prefix for an array or list index.
    /// </summary>
    /// <param name="propertyPrefix">The property prefix of the collection.</param>
    /// <param name="index">The index of the element.</param>
    /// <returns>
    /// A string representing the element prefix, e.g. "CollectionProperty[0]".
    /// </returns>
    public static string BuildElementPrefix(
        this string propertyPrefix, int index) =>  $"{propertyPrefix}[{index}]";

    /// <summary>
    /// Builds a key for a property of an element at a given index.
    /// </summary>
    /// <param name="elementPrefix">The prefix for the element (e.g. "CollectionProperty[0]").</param>
    /// <param name="property">The property of the element.</param>
    /// <returns>
    /// A string representing the full key, e.g. "CollectionProperty[0].PropertyName".
    /// </returns>
    public static string BuildElementPropertyKey(
        this string elementPrefix, PropertyInfo property) => $"{elementPrefix}.{property.Name}";

    /// <summary>
    /// Checks whether the value provider has a non-empty value for the given key.
    /// </summary>
    /// <param name="provider">The value provider to query.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// <c>true</c> if the value provider contains a non-empty value for the key; otherwise <c>false</c>.
    /// </returns>
    public static bool HasValue(
        this IValueProvider provider, string key)
    {
        ValueProviderResult result = provider.GetValue(key);
        return provider.GetValue(key) != ValueProviderResult.None &&
            !string.IsNullOrEmpty(result.FirstValue);
    }

    /// <summary>
    /// Retrieves all <see cref="BindAliasAttribute"/> instances defined on a property.
    /// </summary>
    public static BindAliasAttribute[] GetBindAliases(
        this PropertyInfo property) =>
            [.. property.GetCustomAttributes<BindAliasAttribute>(true)];

    /// <summary>
    /// Determines whether the value provider result contains any non-empty values.
    /// </summary>
    public static bool HasValues(
        this ValueProviderResult result) =>
            result != ValueProviderResult.None &&
            result.Values.Any(str => !string.IsNullOrEmpty(str));

    /// <summary>
    /// Normalises a value provider result into a single comma-separated string.
    /// </summary>
    public static string ToCombinedString(
        this ValueProviderResult result) => string.Join(",", result.Values);
}
