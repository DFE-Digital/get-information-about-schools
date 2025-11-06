using System;

namespace Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;

/// <summary>
/// Defines a contract for converting string values into strongly typed objects.
/// This is typically used in model binding scenarios where input values from query strings,
/// form data, or route parameters need to be converted to specific .NET types.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// Converts a string representation of a value into an object of the specified target type.
    /// </summary>
    /// <param name="value">
    /// The input string value to convert. This may come from user input, query parameters, or form fields.
    /// </param>
    /// <param name="targetType">
    /// The type to convert the input value to. This can be a primitive type, enum, or complex type.
    /// </param>
    /// <returns>
    /// An object of the specified target type, or <c>null</c> if the input is null or empty and the target type allows nulls.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when the input string is not in a valid format for the target type.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when the conversion cannot be performed due to an unsupported target type.
    /// </exception>
    object Convert(string value, Type targetType);
}
