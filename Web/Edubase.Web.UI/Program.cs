using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using Edubase.Web.UI.App_Start;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
Edubase.Common.Config.Feature.Provider =  new ConfigurationFeatureFlagProvider(builder.Configuration);
DbConfiguration.SetConfiguration(new MicrosoftSqlDbConfiguration());

builder.Logging.AddConsole();
builder.Logging.AddDebug();
//builder.Services
//    .AddOpenTelemetry()
//    .UseAzureMonitor(options =>
//    {
//        options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
//    });

builder.Services.AddApplicationServices();
builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddMvcServices();
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddExternalApiServices(builder.Configuration);
builder.Services.AddApplicationAuthentication(builder.Configuration);

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

var app = builder.Build();

app.UseExceptionHandler("/error");
app.UseHsts();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();

if (builder.Configuration.GetValue<bool>("AppSettings:UseBasicAuth"))
{
    app.UseMiddleware<BasicAuthenticationGateMiddleware>();
}

app.UseStaticFiles();
var supportedCultures = new[] { new CultureInfo("en-GB") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-GB"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();
app.UseSession();           // Enables session state
app.MapControllers()
    .RequireSystemWebAdapterSession();

HtmlHelperExtensions.WebRootPath = app.Environment.WebRootPath;

app.UseAuthentication();
app.UseAuthorization();

app.Run();
