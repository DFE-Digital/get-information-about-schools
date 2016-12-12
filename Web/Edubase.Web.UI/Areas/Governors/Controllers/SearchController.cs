using System.Linq;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Data.Entity;
using System.Data.Entity;
using Edubase.Web.UI.Areas.Governors.Models;
using System;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    public class SearchController : Controller
    {        
        public ActionResult Index() => View(new SearchModel());

        public ActionResult Search(SearchModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            else
            {
                using (var dc = new ApplicationDbContext())
                {
                    if(model.RoleId.HasValue) model.RoleName = dc.LookupGovernorRoles.FirstOrDefault(x => x.Id == model.RoleId)?.Name;

                    var query = dc.Governors
                        .Where(x=>x.IsDeleted == false)
                        .Include(x => x.Role)
                        .Include(x => x.AppointingBody)
                        .Include(x => x.Establishment)
                        .Include(x => x.Group);

                    if (model.Forename.Clean() != null) query = query.Where(x => x.Person.FirstName == model.Forename);
                    if (model.Surname.Clean() != null) query = query.Where(x => x.Person.LastName == model.Surname);
                    if (model.RoleId.HasValue) query = query.Where(x => x.RoleId == model.RoleId);

                    var date = model.IncludeHistoric ? DateTime.UtcNow.Date.AddYears(-1) : DateTime.UtcNow.Date;
                    query = query.Where(x => x.AppointmentEndDate == null || x.AppointmentEndDate.Value > date);
                    
                    model.Count = query.Count();
                    model.Results = query.OrderBy(x => x.Person.LastName).Skip(model.StartIndex).Take(50).ToList();
                    model.CalculatePageStats(50);
                    return View("Results", model);
                }
            }
        }


    }
}