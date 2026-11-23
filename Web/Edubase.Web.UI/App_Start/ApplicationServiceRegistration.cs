using System.Web;
using Edubase.Services.Approvals;
using Edubase.Services.Core;
using Edubase.Services.DataQuality;
using Edubase.Services.Downloads;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Geo;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.Approvals;
using Edubase.Services.Texuna.ChangeHistory;
using Edubase.Services.Texuna.Downloads;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Groups;
using Edubase.Web.Resources;
using Edubase.Web.UI.Areas;
using Edubase.Web.UI.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.App_Start
{
    public static class ApplicationServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // service registrations
            services.AddScoped<IEstablishmentReadService, EstablishmentReadApiService>();
            services.AddScoped<IEstablishmentWriteService, EstablishmentWriteApiService>();
            services.AddScoped<IEstablishmentDownloadService, EstablishmentDownloadApiService>();
            services.AddScoped<IGroupReadService, GroupReadApiService>();
            services.AddScoped<IGroupsWriteService, GroupsWriteApiService>();
            services.AddScoped<IGroupDownloadService, GroupDownloadApiService>();
            services.AddTransient<IResourcesHelper, ResourcesHelper>();
            services.AddTransient<ICompaniesHouseService, CompaniesHouseService>();
            services.AddTransient<IDataQualityWriteService, DataQualityWriteService>();
            services.AddTransient<IDataQualityReadService, DataQualityReadService>();
            services.AddTransient<ILayoutHelper, LayoutHelper>();
            services.AddScoped<HttpContextBase>(_ => new HttpContextWrapper(System.Web.HttpContext.Current));
            services.AddScoped<IClientStorage, BrowserClientStorage>();
            services.AddTransient<IGovernorsGridViewModelFactory, GovernorsGridViewModelFactory>();
            services.AddTransient<ISmtpEndPoint, MockSmtpEndPoint>();
            services.AddScoped<ICachedLookupService, CachedLookupService>();
            services.AddScoped<IExternalLookupService, ExternalLookupService>();
            services.AddScoped<IDownloadsService, DownloadsApiService>();

            services.AddTransient<IPlacesLookupService, PlacesLookupService>();
            services.AddTransient<IApprovalService, ApprovalService>();
            services.AddTransient<IChangeHistoryService, ChangeHistoryService>();
        }
    }
}
