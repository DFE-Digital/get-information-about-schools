using Autofac;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Helpers
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateEditEstablishmentModelValidator>()
                    .Keyed<IValidator>(typeof(IValidator<CreateEditEstablishmentModel>))
                    .As<IValidator>();
        }
    }
}