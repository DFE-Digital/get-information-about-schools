using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers;

public class PropertyNameBinderHandlerTests
{
    private readonly Mock<ITypeConverter> _converterMock;
    private readonly PropertyNameBinderHandler _handler;

    public PropertyNameBinderHandlerTests()
    {
        _converterMock = TypeConverterTestDoubles.Default();
        _handler = new PropertyNameBinderHandler(_converterMock.Object);
    }

    private class TestModel
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }

    [Fact]
    public async Task HandleAsync_ShouldBindProperty_WhenValueExists()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "Age", new[] { "42" } }
                });

        PropertyInfo? property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Age));

        TestModel model = new TestModel();

        Mock<ITypeConverter> converter =
            TypeConverterTestDoubles.MockFor("42", typeof(int), 42);

        PropertyNameBinderHandler handler =
            new PropertyNameBinderHandler(converter.Object);

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(42, model.Age);
        converter.Verify();
    }

    [Theory]
    [InlineData("invalid", null, "The value is not valid for UKPRN")]
    [InlineData("2147483648", null, "The value is not valid for UKPRN")]
    [InlineData("12345678", 12345678, null)]
    public async Task HandleAsync_ShouldValidateUkprn(string value, int? expectedValue, string expectedError)
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles.StubForDictionary<GroupEditorViewModel>(
                new Dictionary<string, string[]>
                {
                    { "UKPRN", new[] { value } }
                });
        GroupEditorViewModel model = new();
        PropertyNameBinderHandler handler = new(
            new DefaultTypeConverter(new DefaultValueConverter(), new CollectionFactory(new TypeFactory())));
        PropertyInfo property = typeof(GroupEditorViewModel).GetProperty(nameof(GroupEditorViewModel.UKPRN));

        // Act
        bool result = await handler.HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedValue, model.UKPRN);
        Assert.Equal(value, context.ModelState["UKPRN"].AttemptedValue);
        if (expectedError == null)
        {
            Assert.Equal(0, context.ModelState.ErrorCount);
        }
        else
        {
            Assert.False(context.ModelState.IsValid);
            Assert.Equal(expectedError, Assert.Single(context.ModelState["UKPRN"].Errors).ErrorMessage);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldDelegate_WhenNoValueFound()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>());

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
        Assert.Null(model.Name);
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
            DefaultModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "Age", new[] { "bad" } }
                });

        PropertyInfo? property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Age));

        TestModel model = new TestModel();

        Mock<ITypeConverter> converter =
            TypeConverterTestDoubles
                .Throws("bad", typeof(int), new FormatException());

        PropertyNameBinderHandler handler =
            new PropertyNameBinderHandler(converter.Object);

        // Act
        bool result = await handler.HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.False(context.ModelState.IsValid);
        Assert.Equal("bad", context.ModelState["Age"].AttemptedValue);
        Assert.Equal("The value is not valid for Age", Assert.Single(context.ModelState["Age"].Errors).ErrorMessage);
        Assert.Equal(0, model.Age); // property not set
    }

    [Fact]
    public async Task HandleAsync_ShouldBindProperty_WhenModelNamePrefixUsed()
    {
        // Arrange
        DefaultModelBindingContext context =
            DefaultModelBindingContextTestDoubles.StubForDictionary<TestModel>(
                new Dictionary<string, string[]>
                {
                    { "TestModel.Name", new[] { "Alice" } }
                });

        context.ModelName = "TestModel";

        PropertyInfo? property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Name));

        TestModel model = new TestModel();

        Mock<ITypeConverter> converter =
            TypeConverterTestDoubles
                .MockFor("Alice", typeof(string), "Alice");

        PropertyNameBinderHandler handler =
            new PropertyNameBinderHandler(converter.Object);

        // Act
        bool result =
            await handler
                .HandleAsync(context, model, property);

        // Assert
        Assert.True(result);
        Assert.Equal("Alice", model.Name);
        converter.Verify();
    }
}
