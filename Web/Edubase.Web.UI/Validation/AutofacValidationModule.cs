using Autofac;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models.Validators;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.Models.Validators;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.DataQuality;
using Edubase.Web.UI.Models.DataQuality.Validators;
using Edubase.Web.UI.Models.Establishments;
using Edubase.Web.UI.Models.Establishments.Validators;
using Edubase.Web.UI.Models.Validators;
using FluentValidation;

namespace Edubase.Web.UI.Validation
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EditEstablishmentModelValidator>()
                    .Keyed<IValidator>(typeof(IValidator<EditEstablishmentModel>))
                    .As<IValidator>();

            // temporarily disable local validation while it conflicts with the edit policies coming from Texuna
            //builder.RegisterType<CreateEditGovernorViewModelValidator>()
            //        .Keyed<IValidator>(typeof(IValidator<CreateEditGovernorViewModel>))
            //        .As<IValidator>();

            builder.RegisterType<GovernorsGridViewModelValidator>()
                    .Keyed<IValidator>(typeof(IValidator<GovernorsGridViewModel>))
                    .As<IValidator>();

            builder.RegisterType<ReplaceChairViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<ReplaceChairViewModel>))
                .As<IValidator>();

            builder.RegisterType<EditGroupDelegationInformationViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<EditGroupDelegationInformationViewModel>))
                .As<IValidator>();

            builder.RegisterType<SelectSharedGovernorViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<SelectSharedGovernorViewModel>))
                .As<IValidator>();

            builder.RegisterType<BulkUpdateViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<BulkUpdateViewModel>))
                .As<IValidator>();

            builder.RegisterType<GovernorsBulkUpdateViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<GovernorsBulkUpdateViewModel>))
                .As<IValidator>();

            builder.RegisterType<CreateEstablishmentViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<CreateEstablishmentViewModel>))
                .As<IValidator>();

            builder.RegisterType<EditEstablishmentLinksViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<EditEstablishmentLinksViewModel>))
                .As<IValidator>();

            builder.RegisterType<ChangeHistoryViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<ChangeHistoryViewModel>))
                .As<IValidator>();

            builder.RegisterType<DateTimeViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<DateTimeViewModel>))
                .As<IValidator>();

            builder.RegisterType<DataQualityStatusItemValidator>()
                .Keyed<IValidator>(typeof(IValidator<DataQualityStatusItem>))
                .As<IValidator>();

            builder.RegisterType<ValidateChildrensCentreStep2Validator>()
                .Keyed<IValidator>(typeof(IValidator<ValidateChildrensCentreStep2>))
                .As<IValidator>();
        }
    }
}