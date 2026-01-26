using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using Azure.Data.Tables;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Approvals;
using Edubase.Services.Core;
using Edubase.Services.DataQuality;
using Edubase.Services.Downloads;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Geo;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.IntegrationEndPoints.AzureMaps;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.IntegrationEndPoints.OSPlaces;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Services.Texuna.Approvals;
using Edubase.Services.Texuna.ChangeHistory;
using Edubase.Services.Texuna.Downloads;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Governors;
using Edubase.Services.Texuna.Groups;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using Edubase.Services.Texuna.Serialization;
using Edubase.Web.Resources;
using Edubase.Web.UI.Areas;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models.Validators;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.Models.Validators;
using Edubase.Web.UI.Authentication;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.DataQuality;
using Edubase.Web.UI.Models.DataQuality.Validators;
using Edubase.Web.UI.Models.Notifications;
using Edubase.Web.UI.Models.Notifications.Validators;
using Edubase.Web.UI.Models.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using Sustainsys.Saml2.AspNetCore2;

var builder = WebApplication.CreateBuilder(args);

static JsonMediaTypeFormatter CreateJsonMediaTypeFormatter()
{
    return new JsonMediaTypeFormatter
    {
        SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new TexunaCamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
        }
    };
}

static HttpClient CreateConfiguredClient(
    IConfiguration configuration,
    string baseAddressKey,
    string timeoutKey,
    string usernameKey,
    string passwordKey)
{
    var baseAddress = configuration[baseAddressKey];
    var timeoutSetting = configuration[timeoutKey];
    var username = configuration[usernameKey];
    var password = configuration[passwordKey];

    if (!int.TryParse(timeoutSetting, out var timeoutSeconds))
    {
        timeoutSeconds = 30; // default fallback
    }

    HttpClient client = new(new HttpClientHandler { UseCookies = false })
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
    };

    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
    {
        var credentials = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
    }

    return client;
}

static HttpClient CreateHttpClient(IConfiguration configuration) =>
    CreateConfiguredClient(
        configuration,
        "AppSettings:TexunaApiBaseAddress",
        "AppSettings:HttpClient_Timeout",
        "AppSettings:api:Username",
        "AppSettings:api:Password");

static HttpClient CreateLookupClient(IConfiguration configuration) =>
    CreateConfiguredClient(
        configuration,
        "AppSettings:LookupApiBaseAddress",
        "AppSettings:LookupClient_Timeout",
        "AppSettings:LookupApiUsername",
        "AppSettings:LookupApiPassword");

static HttpClient CreateGovernorSearchClient(IConfiguration configuration) =>
    CreateConfiguredClient(
        configuration,
        "AppSettings:GovernorSearchApiBaseAddress",
        "AppSettings:GovernorSearchClient_Timeout",
        "AppSettings:GovernorSearchApiUsername",
        "AppSettings:GovernorSearchApiPassword"
    );

static HttpClient CreateOSPlacesClient(IConfiguration configuration)
{
    if (!int.TryParse(configuration["OSPlacesApiServices_Timeout"], out var timeoutsettings))
    {
        timeoutsettings = 10;
    }

    var baseAddress = configuration["AppSettings:OSPlacesApiBaseUrl"];
    var apiKey = configuration["AppSettings:OSPlacesApiKey"];

    var client = new HttpClient
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromSeconds(timeoutsettings)
    };

    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    return client;
}

builder.Services.AddDistributedMemoryCache();                               // Required for ASP.NET Core session
builder.Services.AddSession();                                              // ASP.NET Core session middleware
builder.Services.AddSystemWebAdapters().AddWrappedAspNetCoreSession();      // Registers SystemWebAdapters core services

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanManageAcademyOpeningsOrSecureAcademy16To19", policy =>
        policy.RequireRole(
            string.Join(
                AuthorizedRoles.CanManageAcademyOpenings,
                AuthorizedRoles.CanManageSecureAcademy16To19Openings).Split(",")))
    .AddPolicy("CanBulkAssociateEstabs2Groups", policy =>
        policy.RequireRole(AuthorizedRoles.CanBulkAssociateEstabs2Groups.Split(",")))
    .AddPolicy("CanBulkCreateFreeSchools", policy =>
        policy.RequireRole(AuthorizedRoles.CanBulkCreateFreeSchools.Split(",")))
    .AddPolicy("CanBulkUpdateEstablishments", policy =>
        policy.RequireRole(AuthorizedRoles.CanBulkUpdateEstablishments.Split(",")))
    .AddPolicy("EdubasePolicy", policy =>
        policy.Requirements.Add(new EdubaseRequirement("Admin")));

// Model validator registrations. 
builder.Services
    .AddScoped<IValidator<GovernorsBulkUpdateViewModel>, GovernorsBulkUpdateViewModelValidator>()
    .AddScoped<IValidator<BulkUpdateViewModel>, BulkUpdateViewModelValidator>()
    .AddScoped<IValidator<EditEstablishmentModel>, EditEstablishmentModelValidator>()
    .AddScoped<IValidator<GovernorsGridViewModel>, GovernorsGridViewModelValidator>()
    .AddScoped<IValidator<ReplaceChairViewModel>, ReplaceChairViewModelValidator>()
    .AddScoped<IValidator<EditGroupDelegationInformationViewModel>, EditGroupDelegationInformationViewModelValidator>()
    .AddScoped<IValidator<SelectSharedGovernorViewModel>, SelectSharedGovernorViewModelValidator>()
    .AddScoped<IValidator<BulkCreateFreeSchoolsViewModel>, BulkCreateFreeSchoolsViewModelValidator>()
    .AddScoped<IValidator<BulkAssociateEstabs2GroupsViewModel>, BulkAssociateEstabs2GroupsViewModelValidator>()
    .AddScoped<IValidator<GovernorsBulkUpdateViewModel>, GovernorsBulkUpdateViewModelValidator>()
    .AddScoped<IValidator<CreateEstablishmentViewModel>, CreateEstablishmentViewModelValidator>()
    .AddScoped<IValidator<EditEstablishmentLinksViewModel>, EditEstablishmentLinksViewModelValidator>()
    .AddScoped<IValidator<ChangeHistoryViewModel>, ChangeHistoryViewModelValidator>()
    .AddScoped<IValidator<DateTimeViewModel>, DateTimeViewModelValidator>()
    .AddScoped<IValidator<DataQualityStatusItem>, DataQualityStatusItemValidator>()
    .AddScoped<IValidator<ValidateChildrensCentreStep2>, ValidateChildrensCentreStep2Validator>()
    .AddScoped<IValidator<NotificationsBannerViewModel>, NotificationsBannerValidator>();

// service registrations
builder.Services.AddScoped<IEstablishmentReadService, EstablishmentReadApiService>();
builder.Services.AddScoped<IEstablishmentWriteService, EstablishmentWriteApiService>();
builder.Services.AddScoped<IEstablishmentDownloadService, EstablishmentDownloadApiService>();
builder.Services.AddScoped<IGroupReadService, GroupReadApiService>();
builder.Services.AddScoped<IGroupsWriteService, GroupsWriteApiService>();
builder.Services.AddScoped<IGroupDownloadService, GroupDownloadApiService>();
builder.Services.AddScoped<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddTransient<IResourcesHelper, ResourcesHelper>();
builder.Services.AddTransient<ICompaniesHouseService, CompaniesHouseService>();
builder.Services.AddTransient<IDataQualityWriteService, DataQualityWriteService>();
builder.Services.AddTransient<IDataQualityReadService, DataQualityReadService>();
builder.Services.AddTransient<IDataQualityStatusRepository, DataQualityStatusRepository>();
builder.Services.AddTransient<ILocalAuthoritySetRepository, LocalAuthoritySetRepository>();
builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
builder.Services.AddSingleton<IUserPreferenceRepository, UserPreferenceRepository>();
builder.Services.AddTransient<ILayoutHelper, LayoutHelper>();
builder.Services.AddScoped<HttpContextBase>(_ => new HttpContextWrapper(System.Web.HttpContext.Current));
builder.Services.AddScoped<IClientStorage, BrowserClientStorage>();
builder.Services.AddSingleton<ApiRecorderSessionItemRepository>();
builder.Services.AddSingleton<WebLogItemRepository>();
builder.Services.AddSingleton<GlossaryRepository>();
builder.Services.AddSingleton<FaqItemRepository>();
builder.Services.AddSingleton<FaqGroupRepository>();
builder.Services.AddSingleton<NotificationBannerRepository>();
builder.Services.AddSingleton<NotificationTemplateRepository>();
builder.Services.AddSingleton<NewsArticleRepository>();
builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<IGovernorsGridViewModelFactory, GovernorsGridViewModelFactory>();
builder.Services.AddTransient<ISmtpEndPoint, MockSmtpEndPoint>();
builder.Services.AddScoped<ICachedLookupService, CachedLookupService>();
builder.Services.AddScoped<IPlacesLookupService, PlacesLookupService>();
builder.Services.AddScoped<IExternalLookupService, ExternalLookupService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClientStorage, BrowserClientStorage>();
builder.Services.AddScoped<IGovernorDownloadService, GovernorDownloadApiService>();
builder.Services.AddScoped<IDownloadsService, DownloadsApiService>();
builder.Services.AddAutoMapper(typeof(Program));

//Default model binder registrations
builder.Services.AddSingleton<ICollectionFactory, CollectionFactory>();
builder.Services.AddSingleton<ITypeFactory, TypeFactory>();
builder.Services.AddSingleton<IValueConverter, DefaultValueConverter>();
builder.Services.AddSingleton<ITypeConverter, DefaultTypeConverter>();
builder.Services.AddSingleton<IModelBinderProvider, DefaultModelBinderProvider>();
builder.Services.AddTransient<IPropertyBinderHandler, AliasBinderHandler>();
builder.Services.AddTransient<IPropertyBinderHandler, ArrayBinderHandler>();
builder.Services.AddTransient<IPropertyBinderHandler, ListBinderHandler>();
builder.Services.AddTransient<IPropertyBinderHandler, ComplexTypeBinderHandler>();
builder.Services.AddTransient<IPropertyBinderHandler, PropertyNameBinderHandler>();
builder.Services.AddTransient<IPropertyBinderHandler, SimpleTypeBinderHandler>();

const string DataConnectionString = "DataConnectionString";
const string RedisConnectionString = "Redis";
var dataConnectionString =
    builder.Configuration.GetConnectionString(DataConnectionString);
var redisConnectionString =
    builder.Configuration.GetConnectionString(RedisConnectionString);

builder.Services.AddTransient<IBlobService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString(dataConnectionString);
    return new BlobService(connectionString);
});

var tableServiceClient =
    new TableServiceClient(dataConnectionString);

// Register the TableServiceClient instead of CloudStorageAccount
builder.Services.AddSingleton(tableServiceClient);
builder.Services.AddSingleton<ILoggingService>(provider =>
{
    var tableService = provider.GetRequiredService<TableServiceClient>();
    const string tableName = "AZTLoggerMessages";
    return new LoggingService(tableService, tableName);
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
    options.ValueProviderFactories.Add(
        new TokenValueProviderFactory(serviceProvider.GetRequiredService<ITokenRepository>()));
    options.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider(serviceProvider));
});

// Default HttpClient and HttpClientWrapper
HttpClient httpClient = CreateHttpClient(builder.Configuration);
builder.Services.AddSingleton(httpClient);

builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddTransient<HttpClientWrapper>();
builder.Services.AddScoped<ISecurityService, SecurityApiService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

builder.Services.AddSingleton<ICacheAccessor>(provider =>
{
    var converters = provider.GetRequiredService<JsonConverterCollection>();
    var redis = provider.GetRequiredService<IConnectionMultiplexer>();
    return new CacheAccessor(converters, redis);
});

DbGeographyConverter dbGeographyConverter = new();
JsonConverterCollection jsonConverterCollection = [dbGeographyConverter];
builder.Services.AddSingleton(jsonConverterCollection);
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new DbGeographyConverter());
    });

// Named HttpClient for Lookup Service
builder.Services.AddHttpClient("LookupApiClient", client => CreateLookupClient(builder.Configuration));
builder.Services.AddScoped<LookupApiService>(provider =>
{
    var wrapper = provider.GetRequiredService<HttpClientWrapper>();
    var security = provider.GetRequiredService<ISecurityService>();
    return new LookupApiService(wrapper, security);
});
builder.Services.AddScoped<ILookupService>(provider => provider.GetRequiredService<LookupApiService>());
builder.Services.AddScoped<IUserDependentLookupService>(provider => provider.GetRequiredService<LookupApiService>());
builder.Services.AddSingleton<HttpClient>(provider => CreateOSPlacesClient(builder.Configuration));
builder.Services.AddSingleton<IOSPlacesApiService>(provider =>
{
    var osPlacesClient = provider.GetRequiredService<HttpClient>();
    return new OSPlacesApiService(osPlacesClient);
});
builder.Services.AddTransient<IPlacesLookupService, PlacesLookupService>();
builder.Services.AddTransient<IApprovalService, ApprovalService>();
builder.Services.AddTransient<IChangeHistoryService, ChangeHistoryService>();

JsonMediaTypeFormatter jsonFormatter = CreateJsonMediaTypeFormatter();
builder.Services.AddSingleton(jsonFormatter);

// Named HttpClient for Governor Search Service
var governorClient = CreateGovernorSearchClient(builder.Configuration);
builder.Services.AddSingleton(governorClient);
builder.Services.AddScoped<HttpClientWrapper>(provider =>
{
    var formatter = provider.GetRequiredService<JsonMediaTypeFormatter>();
    var storage = provider.GetRequiredService<IClientStorage>();
    var recorder = provider.GetRequiredService<ApiRecorderSessionItemRepository>();
    return new HttpClientWrapper(governorClient, formatter, storage, recorder);
});

// Register the Azure Maps HttpClient
builder.Services.AddHttpClient("AzureMapsClient", client =>
{
    client.BaseAddress =
        new Uri(builder.Configuration["AppSettings:AzureMapsUrl"]);
});
builder.Services.AddSingleton<IAzureMapsService>(provider =>
{
    var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AzureMapsClient");
    return new AzureMapsService(builder.Configuration, client);
});

// CSCP service registrations.
builder.Services.AddHttpClient("FscpdClient", client =>
{
    client.BaseAddress =
        new Uri(builder.Configuration["AppSettings:FscpdURL"]);
});
builder.Services.AddScoped<IFSCPDService>(provider =>
{
    var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("FscpdClient");
    return new FSCPDService(client, builder.Configuration);
});

// Financial Benchmarking service registrations.
builder.Services.AddHttpClient("SfbClient", client =>
{
    client.BaseAddress =
        new Uri(builder.Configuration["AppSettings:FinancialBenchmarkingApiURL"]);
});
builder.Services.AddScoped<IFBService>(provider =>
{
    var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("SfbClient");
    return new FBService(client, builder.Configuration);
});

// Ofsted service registrations.
builder.Services.AddHttpClient("OfstedClient", client =>
{
    client.BaseAddress =
        new Uri(builder.Configuration["AppSettings:OfstedService_BaseAddress"]);
});
builder.Services.AddScoped<IOfstedService>(provider =>
{
    var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("OfstedClient");
    return new OfstedService(client, builder.Configuration);
});

// Governors Read Service
builder.Services.AddScoped<IGovernorsReadService>(provider =>
{
    var defaultWrapper = provider.GetRequiredService<IHttpClientWrapper>() as HttpClientWrapper;
    var governorWrapper = provider.GetServices<HttpClientWrapper>().Last();
    var establishment = provider.GetRequiredService<IEstablishmentReadService>();
    return new GovernorsReadApiService(defaultWrapper, governorWrapper, establishment);
});

// Governors Write Service
builder.Services.AddScoped<IGovernorsWriteService>(provider =>
{
    var defaultWrapper = provider.GetRequiredService<IHttpClientWrapper>() as HttpClientWrapper;
    return new GovernorsWriteApiService(defaultWrapper);
});

// -------------------- Authentication --------------------
//
// Determines whether the simulator SAML2 authentication flow should be used.
// Controlled by the AppSettings:UseSimulatorAuth configuration flag.
bool useSimulator = builder.Configuration.GetValue<bool>("AppSettings:UseSimulatorAuth");

/// <summary>
/// Configures the authentication schemes for the application.
/// Sets up the default cookie schemes and toggles between real and simulator SAML2 flows.
/// </summary>
var authBuilder = builder.Services.AddAuthentication(options =>
{
    // Cookies handle sign-in/out and forbid
    options.DefaultScheme = "ApplicationCookie";

    // SAML is used only when we need to challenge (redirect to IdP)
    options.DefaultChallengeScheme = Saml2Defaults.Scheme;

    // Important: forbid should go to cookies, not SAML
    options.DefaultForbidScheme = "ApplicationCookie";
})
.AddCookie("ApplicationCookie", options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

    // Critical for SAML state cookies to survive IdP redirects
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddCookie("ExternalCookie", options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    // SameSite/Secure here too
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

/// <summary>
/// Registers either the simulator or real SAML2 authentication flow
/// depending on the configuration flag.
/// </summary>
if (useSimulator)
{
    authBuilder.AddSaml2SimulatorAuthenticationFlow(builder.Configuration);
}
else
{
    authBuilder.AddSaml2AuthenticationFlow(builder.Configuration);
}
//
// -------------------- End of Authentication --------------------

// ---------------------- Authorization ------------------------
//
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EdubasePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});
//
// -------------------- End of Authorization --------------------


builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error"); // Optional: route to a custom error page
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
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
app.UseAuthorization();
app.UseSession();
app.MapControllers()
    .RequireSystemWebAdapterSession();

app.UseDeveloperExceptionPage();

HtmlHelperExtensions.WebRootPath = app.Environment.WebRootPath;

app.Run();
