using System;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.Factories;

public class TypeFactoryTests
{
    private readonly TypeFactory _factory = new();

    // Test classes
    private class ParameterlessClass
    {
        public string Message { get; } = "Hello";
    }

    private class ParameterizedClass(int number, string text)
    {
        public int Number { get; } = number;
        public string Text { get; } = text;
    }

    private class ValueTypeCtorClass(double value)
    {
        public double Value { get; } = value;
    }

    private abstract class AbstractClass
    {
        protected AbstractClass() { }
    }

    [Fact]
    public void CreateInstance_ShouldCreateObject_WithParameterlessConstructor()
    {
        // Arrange
        Type type = typeof(ParameterlessClass);

        // Act
        object instance =
            _factory.CreateInstance(type);

        // Assert
        Assert.NotNull(instance);
        Assert.IsType<ParameterlessClass>(instance);
        Assert.Equal("Hello", ((ParameterlessClass) instance).Message);
    }

    [Fact]
    public void CreateInstance_Generic_ShouldCreateObject_WithParameterlessConstructor()
    {
        // Act
        ParameterlessClass instance =
            _factory.CreateInstance<ParameterlessClass>();

        // Assert
        Assert.NotNull(instance);
        Assert.Equal("Hello", instance.Message);
    }

    [Fact]
    public void CreateInstance_ShouldCreateObject_WithParameterizedConstructor()
    {
        // Arrange
        Type type = typeof(ParameterizedClass);
        object[] args = [42, "Test"];

        // Act
        object instance =
            _factory.CreateInstance(type, args);

        // Assert
        Assert.NotNull(instance);
        Assert.IsType<ParameterizedClass>(instance);
        ParameterizedClass typed = (ParameterizedClass) instance;
        Assert.Equal(42, typed.Number);
        Assert.Equal("Test", typed.Text);
    }

    [Fact]
    public void CreateInstance_ShouldHandleValueTypeArguments()
    {
        // Arrange
        Type type = typeof(ValueTypeCtorClass);
        object[] args = [3.14];

        // Act
        object instance =
            _factory.CreateInstance(type, args);

        // Assert
        Assert.NotNull(instance);
        Assert.IsType<ValueTypeCtorClass>(instance);
        Assert.Equal(3.14, ((ValueTypeCtorClass) instance).Value);
    }

    [Fact]
    public void CreateInstance_ShouldThrow_WhenTypeIsNull()
    {
        // Arrange
        Type type = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _factory.CreateInstance(type));
    }

    [Fact]
    public void CreateInstance_ShouldThrow_WhenNoMatchingConstructor()
    {
        // Arrange
        Type type = typeof(ParameterizedClass);
        object[] args = [42]; // missing string argument.

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => _factory.CreateInstance(type, args));
    }

    [Fact]
    public void CreateInstance_ShouldThrow_WhenTypeIsAbstract()
    {
        // Arrange
        Type type = typeof(AbstractClass);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => _factory.CreateInstance(type));
    }

    [Fact]
    public void CreateInstance_ShouldReuseCachedDelegate()
    {
        // Arrange
        Type type = typeof(ParameterlessClass);

        // Act
        object first = _factory.CreateInstance(type);
        object second = _factory.CreateInstance(type);

        // Assert
        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.IsType<ParameterlessClass>(first);
        Assert.IsType<ParameterlessClass>(second);
        Assert.NotSame(first, second); // different instances.
    }
}

