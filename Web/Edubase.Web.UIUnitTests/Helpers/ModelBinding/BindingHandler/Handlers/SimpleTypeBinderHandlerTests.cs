using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler;

public class SimpleTypeBinderHandlerTests
{
    private class TestModel
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public Guid Identifier { get; set; }
        public DateTime BirthDate { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan Duration { get; set; }
        public ComplexType Complex { get; set; }
    }

    private class ComplexType
    {
        public string Value { get; set; }
    }

    [Fact]
    public async Task HandleAsync_ShouldBindIntProperty_WhenValueExists()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
            .StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "Age", new[] { "42" } }
                });

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Age));

        TestModel model = new();

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(42, model.Age);
    }

    [Fact]
    public async Task HandleAsync_ShouldBindStringProperty_WhenValueExists()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<TestModel>(
                    new Dictionary<string, string[]>
                    {
                        { "Name", new[] { "Alice" } }
                    });

        PropertyInfo property =
            typeof(TestModel).GetProperty(nameof(TestModel.Name));

        TestModel model = new();

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal("Alice", model.Name);
    }

    [Fact]
    public async Task HandleAsync_ShouldBindGuidProperty_WhenValueExists()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<TestModel>(
                    new Dictionary<string, string[]>
                    {
                        { "Identifier", new[] { guid.ToString() } }
                    });

        PropertyInfo property =
            typeof(TestModel).GetProperty(nameof(TestModel.Identifier));

        TestModel model = new();

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(guid, model.Identifier);
    }

    [Fact]
    public async Task HandleAsync_ShouldBindEnumProperty_WhenValueExists()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<TestModel>(
                    new Dictionary<string, string[]>
                    {
                        { "Day", new[] { "Friday" } }
                    });

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Day));

        TestModel model = new();

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(DayOfWeek.Friday, model.Day);
    }

    [Fact]
    public async Task HandleAsync_ShouldFail_WhenConversionThrows()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<TestModel>(
                    new Dictionary<string, string[]>
                    {
                        { "Age", new[] { "bad" } }
                    });

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Age));

        TestModel model = new();

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.False(result);
        Assert.Equal(ModelBindingResult.Failed(), context.Result);
    }

    [Fact]
    public async Task HandleAsync_ShouldDelegate_WhenPropertyIsNotSimpleType()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<TestModel>(
                    new Dictionary<string, string[]>
                    {
                        { "Complex.Value", new[] { "X" } }
                    });

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Complex));

        TestModel model = new();

        Mock<IPropertyBinderHandler> nextHandler =
            PropertyBinderHandlerTestDoubles
                .AlwaysTrue(context, model, property);

        SimpleTypeBinderHandler handler = new();
        handler.SetNext(nextHandler.Object);

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        nextHandler.Verify(handler =>
            handler.HandleAsync(context, model, property), Times.Once);
    }

    [Fact]
    public async Task BindModelAsync_ShouldBindTopLevelIntModel()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<int>(
                    new Dictionary<string, string[]>
                    {
                        { "", new[] { "123" } }
                    });

        context.ModelName = "";
        context.ModelMetadata =
            new EmptyModelMetadataProvider()
                .GetMetadataForType(typeof(int));

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .BindModelAsync(context);

        // Assert
        Assert.True(result);
        Assert.Equal(ModelBindingResult.Success(123), context.Result);
    }

    [Fact]
    public async Task BindModelAsync_ShouldFail_WhenNoValueFound()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<int>(
                    new Dictionary<string, string[]>());

        context.ModelName = "Age";
        context.ModelMetadata =
            new EmptyModelMetadataProvider()
                .GetMetadataForType(typeof(int));

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .BindModelAsync(context);

        // Assert
        Assert.True(result);
        Assert.Equal(ModelBindingResult.Failed(), context.Result);
    }

    [Fact]
    public async Task BindModelAsync_ShouldFail_WhenConversionThrows()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<int>(
                    new Dictionary<string, string[]>
                    {
                        { "Age", new[] { "bad" } }
                    });

        context.ModelName = "Age";
        context.ModelMetadata =
            new EmptyModelMetadataProvider()
                .GetMetadataForType(typeof(int));

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .BindModelAsync(context);

        // Assert
        Assert.True(result);
        Assert.Equal(ModelBindingResult.Failed(), context.Result);
    }

    [Fact]
    public async Task BindModelAsync_ShouldReturnFalse_WhenTypeIsNotSimple()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .StubForDictionary<TestModel>([]);

        context.ModelName = "Complex";
        context.ModelMetadata =
            new EmptyModelMetadataProvider()
                .GetMetadataForType(typeof(TestModel));

        SimpleTypeBinderHandler handler = new();

        // Act
        bool result =
            await handler
                .BindModelAsync(context);

        // Assert
        Assert.False(result);
    }
}
