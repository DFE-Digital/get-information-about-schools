using Autofac;
using Autofac.Integration.Mvc;
using FluentValidation;
using System;

namespace Edubase.Web.UI.Validation
{
    public class AutofacValidatorFactory : ValidatorFactoryBase
    {
        private readonly AutofacDependencyResolver _resolver;

        public AutofacValidatorFactory(AutofacDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public override IValidator CreateInstance(Type validatorType) => _resolver.RequestLifetimeScope.ResolveOptionalKeyed<IValidator>(validatorType);
    }
}