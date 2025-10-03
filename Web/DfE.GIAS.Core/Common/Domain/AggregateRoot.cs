namespace DfE.GIAS.Core.Common.Domain;

/// <summary>
/// Abstract base class for an aggregate root in a domain-driven design context.
/// </summary>
/// <typeparam name="TIdentifier">
/// The type of the identifier for the aggregate root, which must be a value object.
/// </typeparam>
public abstract class AggregateRoot<TIdentifier> : Entity<TIdentifier>, IAggregateRoot<TIdentifier>
        where TIdentifier : ValueObject<TIdentifier>
{
    /// <summary>
    /// Base constructor to set the base properties of any aggregate root.
    /// </summary>
    /// <param name="identifier">
    /// The unique identifier for the aggregate root, which is used to distinguish it from other aggregates.
    /// </param>
    protected AggregateRoot(TIdentifier identifier) : base(identifier){
    }

    /// <summary>
    /// Gets the unique identifier for the aggregate root.
    /// </summary>
    public TIdentifier AggregateId => Identifier;
}
