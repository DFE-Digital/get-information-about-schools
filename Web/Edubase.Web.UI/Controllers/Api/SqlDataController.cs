using System;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Data.Repositories.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class SqlDataController : ControllerBase
    {
        private readonly IAzLogger _logger;
        private readonly FrontEndDbContext _frontEndDbContext;

        public SqlDataController(
            IAzLogger logger,
            FrontEndDbContext frontEndDbContext)
        {
            _logger = logger;
            _frontEndDbContext = frontEndDbContext;
        }


        [Route("api/sql"), HttpGet]
        public async Task<IActionResult> Sql()
        {
            try
            {
                var serverDate = await _frontEndDbContext.Database
                    .SqlQuery<DateTime>("SELECT GETUTCDATE()")
                    .SingleAsync();

                return Ok(new { serverDate, status = "Connected" });
            }
            catch (Exception ex)
            {
                _logger.Log(new WebLogMessage
                {
                    Level = LogMessage.LogLevel.ERROR,
                    Environment = ConfigurationManager.AppSettings["Environment"],
                    Message = $"[api/sql] connection failed. {ex.GetType().Name}: {ex.Message}",
                    Exception = ex.ToString()
                });

                return StatusCode(503);
            }
        }
    }
}
