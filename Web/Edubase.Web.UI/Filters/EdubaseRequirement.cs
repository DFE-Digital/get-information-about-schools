using Microsoft.AspNetCore.Authorization;

public class EdubaseRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public EdubaseRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles ?? [];
    }
}
