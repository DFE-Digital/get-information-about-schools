using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Edubase.Common.Cache;
using Edubase.Web.UI.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;
using AzureTableLogger.Services;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Models.Admin;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Admin"), Route("{action=Logs}"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
    public class AdminController : EduBaseController
    {
        private readonly ErrorWebLogItemRepository _errorWebLogItemRepository;

        private readonly ILoggingService _loggingService;

        public AdminController(ILoggingService loggingService, ErrorWebLogItemRepository errorWebLogItemRepository)
        {
            _loggingService = loggingService;
            _errorWebLogItemRepository = errorWebLogItemRepository;
        }

        [HttpGet, Route("GetPendingErrors")]
        public ActionResult GetPendingErrors(string pwd)
        {
            return pwd == "c7634" ? (ActionResult) Json(_loggingService.GetPending()) : new EmptyResult();
        }

        [HttpGet, Route("GetPendingErrorsId")]
        public ActionResult GetPendingErrorsId()
        {
            return Json(_loggingService.InstanceId);
        }

        [Route("ClearCache")]
        public async Task<ActionResult> ClearCache()
        {
            await IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ICacheAccessor>().ClearAsync();
            return Content("Redis cache and MemoryCache cleared successfully.", "text/plain");
        }

        public async Task FlushErrors() => await _loggingService.FlushAsync();

        public async Task<ActionResult> ViewErrorLogs(ErrorLogsViewModel model)
        {
            var queryString = model.Query ?? "";
            var includePurgeZeroLogsMessage = model.IncludePurgeZeroLogsMessage;

            DateTime? startDate = DateTime.Today.AddDays(-28); // Default to 28 days ago
            DateTime? endDate = DateTime.Today;

            // Check if the separate date fields are filled in to construct the StartDate and EndDate
            // Additionally, check whether the date is valid
            if (model.StartDateDay.HasValue && model.StartDateMonth.HasValue && model.StartDateYear.HasValue &&
                DateIsValid(model.StartDateYear.Value, model.StartDateMonth.Value, model.StartDateDay.Value))
            {
                startDate = new DateTime(model.StartDateYear.Value, model.StartDateMonth.Value,
                    model.StartDateDay.Value);
            }

            if (model.EndDateDay.HasValue && model.EndDateMonth.HasValue && model.EndDateYear.HasValue &&
                DateIsValid(model.EndDateYear.Value, model.EndDateMonth.Value, model.EndDateDay.Value))
            {
                endDate = new DateTime(model.EndDateYear.Value, model.EndDateMonth.Value, model.EndDateDay.Value);
            }

            endDate = endDate.Value.AddDays(1); // Add a day to the end date to include the end date within the search

            var webLogMessages = await _errorWebLogItemRepository.GetWithinDateRange(startDate.Value, endDate.Value);

            webLogMessages = FilterByAllTextColumns(webLogMessages, queryString);
            webLogMessages = FilterPurgeZeroLogsMessage(webLogMessages, includePurgeZeroLogsMessage);

            var viewModel = new ErrorLogsViewModel
            {
                Messages = webLogMessages,
                Query = queryString,
                StartDateDay = startDate.Value.Day,
                StartDateMonth = startDate.Value.Month,
                StartDateYear = startDate.Value.Year,
                EndDateDay = endDate.Value.Day,
                EndDateMonth = endDate.Value.Month,
                EndDateYear = endDate.Value.Year,
                IncludePurgeZeroLogsMessage = includePurgeZeroLogsMessage
            };

            return View(viewModel);
        }

        private bool DateIsValid(int year, int month, int day)
        {
            return DateTime.TryParse($"{year}-{month}-{day}", out _);
        }

        private List<AZTLoggerMessages> FilterPurgeZeroLogsMessage(List<AZTLoggerMessages> webLogMessages, bool includePurgeZeroLogsMessage)
        {
            if (includePurgeZeroLogsMessage)
            {
                return webLogMessages;
            }

            webLogMessages = webLogMessages
                .Where(m => !m.Message.Equals("LOG PURGE REPORT: There were 0 logs purged from storage."))
                .ToList();

            return webLogMessages;
        }

        private static List<AZTLoggerMessages> FilterByAllTextColumns(List<AZTLoggerMessages> webLogMessages,
            string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return webLogMessages;
            }

            var lowerQueryString = queryString.ToLowerInvariant();

            webLogMessages = webLogMessages.Where(m =>
                {
                    if (!string.IsNullOrWhiteSpace(m.Id) && m.Id.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Level) && m.Level.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Environment) && m.Environment.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Message) && m.Message.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Exception) && m.Exception.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Url) && m.Url.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserAgent) && m.UserAgent.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.ClientIpAddress) && m.ClientIpAddress.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.ReferrerUrl) && m.ReferrerUrl.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.HttpMethod) && m.HttpMethod.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.RequestJsonBody) && m.RequestJsonBody.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserId) && m.UserId.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserName) && m.UserName.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    return false;
                }
            ).ToList();

            return webLogMessages;
        }
    }
}
