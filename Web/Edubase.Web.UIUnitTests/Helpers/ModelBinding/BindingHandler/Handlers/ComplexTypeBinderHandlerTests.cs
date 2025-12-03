using System;
using System.Collections.Generic;
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
                    valueProvider: new ValueProviderTestDoubles.FakeValueProvider([]));

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
                    valueProvider: new ValueProviderTestDoubles.FakeValueProvider(values));

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
                    valueProvider: new ValueProviderTestDoubles.FakeValueProvider([]));
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
                    valueProvider: new ValueProviderTestDoubles.FakeValueProvider(values));

        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

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
                    valueProvider: new ValueProviderTestDoubles.FakeValueProvider([]));

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
                    valueProvider: new ValueProviderTestDoubles.FakeValueProvider([]));

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

internal class ActionContextMock : ActionContext
{
    public ActionContextMock(IServiceProvider services)
    {
        HttpContext = new DefaultHttpContext { RequestServices = services };
    }
}

internal class FakeModelBinderFactory : IModelBinderFactory
{
    public IModelBinder CreateBinder(ModelBinderFactoryContext context) =>
        new ModelBinderTestDoubles.FakeModelBinder(context.Metadata.ModelType);
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
