using System;
using System.Collections.Generic;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.TypeConverters.TestDoubles;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.TypeConverters;

public class DefaultTypeConverterTests
{
    private readonly ValueConverterTestDoubles.FakeValueConverterDouble _valueConverter = new();
    private readonly CollectionFactoryTestDoubles.FakeCollectionFactoryDouble _collectionFactory = new();

    [Fact]
    public void Convert_ShouldReturnNull_ForWhitespaceInput()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("   ", typeof(int));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ShouldHandleSimpleInt()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("42", typeof(int));

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Convert_ShouldHandleNullableInt()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("99", typeof(int?));

        // Assert
        Assert.Equal(99, result);
    }

    [Fact]
    public void Convert_ShouldHandleEnum()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("Friday", typeof(DayOfWeek));

        // Assert
        Assert.Equal(DayOfWeek.Friday, result);
    }

    [Fact]
    public void Convert_ShouldHandleStringCollection_Array()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("A,B,C", typeof(string[]));

        // Assert
        Assert.IsType<string[]>(result);
        string[] array = (string[]) result;
        Assert.Equal(new[] { "A", "B", "C" }, array);
    }

    [Fact]
    public void Convert_ShouldHandleIntCollection_List()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("1,2,3", typeof(List<int>));

        // Assert
        Assert.IsType<List<int>>(result);
        List<int> list = (List<int>) result;
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void Convert_ShouldSkipInvalidEntries_InCollection()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("1,abc,2", typeof(int[]));

        // Assert
        int[] array = (int[]) result;
        Assert.Equal(new[] { 1, 2 }, array); // "abc" skipped
    }

    [Fact]
    public void Convert_ShouldThrow_WhenElementTypeCannotBeDetermined()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act & Assert
        Assert.Throws<InvalidCastException>(
            () => converter.Convert("value", typeof(IEnumerable<>)));
    }

    [Fact]
    public void Convert_ShouldHandleNonPrimitiveType_AsSingleItem()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        Guid guid = Guid.NewGuid();

        // Act
        object result =
            converter.Convert(guid.ToString(), typeof(Guid));

        // Assert
        Assert.IsType<Guid>(result);
        Assert.Equal(guid, result);
    }

    [Fact]
    public void Convert_ShouldHandleDoubleCollection()
    {
        // Arrange
        DefaultTypeConverter converter =
            new(_valueConverter, _collectionFactory);

        // Act
        object result =
            converter.Convert("1.1,2.2", typeof(List<double>));

        // Assert
        Assert.IsType<List<double>>(result);
        List<double> list = (List<double>) result;
        Assert.Equal(new[] { 1.1, 2.2 }, list);
    }
}
