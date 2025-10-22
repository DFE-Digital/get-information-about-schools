using System;
using Microsoft.AspNetCore.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class EdubaseAuthorizeAttribute : AuthorizeAttribute
{
    public EdubaseAuthorizeAttribute() : base("EdubasePolicy") { }

    public string Roles
    {
        get => base.Roles;
        set => base.Roles = value;
    }
}
