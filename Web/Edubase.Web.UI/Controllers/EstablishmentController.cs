using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Edubase.Web.UI.Models;
using Edubase.Data.Entity;
using AutoMapper;
using Edubase.Web.UI.Identity;

namespace Edubase.Web.UI.Controllers
{
    [Authorize(Roles = IdentityConstants.AccessAllSchoolsRoleName)]
    public class EstablishmentController : Controller
    {
        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var dc = ApplicationDbContext.Create())
            {
                var dataModel = dc.Establishments.FirstOrDefault(x => x.Urn == id);
                var viewModel = Mapper.Map<Establishment, CreateEditEstablishmentModel>(dataModel);
                return View("CreateEdit", viewModel);
            }
        }

        [HttpPost]
        public ActionResult Edit(CreateEditEstablishmentModel model)
        {
            if (model.OpenDate.ToDateTime() == null)
                ModelState.AddModelError("opendate", "Open date is a required field");

            if (ModelState.IsValid)
            {
                using (var dc = ApplicationDbContext.Create())
                {
                    var dataModel = dc.Establishments.FirstOrDefault(x => x.Urn == model.Urn);
                    Mapper.Map(model, dataModel);
                    dc.SaveChanges();
                }
            }

            return View("CreateEdit", model);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View("CreateEdit", new CreateEditEstablishmentModel());
        }


        [HttpPost]
        public ActionResult Create(CreateEditEstablishmentModel model)
        {
            if (model.OpenDate.ToDateTime() == null)
                ModelState.AddModelError("opendate", "Open date is a required field");

            if (ModelState.IsValid)
            {
                using (var dc = ApplicationDbContext.Create())
                {
                    var dataModel = Mapper.Map<Establishment>(model);
                    dc.Establishments.Add(dataModel);
                    dc.SaveChanges();
                    return RedirectToAction("Edit", new { id = dataModel.Urn });
                }
            }
            else return View("CreateEdit", model);
        }

        




    }
}