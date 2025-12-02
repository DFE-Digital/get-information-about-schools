using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.TestDoubles;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;
using PropertyBinderHandlerTestDoubles =
    Edubase.Web.UIUnitTests.Helpers.ModelBinding.TestDoubles.PropertyBinderHandlerTestDoubles;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding;

public class DefaultModelBinderTests
{
    public class ComplexModel
    {
        public Names Name { get; set; }
        public int Age { get; set; }

        public class Names
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }

    public class SimpleModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Fact]
    public void Constructor_NoHandlers_ThrowsInvalidOperationException()
    {
        // Arrange
        IEnumerable<IPropertyBinderHandler> handlers = [];
        ITypeFactory factory =
            new TypeFactoryTestDoubles(type => new object());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            new DefaultModelBinder(handlers, factory));
    }

    [Fact]
    public async Task BindModelAsync_SimpleType_DelegatesToHandler()
    {
        // Arrange
        PropertyBinderHandlerTestDoubles handler = new();

        ITypeFactory factory =
            new TypeFactoryTestDoubles(type =>
                Activator.CreateInstance(type));

        DefaultModelBinder binder = new([handler], factory);

        DefaultModelBindingContext context = new()
        {
            ModelMetadata =
                new EmptyModelMetadataProvider()
                    .GetMetadataForType(typeof(SimpleModel)),

            ModelName = "simple",

            ValueProvider =
                new ValueProviderTestDoubles
                    .FakeValueProvider([])
        };

        // Act
        await binder.BindModelAsync(context);

        // Assert
        Assert.True(context.Result.IsModelSet);
        Assert.IsType<SimpleModel>(context.Result.Model);
        Assert.True(handler.WasCalled);
        Assert.NotNull(handler.LastProperty);
    }

    [Fact]
    public async Task BindModelAsync_ComplexType_CreatesInstanceAndCallsHandler()
    {
        // Arrange
        PropertyBinderHandlerTestDoubles handler = new();

        ITypeFactory factory =
            new TypeFactoryTestDoubles(type =>
                Activator.CreateInstance(type));

        DefaultModelBinder binder = new([handler], factory);

        DefaultModelBindingContext context = new()
        {
            ModelMetadata =
                new EmptyModelMetadataProvider()
                    .GetMetadataForType(typeof(ComplexModel)),

            ModelName = "complex",

            ValueProvider =
                new ValueProviderTestDoubles
                    .FakeValueProvider([])
        };

        // Act
        await binder.BindModelAsync(context);

        // Assert
        Assert.True(context.Result.IsModelSet);
        Assert.IsType<ComplexModel>(context.Result.Model);
        Assert.True(handler.WasCalled);
        Assert.NotNull(handler.LastProperty);
    }

    [Fact]
    public async Task BindModelAsync_FactoryThrows_SetsFailedResult()
    {
        // Arrange
        PropertyBinderHandlerTestDoubles handler = new();

        ITypeFactory factory =
            new TypeFactoryTestDoubles(type =>
                throw new Exception("fail"));

        DefaultModelBinder binder = new([handler], factory);

        DefaultModelBindingContext context = new()
        {
            ModelMetadata =
                new EmptyModelMetadataProvider()
                    .GetMetadataForType(typeof(ComplexModel)),

            ModelName = "complex",

            ValueProvider =
                new ValueProviderTestDoubles
                    .FakeValueProvider([])
        };

        // Act
        await binder.BindModelAsync(context);

        // Assert
        Assert.False(context.Result.IsModelSet);
    }

    [Fact]
    public async Task BindModelAsync_NullContext_ThrowsArgumentNullException()
    {
        // Arrange
        PropertyBinderHandlerTestDoubles handler = new();

        ITypeFactory factory =
            new TypeFactoryTestDoubles(type => new object());

        DefaultModelBinder binder = new([handler], factory);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            binder.BindModelAsync(null));
    }
}
