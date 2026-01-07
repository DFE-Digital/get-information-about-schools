using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="ValueProviderResult"/> instances,
/// including convenience helpers for checking value presence and normalizing values.
/// </summary>
public static class ValueProviderResultExtensions
{
    /// <summary>
    /// Determines whether the <see cref="ValueProviderResult"/> contains any non-empty values.
    /// </summary>
    /// <param name="result">The value provider result to inspect.</param>
    /// <returns>
    /// <c>true</c> if the result contains at least one non-null, non-empty value;
    /// otherwise <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method treats <see cref="ValueProviderResult.None"/> as having no values.
    /// It also filters out empty strings when determining whether values exist.
    /// </remarks>
    public static bool HasValues(this ValueProviderResult result) =>
        result != ValueProviderResult.None &&
        result.Values.Any(str => !string.IsNullOrEmpty(str));

    /// <summary>
    /// Normalizes all values in the <see cref="ValueProviderResult"/> into a single
    /// comma-separated string.
    /// </summary>
    /// <param name="result">The value provider result whose values should be combined.</param>
    /// <returns>
    /// A comma-separated string containing all values from the result.
    /// If no values exist, an empty string is returned.
    /// </returns>
    /// <remarks>
    /// This method does not trim or sanitize individual values; it simply joins them
    /// using a comma delimiter. It is typically used when binding simple list types.
    /// </remarks>
    public static string ToCombinedString(this ValueProviderResult result) =>
        string.Join(",", result.Values);
}
