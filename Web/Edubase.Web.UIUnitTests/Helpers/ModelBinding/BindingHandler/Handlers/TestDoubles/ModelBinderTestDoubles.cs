using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable test doubles for <see cref="IModelBinder"/>.
/// These helpers simplify unit test setup by creating mocks
/// with common behaviors (success, failure, or throwing).
/// </summary>
internal static class ModelBinderTestDoubles
{
    /// <summary>
    /// Returns a blank mock of IModelBinder with no setup.
    /// Useful when you want to configure behavior manually in a test.
    /// </summary>
    internal static Mock<IModelBinder> Default() => new();

    /// <summary>
    /// Creates a mock IModelBinder that will succeed with the given model
    /// whenever BindModelAsync is called.
    /// </summary>
    internal static Mock<IModelBinder> Success(object model) =>
        CreateBinder(modelBindingContext =>
            modelBindingContext.Result = ModelBindingResult.Success(model));

    /// <summary>
    /// Creates a mock IModelBinder that will fail binding.
    /// </summary>
    internal static Mock<IModelBinder> Failure() =>
        CreateBinder(modelBindingContext =>
            modelBindingContext.Result = ModelBindingResult.Failed());

    /// <summary>
    /// Creates a mock IModelBinder that throws the given exception
    /// when BindModelAsync is called.
    /// </summary>
    internal static Mock<IModelBinder> Throws(Exception exception)
    {
        Mock<IModelBinder> mock = Default();
        mock.Setup(modelBinder =>
            modelBinder.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Throws(exception);
        return mock;
    }

    /// <summary>
    /// Centralized binder creation logic to avoid repetition.
    /// Accepts a configuration action to set the binding result.
    /// </summary>
    private static Mock<IModelBinder> CreateBinder(Action<ModelBindingContext> configure)
    {
        Mock<IModelBinder> mock = Default();
        mock.Setup(modelBinder =>
            modelBinder.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Callback(configure)
            .Returns(Task.CompletedTask);
        return mock;
    }
}
