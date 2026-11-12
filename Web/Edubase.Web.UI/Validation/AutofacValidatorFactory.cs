using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Edubase.Web.UI.Validation
{
    public class AutofacValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceProvider _serviceProvider;

        public AutofacValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType)
            => _serviceProvider.GetService(validatorType) as IValidator;
    }
}
