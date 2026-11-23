using System;
using Edubase.Data;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models.Validators;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.Models.Validators;
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
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.App_Start
{
    public static class MvcServiceRegistration
    {
        public static void AddMvcServices(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();                               // Required for ASP.NET Core session
            services.AddSystemWebAdapters().AddWrappedAspNetCoreSession();      // Registers SystemWebAdapters core services

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Model validator registrations
            services
                .AddScoped<IValidator<GovernorsBulkUpdateViewModel>, GovernorsBulkUpdateViewModelValidator>()
                .AddScoped<IValidator<BulkUpdateViewModel>, BulkUpdateViewModelValidator>()
                .AddScoped<IValidator<EditEstablishmentModel>, EditEstablishmentModelValidator>()
                .AddScoped<IValidator<GovernorsGridViewModel>, GovernorsGridViewModelValidator>()
                .AddScoped<IValidator<ReplaceChairViewModel>, ReplaceChairViewModelValidator>()
                .AddScoped<IValidator<EditGroupDelegationInformationViewModel>, EditGroupDelegationInformationViewModelValidator>()
                .AddScoped<IValidator<SelectSharedGovernorViewModel>, SelectSharedGovernorViewModelValidator>()
                .AddScoped<IValidator<BulkCreateFreeSchoolsViewModel>, BulkCreateFreeSchoolsViewModelValidator>()
                .AddScoped<IValidator<BulkAssociateEstabs2GroupsViewModel>, BulkAssociateEstabs2GroupsViewModelValidator>()
                .AddScoped<IValidator<CreateEstablishmentViewModel>, CreateEstablishmentViewModelValidator>()
                .AddScoped<IValidator<EditEstablishmentLinksViewModel>, EditEstablishmentLinksViewModelValidator>()
                .AddScoped<IValidator<ChangeHistoryViewModel>, ChangeHistoryViewModelValidator>()
                .AddScoped<IValidator<DateTimeViewModel>, DateTimeViewModelValidator>()
                .AddScoped<IValidator<DataQualityStatusItem>, DataQualityStatusItemValidator>()
                .AddScoped<IValidator<ValidateChildrensCentreStep2>, ValidateChildrensCentreStep2Validator>()
                .AddScoped<IValidator<NotificationsBannerViewModel>, NotificationsBannerValidator>();

            // Default model binder registrations
            services.AddSingleton<ICollectionFactory, CollectionFactory>();
            services.AddSingleton<ITypeFactory, TypeFactory>();
            services.AddSingleton<IValueConverter, DefaultValueConverter>();
            services.AddSingleton<ITypeConverter, DefaultTypeConverter>();
            services.AddTransient<DefaultModelBinder>();
            services.AddTransient<IPropertyBinderHandler, AliasBinderHandler>();
            services.AddTransient<IPropertyBinderHandler, ArrayBinderHandler>();
            services.AddTransient<IPropertyBinderHandler, ListBinderHandler>();
            services.AddTransient<IPropertyBinderHandler, ComplexTypeBinderHandler>();
            services.AddTransient<IPropertyBinderHandler, PropertyNameBinderHandler>();
            services.AddTransient<IPropertyBinderHandler, SimpleTypeBinderHandler>();

            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(AutoMapperWebProfile));

            // Add services to the container.
            services.AddControllersWithViews(options =>
            {
                options.ValueProviderFactories.Add(new TokenValueProviderFactory());
                options.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider());
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new DbGeographyConverter());
                });
        }
    }
}
