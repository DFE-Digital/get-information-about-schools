using System;
using System.Linq;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;

namespace Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;

/// <summary>
/// Provides a default implementation of <see cref="ITypeConverter"/> for converting string values
/// into strongly typed .NET objects. Supports primitive types, enums, and collections.
/// </summary>
public class DefaultTypeConverter : ITypeConverter
{
    private readonly IValueConverter _valueConverter;
    private readonly ICollectionFactory _collectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTypeConverter"/> class.
    /// </summary>
    /// <param name="valueConverter">
    /// A value converter used to convert individual string values to specific types.
    /// </param>
    public DefaultTypeConverter(
        IValueConverter valueConverter,
        ICollectionFactory collectionFactory)
    {
        _valueConverter = valueConverter;
        _collectionFactory = collectionFactory;
    }

    /// <summary>
    /// Converts a string value into an object of the specified target type.
    /// Handles both simple types and collections.
    /// </summary>
    /// <param name="value">The input string to convert.</param>
    /// <param name="targetType">The target type to convert to.</param>
    /// <returns>
    /// An object of the target type, or <c>null</c> if the input is null or whitespace.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Thrown when the target type is a collection but its element type cannot be determined.
    /// </exception>
    public object Convert(string value, Type targetType)
    {
        // Return null for empty or whitespace input
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // Unwrap nullable types to get the actual underlying type
        Type underlyingType =
            Nullable.GetUnderlyingType(targetType) ?? targetType;

        // If the type is not a collection, delegate to the value converter
        if (!underlyingType.IsCollectionType())
        {
            return _valueConverter.Convert(value, underlyingType);
        }

        // Determine the element type of the collection
        Type elementType =
            underlyingType.GetElementType()
                ?? throw new InvalidCastException(
                    $"Cannot determine element type for {targetType.Name}");

        // Parse and convert the collection items
        object[] items = ParseCollectionItems(value, elementType);

        // Create and return the populated collection instance
        return _collectionFactory.CreateListInstance(underlyingType, elementType, items);
    }

    /// <summary>
    /// Parses a string into an array of converted items based on the element type.
    /// </summary>
    /// <param name="value">The input string representing one or more items.</param>
    /// <param name="elementType">The type of each item in the collection.</param>
    /// <returns>An array of converted items.</returns>
    private object[] ParseCollectionItems(string value, Type elementType)
    {
        // If the element type is primitive or enum, split and convert each segment
        if (elementType.IsPrimitiveType() || elementType.IsEnum)
        {
            return [.. value
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(str =>
                {
                    try
                    {
                        return _valueConverter.Convert(str.Trim(), elementType);
                    }
                    catch
                    {
                        // Skip invalid entries
                        return null;
                    }
                })
                .Where(obj => obj != null)];
        }

        // For non-primitive types, treat the entire string as a single item
        return [_valueConverter.Convert(value, elementType)];
    }
}
