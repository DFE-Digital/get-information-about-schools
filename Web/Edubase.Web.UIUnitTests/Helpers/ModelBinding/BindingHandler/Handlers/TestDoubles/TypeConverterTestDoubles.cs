using System;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Moq;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable test doubles for ITypeConverter.
/// These helpers simplify unit test setup by creating mocks
/// with common behaviors (default, specific response, or throwing).
/// </summary>
internal static class TypeConverterTestDoubles
{
    /// <summary>
    /// Returns a blank mock of ITypeConverter with no setup.
    /// Useful when you want to configure behavior manually in a test.
    /// </summary>
    internal static Mock<ITypeConverter> Default() => new();

    /// <summary>
    /// Creates a mock ITypeConverter that will return a specific response
    /// when Convert is called with the given value and target type.
    /// </summary>
    /// <param name="value">The input value expected by the converter.</param>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="response">The object to return when conversion succeeds.</param>
    /// <returns>A configured mock ITypeConverter.</returns>
    internal static Mock<ITypeConverter> MockFor(
        string value, Type targetType, object response)
    {
        Mock<ITypeConverter> mock = Default();
        mock.Setup(
            typeConverter =>
                typeConverter.Convert(value, targetType))
            .Returns(response)   // return the provided response.
            .Verifiable();       // mark this setup as verifiable in tests.

        return mock;
    }

    /// <summary>
    /// Creates a mock ITypeConverter that will throw the given exception
    /// whenever Convert is called with any string and the specified target type.
    /// </summary>
    /// <param name="value">The input value (not used, since It.IsAny is applied).</param>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="exception">The exception to throw when conversion is attempted.</param>
    /// <returns>A configured mock ITypeConverter that throws.</returns>
    internal static Mock<ITypeConverter> Throws(
        string value, Type targetType, Exception exception)
    {
        Mock<ITypeConverter> mock = Default();
        mock.Setup(typeConverter =>
            typeConverter.Convert(
                It.IsAny<string>(), targetType))
            .Throws(exception);   // throw the provided exception.

        return mock;
    }
}
