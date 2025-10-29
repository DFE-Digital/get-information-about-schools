using System;
using System.Threading.Tasks;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSystemWebAdapters()
//    .AddWrappedAspNetCoreSession()
//    .AddJsonSessionSerializer(options =>
//    {
//        options.RegisterKey<string>("MachineName");
//        options.RegisterKey<string>("SessionStartTime");
//    })
//    .AddHttpApplication<MvcApplication>();

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
