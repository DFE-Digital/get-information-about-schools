using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Repositories.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class SqlDataController : ControllerBase
    {
        private readonly ILogger<SqlDataController> _logger;
        private readonly FrontEndDbContext _frontEndDbContext;

        public SqlDataController(
            ILogger<SqlDataController> logger,
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
                _logger.LogError(ex, "[api/sql] connection failed in environment {Environment}", ConfigurationManager.AppSettings["Environment"]);
                return StatusCode(503);
            }
        }
    }
}
