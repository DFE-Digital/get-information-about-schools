using System.Linq;
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
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Fail(); // triggers 401
            return Task.CompletedTask;
        }

        if (requirement.AllowedRoles.Length == 0 ||
            requirement.AllowedRoles.Any(role => context.User.IsInRole(role)))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(); // triggers 403
        }

        return Task.CompletedTask;
    }
}

