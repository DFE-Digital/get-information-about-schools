using System;
using System.Threading.Tasks;
using Edubase.Web.UI;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemWebAdapters()
    .AddWrappedAspNetCoreSession()
    .AddJsonSessionSerializer(options =>
    {
        options.RegisterKey<string>("MachineName");
        options.RegisterKey<string>("SessionStartTime");
    })
    .AddHttpApplication<MvcApplication>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanManageAcademyOpeningsOrSecureAcademy16To19", policy =>
        policy.RequireRole(
            AuthorizedRoles.CanManageAcademyOpenings,
            AuthorizedRoles.CanManageSecureAcademy16To19Openings))
    .AddPolicy("CanBulkAssociateEstabs2Groups", policy =>
        policy.RequireRole(AuthorizedRoles.CanBulkAssociateEstabs2Groups))
    .AddPolicy("CanBulkCreateFreeSchools", policy =>
        policy.RequireRole(AuthorizedRoles.CanBulkCreateFreeSchools))
    .AddPolicy("EdubasePolicy", policy =>
        policy.Requirements.Add(new EdubaseRequirement()));


builder.Services.AddAuthentication("Saml2")
    .AddCookie("Saml2", options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            var returnUrl = context.Request.Path + context.Request.QueryString;
            var redirectUrl = $"/Account/ExternalLoginCallback?ReturnUrl={Uri.EscapeDataString(returnUrl)}";
            context.Response.Redirect(redirectUrl);
            return Task.CompletedTask;
        };
    });


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseSystemWebAdapters();

app.MapControllers()
    .RequireSystemWebAdapterSession();

app.Run();
