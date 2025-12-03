using System;
using System.Collections.Generic;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.TypeConverters.TestDoubles;

/// <summary>
/// Provides reusable test doubles for <see cref="IValueConverter"/>.
/// These helpers simplify unit test setup by offering fake implementations
/// that use delegate dictionaries for predictable conversions.
/// </summary>
internal static class ValueConverterTestDoubles
{
    /// <summary>
    /// A fake implementation of <see cref="IValueConverter"/> for unit testing.
    /// Uses a dictionary of delegates to convert strings into strongly typed values.
    /// </summary>
    internal sealed class FakeValueConverterDouble : IValueConverter
    {
        private readonly Dictionary<Type, Func<string, object>> _converters;

        /// <summary>
        /// Initializes the fake converter with common type mappings.
        /// </summary>
        public FakeValueConverterDouble()
        {
            _converters = new Dictionary<Type, Func<string, object>>
            {
                { typeof(int), str => int.Parse(str) },
                { typeof(string), str => str },
                { typeof(DayOfWeek), str => Enum.Parse(typeof(DayOfWeek), str) },
                { typeof(Guid), str => Guid.Parse(str) },
                { typeof(double), str => double.Parse(str) },
                { typeof(DateTime), str => DateTime.Parse(str) }
            };
        }

        /// <summary>
        /// Converts the given string into the specified target type.
        /// Throws <see cref="InvalidCastException"/> if the type is null or an open generic.
        /// Falls back to <see cref="Activator.CreateInstance(Type)"/> if no converter is registered.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <returns>An object of the target type.</returns>
        public object Convert(string value, Type targetType)
        {
            return targetType == null || targetType.ContainsGenericParameters
                ? throw new InvalidCastException(
                    $"Cannot determine element type for {targetType?.Name ?? "null"}")
                : _converters.TryGetValue(targetType, out var converter)
                ? converter(value)
                : Activator.CreateInstance(targetType);
        }
    }
}
