using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable test doubles for <see cref="IModelBinder"/>.
/// Includes both Moq-based helpers and a simple concrete binder.
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
        CreateBinder(ctx => ctx.Result = ModelBindingResult.Success(model));

    /// <summary>
    /// Creates a mock IModelBinder that will fail binding.
    /// </summary>
    internal static Mock<IModelBinder> Failure() =>
        CreateBinder(ctx => ctx.Result = ModelBindingResult.Failed());

    /// <summary>
    /// Creates a mock IModelBinder that throws the given exception
    /// when BindModelAsync is called.
    /// </summary>
    internal static Mock<IModelBinder> Throws(Exception exception)
    {
        var mock = Default();
        mock.Setup(binder => binder.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Throws(exception);
        return mock;
    }

    /// <summary>
    /// Centralized binder creation logic to avoid repetition.
    /// Accepts a configuration action to set the binding result.
    /// </summary>
    private static Mock<IModelBinder> CreateBinder(Action<ModelBindingContext> configure)
    {
        var mock = Default();
        mock.Setup(binder => binder.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Callback(configure)
            .Returns(Task.CompletedTask);
        return mock;
    }


    /// <summary>
    /// A simple test double implementation of <see cref="IModelBinder"/>.
    /// Creates an instance of the specified type and, if a "Name" property exists,
    /// attempts to populate it from the value provider.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FakeModelBinder"/> class.
    /// </remarks>
    /// <param name="modelType">
    /// The <see cref="Type"/> of the model to be created and bound.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="modelType"/> is <c>null</c>.
    /// </exception>
    internal class FakeModelBinder(Type modelType) : IModelBinder
    {
        private readonly Type _modelType = modelType ??
            throw new ArgumentNullException(nameof(modelType));

        /// <summary>
        /// Binds a model instance of the configured type to the given <see cref="ModelBindingContext"/>.
        /// </summary>
        /// <param name="bindingContext">
        /// The context for the model binding operation, which provides access to
        /// metadata, value providers, and the binding result.
        /// </param>
        /// <returns>
        /// A completed <see cref="Task"/> once binding is finished. The
        /// <see cref="ModelBindingContext.Result"/> will be set to
        /// <see cref="ModelBindingResult.Success(object)"/> with the created instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="bindingContext"/> is <c>null</c>.
        /// </exception>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            object instance =
                _modelType.IsArray
                    ? Array.CreateInstance(_modelType.GetElementType()!, 0)
                    : CreateAndPopulateInstance(bindingContext);

            bindingContext.Result = ModelBindingResult.Success(instance);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates an instance of the configured type and attempts to populate
        /// its "Name" property from the value provider, if such a property exists.
        /// </summary>
        /// <param name="bindingContext">
        /// The context used to retrieve values for binding.
        /// </param>
        /// <returns>
        /// A new instance of the configured type with its "Name" property set
        /// if a matching value is found in the value provider.
        /// </returns>
        private object CreateAndPopulateInstance(ModelBindingContext bindingContext)
        {
            object instance = Activator.CreateInstance(_modelType)!;

            PropertyInfo? nameProp = _modelType.GetProperty("Name");

            if (nameProp != null)
            {
                ValueProviderResult result =
                    bindingContext.ValueProvider
                        .GetValue(bindingContext.ModelName + ".Name");

                if (result != ValueProviderResult.None)
                {
                    nameProp.SetValue(instance, result.FirstValue);
                }
            }

            return instance;
        }
    }
}
