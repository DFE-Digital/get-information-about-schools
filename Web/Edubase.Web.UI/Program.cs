using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var sessionExpireValue = builder.Configuration["SessionExpireTimeSpan"];
if (!TimeSpan.TryParse(sessionExpireValue, out var sessionExpireTimeSpan))
{
    sessionExpireTimeSpan = TimeSpan.FromHours(8);
}

builder.Services.AddRouting();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = sessionExpireTimeSpan;
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = sessionExpireTimeSpan;
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.HeaderName = "X-CSRF-TOKEN";
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

