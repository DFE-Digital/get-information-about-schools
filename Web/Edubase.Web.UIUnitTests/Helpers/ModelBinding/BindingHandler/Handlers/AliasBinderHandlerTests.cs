using System;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers;

public class AliasBinderHandlerTests
{
    private const string BindingAlias = "aliasName";
    private const string TestNameParam1 = "test_name_1";
    private const string TestNameParam2 = "test_name_2";

    private class TestModel
    {
        [BindAlias(BindingAlias)]
        public string Name { get; set; }
    }

    private static PropertyInfo GetNameProperty() =>
        typeof(TestModel).GetProperty(nameof(TestModel.Name));

    [Theory]
    [InlineData(TestNameParam1)]
    [InlineData(TestNameParam2)]
    public async Task HandleAsync_BindsProperty_WhenAliasMatches(string input)
    {
        // Arrange
        TestModel model = new TestModel();

        Mock<ITypeConverter> converterMock =
            TypeConverterTestDoubles.MockFor(
                value: input,
                targetType: typeof(string),
                response: input);

        Mock<IValueProvider> valueProviderMock =
            ValueProviderTestDoubles.MockFor(
                key: BindingAlias,
                response: new ValueProviderResult(input));

        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .Stub(valueProviderMock.Object, typeof(TestModel));

        AliasBinderHandler handler =
            new AliasBinderHandler(converterMock.Object);
        PropertyInfo property = GetNameProperty();

        // Act
        bool result =
            await handler.HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(input, model.Name);
    }

    [Fact]
    public async Task HandleAsync_FailsBinding_WhenConversionThrows()
    {
        // Arrange
        TestModel model = new TestModel();

        Mock<ITypeConverter> converterMock =
            TypeConverterTestDoubles.Throws(
                value: TestNameParam1,
                targetType: typeof(string),
                exception: new Exception("Conversion failed"));

        Mock<IValueProvider> valueProviderMock =
            ValueProviderTestDoubles.MockFor(
                key: BindingAlias,
                response: new ValueProviderResult("BadValue"));

        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .Stub(valueProviderMock.Object, typeof(TestModel));

        AliasBinderHandler handler =
            new AliasBinderHandler(converterMock.Object);
        PropertyInfo property = GetNameProperty();

        // Act
        bool result =
            await handler.HandleAsync(context, model, property);

        // Assert
        Assert.False(result); // falls back to base handler.
        Assert.Equal(ModelBindingResult.Failed(), context.Result);
    }

    [Fact]
    public async Task HandleAsync_DelegatesToBase_WhenNoAliasValueFound()
    {
        // Arrange
        TestModel model = new TestModel();

        Mock<ITypeConverter> converterMock =
            TypeConverterTestDoubles.Default();

        Mock<IValueProvider> valueProviderMock =
            ValueProviderTestDoubles.MockFor(
                key: BindingAlias,
                response: ValueProviderResult.None);

        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles
                .Stub(valueProviderMock.Object, typeof(TestModel));

        AliasBinderHandler handler =
            new AliasBinderHandler(converterMock.Object);
        PropertyInfo property = GetNameProperty();

        // Act
        bool result =
            await handler.HandleAsync(context, model, property);

        // Assert
        Assert.False(result);   // no binding performed.
        Assert.Null(model.Name);
    }
}
