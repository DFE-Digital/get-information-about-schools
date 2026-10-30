using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Edubase.Data.Repositories.EF;
using Edubase.Services;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class SqlDataController : ControllerBase
    {
        private readonly ILoggingService _logger;
        private readonly FrontEndDbContext _frontEndDbContext;

        public SqlDataController(
            ILoggingService logger,
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
                await _logger.LogAsync($"[api/sql] connection failed. {ex}", "Error");
                return StatusCode((int) HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
