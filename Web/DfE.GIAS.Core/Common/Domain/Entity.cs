namespace DfE.GIAS.Core.Common.Domain;

/// <summary>
/// Abstract Entity base class which is used as the basis of any domain concept when the
/// model requires identification, thus affording the ability to distinguish one entity from the other,
/// which is a key design consideration. The main aspects of a domain entity are as follows:
/// <list type="bullet">
/// <item>
/// <term>TIdentifier</term>
/// <description>
/// The identifier is the value used to give an entity a unique identity.
/// This can be assigned any sensible, meaningful unique value such as a long, Guid, etc
/// </description>
/// </item>
/// <item>
/// <term>Equals</term>
/// <description>
/// Used to determine whether an entity is referentially equivalent to another entity.
/// If referential equality doesn't determine that two entities are the same, we
/// compare the Identifiers of the comparable entities.
/// </description>
/// </item>
/// <item>
/// <term>Operator Overloading on == and !=</term>
/// <description>
/// Allows equality checking to be performed on the reference identities of two objects.
/// </description>
/// </item>
/// </list>
/// </summary>
/// <typeparam name="TIdentifier">The runtime type definition of the entity's identifier object.</typeparam>
public abstract class Entity<TIdentifier> where TIdentifier : ValueObject<TIdentifier>
{
    /// <summary>
    /// The unique identifier for the entity, which is used to distinguish it from other entities.
    /// </summary>
    public virtual TIdentifier Identifier { get; init; }

    /// <summary>
    /// The instance of the entity itself, used for referential equality checks.
    /// </summary>
    protected virtual object EntityInstance => this;

    /// <summary>
    /// Base constructor to set the base properties of any entity
    /// </summary>
    /// <param name="identifier"></param>
    protected Entity(TIdentifier identifier)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
    }

    /// <summary>
    /// Method used to check referential equality between to entity objects.
    /// </summary>
    /// <param name="obj">The entity object to compare against.</param>
    /// <returns>A result predicated on equivalence, or otherwise (i.e. true or false).</returns>
    public override bool Equals(object? obj) =>
        obj is Entity<TIdentifier> comparableEntity &&
        (ReferenceEquals(this, comparableEntity) ||
        (EntityInstance.GetType() == comparableEntity.GetType() &&
        Identifier.Equals(comparableEntity.Identifier)));

    /// <summary>
    /// Gets the hash code for the entity based on its identifier and type.
    /// </summary>
    /// <returns>
    /// A hash code that represents the entity, calculated from its identifier and type.
    /// </returns>
    public override int GetHashCode() => HashCode.Combine(EntityInstance.GetType(), Identifier);
}
