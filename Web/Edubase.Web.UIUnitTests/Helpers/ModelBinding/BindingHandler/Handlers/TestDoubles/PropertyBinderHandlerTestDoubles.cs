using System.Reflection;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable test doubles for <see cref="IPropertyBinderHandler"/>.
/// These helpers simplify unit test setup by creating mocks with common behaviors.
/// </summary>
internal static class PropertyBinderHandlerTestDoubles
{
    /// <summary>
    /// Creates a mock <see cref="IPropertyBinderHandler"/> that will return <c>true</c>
    /// when <see cref="IPropertyBinderHandler.HandleAsync"/> is called with the given context,
    /// model, and property.
    /// </summary>
    internal static Mock<IPropertyBinderHandler> AlwaysTrue(
        ModelBindingContext context,
        object model,
        PropertyInfo property) =>
            CreateMock(context, model, property, true);

    /// <summary>
    /// Creates a mock <see cref="IPropertyBinderHandler"/> that will return <c>false</c>
    /// when <see cref="IPropertyBinderHandler.HandleAsync"/> is called with the given context,
    /// model, and property.
    /// </summary>
    internal static Mock<IPropertyBinderHandler> AlwaysFalse(
        ModelBindingContext context,
        object model,
        PropertyInfo property) =>
            CreateMock(context, model, property, false);

    /// <summary>
    /// Consolidated private helper to configure the mock with the desired return value.
    /// </summary>
    private static Mock<IPropertyBinderHandler> CreateMock(
        ModelBindingContext context,
        object model,
        PropertyInfo property,
        bool returnValue)
    {
        Mock<IPropertyBinderHandler> mock = new();
        mock.Setup(handler =>
            handler.HandleAsync(context, model, property))
                .ReturnsAsync(returnValue)
                .Verifiable();

        return mock;
    }
}
