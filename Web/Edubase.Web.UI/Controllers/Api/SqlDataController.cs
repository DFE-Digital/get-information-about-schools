using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class SqlDataController : ApiController
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
        public async Task<IHttpActionResult> Sql()
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

                return StatusCode(HttpStatusCode.ServiceUnavailable);
            }
        }      
    }
}
