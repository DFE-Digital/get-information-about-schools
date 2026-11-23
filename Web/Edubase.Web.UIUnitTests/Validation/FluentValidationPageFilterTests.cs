using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.DataQuality;
using Edubase.Web.UI.Models.DataQuality.Validators;
using Edubase.Web.UI.Models.Validators;
using Edubase.Web.UI.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Edubase.Web.UIUnitTests.Validation
{
    public class FluentValidationPageFilterTests
    {
        [Fact]
        public async Task OnActionExecutionAsync_AddsValidationErrorsForPageController()
        {
            var context = CreateContext(new PageController());

            await RunFilter(context);

            Assert.Contains(context.ModelState[nameof(TestModel.Name)].Errors,
                error => error.ErrorMessage == "Please enter a name");
        }

        [Fact]
        public async Task OnActionExecutionAsync_DoesNotValidateApiController()
        {
            var context = CreateContext(new ApiTestController());

            await RunFilter(context);

            Assert.True(context.ModelState.IsValid);
        }

        [Fact]
        public async Task OnActionExecutionAsync_ValidatesDateInCollectionWithFieldKey()
        {
            var model = new EditDataQualityStatusViewModel
            {
                Items = new List<DataQualityStatusItem>
                {
                    new DataQualityStatusItem
                    {
                        LastUpdated = new DateTimeViewModel { Day = 31, Month = 2, Year = 2026 }
                    }
                }
            };
            var context = CreateContext(new PageController(), model, services =>
                services.AddSingleton<IValidator<DataQualityStatusItem>, DataQualityStatusItemValidator>());

            await RunFilter(context);

            Assert.Contains(context.ModelState["Items[0].LastUpdated"].Errors,
                error => error.ErrorMessage == "Please correct the date");
        }

        [Fact]
        public async Task OnActionExecutionAsync_ValidatesNestedDate()
        {
            var model = new NestedTestModel
            {
                Date = new DateTimeViewModel { Day = 31, Month = 2, Year = 2026 }
            };
            var context = CreateContext(new PageController(), model, services =>
                services.AddSingleton<IValidator<DateTimeViewModel>, DateTimeViewModelValidator>());

            await RunFilter(context);

            Assert.Contains(context.ModelState[nameof(NestedTestModel.Date)].Errors,
                error => error.ErrorMessage == "The date specified is not valid");
        }

        private static ActionExecutingContext CreateContext(object controller, object model = null,
            Action<IServiceCollection> configureServices = null)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IValidator<TestModel>, TestModelValidator>();
            configureServices?.Invoke(services);
            var provider = services.BuildServiceProvider();
            var actionContext = new ActionContext(
                new DefaultHttpContext { RequestServices = provider },
                new RouteData(),
                new ActionDescriptor(),
                new ModelStateDictionary());

            return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(),
                new Dictionary<string, object> { ["model"] = model ?? new TestModel() }, controller);
        }

        private static Task RunFilter(ActionExecutingContext context)
        {
            return new FluentValidationPageFilter().OnActionExecutionAsync(context, () =>
                Task.FromResult(new ActionExecutedContext(context, new List<IFilterMetadata>(), context.Controller)));
        }

        private class PageController : Controller
        {
        }

        [ApiController]
        private class ApiTestController : Controller
        {
        }

        private class TestModel
        {
            public string Name { get; set; }
        }

        private class NestedTestModel
        {
            public DateTimeViewModel Date { get; set; }
        }

        private class TestModelValidator : AbstractValidator<TestModel>
        {
            public TestModelValidator()
            {
                RuleFor(x => x.Name)
                    .MustAsync((name, cancellationToken) => Task.FromResult(!string.IsNullOrWhiteSpace(name)))
                    .WithMessage("Please enter a name");
            }
        }
    }
}
