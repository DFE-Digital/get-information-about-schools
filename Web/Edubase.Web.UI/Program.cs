using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Autofac.Core;
using AzureTableLogger;
using AzureTableLogger.Services;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.Repositories;
using Edubase.Services;
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
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<IGovernorsGridViewModelFactory, GovernorsGridViewModelFactory>();
builder.Services.AddTransient<ISmtpEndPoint, MockSmtpEndPoint>();
builder.Services.AddSingleton<IAzLogger, AzLogger>();
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

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
    options.ValueProviderFactories.Add(new TokenValueProviderFactory(serviceProvider.GetRequiredService<ITokenRepository>()));
    options.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider(serviceProvider));
});

// Default HttpClient and HttpClientWrapper
HttpClient httpClient = CreateHttpClient(builder.Configuration);
builder.Services.AddSingleton(httpClient);

builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddTransient<HttpClientWrapper>();
builder.Services.AddScoped<ISecurityService, SecurityApiService>();

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
