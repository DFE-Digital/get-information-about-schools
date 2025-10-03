namespace DfE.GIAS.Core.Common.Application;

/// <summary>
/// Marker contract which defines the response object to be associated with a use-case request.
/// </summary>
/// <typeparam name="TUseCaseResponse">The runtime type definition of the use-case response object.</typeparam>
public interface IUseCaseRequest<out TUseCaseResponse> where TUseCaseResponse : class{
}

/// <summary>
/// Marker contract which defines response-less use-case request.
/// </summary>
public interface IUseCaseRequest{
}
