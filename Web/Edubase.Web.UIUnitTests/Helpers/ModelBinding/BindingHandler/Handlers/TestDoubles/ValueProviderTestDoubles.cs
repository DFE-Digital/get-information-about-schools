using System.Collections.Generic;
using System.Globalization;
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
}
