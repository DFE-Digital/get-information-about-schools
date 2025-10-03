namespace DfE.GIAS.Core.Common.Application;

/// <summary>
/// Defines a use-case with an input request and an expected response.
/// </summary>
/// <typeparam name="TUseCaseRequest">
/// The type of request (input port) passed to a use case.</typeparam>
/// <typeparam name="TUseCaseResponse">
/// The type of response (output) returned from the use case.
/// </typeparam>
public interface IUseCase<in TUseCaseRequest, TUseCaseResponse>
    where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse>
    where TUseCaseResponse : class
{
    /// <summary>
    /// Handles the request and returns the response.
    /// </summary>
    /// <param name="request">The input request.</param>
    /// <returns>The output response object.</returns>
    Task<TUseCaseResponse> HandleRequestAsync(TUseCaseRequest request);
}

/// <summary>
/// Defines a use-case where only a request is provided, but no response is returned.
/// Useful for commands or operations without direct output.
/// </summary>
/// <typeparam name="TUseCaseRequest">
/// The output response type expected to be returned by the use case.
/// </typeparam>
public partial interface IUseCaseRequestOnly<in TUseCaseRequest>
    where TUseCaseRequest : IUseCaseRequest
{
    /// <summary>
    /// Handles the request without returning a response.
    /// </summary>
    /// <param name="request">The input request.</param>
    Task HandleRequestAsync(TUseCaseRequest request);
}

/// <summary>
/// Defines a use-case where only no request is provided, but a response is returned.
/// </summary>
/// <typeparam name="TUseCaseResponse">
/// The output response type expected to be returned by the use case.
/// </typeparam>
public partial interface IUseCaseResponseOnly<TUseCaseResponse>
    where TUseCaseResponse : class
{
    /// <summary>
    /// Handles the no request case and returns a response.
    /// </summary>
    /// <returns>The output response.</returns>
    Task<TUseCaseResponse> HandleRequestAsync();
}
