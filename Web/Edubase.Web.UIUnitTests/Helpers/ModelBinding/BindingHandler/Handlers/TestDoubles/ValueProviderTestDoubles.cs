using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable test doubles for <see cref="IValueProvider"/>.
/// These helpers simplify unit test setup by creating mocks
/// with common behaviors (default, keyed response, or array element values).
/// </summary>
internal static class ValueProviderTestDoubles
{
    /// <summary>
    /// Minimal fake value provider for unit testing.
    /// Accepts a dictionary of key → string[] values.
    /// </summary>
    internal sealed class TestValueProvider(
        Dictionary<string, string[]> values) : IValueProvider
    {
        public ValueProviderResult GetValue(string key) =>
            values.TryGetValue(key, out var vals) && vals.Length > 0
                ? new ValueProviderResult(vals, CultureInfo.InvariantCulture)
                : ValueProviderResult.None;

        public bool ContainsPrefix(string prefix) => values.ContainsKey(prefix);
    }

    /// <summary>
    /// Returns a blank mock of IValueProvider with no setup.
    /// Useful when you want to configure behavior manually in a test.
    /// </summary>
    internal static Mock<IValueProvider> Default() => new();

    /// <summary>
    /// Creates a mock IValueProvider that will return a specific ValueProviderResult
    /// when GetValue is called with the given key.
    /// </summary>
    internal static Mock<IValueProvider> MockFor(
        string key,
        ValueProviderResult response)
    {
        var mock = Default();
        mock.Setup(valueProvider =>
            valueProvider.GetValue(key))
            .Returns(response)
            .Verifiable();

        return mock;
    }

    /// <summary>
    /// Creates a mock IValueProvider that simulates values for array elements,
    /// e.g. "PreviousAddresses[0]" and "PreviousAddresses[0].Street".
    /// </summary>
    internal static Mock<IValueProvider> ArrayElement(
        string propertyPrefix,
        int index,
        string elementProperty,
        string response)
    {
        var mock = Default();
        string key = $"{propertyPrefix}[{index}].{elementProperty}";

        mock.Setup(valueProvider =>
            valueProvider.GetValue(key))
            .Returns(new ValueProviderResult(
                response, CultureInfo.InvariantCulture))
            .Verifiable();

        return mock;
    }

    /// <summary>
    /// Minimal fake implementation of <see cref="IValueProvider"/> for unit testing.
    /// Accepts a dictionary of key → string values and returns them when queried.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TestValueProvider"/> class.
    /// </remarks>
    /// <param name="values">
    /// A dictionary of key → string values to be returned by the provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="values"/> is <c>null</c>.
    /// </exception>
    internal class FakeValueProvider(Dictionary<string, string> values) : IValueProvider
    {
        private readonly Dictionary<string, string> _values = values ??
            throw new ArgumentNullException(nameof(values));

        /// <summary>
        /// Determines whether the provider contains the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to check against the keys.</param>
        /// <returns>
        /// <c>true</c> if any key starts with the given prefix (case-insensitive);
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsPrefix(string prefix) =>
            _values.Keys.Any(str =>
                str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Retrieves the value associated with the given key.
        /// </summary>
        /// <param name="key">The key to look up in the dictionary.</param>
        /// <returns>
        /// A <see cref="ValueProviderResult"/> containing the value if found;
        /// otherwise, <see cref="ValueProviderResult.None"/>.
        /// </returns>
        public ValueProviderResult GetValue(string key) =>
            _values.TryGetValue(key, out string? value)
                ? new ValueProviderResult(value, CultureInfo.InvariantCulture)
                : ValueProviderResult.None;

        /// <summary>
        /// Determines whether the provider contains an exact value for the given key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// <c>true</c> if the dictionary contains the key; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValue(string key) => _values.ContainsKey(key);
    }
}
