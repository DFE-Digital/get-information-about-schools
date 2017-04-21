using Autofac;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Areas.Groups.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.Models.Validators;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Validation
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateEditEstablishmentModelValidator>()
                    .Keyed<IValidator>(typeof(IValidator<EditEstablishmentModel>))
                    .As<IValidator>();

            builder.RegisterType<CreateEditGovernorViewModelValidator>()
                    .Keyed<IValidator>(typeof(IValidator<CreateEditGovernorViewModel>))
                    .As<IValidator>();

            builder.RegisterType<GovernorsGridViewModelValidator>()
                    .Keyed<IValidator>(typeof(IValidator<GovernorsGridViewModel>))
                    .As<IValidator>();

            builder.RegisterType<ReplaceChairViewModelValidator>()
                .Keyed<IValidator>(typeof(IValidator<ReplaceChairViewModel>))
                .As<IValidator>();
        }
    }
}