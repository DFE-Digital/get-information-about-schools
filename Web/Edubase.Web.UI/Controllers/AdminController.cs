using Edubase.Data.Repositories;
using Edubase.Services;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Web.UI.Models.Admin;

namespace Edubase.Web.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : EduBaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Logs(string date, string skipToken)
        {
            var dto = await new LogMessageReadService().GetAllAsync(10, skipToken, date.ToDateTime("yyyyMMdd"));
            var viewModel = new LogMessagesViewModel(dto) { DateFilter = date };
            return View(viewModel);
        }

        public async Task<ActionResult> LogDetail(string id)
        {
            var message = await new LogMessageReadService().GetAsync(id);
            return View(message);
        }
    }
}