using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable test doubles for IValueProvider.
/// These helpers simplify unit test setup by creating mocks
/// with common behaviors (default or keyed response).
/// </summary>
internal static class ValueProviderTestDoubles
{
    /// <summary>
    /// Returns a blank mock of IValueProvider with no setup.
    /// Useful when you want to configure behavior manually in a test.
    /// </summary>
    internal static Mock<IValueProvider> Default() => new();

    /// <summary>
    /// Creates a mock IValueProvider that will return a specific ValueProviderResult
    /// when GetValue is called with the given key.
    /// </summary>
    /// <param name="key">The key to look up in the value provider (e.g., alias name).</param>
    /// <param name="response">The ValueProviderResult to return for that key.</param>
    /// <returns>A configured mock IValueProvider.</returns>
    internal static Mock<IValueProvider> MockFor(string key, ValueProviderResult response)
    {
        Mock<IValueProvider> mock = Default();

        // Setup the mock so that when GetValue(key) is called,
        // it returns the provided response.
        mock.Setup(
            valueProvider =>
                valueProvider.GetValue(key))
            .Returns(response)
            .Verifiable(); // marks this setup as verifiable in tests

        return mock;
    }
}
