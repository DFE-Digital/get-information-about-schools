using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using Edubase.Data;
using Edubase.Data.Repositories.EF;
using Edubase.Web.UI.Controllers.Api;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ValueProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.App_Start;

public static class CoreServiceRegistration
{
    public static void AddWebUiFoundationServices(this IServiceCollection services, IConfiguration configuration)
    {
        DbConfiguration.SetConfiguration(new MicrosoftSqlDbConfiguration());
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);

        var sessionExpireValue = configuration["SessionExpireTimeSpan"];
        if (!TimeSpan.TryParse(sessionExpireValue, out var sessionExpireTimeSpan))
        {
            sessionExpireTimeSpan = TimeSpan.FromHours(8);
        }

        services.AddRouting();
        services.AddHttpContextAccessor();
        services.AddScoped<DbConnection>(_ => new Microsoft.Data.SqlClient.SqlConnection(SqlLaNameCodeRepository.BuildConnectionString(configuration)));
        services.AddScoped<FrontEndDbContext>();
        services.AddScoped<INewsArticleRepository, SqlNewsArticleRepository>();
        services.AddControllersWithViews(options =>
        {
            CoreFilterConfig.RegisterGlobalFilters(options);
            options.ValueProviderFactories.Insert(0, new BindAliasValueProviderFactory());
            options.ValueProviderFactories.Add(new TokenValueProviderFactory());
        });

        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.IdleTimeout = sessionExpireTimeSpan;
        });

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = sessionExpireTimeSpan;
                options.SlidingExpiration = true;
            });

        services.AddAuthorization();
        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.HeaderName = "X-CSRF-TOKEN";
        });
    }
}
