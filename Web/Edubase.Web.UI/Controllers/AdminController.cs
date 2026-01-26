using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common.Cache;
using Edubase.Common.Logging;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("Admin")]
    [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
    public class AdminController : EduBaseController
    {
        private readonly WebLogItemRepository _webLogItemRepository;
        private readonly ILoggingService _loggingService;
        private readonly ICacheAccessor _cacheAccessor;

        public AdminController(
            ILoggingService loggingService,
            WebLogItemRepository webLogItemRepository,
            ICacheAccessor cacheAccessor)
        {
            _loggingService = loggingService;
            _webLogItemRepository = webLogItemRepository;
            _cacheAccessor = cacheAccessor;
        }

        [HttpGet("GetPendingErrors")]
        public IActionResult GetPendingErrors(string pwd)
        {
            return pwd == "c7634"
                ? Json(_loggingService.GetPending())
                : new EmptyResult();
        }

        [HttpGet("GetPendingErrorsId")]
        public IActionResult GetPendingErrorsId()
        {
            return Json(_loggingService.InstanceId);
        }

        [HttpGet("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            await _cacheAccessor.ClearAsync();
            return Content("Redis cache and MemoryCache cleared successfully.", "text/plain");
        }

        public async Task FlushErrors() => await _loggingService.FlushAsync();

        [HttpGet("Logs")]
        public async Task<IActionResult> ViewLogs([FromQuery] LogsViewModel model)
        {
            var queryString = model.Query ?? "";
            var includePurgeZeroLogsMessage = model.IncludePurgeZeroLogsMessage;

            var webLogMessages = new List<WebLogMessage>();

            // Search by ID
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                var searchByIdResults = await _webLogItemRepository.GetById(model.Query.ToLowerInvariant());
                if (searchByIdResults.Count() == 1)
                {
                    webLogMessages = [.. searchByIdResults];
                }
            }

            // Search by date range
            if (webLogMessages.Count == 0)
            {
                var startDate = model.StartDate.ToDateTime();
                var endDate = model.EndDate.ToDateTime();

                if (model.StartDate.IsValid() && model.EndDate.IsValid() &&
                    startDate.HasValue && endDate.HasValue)
                {
                    webLogMessages = [.. await _webLogItemRepository
                        .GetWithinDateRange(startDate.Value, endDate.Value.AddDays(1))];

                    webLogMessages = WebLogItemRepository.FilterByAllTextColumns(webLogMessages, queryString);
                    webLogMessages = WebLogItemRepository.FilterPurgeZeroLogsMessage(webLogMessages, includePurgeZeroLogsMessage);
                }
            }

            var viewModel = new LogsViewModel
            {
                Messages = webLogMessages,
                Query = queryString,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IncludePurgeZeroLogsMessage = includePurgeZeroLogsMessage
            };

            return View(viewModel);
        }
    }
}
