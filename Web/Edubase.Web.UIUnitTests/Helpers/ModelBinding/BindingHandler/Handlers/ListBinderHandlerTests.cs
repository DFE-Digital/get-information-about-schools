using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers;

public class ListBinderHandlerTests
{
    private readonly Mock<ITypeConverter> _converterMock;
    private readonly ListBinderHandler _handler;

    public ListBinderHandlerTests()
    {
        _converterMock = TypeConverterTestDoubles.Default();
        _handler = new ListBinderHandler(_converterMock.Object);
    }

    private class TestModel
    {
        public List<int> Numbers { get; set; }
        public string Name { get; set; }
    }

    [Fact]
    public async Task HandleAsync_ShouldDelegate_WhenPropertyIsNotList()
    {
        // Arrange
        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "Name", new[] { "John" } }
                });

        PropertyInfo? property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Name));

        TestModel model = new TestModel();

        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.False(result);
        _converterMock.Verify(typeConverter =>
            typeConverter.Convert(
                It.IsAny<string>(),
                It.IsAny<Type>()),
                Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldBindList_WhenValuesExist()
    {
        // Arrange
        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "Numbers", new[] { "1", "2", "3" } }
                });

        PropertyInfo? property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Numbers));

        TestModel model = new TestModel();

        Mock<ITypeConverter> converter =
            TypeConverterTestDoubles.MockFor(
                "1,2,3", typeof(List<int>), new List<int> { 1, 2, 3 });

        ListBinderHandler handler =
            new ListBinderHandler(converter.Object);

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(new List<int> { 1, 2, 3 }, model.Numbers);
        converter.Verify();
    }

    [Fact]
    public async Task HandleAsync_ShouldDelegate_WhenNoValuesFound()
    {
        // Arrange
        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "Numbers", Array.Empty<string>() }
                });

        PropertyInfo? property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Numbers));

        TestModel model = new TestModel();

        // Act
        bool result =
            await _handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.False(result);
        Assert.Null(model.Numbers);
        _converterMock.Verify(typeConverter =>
            typeConverter.Convert(
                It.IsAny<string>(),
                It.IsAny<Type>()),
                Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldFail_WhenConversionThrows()
    {
        // Arrange
        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "Numbers", new[] { "bad" } }
                });

        PropertyInfo? property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Numbers));

        TestModel model = new TestModel();

        Mock<ITypeConverter> converter =
            TypeConverterTestDoubles.Throws(
                "bad", typeof(List<int>), new FormatException());

        ListBinderHandler handler =
            new ListBinderHandler(converter.Object);

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.False(result);
        Assert.Equal(ModelBindingResult.Failed(), context.Result);
        Assert.Null(model.Numbers);
    }
}
