using System;
using System.Collections.Generic;
using System.Reflection;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;
using EmptyModelMetadataProvider = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider;
using ValueProviderResult = Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;

public class ModelBindingExtensionsTests
{
    private class TestModel
    {
        [BindAlias("AliasName")]
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Fact]
    public void BuildPropertyPrefix_ShouldReturnPropertyName_WhenModelNameIsEmpty()
    {
        // Arrange
        DefaultModelBindingContext context = new()
        {
            ModelName = string.Empty,
            ModelMetadata =
                new EmptyModelMetadataProvider()
                    .GetMetadataForType(typeof(TestModel))
        };

        PropertyInfo property =
            typeof(TestModel)
             .GetProperty(nameof(TestModel.Name));

        // Act
        string prefix = context.BuildPropertyPrefix(property);

        // Assert
        Assert.Equal("Name", prefix);
    }

    [Fact]
    public void BuildPropertyPrefix_ShouldReturnPrefixedPropertyName_WhenModelNameIsSet()
    {
        // Arrange
        DefaultModelBindingContext context = new()
        {
            ModelName = "Parent",
            ModelMetadata =
                new EmptyModelMetadataProvider()
                    .GetMetadataForType(typeof(TestModel))
        };

        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Age));

        // Act
        string prefix = context.BuildPropertyPrefix(property);

        // Assert
        Assert.Equal("Parent.Age", prefix);
    }

    [Fact]
    public void BuildElementPrefix_ShouldReturnIndexedPrefix()
    {
        // Arrange
        string propertyPrefix = "Numbers";

        // Act
        string elementPrefix = propertyPrefix.BuildElementPrefix(0);

        // Assert
        Assert.Equal("Numbers[0]", elementPrefix);
    }

    [Fact]
    public void BuildElementPropertyKey_ShouldReturnIndexedPropertyKey()
    {
        // Arrange
        string elementPrefix = "Numbers[1]";
        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Name));

        // Act
        string key = elementPrefix.BuildElementPropertyKey(property);

        // Assert
        Assert.Equal("Numbers[1].Name", key);
    }

    [Fact]
    public void HasValue_ShouldReturnTrue_WhenProviderHasNonEmptyValue()
    {
        // Arrange
        IValueProvider provider =
            new ValueProviderTestDoubles.TestValueProvider(
                new Dictionary<string, string[]>
                {
                    { "Name", new[] { "Alice" } }
                });

        // Act
        bool result = provider.HasValue("Name");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasValue_ShouldReturnFalse_WhenProviderHasEmptyValue()
    {
        // Arrange
        IValueProvider provider =
            new ValueProviderTestDoubles.TestValueProvider(
                new Dictionary<string, string[]>
                {
                    { "Name", new[] { "" } }
                });

        // Act
        bool result = provider.HasValue("Name");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasValue_ShouldReturnFalse_WhenProviderHasNoValue()
    {
        // Arrange
        IValueProvider provider =
            new ValueProviderTestDoubles.TestValueProvider([]);

        // Act
        bool result = provider.HasValue("Missing");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetBindAliases_ShouldReturnAliasAttributes()
    {
        // Arrange
        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Name));

        // Act
        BindAliasAttribute[] aliases = property.GetBindAliases();

        // Assert
        Assert.Single(aliases);
        Assert.Equal("AliasName", aliases[0].Alias);
    }

    [Fact]
    public void GetBindAliases_ShouldReturnEmptyArray_WhenNoAliasesDefined()
    {
        // Arrange
        PropertyInfo property =
            typeof(TestModel)
                .GetProperty(nameof(TestModel.Age));

        // Act
        BindAliasAttribute[] aliases = property.GetBindAliases();

        // Assert
        Assert.Empty(aliases);
    }

    [Fact]
    public void HasValues_ShouldReturnTrue_WhenResultHasNonEmptyValues()
    {
        // Arrange
        ValueProviderResult result =
            new ValueProviderResult(new[] { "Alice", "Bob" });

        // Act
        bool hasValues = result.HasValues();

        // Assert
        Assert.True(hasValues);
    }

    [Fact]
    public void HasValues_ShouldReturnFalse_WhenResultIsNone()
    {
        // Arrange
        ValueProviderResult result = ValueProviderResult.None;

        // Act
        bool hasValues = result.HasValues();

        // Assert
        Assert.False(hasValues);
    }

    [Fact]
    public void HasValues_ShouldReturnFalse_WhenAllValuesAreEmpty()
    {
        // Arrange
        ValueProviderResult result =
            new ValueProviderResult(new[] { "", "" });

        // Act
        bool hasValues = result.HasValues();

        // Assert
        Assert.False(hasValues);
    }

    [Fact]
    public void ToCombinedString_ShouldReturnCommaSeparatedValues()
    {
        // Arrange
        ValueProviderResult result =
            new ValueProviderResult(new[] { "Alice", "Bob", "Charlie" });

        // Act
        string combined = result.ToCombinedString();

        // Assert
        Assert.Equal("Alice,Bob,Charlie", combined);
    }

    [Fact]
    public void ToCombinedString_ShouldReturnEmptyString_WhenNoValues()
    {
        // Arrange
        ValueProviderResult result =
            new ValueProviderResult(Array.Empty<string>());

        // Act
        string combined = result.ToCombinedString();

        // Assert
        Assert.Equal(string.Empty, combined);
    }
}
