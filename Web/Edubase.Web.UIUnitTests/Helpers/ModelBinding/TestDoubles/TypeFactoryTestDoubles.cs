using System;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.TestDoubles
{
    /// <summary>
    /// Test double implementation of <see cref="ITypeFactory"/> for unit testing.
    /// Allows you to inject a delegate to control how instances are created.
    /// </summary>
    internal class TypeFactoryTestDoubles : ITypeFactory
    {
        private readonly Func<Type, object> _create;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFactoryTestDoubles"/> class
        /// with a default delegate that uses <see cref="Activator.CreateInstance(Type)"/>.
        /// </summary>
        public TypeFactoryTestDoubles()
            : this(type => Activator.CreateInstance(type)){
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFactoryTestDoubles"/> class
        /// with a custom delegate.
        /// </summary>
        /// <param name="create">
        /// A delegate that defines how to create an instance for a given <see cref="Type"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="create"/> is <c>null</c>.
        /// </exception>
        public TypeFactoryTestDoubles(Func<Type, object> create)
        {
            _create = create ??
                throw new ArgumentNullException(nameof(create));
        }

        /// <summary>
        /// Creates an instance of the specified type using the injected delegate.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object CreateInstance(Type type) => _create(type);

        /// <summary>
        /// Creates an instance of the specified type using the injected delegate,
        /// or falls back to <see cref="Activator.CreateInstance(Type, object[])"/> if arguments are provided.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <param name="args">Optional constructor arguments.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object CreateInstance(Type type, params object[] args) =>
            args == null || args.Length == 0
                ? _create(type)
                : Activator.CreateInstance(type, args);

        /// <summary>
        /// Creates an instance of the specified generic type using the injected delegate.
        /// </summary>
        /// <typeparam name="TObject">The type to instantiate.</typeparam>
        /// <returns>A new instance of the specified generic type.</returns>
        public TObject CreateInstance<TObject>() =>
            (TObject) _create(typeof(TObject));
    }
}
