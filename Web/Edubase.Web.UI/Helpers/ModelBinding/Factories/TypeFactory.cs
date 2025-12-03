using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Edubase.Web.UI.Helpers.ModelBinding.Factories;

/// <summary>
/// Provides fast instance creation using dynamic methods and caching,
/// similar to <see cref="Activator.CreateInstance(Type)"/>.
/// </summary>
public sealed class TypeFactory : ITypeFactory
{
    // Thread-safe cache of compiled delegates keyed by (Type, signature string)
    private static readonly ConcurrentDictionary<(Type, string), Delegate> Cache = new();

    /// <summary>
    /// Creates an instance of the specified type using its parameterless constructor.
    /// </summary>
    public object CreateInstance(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        // Retrieve or build a delegate for parameterless constructor
        Func<object> creator =
            (Func<object>) GetOrAddCreator(
                type, argTypes: Type.EmptyTypes);

        return creator();
    }

    /// <summary>
    /// Creates an instance of the specified type using a constructor that matches the given argument types.
    /// </summary>
    public object CreateInstance(Type type, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(type);
        args ??= [];

        // Determine argument types from provided values
        Type[] argTypes =
            Array.ConvertAll(
                array: args,
                converter: obj => obj?.GetType() ?? typeof(object));

        // Retrieve or build a delegate for parameterized constructor
        Func<object[], object> creator =
            (Func<object[], object>) GetOrAddCreator(type, argTypes);

        return creator(args);
    }

    /// <summary>
    /// Generic convenience overload for parameterless constructors.
    /// </summary>
    public TObject CreateInstance<TObject>() =>
        (TObject) CreateInstance(typeof(TObject));

    /// <summary>
    /// Retrieves a cached delegate for the given type/argument signature,
    /// or builds a new one if not cached.
    /// </summary>
    private Delegate GetOrAddCreator(Type type, Type[] argTypes)
    {
        // Build a signature key string from argument types
        string sigKey =
            string.Join(",", Array.ConvertAll(
                argTypes, type => type.FullName ?? "null"));

        return Cache.GetOrAdd((type, sigKey), _ =>
        {
            // Find matching constructor
            ConstructorInfo ctor =
                type.GetConstructor(argTypes)
                    ?? throw new InvalidOperationException(
                        $"No matching constructor found for {type.FullName}.");

            // Choose appropriate delegate builder
            return (Delegate) (
                argTypes.Length == 0
                    ? BuildParameterlessCreator(ctor, type)
                    : BuildParameterizedCreator(ctor, argTypes));
        });
    }

    /// <summary>
    /// Builds a delegate for parameterless constructors using IL.
    /// </summary>
    private static object BuildParameterlessCreator(ConstructorInfo ctor, Type type)
    {
        // Create a dynamic method that returns object
        DynamicMethod dynamicMethod =
            new DynamicMethod(
                name: $"{type.Name}Ctor",
                returnType: typeof(object),
                parameterTypes: Type.EmptyTypes,
                m: typeof(object).Module);

        // Get IL generator
        ILGenerator intermediateLanguageGenerator =
            dynamicMethod.GetILGenerator();

        // Emit IL: newobj ctor → ret
        intermediateLanguageGenerator.Emit(OpCodes.Newobj, ctor);
        intermediateLanguageGenerator.Emit(OpCodes.Ret);

        // Return compiled delegate
        return dynamicMethod.CreateDelegate(
            delegateType: typeof(Func<object>));
    }

    /// <summary>
    /// Builds a delegate for parameterized constructors using IL.
    /// </summary>
    private object BuildParameterizedCreator(
        ConstructorInfo ctor, Type[] argTypes)
    {
        // Create a dynamic method that takes object[] and returns object
        DynamicMethod dynamicMethod =
            new DynamicMethod(
                 name: "${type.Name}CtorArgs",
                 returnType: typeof(object),
                 parameterTypes: [typeof(object[])],
                 m: typeof(object).Module);

        // Get IL generator
        ILGenerator intermediateLanguageGenerator = dynamicMethod.GetILGenerator();

        // Emit IL to load each argument from object[] and cast/unbox
        for (int i = 0; i < argTypes.Length; i++)
        {
            EmitArgumentLoad(
                intermediateLanguageGenerator, index: i, argType: argTypes[i]);
        }

        // Emit IL: newobj ctor → ret
        intermediateLanguageGenerator.Emit(opcode: OpCodes.Newobj, ctor);
        intermediateLanguageGenerator.Emit(opcode: OpCodes.Ret);

        // Return compiled delegate
        return dynamicMethod.CreateDelegate(
            delegateType: typeof(Func<object[], object>));
    }

    /// <summary>
    /// Emits IL instructions to load and cast/unbox a constructor argument from an object array.
    /// </summary>
    private void EmitArgumentLoad(
        ILGenerator intermediateLanguageGenerator, int index, Type argType)
    {
        // Load object[] argument
        intermediateLanguageGenerator.Emit(opcode: OpCodes.Ldarg_0);        // push object[] onto stack
        intermediateLanguageGenerator.Emit(opcode: OpCodes.Ldc_I4, index);  // push index
        intermediateLanguageGenerator.Emit(opcode: OpCodes.Ldelem_Ref);     // load element at index

        // Cast or unbox depending on type
        if (argType.IsValueType){
            intermediateLanguageGenerator.Emit(
                opcode: OpCodes.Unbox_Any, argType);
        }
        else{
            intermediateLanguageGenerator.Emit(
                opcode: OpCodes.Castclass, argType);
        }
    }
}
