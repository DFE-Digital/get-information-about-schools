using System;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers;

public class ArrayBinderHandlerTests
{
    // Centralised constants for reuse
    private const string NamesPropertyKey = "Names";
    private const string AgePropertyKey = "Age";
    private const string CombinedNamesValue = "Alice,Bob";
    private const string SingleBadValue = "BadValue";
    private const string AgeValue = "42";

    private static readonly string[] ResponseArray = ["Alice", "Bob"];
    private static readonly string[] BadValuesArray = [SingleBadValue];

    private class TestModel
    {
        public string[] Names { get; set; }
        public int Age { get; set; } // non-array property for delegation test
    }

    private static PropertyInfo GetProperty(string name) =>
        typeof(TestModel).GetProperty(name);

    [Fact]
    public async Task HandleAsync_BindsArrayProperty_WhenValuesProvided()
    {
        // Arrange
        TestModel model = new TestModel();

        Mock<ITypeConverter> converterMock =
            TypeConverterTestDoubles.MockFor(
                value: CombinedNamesValue,
                targetType: typeof(string[]),
                response: ResponseArray);

        Mock<IValueProvider> valueProviderMock =
            ValueProviderTestDoubles.MockFor(
                key: NamesPropertyKey,
                response: new ValueProviderResult(ResponseArray));

        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles
                .Stub(valueProviderMock.Object, typeof(TestModel));

        ArrayBinderHandler handler =
            new ArrayBinderHandler(converterMock.Object);

        PropertyInfo property =
            GetProperty(nameof(TestModel.Names));

        // Act
        bool result =
            await handler.HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(ResponseArray, model.Names);
    }

    [Fact]
    public async Task HandleAsync_DelegatesToBase_WhenPropertyIsNotArray()
    {
        // Arrange
        TestModel model = new TestModel();

        Mock<ITypeConverter> converterMock =
            TypeConverterTestDoubles.Default();

        Mock<IValueProvider> valueProviderMock =
            ValueProviderTestDoubles.MockFor(
                key: AgePropertyKey,
                response: new ValueProviderResult(AgeValue));

        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles
                .Stub(valueProviderMock.Object, typeof(TestModel));

        ArrayBinderHandler handler =
            new ArrayBinderHandler(converterMock.Object);

        PropertyInfo property =
            GetProperty(nameof(TestModel.Age));

        // Act
        bool result =
            await handler.HandleAsync(context, model, property);

        // Assert
        Assert.False(result);       // should delegate
        Assert.Equal(0, model.Age); // unchanged
    }

    [Fact]
    public async Task HandleAsync_DelegatesToBase_WhenNoValuesFound()
    {
        // Arrange
        TestModel model = new TestModel();

        Mock<ITypeConverter> converterMock =
            TypeConverterTestDoubles.Default();

        Mock<IValueProvider> valueProviderMock =
            ValueProviderTestDoubles.MockFor(
                key: NamesPropertyKey,
                response: ValueProviderResult.None);

        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles
                .Stub(valueProviderMock.Object, typeof(TestModel));

        ArrayBinderHandler handler =
            new ArrayBinderHandler(converterMock.Object);

        PropertyInfo property =
            GetProperty(nameof(TestModel.Names));

        // Act
        bool result =
            await handler.HandleAsync(context, model, property);

        // Assert
        Assert.False(result);   // no binding performed
        Assert.Null(model.Names);
    }

    [Fact]
    public async Task HandleAsync_FailsBinding_WhenConversionThrows()
    {
        // Arrange
        TestModel model = new TestModel();

        Mock<ITypeConverter> converterMock =
            TypeConverterTestDoubles.Throws(
                value: SingleBadValue,
                targetType: typeof(string[]),
                exception: new Exception("Conversion failed"));

        Mock<IValueProvider> valueProviderMock =
            ValueProviderTestDoubles.MockFor(
                key: NamesPropertyKey,
                response: new ValueProviderResult(BadValuesArray));

        DefaultModelBindingContext context =
            ModelBindingContextTestDoubles
             .Stub(valueProviderMock.Object, typeof(TestModel));

        ArrayBinderHandler handler =
            new ArrayBinderHandler(converterMock.Object);

        PropertyInfo property =
            GetProperty(nameof(TestModel.Names));

        // Act
        bool result =
            await handler.HandleAsync(context, model, property);

        // Assert
        Assert.False(result); // falls back to base handler
        Assert.Equal(ModelBindingResult.Failed(), context.Result);
    }
}
