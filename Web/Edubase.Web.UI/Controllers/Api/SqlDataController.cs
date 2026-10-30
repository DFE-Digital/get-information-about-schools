using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Edubase.Common.Logging;

//namespace Edubase.Web.UI.Controllers.Api
//{
//    [Authorize]
//    public class SqlDataController : ApiController
//    {
//        private readonly FrontEndDbContext _frontEndDbContext;

//        public SqlDataController(FrontEndDbContext frontEndDbContext)
//        {
//            _frontEndDbContext = frontEndDbContext;
//        }

//        [Route("api/sql"), HttpGet]
//        public async Task<IHttpActionResult> Sql()
//        {
//            var serverDate = await _frontEndDbContext.Database
//                .SqlQuery<DateTime>("SELECT GETUTCDATE()")
//                .SingleAsync();

//            return Ok(new { serverDate, status = "Connected" });
//        }
//    }
//}
