using Autofac;
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

            //builder.RegisterType<GroupEditorViewModelValidator>()
            //        .Keyed<IValidator>(typeof(IValidator<GroupEditorViewModel>))
            //        .As<IValidator>();
        }
    }
}