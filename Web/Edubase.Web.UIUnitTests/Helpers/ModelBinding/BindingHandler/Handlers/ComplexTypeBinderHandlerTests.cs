using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers;

public class ComplexTypeBinderHandlerTests
{
    private readonly ComplexTypeBinderHandler _handler;

    public ComplexTypeBinderHandlerTests()
    {
        _handler =
            new ComplexTypeBinderHandler(
                new EmptyModelMetadataProvider());
    }

    [Fact]
    public async Task HandleAsync_SimpleTypeProperty_DelegatesToBase()
    {
        // Arrange
        TestModel model = new();

        PropertyInfo property =
            typeof(TestModel).GetProperty(nameof(TestModel.SimpleProp))!;

        ModelBindingContext context =
            ModelBindingContextTestDoubles
                .CreateBindingContext(
                    modelName: "TestModel",
                    services: BuildServiceProvider(),
                    valueProvider: new TestValueProvider([]));

        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HandleAsync_ArrayOfComplexTypes_BindsSuccessfully()
    {
        // Arrange
        TestModel model = new();

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.ComplexArray))!;

        Dictionary<string, string> values = new()
        {
            { "TestModel.ComplexArray[0].Name", "Item1" },
            { "TestModel.ComplexArray[1].Name", "Item2" }
        };

        ModelBindingContext context =
            ModelBindingContextTestDoubles
                .CreateBindingContext(
                    modelName: "TestModel",
                    services: BuildServiceProvider(),
                    valueProvider: new TestValueProvider(values));

        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.NotNull(model.ComplexArray);
        Assert.Equal(2, model.ComplexArray.Length);
        Assert.Equal("Item1", model.ComplexArray[0].Name);
        Assert.Equal("Item2", model.ComplexArray[1].Name);
    }

    [Fact]
    public async Task HandleAsync_ArrayOfComplexTypes_NoValues_ReturnsTrueButEmpty()
    {
        // Arrange
        TestModel model = new();

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.ComplexArray))!;

        ModelBindingContext context =
            ModelBindingContextTestDoubles
                .CreateBindingContext(
                    modelName: "TestModel",
                    services: BuildServiceProvider(),
                    valueProvider: new TestValueProvider([]));
        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.NotNull(model.ComplexArray);
        Assert.Empty(model.ComplexArray);
    }

    [Fact]
    public async Task HandleAsync_NestedComplexType_BindsSuccessfully()
    {
        // Arrange
        TestModel model = new();

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Nested))!;

        Dictionary<string, string> values = new()
        {
            { "TestModel.Nested.Name", "NestedItem" }
        };

        ModelBindingContext context =
            ModelBindingContextTestDoubles
                .CreateBindingContext(
                    modelName: "TestModel",
                    services: BuildServiceProvider(),
                    valueProvider: new TestValueProvider(values));

        // Act
        bool result = await _handler.HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.NotNull(model.Nested);
        Assert.Equal("NestedItem", model.Nested.Name);
    }

    [Fact]
    public async Task HandleAsync_NestedComplexType_NoValues_ReturnsTrueButNullPropertyValue()
    {
        // Arrange
        TestModel model = new();

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Nested))!;

        ModelBindingContext context =
            ModelBindingContextTestDoubles
                .CreateBindingContext(
                    modelName: "TestModel",
                    services: BuildServiceProvider(),
                    valueProvider: new TestValueProvider([]));

        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.NotNull(model.Nested);
        Assert.Null(model.Nested.Name);
    }

    [Fact]
    public async Task HandleAsync_NestedComplexType_WrongTypeFromBinder_FallsBackToBase()
    {
        // Arrange
        TestModel model = new();

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Nested))!;

        ServiceCollection services = new();
        services.AddSingleton<IModelBinderFactory, AlwaysWrongBinderFactory>();
        services.AddSingleton<IModelMetadataProvider, EmptyModelMetadataProvider>();
        IServiceProvider provider = services.BuildServiceProvider();

        ModelBindingContext context =
            ModelBindingContextTestDoubles
                .CreateBindingContext(
                    modelName: "TestModel",
                    services: provider,
                    valueProvider: new TestValueProvider([]));

        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.False(result);
        Assert.Null(model.Nested);
    }

    private static IServiceProvider BuildServiceProvider()
    {
        ServiceCollection services = new();
        services.AddSingleton<IModelBinderFactory, FakeModelBinderFactory>();
        services.AddSingleton<IModelMetadataProvider, EmptyModelMetadataProvider>();
        return services.BuildServiceProvider();
    }

    // Test model classes
    public class TestModel
    {
        public int SimpleProp { get; set; }
        public ComplexType[] ComplexArray { get; set; }
        public ComplexType Nested { get; set; }
    }

    public class ComplexType
    {
        public string Name { get; set; }
    }
}

// Minimal ActionContext mock
internal class ActionContextMock : ActionContext
{
    public ActionContextMock(IServiceProvider services)
    {
        HttpContext = new DefaultHttpContext { RequestServices = services };
    }
}

// Fake binder factory that always returns a binder which sets models from value provider
internal class FakeModelBinderFactory : IModelBinderFactory
{
    public IModelBinder CreateBinder(ModelBinderFactoryContext context)
    {
        return new FakeModelBinder(context.Metadata.ModelType);
    }
}

internal class FakeModelBinder : IModelBinder
{
    private readonly Type _type;

    public FakeModelBinder(Type type)
    {
        _type = type;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        object instance;

        if (_type.IsArray)
        {
            instance = Array.CreateInstance(_type.GetElementType()!, 0);
        }
        else
        {
            instance = Activator.CreateInstance(_type)!;

            PropertyInfo? nameProp = _type.GetProperty("Name");
            if (nameProp != null)
            {
                ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Name");
                if (result != ValueProviderResult.None)
                {
                    nameProp.SetValue(instance, result.FirstValue);
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(instance);
        return Task.CompletedTask;
    }
}

internal class AlwaysWrongBinderFactory : IModelBinderFactory
{
    public IModelBinder CreateBinder(ModelBinderFactoryContext context)
    {
        return new WrongTypeBinder();
    }
}

internal class WrongTypeBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        bindingContext.Result = ModelBindingResult.Success("not a complex type");
        return Task.CompletedTask;
    }
}

// Simple test value provider
internal class TestValueProvider : IValueProvider
{
    private readonly Dictionary<string, string> _values;

    public TestValueProvider(Dictionary<string, string> values)
    {
        _values = values;
    }

    public bool ContainsPrefix(string prefix) =>
        _values.Keys.Any(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

    public ValueProviderResult GetValue(string key)
    {
        return _values.TryGetValue(key, out string? value)
            ? new ValueProviderResult(value)
            : ValueProviderResult.None;
    }

    public bool HasValue(string key) => _values.ContainsKey(key);
}
