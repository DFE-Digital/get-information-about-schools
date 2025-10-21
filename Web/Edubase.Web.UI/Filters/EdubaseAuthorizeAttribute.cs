using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Custom authorization handler for the EdubasePolicy.
/// Determines how to respond when a user does or does not meet the authorization requirement.
/// </summary>
public class EdubaseAuthorizationHandler : AuthorizationHandler<EdubaseRequirement>
{
    /// <summary>
    /// Handles the authorization logic for the EdubaseRequirement.
    /// </summary>
    /// <param name="context">Provides information about the current authorization attempt, including the user and resource.</param>
    /// <param name="requirement">The specific requirement being evaluated.</param>
    /// <returns>A completed task indicating the result of the authorization check.</returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EdubaseRequirement requirement)
    {
        // Check if the user is authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // User is authenticated but does not meet the requirement
            // Authorization fails and results in a 403 Forbidden response
            context.Fail();
        }
        else
        {
            // User is not authenticated
            // Authorization fails and triggers an authentication challenge (401 Unauthorized)
            context.Fail();
        }

        // Return a completed task since no asynchronous operations are needed
        return Task.CompletedTask;
    }
}

/// <summary>
/// Represents a custom authorization requirement for Edubase.
/// This class can be extended to include additional logic or parameters.
/// </summary>
public class EdubaseRequirement : IAuthorizationRequirement
{
    // Currently empty — can be extended with custom properties or logic if needed
}
