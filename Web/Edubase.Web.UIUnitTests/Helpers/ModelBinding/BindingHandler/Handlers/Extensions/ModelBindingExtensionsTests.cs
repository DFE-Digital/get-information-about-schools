using System;
using System.Reflection;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;
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
