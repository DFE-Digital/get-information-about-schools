using System;
using System.Collections.Generic;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.Factories.TestDoubles;

/// <summary>
/// Provides reusable test doubles for <see cref="ITypeFactory"/>.
/// These helpers simplify unit test setup by offering fake implementations
/// that either use <see cref="Activator"/> or return preconfigured instances.
/// </summary>
internal static class TypeFactoryTestDoubles
{
    /// <summary>
    /// A fake type factory that always creates new instances using <see cref="Activator"/>.
    /// This behaves similarly to the real <see cref="TypeFactory"/> but without IL generation or caching.
    /// Useful for simple tests where constructor invocation is sufficient.
    /// </summary>
    internal sealed class ActivatorTypeFactory : ITypeFactory
    {
        /// <summary>
        /// Creates a new instance of the specified type using its parameterless constructor.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object CreateInstance(Type type) => Activator.CreateInstance(type);

        /// <summary>
        /// Creates a new instance of the specified type using a constructor that matches the given arguments.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <param name="args">Constructor arguments to pass when creating the instance.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object CreateInstance(Type type, params object[] args) =>
            Activator.CreateInstance(type, args);

        /// <summary>
        /// Creates a new instance of the specified generic type using its parameterless constructor.
        /// </summary>
        /// <typeparam name="TObject">The type to instantiate.</typeparam>
        /// <returns>A new instance of the specified generic type.</returns>
        public TObject CreateInstance<TObject>() =>
            (TObject) Activator.CreateInstance(typeof(TObject));
    }

    /// <summary>
    /// A fake type factory that returns preconfigured stub instances for specific types.
    /// Useful when you want predictable objects without invoking constructors,
    /// or when you need to bypass complex initialization logic.
    /// </summary>
    internal sealed class StubTypeFactory : ITypeFactory
    {
        private readonly Dictionary<Type, object> _instances = new();

        /// <summary>
        /// Registers a stub instance for the given type.
        /// When <see cref="CreateInstance(Type)"/> is called with this type,
        /// the registered instance will be returned.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance to register.</typeparam>
        /// <param name="instance">The instance to return for the given type.</param>
        public void Register<TInstance>(TInstance instance)
        {
            _instances[typeof(TInstance)] = instance!;
        }

        /// <summary>
        /// Returns a preconfigured instance for the specified type.
        /// Throws an <see cref="InvalidOperationException"/> if no instance has been registered.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <returns>The registered instance for the specified type.</returns>
        public object CreateInstance(Type type) =>
            _instances.TryGetValue(type, out var instance)
                ? instance
                : throw new InvalidOperationException(
                    $"No stub registered for {type.FullName}");

        /// <summary>
        /// Returns a preconfigured instance for the specified type, ignoring constructor arguments.
        /// Throws an <see cref="InvalidOperationException"/> if no instance has been registered.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <param name="args">Constructor arguments (ignored).</param>
        /// <returns>The registered instance for the specified type.</returns>
        public object CreateInstance(Type type, params object[] args) => CreateInstance(type);

        /// <summary>
        /// Returns a preconfigured instance for the specified generic type.
        /// Throws an <see cref="InvalidOperationException"/> if no instance has been registered.
        /// </summary>
        /// <typeparam name="TObject">The type to instantiate.</typeparam>
        /// <returns>The registered instance for the specified generic type.</returns>
        public TObject CreateInstance<TObject>() =>
            (TObject) CreateInstance(typeof(TObject));
    }
}
