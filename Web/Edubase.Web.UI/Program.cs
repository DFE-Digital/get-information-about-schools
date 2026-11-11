using System;
using System.Security.Cryptography;
using Edubase.Web.UI.App_Start;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebUiFoundationServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/500");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-AspNet-Version");

    var securityPolicy = app.Configuration["Content-Security-Policy"];
    if (!string.IsNullOrWhiteSpace(securityPolicy) && securityPolicy.Contains("nonce-inject", StringComparison.Ordinal))
    {
        var nonceBytes = RandomNumberGenerator.GetBytes(32);
        var nonce = Convert.ToBase64String(nonceBytes);

        context.Items["ScriptNonce"] = nonce;
        securityPolicy = securityPolicy.Replace("nonce-inject", $"nonce-{nonce}", StringComparison.Ordinal);
        context.Response.Headers["Content-Security-Policy"] = securityPolicy;
        context.Response.Headers["X-Content-Security-Policy"] = securityPolicy;
    }

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapGet("/health", () => Results.Ok("ok"));
app.MapGet("/service.wsdl", () => Results.NotFound());

CoreRouteConfig.MapRoutes(app);

app.Run();

