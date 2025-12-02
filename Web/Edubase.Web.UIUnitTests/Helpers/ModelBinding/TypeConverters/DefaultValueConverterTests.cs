using System;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.TypeConverters;

public class DefaultValueConverterTests
{
    private enum TestEnum
    {
        First,
        Second
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Convert_NullOrWhitespace_ReturnsNull(string input)
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert(input, typeof(int));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_PrimitiveInt_ReturnsInt()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert("42", typeof(int));

        // Assert
        Assert.Equal(42, (int) result);
    }

    [Fact]
    public void Convert_Decimal_ReturnsDecimal()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert("123.45", typeof(decimal));

        // Assert
        Assert.Equal(123.45m, (decimal) result);
    }

    [Fact]
    public void Convert_CommaSeparatedPrimitive_ThrowsFormatException()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            converter.Convert("1,2,3", typeof(int)));
    }

    [Fact]
    public void Convert_CommaSeparatedDecimal_ThrowsFormatException()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            converter.Convert("1,2,3", typeof(decimal)));
    }

    [Fact]
    public void Convert_EnumValue_ReturnsEnum()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert("First", typeof(TestEnum));

        // Assert
        Assert.Equal(TestEnum.First, (TestEnum) result);
    }

    [Fact]
    public void Convert_EnumValue_IgnoresCase()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert("second", typeof(TestEnum));

        // Assert
        Assert.Equal(TestEnum.Second, (TestEnum) result);
    }

    [Fact]
    public void Convert_DateTime_UkFormat_ReturnsParsedDate()
    {
        // Arrange
        DefaultValueConverter converter = new ();
        string input = "25/12/2025 15:30:00";

        // Act
        object result = converter.Convert(input, typeof(DateTime));

        // Assert
        Assert.Equal(new DateTime(2025, 12, 25, 15, 30, 0), (DateTime) result);
    }

    [Fact]
    public void Convert_DateTime_EnGbCulture_ReturnsParsedDate()
    {
        // Arrange
        DefaultValueConverter converter = new();
        string input = "25 December 2025";

        // Act
        object result = converter.Convert(input, typeof(DateTime));

        // Assert
        Assert.Equal(new DateTime(2025, 12, 25), ((DateTime) result).Date);
    }

    [Fact]
    public void Convert_DateTime_FallbackInvariant_ReturnsParsedDate()
    {
        // Arrange
        DefaultValueConverter converter = new();
        string input = "2025-12-25";

        // Act
        object result = converter.Convert(input, typeof(DateTime));

        // Assert
        Assert.Equal(new DateTime(2025, 12, 25), ((DateTime) result).Date);
    }

    [Fact]
    public void Convert_NullableInt_ReturnsInt()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert("123", typeof(int?));

        // Assert
        Assert.Equal(123, (int) result);
    }

    [Fact]
    public void Convert_TypeWithoutConverter_ThrowsInvalidCastException()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act & Assert
        Assert.Throws<InvalidCastException>(() =>
            converter.Convert("value", typeof(DefaultValueConverterTests)));
    }

    [Fact]
    public void Convert_Boolean_ReturnsBool()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert("true", typeof(bool));

        // Assert
        Assert.True((bool) result);
    }

    [Fact]
    public void Convert_Double_ReturnsDouble()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act
        object result = converter.Convert("3.14159", typeof(double));

        // Assert
        Assert.Equal(3.14159, (double) result, 5);
    }

    [Fact]
    public void Convert_DateTime_InvalidString_ThrowsFormatException()
    {
        // Arrange
        DefaultValueConverter converter = new();

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            converter.Convert("not-a-date", typeof(DateTime)));
    }
}
