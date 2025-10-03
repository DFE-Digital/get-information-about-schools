namespace DfE.GIAS.Core.Common.Domain;

/// <summary>
/// Abstract base class for Value Objects in Domain-Driven Design.
/// Value Objects are defined by their attributes rather than identity.
/// Equality is determined by comparing all relevant attributes.
/// 
/// Key responsibilities:
/// - <b>Equals</b>: Compares all equality components for equivalence.
/// - <b>GetHashCode</b>: Generates a consistent hash based on attributes.
/// </summary>
/// <typeparam name="TValueObject">Concrete type inheriting from this base.</typeparam>
public abstract class ValueObject<TValueObject> where TValueObject : ValueObject<TValueObject>
{
    /// <summary>
    /// Determines whether the current object is equal to another object of the same type.
    /// Equality is based on the sequence of equality components.
    /// </summary>
    /// <param name="obj">Object to compare with the current instance.</param>
    /// <returns>True if all components are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) =>
        obj is TValueObject other &&
               GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    /// <summary>
    /// Generates a hash code based on the object's equality components.
    /// Ensures correct behavior in hash-based collections.
    /// </summary>
    /// <returns>Hash code representing the value object.</returns>
    public override int GetHashCode()
    {
        int hash = 17;

        foreach (object component in GetEqualityComponents())
        {
            hash = (hash * 31) + (component?.GetHashCode() ?? 0);
        }

        return hash;
    }

    /// <summary>
    /// Provides the components used to determine equality.
    /// Derived classes must expose all relevant attributes here.
    /// </summary>
    /// <returns>Enumerable of objects representing equality components.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Equality operator override for value object comparison.
    /// </summary>
    public static bool operator ==(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        => Equals(left, right);

    /// <summary>
    /// Inequality operator override for value object comparison.
    /// </summary>
    public static bool operator !=(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        => !Equals(left, right);
}
