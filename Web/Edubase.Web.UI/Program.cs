using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using AzureTableLogger;
using AzureTableLogger.Services;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Core;
using Edubase.Services.DataQuality;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Geo;
using Edubase.Services.Governors;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.IntegrationEndPoints.AzureMaps;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.IntegrationEndPoints.OSPlaces;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
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
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.DataQuality;
using Edubase.Web.UI.Models.DataQuality.Validators;
using Edubase.Web.UI.Models.Notifications;
using Edubase.Web.UI.Models.Notifications.Validators;
using Edubase.Web.UI.Models.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;


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

static HttpClient CreateHttpClient(IConfiguration configuration)
{
    var timeoutSetting = configuration["AppSettings:HttpClient_Timeout"];
    var baseAddress = configuration["AppSettings:TexunaApiBaseAddress"];
    var apiUsername = configuration["AppSettings:api:Username"];
    var apiPassword = configuration["AppSettings:api:Password"];

    if (!int.TryParse(timeoutSetting, out var timeoutSeconds))
    {
        timeoutSeconds = 30; // default fallback
    }

    var client = new HttpClient(new HttpClientHandler { UseCookies = false })
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
    };

    if (!string.IsNullOrEmpty(apiUsername) && !string.IsNullOrEmpty(apiPassword))
    {
        var credentials = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes($"{apiUsername}:{apiPassword}")
        );
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }

    return client;
}

//static HttpClient CreateFscpdClient(IConfiguration configuration)
//{
//    var timeoutSetting = configuration["AppSettings:FscpdClient_Timeout"];
//    var baseAddress = configuration["AppSettings:FscpdURL"];
//    var apiUsername = configuration["AppSettings:FscpdUsername"];
//    var apiPassword = configuration["AppSettings:FscpdPassword"];

//    if (!int.TryParse(timeoutSetting, out var timeoutSeconds))
//    {
//        timeoutSeconds = 10;
//    }

//    var client = new HttpClient(new HttpClientHandler { UseCookies = false })
//    {
//        BaseAddress = new Uri(baseAddress),
//        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
//    };

//    if (!string.IsNullOrEmpty(apiUsername) && !string.IsNullOrEmpty(apiPassword))
//    {
//        var credentials = Convert.ToBase64String(
//            System.Text.Encoding.ASCII.GetBytes($"{apiUsername}:{apiPassword}")
//        );
//        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
//    }

//    return client;
//}

//static HttpClient CreateSfbClient(IConfiguration configuration)
//{
//    var timeoutSetting = configuration["AppSettings:FBService_Timeout"];
//    var baseAddress = configuration["AppSettings:FinancialBenchmarkingApiURL"];
//    var apiUsername = configuration["AppSettings:FinancialBenchmarkingUsername"];
//    var apiPassword = configuration["AppSettings:FinancialBenchmarkingPassword"];

//    if (!int.TryParse(timeoutSetting, out var timeoutSeconds))
//    {
//        timeoutSeconds = 10;
//    }

//    var client = new HttpClient(new HttpClientHandler { UseCookies = false })
//    {
//        BaseAddress = new Uri(baseAddress),
//        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
//    };

//    if (!string.IsNullOrEmpty(apiUsername) && !string.IsNullOrEmpty(apiPassword))
//    {
//        var credentials = Convert.ToBase64String(
//            System.Text.Encoding.ASCII.GetBytes($"{apiUsername}:{apiPassword}")
//        );
//        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
//    }

//    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
//    var productValue = new ProductInfoHeaderValue("GIAS", version);
//    var commentValue = new ProductInfoHeaderValue("(Chrome; Edge; Mozilla; +https://www.get-information-schools.service.gov.uk)");

//    client.DefaultRequestHeaders.UserAgent.Add(productValue);
//    client.DefaultRequestHeaders.UserAgent.Add(commentValue);

//    return client;
//}

//static HttpClient CreateOfstedClient(IConfiguration configuration)
//{
//    var timeoutSetting = configuration["AppSettings:OfstedService_TimeoutSeconds"];
//    var baseAddress = configuration["AppSettings:OfstedService_BaseAddress"];
//    var apiUsername = configuration["AppSettings:OfstedService_Username"];
//    var apiPassword = configuration["AppSettings:OfstedService_Password"];

//    if (!int.TryParse(timeoutSetting, out var timeoutSeconds))
//    {
//        timeoutSeconds = 10;
//    }

//    var client = new HttpClient(new HttpClientHandler { UseCookies = false })
//    {
//        BaseAddress = new Uri(baseAddress),
//        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
//    };

//    if (!string.IsNullOrEmpty(apiUsername) && !string.IsNullOrEmpty(apiPassword))
//    {
//        var credentials = Convert.ToBase64String(
//            System.Text.Encoding.ASCII.GetBytes($"{apiUsername}:{apiPassword}")
//        );
//        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
//    }

//    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
//    var productValue = new ProductInfoHeaderValue("GIAS", version);
//    var commentValue = new ProductInfoHeaderValue("(Chrome; Edge; Mozilla; +https://www.get-information-schools.service.gov.uk)");

//    client.DefaultRequestHeaders.UserAgent.Add(productValue);
//    client.DefaultRequestHeaders.UserAgent.Add(commentValue);

//    return client;
//}

static HttpClient CreateLookupClient(IConfiguration configuration)
{
    var baseAddress = configuration["AppSettings:LookupApiBaseAddress"];
    var username = configuration["AppSettings:LookupApiUsername"];
    var password = configuration["AppSettings:LookupApiPassword"];
    var timeoutSetting = configuration["AppSettings:LookupClient_Timeout"];

    var lookupUri = new Uri(baseAddress);

    if (!int.TryParse(timeoutSetting, out var timeoutsettings))
    {
        timeoutsettings = 30;
    }

    var client = new HttpClient(new HttpClientHandler { UseCookies = false })
    {
        BaseAddress = lookupUri,
        Timeout = TimeSpan.FromSeconds(timeoutsettings)
    };

    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
    {
        var credentials = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")
        );
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }

    return client;
}

static HttpClient CreateGovernorSearchClient(IConfiguration configuration)
{
    var baseAddress = configuration["AppSettings:GovernorSearchApiBaseAddress"];
    var username = configuration["AppSettings:GovernorSearchApiUsername"];
    var password = configuration["AppSettings:GovernorSearchApiPassword"];
    var timeoutSetting = configuration["AppSettings:GovernorSearchClient_Timeout"];

    if (!int.TryParse(timeoutSetting, out var timeoutSeconds))
    {
        timeoutSeconds = 30;
    }

    var client = new HttpClient(new HttpClientHandler { UseCookies = false })
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
    };

    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
    {
        var credentials = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")
        );
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }

    return client;
}

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

static HttpClient CreateAzureMapsClient(IConfiguration configuration)
{
    var timeoutSetting = configuration["AppSettings:AzureMapService_Timeout"];
    var baseUrl = configuration["AppSettings:AzureMapsUrl"];

    if (!int.TryParse(timeoutSetting, out var timeoutSeconds))
    {
        timeoutSeconds = 10;
    }

    var client = new HttpClient
    {
        BaseAddress = new Uri(baseUrl),
        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
    };

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
        policy.Requirements.Add(new EdubaseRequirement()))
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
builder.Services.AddScoped<IEstablishmentDownloadService, EstablishmentDownloadApiService>();
builder.Services.AddScoped<IGroupReadService, GroupReadApiService>();
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
builder.Services.AddTransient<IBlobService, BlobService>();
builder.Services.AddTransient<ILayoutHelper, LayoutHelper>();
builder.Services.AddScoped<HttpContextBase>(_ => new HttpContextWrapper(HttpContext.Current));
builder.Services.AddScoped<IClientStorage, BrowserClientStorage>();
builder.Services.AddSingleton<ApiRecorderSessionItemRepository>();
builder.Services.AddSingleton<WebLogItemRepository>();
builder.Services.AddSingleton<GlossaryRepository>();
builder.Services.AddSingleton<FaqItemRepository>();
builder.Services.AddSingleton<FaqGroupRepository>();
builder.Services.AddSingleton<NotificationBannerRepository>();
builder.Services.AddSingleton<NotificationTemplateRepository>();
builder.Services.AddSingleton<NewsArticleRepository>();
builder.Services.AddTransient<IGovernorsGridViewModelFactory, GovernorsGridViewModelFactory>();
builder.Services.AddTransient<ISmtpEndPoint, MockSmtpEndPoint>();
builder.Services.AddSingleton<IAzLogger, AzLogger>();
builder.Services.AddScoped<ICachedLookupService, CachedLookupService>();
builder.Services.AddSingleton<IPlacesLookupService, PlacesLookupService>();

// Default HttpClient and HttpClientWrapper
var httpClient = CreateHttpClient(builder.Configuration); // Make sure this method doesn't rely on ConfigurationManager
builder.Services.AddSingleton(httpClient);

builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddTransient<HttpClientWrapper>();
builder.Services.AddScoped<ISecurityService, SecurityApiService>();

// Register the Azure Maps HttpClient
var azureMapsClient = CreateAzureMapsClient(builder.Configuration);
builder.Services.AddSingleton(azureMapsClient);

// Register the OS Places HttpClient
var osPlacesClient = CreateOSPlacesClient(builder.Configuration);
builder.Services.AddSingleton(osPlacesClient);

builder.Services.AddSingleton<IOSPlacesApiService>(provider =>
{
    var client = provider.GetRequiredService<HttpClient>();
    return new OSPlacesApiService(client);
});

builder.Services.AddSingleton<IPlacesLookupService, PlacesLookupService>();

builder.Services.AddSingleton<IAzureMapsService>(provider =>
{
    var client = provider.GetRequiredService<HttpClient>();
    return new AzureMapsService(builder.Configuration, client);
});

// Register the AzureMapsService
builder.Services.AddSingleton<IAzureMapsService>(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new AzureMapsService(builder.Configuration, httpClient);
});

var dataConnectionString = builder.Configuration.GetConnectionString("DataConnectionString");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

var cloudStorageAccount = CloudStorageAccount.Parse(dataConnectionString);
builder.Services.AddTransient<IBlobService, BlobService>();
builder.Services.AddSingleton(cloudStorageAccount);

builder.Services.AddSingleton(new LoggingServicePolicy
{
    FlushInterval = TimeSpan.FromSeconds(30),
    RetentionCheckInterval = TimeSpan.FromDays(1),
    RetentionCutoffAge = 90,
    UsagePolicy = UsagePolicy.SCHEDULED
});

builder.Services.AddSingleton<ILoggingService>(provider =>
{
    var policy = provider.GetRequiredService<LoggingServicePolicy>();
    var account = provider.GetRequiredService<CloudStorageAccount>();
    var tableName = "AZTLoggerMessages"; // or pull from config if needed

    return new LoggingService(policy, account, tableName);
});

builder.Services.AddSingleton(cloudStorageAccount);

builder.Services.AddSingleton<ICacheAccessor>(provider =>
{
    var converters = provider.GetRequiredService<JsonConverterCollection>();
    return new CacheAccessor(converters);
});

var dbGeographyConverter = new DbGeographyConverter();
var jsonConverterCollection = new JsonConverterCollection { dbGeographyConverter };
builder.Services.AddSingleton(jsonConverterCollection);

var jsonFormatter = CreateJsonMediaTypeFormatter();
builder.Services.AddSingleton(jsonFormatter);

// Named HttpClient for Lookup Service
var lookupClient = CreateLookupClient(builder.Configuration);
builder.Services.AddSingleton(lookupClient);
builder.Services.AddSingleton<HttpClientWrapper>(provider =>
{
    var formatter = provider.GetRequiredService<JsonMediaTypeFormatter>();
    var storage = provider.GetRequiredService<IClientStorage>();
    var recorder = provider.GetRequiredService<ApiRecorderSessionItemRepository>();
    return new HttpClientWrapper(lookupClient, formatter, storage, recorder);
});

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


builder.Services.AddScoped<LookupApiService>(provider =>
{
    var wrapper = provider.GetRequiredService<HttpClientWrapper>();
    var security = provider.GetRequiredService<ISecurityService>();
    return new LookupApiService(wrapper, security);
});

builder.Services.AddScoped<ILookupService>(provider => provider.GetRequiredService<LookupApiService>());
builder.Services.AddScoped<IUserDependentLookupService>(provider => provider.GetRequiredService<LookupApiService>());


builder.Services.AddScoped<IUserDependentLookupService>(provider =>
    provider.GetRequiredService<LookupApiService>());


// Governors Read Service
builder.Services.AddScoped<IGovernorsReadService>(provider =>
{
    var defaultWrapper = provider.GetRequiredService<IHttpClientWrapper>() as HttpClientWrapper;
    var governorWrapper = provider.GetServices<HttpClientWrapper>().Last(); // Assumes order
    var establishment = provider.GetRequiredService<IEstablishmentReadService>();
    return new GovernorsReadApiService(defaultWrapper, governorWrapper, establishment);
});

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

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Unhandled exception:");
        Console.WriteLine(ex.ToString());
        throw;
    }
});

app.UseRouting();
app.UseSession();           // Enables session state
app.UseAuthorization();
app.UseSession();
app.MapControllers()
    .RequireSystemWebAdapterSession();

app.UseDeveloperExceptionPage();

app.Run();
