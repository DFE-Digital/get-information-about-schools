using System;

namespace Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;

/// <summary>
/// Defines a contract for converting string values into strongly typed objects.
/// This interface is typically used in scenarios such as model binding, configuration parsing,
/// or data transformation where input values are received as strings and need to be converted
/// into specific .NET types.
/// </summary>
public interface ITypeConverter
{
    /// <summary>
    /// Converts a string representation of a value into an object of the specified target type.
    /// </summary>
    /// <param name="value">
    /// The input string to convert. This may come from user input, query parameters, form fields,
    /// configuration files, or other external sources.
    /// </param>
    /// <param name="targetType">
    /// The type to convert the input string to. This can be a primitive type, enum, nullable type,
    /// or any other type supported by the converter implementation.
    /// </param>
    /// <returns>
    /// An object of the specified target type, or <c>null</c> if the input is null or empty and the target type allows nulls.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when the input string is not in a valid format for the target type.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when the conversion cannot be performed due to an unsupported target type or incompatible input.
    /// </exception>
    object Convert(string value, Type targetType);
}
