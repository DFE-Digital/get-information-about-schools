using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.Extensions;

public class TypeExtensionsTests
{
    [Fact]
    public void GetElementType_ShouldReturnElementType_ForArray()
    {
        // Arrange
        Type arrayType = typeof(int[]);

        // Act
        Type elementType = arrayType.GetElementType();

        // Assert
        Assert.Equal(typeof(int), elementType);
    }

    [Fact]
    public void GetElementType_ShouldReturnGenericArgument_ForGenericList()
    {
        // Arrange
        Type listType = typeof(List<string>); // must be closed generic

        // Act
        Type elementType = TypeExtensions.GetElementType(listType);

        // Assert
        Assert.Equal(typeof(string), elementType);
    }

    private class CustomList : List<double> { }

    [Fact]
    public void GetElementType_ShouldReturnGenericArgument_FromBaseType()
    {
        // Arrange
        Type customType = typeof(CustomList);

        // Act
        Type elementType = TypeExtensions.GetElementType(customType);

        // Assert
        Assert.Equal(typeof(double), elementType);
    }

    [Fact]
    public void GetElementType_ShouldReturnNull_WhenNotCollection()
    {
        // Arrange
        Type nonCollectionType = typeof(DateTime);

        // Act
        Type elementType = nonCollectionType.GetElementType();

        // Assert
        Assert.Null(elementType);
    }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(Guid), true)]
    [InlineData(typeof(DayOfWeek), true)] // enum
    [InlineData(typeof(List<int>), false)]
    [InlineData(typeof(object), false)]
    public void IsSimpleType_ShouldIdentifyCorrectly(Type type, bool expected)
    {
        // Act
        bool result = type.IsSimpleType();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(decimal?), true)]
    [InlineData(typeof(string), false)]
    [InlineData(typeof(DateTime), false)]
    [InlineData(typeof(Guid), false)]
    public void IsPrimitiveType_ShouldIdentifyCorrectly(Type type, bool expected)
    {
        // Act
        bool result = type.IsPrimitiveType();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsCollectionType_ShouldReturnTrue_ForArray()
    {
        // Arrange
        Type arrayType = typeof(int[]);

        // Act
        bool result = arrayType.IsCollectionType();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCollectionType_ShouldReturnTrue_ForGenericList()
    {
        // Arrange
        Type listType = typeof(List<string>);

        // Act
        bool result = listType.IsCollectionType();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCollectionType_ShouldReturnFalse_ForString()
    {
        // Arrange
        Type stringType = typeof(string);

        // Act
        bool result = stringType.IsCollectionType();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCollectionType_ShouldReturnFalse_ForNonCollection()
    {
        // Arrange
        Type nonCollectionType = typeof(int);

        // Act
        bool result = nonCollectionType.IsCollectionType();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ResolveConcreteType_ShouldReturnSameType_ForConcreteList()
    {
        // Arrange
        Type listType = typeof(List<int>);

        // Act
        Type resolved = listType.ResolveConcreteType();

        // Assert
        Assert.Equal(listType, resolved);
    }

    [Fact]
    public void ResolveConcreteType_ShouldReturnConcreteList_ForIEnumerable()
    {
        // Arrange
        Type enumerableType = typeof(IEnumerable<string>);

        // Act
        Type resolved = enumerableType.ResolveConcreteType();

        // Assert
        Assert.Equal(typeof(List<string>), resolved);
    }

    [Fact]
    public void ResolveConcreteType_ShouldReturnConcreteList_ForICollection()
    {
        // Arrange
        Type collectionType = typeof(ICollection<Guid>);

        // Act
        Type resolved = collectionType.ResolveConcreteType();

        // Assert
        Assert.Equal(typeof(List<Guid>), resolved);
    }

    [Fact]
    public void ResolveConcreteType_ShouldReturnConcreteList_ForIList()
    {
        // Arrange
        Type listInterfaceType = typeof(IList<DateTime>);

        // Act
        Type resolved = listInterfaceType.ResolveConcreteType();

        // Assert
        Assert.Equal(typeof(List<DateTime>), resolved);
    }

    [Fact]
    public void ResolveConcreteType_ShouldReturnOriginalType_ForNonGenericInterface()
    {
        // Arrange
        Type nonGenericInterface = typeof(IEnumerable);

        // Act
        Type resolved = nonGenericInterface.ResolveConcreteType();

        // Assert
        Assert.Equal(nonGenericInterface, resolved);
    }

    [Fact]
    public void ResolveConcreteType_ShouldReturnOriginalType_ForAbstractClass()
    {
        // Arrange
        Type abstractType = typeof(AbstractCollection);

        // Act
        Type resolved = abstractType.ResolveConcreteType();

        // Assert
        Assert.Equal(abstractType, resolved);
    }

    private abstract class AbstractCollection : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() =>
            Enumerable.Empty<int>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
