namespace DfE.GIAS.Core.Common.CrossCutting;

/// <summary>
/// Define the contract used to define mapping behaviour across all application concerns.
/// </summary>
/// <typeparam name="TMapFrom">
/// The type to map from.
/// </typeparam>
/// <typeparam name="TMapTo">
/// The type to map to.
/// </typeparam>
public interface IMapper<in TMapFrom, out TMapTo>
{
    /// <summary>
    /// Defines the method call for mapping one type to another.
    /// </summary>
    /// <param name="input">
    /// The type to map from.
    /// </param>
    /// <returns>
    /// The type to map to.
    /// </returns>
    TMapTo Map(TMapFrom input);
}
