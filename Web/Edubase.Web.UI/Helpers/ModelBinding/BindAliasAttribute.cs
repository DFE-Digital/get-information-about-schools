using System;

/// <summary>
/// Specifies an alternate name (alias) that can be used to bind incoming request
/// data to a model property during model binding.
/// </summary>
/// <remarks>
/// <para>
/// This attribute allows a property to be bound from multiple possible field names
/// in the incoming request. It is especially useful when supporting legacy field names,
/// external API variations, or multiple naming conventions.
/// </para>
/// <para>
/// Multiple aliases may be applied to the same property.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class BindAliasAttribute(string alias) : Attribute
{
    /// <summary>
    /// Gets the alias name that can be used to bind request data to the decorated property.
    /// </summary>
    public string Alias { get; private set; } = alias;
}
