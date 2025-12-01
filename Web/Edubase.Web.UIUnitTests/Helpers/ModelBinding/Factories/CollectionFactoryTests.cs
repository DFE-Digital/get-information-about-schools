using System;
using System.Collections.Generic;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.Factories.TestDoubles;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.Factories;

public class CollectionFactoryTests
{
    [Fact]
    public void CreateListInstance_ShouldCreateArray_AndPopulateItems()
    {
        // Arrange
        CollectionFactory factory =
            new CollectionFactory(
                new TypeFactoryTestDoubles.ActivatorTypeFactory());

        Type listType = typeof(int[]);
        Type elementType = typeof(int);
        object[] items = [1, 2, 3];

        // Act
        object result =
            factory.CreateListInstance(
                listType, elementType, items);

        // Assert
        Assert.IsType<int[]>(result);
        int[] array = (int[]) result;
        Assert.Equal(new[] { 1, 2, 3 }, array);
    }

    [Fact]
    public void CreateListInstance_ShouldCreateList_WhenInterfaceTypeProvided()
    {
        // Arrange
        TypeFactoryTestDoubles.StubTypeFactory stubFactory = new ();
        List<string> preconfiguredList = [];
        stubFactory.Register(preconfiguredList);

        CollectionFactory factory = new(stubFactory);
        Type listType = typeof(IEnumerable<string>);
        Type elementType = typeof(string);
        object[] items = ["A", "B"];

        // Act
        object result =
            factory.CreateListInstance(
                listType, elementType, items);

        // Assert
        Assert.Same(preconfiguredList, result);
        Assert.Equal(new[] { "A", "B" }, preconfiguredList);
    }

    private abstract class AbstractList : List<int> { }

    [Fact]
    public void CreateListInstance_ShouldCreateList_WhenAbstractTypeProvided()
    {
        // Arrange
        TypeFactoryTestDoubles.StubTypeFactory stubFactory = new();
        List<int> preconfiguredList = [];
        stubFactory.Register(preconfiguredList);

        CollectionFactory factory = new(stubFactory);
        Type listType = typeof(AbstractList);
        Type elementType = typeof(int);
        object[] items = [10, 20];

        // Act
        object result =
            factory.CreateListInstance(
                listType, elementType, items);

        // Assert
        Assert.Same(preconfiguredList, result);
        Assert.Equal(new[] { 10, 20 }, preconfiguredList);
    }

    [Fact]
    public void CreateListInstance_ShouldCreateConcreteList_AndPopulateItems()
    {
        // Arrange
        TypeFactoryTestDoubles.StubTypeFactory stubFactory = new();
        List<double> preconfiguredList = [];
        stubFactory.Register(preconfiguredList);

        CollectionFactory factory = new(stubFactory);
        Type listType = typeof(List<double>);
        Type elementType = typeof(double);
        object[] items = [1.1, 2.2];

        // Act
        object result =
            factory.CreateListInstance(
                listType, elementType, items);

        // Assert
        Assert.Same(preconfiguredList, result);
        Assert.Equal(new[] { 1.1, 2.2 }, preconfiguredList);
    }

    private class NonListType
    {
        public NonListType() { }
    }

    [Fact]
    public void CreateListInstance_ShouldThrow_WhenUnsupportedType()
    {
        // Arrange
        CollectionFactory factory =
            new(new TypeFactoryTestDoubles.ActivatorTypeFactory());

        Type listType = typeof(NonListType);
        Type elementType = typeof(string);
        object[] items = ["X"];

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => factory.CreateListInstance(listType, elementType, items));
    }

    [Fact]
    public void CreateListInstance_ShouldThrow_WhenTypeFactoryReturnsNonIList()
    {
        // Arrange
        TypeFactoryTestDoubles.StubTypeFactory stubFactory = new();
        NonListType nonListInstance = new NonListType();
        stubFactory.Register(nonListInstance); // not an IList

        CollectionFactory factory = new(stubFactory);
        Type listType = typeof(List<int>);
        Type elementType = typeof(int);
        object[] items = [1];

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => factory.CreateListInstance(listType, elementType, items));
    }
}
