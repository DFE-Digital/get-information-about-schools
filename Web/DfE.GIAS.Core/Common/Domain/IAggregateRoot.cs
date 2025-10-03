namespace DfE.GIAS.Core.Common.Domain;

/// <summary>
/// Interface representing an aggregate root in a domain-driven design context.
/// </summary>
/// <typeparam name="TIdentifier">
/// The type of the identifier for the aggregate root, which must be a value object.
/// </typeparam>
public interface IAggregateRoot<TIdentifier> where TIdentifier : ValueObject<TIdentifier>
{
    /// <summary>
    /// Gets the unique identifier for the aggregate root.
    /// </summary>
    TIdentifier AggregateId { get; }
}
