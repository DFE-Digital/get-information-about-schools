using Edubase.Data.Identity;
using Edubase.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class ApprovalController : Controller
    {
        // GET: Approval
        public ActionResult Index(int? urn = null, int skip = 0, int take = 10)
        {
            var dto = new ApprovalService().GetAll(User as ClaimsPrincipal, skip, take, urn);
            return View(dto);
        }

        
    }
}